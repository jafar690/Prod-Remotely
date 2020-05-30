using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Silgred.Event.Hookup;
using Silgred.ScreenCast.Core;
using Silgred.ScreenCast.Core.Communication;
using Silgred.ScreenCast.Core.Interfaces;
using Silgred.ScreenCast.Core.Models;
using Silgred.ScreenCast.Core.Services;
using Silgred.ScreenCast.Win.Capture;
using Silgred.ScreenCast.Win.Helpers;
using Silgred.ScreenCast.Win.Services;
using Silgred.Shared.Models;
using Silgred.Win.Pages;
using Silgred.Win.Services;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Point = System.Drawing.Point;

namespace Silgred.Win.ViewModels
{
    public class StartSessionPageViewModel : BaseViewModel, IDisposable
    {
        private IKeyboardMouseEvents _globalHook;

        private string _host;
        private ObservableCollection<ScreenCastRequest> _lobbyViewers;
        private string _sessionId;
        private string _sessionUrl;
        private string _theme;
        private ObservableCollection<Viewer> _viewers;


        public StartSessionPageViewModel()
        {
            Current = this;
            Viewers = new ObservableCollection<Viewer>();
            LobbyViewers = new ObservableCollection<ScreenCastRequest>();

            BuildServices();

            CursorIconWatcher = Services.GetRequiredService<CursorIconWatcher>();
            Subscribe();
            Services.GetRequiredService<IClipboardService>().BeginWatching();
            Conductor = Services.GetRequiredService<Conductor>();
            Conductor.SessionIDChanged += SessionIdChanged;
            Conductor.ViewerRemoved += ViewerRemoved;
            Conductor.ViewerAdded += ViewerAdded;
            Conductor.ScreenCastRequested += ScreenCastRequested;
        }

        private string Host
        {
            get => _host;
            set
            {
                _host = value;
                OnPropertyChanged(nameof(Host));
            }
        }

        public string Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                OnPropertyChanged(nameof(Theme));
            }
        }

        public string SessionId
        {
            get => _sessionId;
            set
            {
                _sessionId = value;
                OnPropertyChanged(nameof(SessionId));
            }
        }
        

        public string SessionUrl
        {
            get => _sessionUrl;
            set
            {
                _sessionUrl = value;
                OnPropertyChanged(nameof(SessionUrl));
            }
        }

        public static Conductor Conductor { get; set; }
        private CursorIconWatcher CursorIconWatcher { get; }

        public ObservableCollection<Viewer> Viewers
        {
            get => _viewers;
            set
            {
                _viewers = value;
                OnPropertyChanged(nameof(Viewers));
            }
        }

        public ObservableCollection<ScreenCastRequest> LobbyViewers
        {
            get => _lobbyViewers;
            set
            {
                _lobbyViewers = value;
                OnPropertyChanged(nameof(LobbyViewers));
            }
        }

        public static StartSessionPageViewModel Current { get; private set; }

        private static IServiceProvider Services => ServiceContainer.Instance;

        public ICommand RemoveViewersCommand
        {
            get
            {
                return new Executor(async param =>
                    {
                        foreach (Viewer viewer in ((IList<object>) param).ToArray())
                        {
                            viewer.DisconnectRequested = true;
                            ViewerRemoved(this, viewer.ViewerConnectionID);
                            await Conductor.CasterSocket.SendViewerRemoved(viewer.ViewerConnectionID);
                        }
                    },
                    param => (param as IList<object>)?.Count > 0);
            }
        }

        public void Dispose()
        {
            UnSubscribe();
        }

        public async void AcceptViewer(ScreenCastRequest request)
        {
            await Conductor.CasterSocket.SendCursorChange(CursorIconWatcher.GetCurrentCursor(),
                new List<string> {request.ViewerID});
            await Services.GetRequiredService<IScreenCaster>().BeginScreenCasting(request);
            LobbyViewers.Remove(request);
            StartSession?.Invoke();
        }

        public async void DismissViewer(ScreenCastRequest request)
        {
            await Conductor.CasterSocket.SendConnectionFailedToViewers(new List<string> {request.ViewerID});
            LobbyViewers.Remove(request);
        }

        public event Action StartSession;

        public void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            _globalHook = Hook.GlobalEvents();

            _globalHook.MouseMove += GlobalHookMouseMove;
        }

        public void UnSubscribe()
        {
            _globalHook.MouseMove -= GlobalHookMouseMove;

            _globalHook.Dispose();
        }

        private async void GlobalHookMouseMove(object sender, MouseEventArgs e)
        {
            if (Viewers == null || Viewers.Count <= 0) return;
            var cursor = CursorIconWatcher.GetCurrentCursor();
            var cursorPos = CursorHelper.GetCursorPos();
            cursor = new CursorInfo(cursor.ImageBytes, new Point(cursorPos.X, cursorPos.Y));
            if (Conductor?.CasterSocket != null)
                await Conductor?.CasterSocket?.SendCursorChange(cursor, Conductor.Viewers.Keys.ToList());
        }

        public async Task Init()
        {
            SessionId = "Retrieving...";

            Host = Config.GetConfig().Host;
            Theme = Config.GetConfig().Theme;

            while (string.IsNullOrWhiteSpace(Host))
            {
                Host = "https://";
                if (PageHelper.StartSessionPage.NavigationService != null)
                    PageHelper.StartSessionPage.NavigationService.Navigate(new SettingsPage());
            }

            Conductor.ProcessArgs(new[] {"-mode", "Normal", "-host", Host});
            try
            {
                await Conductor.Connect();


                Conductor.CasterSocket.Connection.Closed += async ex =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Viewers.Clear();
                        SessionId = "Disconnected";
                    });
                };

                Conductor.CasterSocket.Connection.Reconnecting += async ex =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Viewers.Clear();
                        SessionId = "Reconnecting";
                    });
                };

                Conductor.CasterSocket.Connection.Reconnected += async arg => { await GetSessionID(); };

                await GetSessionID();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                if (Application.Current.MainWindow != null)
                    MessageBox.Show(Application.Current.MainWindow, "Failed to connect to server.", "Connection Failed",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task GetSessionID()
        {
            await Conductor.CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName,
                Conductor.DeviceID);
            await Conductor.CasterSocket.GetSessionID();
        }

        private static void BuildServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => { builder.AddConsole().AddEventLog(); });

            serviceCollection.AddSingleton<CursorIconWatcher>();
            serviceCollection.AddSingleton<IScreenCaster, WinScreenCaster>();
            serviceCollection.AddSingleton<IKeyboardMouseInput, WinInput>();
            serviceCollection.AddSingleton<IClipboardService, WinClipboardService>();
            serviceCollection.AddSingleton<IAudioCapturer, WinAudioCapturer>();
            serviceCollection.AddSingleton<CasterSocket>();
            serviceCollection.AddSingleton<IdleTimer>();
            serviceCollection.AddSingleton<Conductor>();
            serviceCollection.AddTransient<ICapturer>(provider =>
            {
                try
                {
                    var dxCapture = new DXCapture();
                    if (dxCapture.GetScreenCount() == Screen.AllScreens.Length) return dxCapture;

                    Logger.Write("DX screen count doesn't match.  Using CPU capturer instead.");
                    dxCapture.Dispose();
                    return new BitBltCapture();
                }
                catch
                {
                    return new BitBltCapture();
                }
            });


            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }

        private void ScreenCastRequested(object sender, ScreenCastRequest screenCastRequest)
        {
            Application.Current.Dispatcher.Invoke(() => { LobbyViewers.Add(screenCastRequest); });
        }

        private void SessionIdChanged(object sender, string sessionId)
        {
            var formattedSessionId = "";
            for (var i = 0; i < sessionId.Length; i += 3) formattedSessionId += sessionId.Substring(i, 3) + " ";
            SessionId = formattedSessionId.Trim();
            SessionUrl = GetSessionUrl();
        }

        private string GetSessionUrl()
        {
            Host = Config.GetConfig().Host;
            return $"{Host}/RemoteControl?sessionID={SessionId?.Replace(" ", "")}";
        }

        private void ViewerAdded(object sender, Viewer viewer)
        {
            viewer.HasControl = false;
            Application.Current.Dispatcher.Invoke(() => { Viewers.Add(viewer); });
        }

        private void ViewerRemoved(object sender, string viewerId)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var viewer = Viewers.FirstOrDefault(x => x.ViewerConnectionID == viewerId);
                if (viewer == null) return;
                Viewers.Remove(viewer);
                viewer.Dispose();
            });
        }
    }
}