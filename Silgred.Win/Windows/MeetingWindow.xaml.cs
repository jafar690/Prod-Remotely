using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Silgred.ScreenCast.Core.Models;
using Silgred.Win.Annotations;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.SignalR.Client;
using Silgred.Event.Hookup;
using Silgred.ScreenCast.Core.Services;
using Silgred.ScreenCast.Win.Audio;
using Silgred.ScreenCast.Win.Helpers;
using Silgred.Shared.Models;
using Silgred.Win.Interfaces;
using Application = System.Windows.Application;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Silgred.Win.Windows
{
    /// <summary>
    /// Interaction logic for MeetingWindow.xaml
    /// </summary>
    public partial class MeetingWindow : Window, INotifyPropertyChanged
    {
        private const int AUDIO_PORT = 554;
        private readonly Window _mainWindow;

        private readonly IRCSocket _rCSocket;
        private readonly INetworkChatCodec _selectedCodec;

        private readonly string _userName;
        private NetworkAudioSender _audioSender;
        private ObservableCollection<Chat> _chats;
        private BitmapImage _cursor;
        private WriteableBitmap _cursorBitmap;
        private IKeyboardMouseEvents _globalHook;
        private bool _isAudioConnected;
        private NetworkAudioPlayer _player;
        private int _serverScreenHeight;
        private int _serverScreenWidth;
        private SignalrAudioReceiver _signalrAudioReceiver;
        private SignalrAudioSender _signalrAudioSender;
        private WriteableBitmap _writeableBitmap;

        public ObservableCollection<Chat> Chats
        {
            get => _chats;
            set
            {
                _chats = value;
                OnPropertyChanged(nameof(Chats));
            }
        }

        public MeetingWindow(IRCSocket rcSocket, string userName, Window mainWindow, string machineName)
        {
            InitializeComponent();
            _rCSocket = rcSocket;
            _userName = userName;
            _mainWindow = mainWindow;
            _rCSocket.MessageReceived += RCSocket_MessageReceived;
            _selectedCodec = new MicrosoftAdpcmChatCodec();
            Chats = new ObservableCollection<Chat>();

            Title = $"{machineName} - Silgred Session";
        }
        
        private void RCSocket_MessageReceived(object sender, Chat chat)
        {
            chat.ChatType = chat.Name == _userName ? ChatType.Sender : ChatType.Receiver;
            chat.Time = chat.Time.ToLocalTime();
            Application.Current.Dispatcher.Invoke(() => { Chats.Add(chat); });
        }


        private void ChatButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SidePanel.Visibility == Visibility.Visible)
            {
                Grid.SetColumn(ScreenImage, 0);
                Grid.SetColumnSpan(ScreenImage, 2);
                Grid.SetColumn(BorderControls,0);
                Grid.SetColumnSpan(BorderControls,2);

                SidePanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                Grid.SetColumn(ScreenImage, 1);
                Grid.SetColumn(BorderControls,1);

                SidePanel.Visibility = Visibility.Visible;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MeetingWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _writeableBitmap = new WriteableBitmap(
                (int)ActualWidth,
                (int)ActualHeight,
                96,
                96,
                PixelFormats.Bgra32, null);
            ScreenImage.Stretch = Stretch.Uniform;
            var audioReceiver = new UdpAudioReceiver(AUDIO_PORT);
            _player = new NetworkAudioPlayer(_selectedCodec, audioReceiver);
            OnScreenSizeReceived();

            // TODO : Remove this method since it is already called in the on OnScreenSizeReceived method
            // SubscribeToRemoteEvents();
        }
        private (int width, int height) RescaleScreenImage(int receivedImageWidth, int receivedImageHeight)
        {
            var actualScreenImageWidth = (int)ScreenImage.ActualWidth;
            var diffWidth = Math.Abs(receivedImageWidth - actualScreenImageWidth);
            var diffFactor = receivedImageWidth / diffWidth;
            var newWidth = actualScreenImageWidth;
            var newHeight = receivedImageHeight - receivedImageHeight / diffFactor;
            return (newWidth, newHeight);
        }

        private void OnScreenSizeReceived()
        {
            _rCSocket.Connection.On<int, int>("ScreenSize", (width, height) =>
            {
                _serverScreenHeight = height;
                _serverScreenWidth = width;

                _writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                _cursorBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                CursorImage.Stretch = Stretch.Uniform;

                //Subscribe to events only when connection has been established
                try
                {
                    SubscribeToRemoteEvents();
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });
        }

        private void SubscribeToRemoteEvents()
        {
            OnScreenCapture();
            OnCursorChange();
            Subscribe();
            _signalrAudioReceiver = new SignalrAudioReceiver(_rCSocket.Connection);
            _signalrAudioSender = new SignalrAudioSender(_rCSocket.SendAudioSample);
            _audioSender = new NetworkAudioSender(_selectedCodec, 0, _signalrAudioSender);
            _player = new NetworkAudioPlayer(_selectedCodec, _signalrAudioReceiver);
        }

        private void OnScreenCapture()
        {
            _rCSocket.Connection.On<byte[], int, int, int, int, DateTime>("ScreenCapture",
                (captureBytes, left, top, width, height, captureTime) =>
                {
                    _rCSocket.SendLatencyUpdate(captureTime, captureBytes.Length);
                    var image = GetBitmapImage(captureBytes);
                    var rect = new Int32Rect(left, top, width, height);
                    var bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8; // general formula
                    var stride = bytesPerPixel * width;
                    var pixels = new byte[height * stride];
                    image.CopyPixels(pixels, stride, 0);
                    if (pixels.Length != 0)
                    {
                        _writeableBitmap.WritePixels(rect, pixels, stride, 0);
                        ScreenImage.Source = _writeableBitmap;
                    }
                });
        }
        private BitmapImage GetBitmapImage(byte[] array)
        {
            if (array.Length != 0)
                using (var ms = new MemoryStream(array))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }

            return null;
        }

        private void OnCursorChange()
        {
            _rCSocket.Connection.On<CursorInfo>("CursorChange", cursorInfo =>
            {
                if (cursorInfo != null)
                {
                    if (cursorInfo.ImageBytes.Length > 0) _cursor = GetBitmapImage(cursorInfo.ImageBytes);

                    if (_cursor == null)
                        return; //prevent exeption when cursor pointer image has not been sent

                    var rect = new Int32Rect(cursorInfo.HotSpot.X, cursorInfo.HotSpot.Y, _cursor.PixelWidth,
                        _cursor.PixelHeight);
                    var bytesPerPixel = (_cursor.Format.BitsPerPixel + 7) / 8;
                    var stride = bytesPerPixel * _cursor.PixelWidth;
                    var pixels = new byte[_cursor.PixelHeight * stride];
                    _cursor.CopyPixels(pixels, stride, 0);
                    if (cursorInfo.ImageBytes.Length != 0)
                        try
                        {
                            _cursorBitmap.Clear(Color.FromArgb(0, 0, 0, 0));
                            _cursorBitmap.WritePixels(rect, pixels, stride, 0);
                            CursorImage.Source = _cursorBitmap;
                        }
                        catch (ArgumentException ex)
                        {
                            //Ignore this cursor change
                            Console.WriteLine(ex.Message);
                        }
                }
            });
        }

        public void Subscribe()
        {
            _globalHook = Hook.GlobalEvents();
            _isAudioConnected = true;
            _globalHook.MouseMove += GlobalHookMouseMove;
            _globalHook.MouseDownExt += GlobalHookMouseDownExt;
            _globalHook.MouseUpExt += GlobalHookMouseUpExt;
            _globalHook.KeyPress += GlobalHookKeyPress;
        }

        private async void GlobalHookMouseMove(object sender, MouseEventArgs e)
        {
            var mouse = GetMouseCooridinates();
            await _rCSocket.SendMouseMove(mouse.X, mouse.Y);
        }

        private async void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            await _rCSocket.KeyPress(e.KeyChar.ToString());
        }

        private async void GlobalHookMouseUpExt(object sender, MouseEventExtArgs e)
        {
            var button = e.Button;
            var position = GetMouseCooridinates();
            await _rCSocket.SendMouseUp(button == MouseButtons.Left ? 0 : 2, position.X, position.Y);
        }

        private async void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            var button = e.Button;
            var position = GetMouseCooridinates();
            await _rCSocket.SendMouseDown(button == MouseButtons.Left ? 0 : 2, position.X, position.Y);
        }

        public Point ScaleScreenCoordinates(System.Drawing.Point currentPosition, int currentWidth, int currentHeight)
        {
            if (_serverScreenHeight == 0 && _serverScreenWidth == 0)
                return new Point(0, 0);
            return new Point(currentPosition.X * (currentWidth / _serverScreenWidth),
                currentPosition.Y * (currentHeight / _serverScreenHeight));
        }
        public Point ScaleScreenCoordinates(Point currentPosition, double currentWidth, double currentHeight)
        {
            if (_serverScreenHeight == 0 && _serverScreenWidth == 0)
                return new Point(0, 0);
            return new Point(currentPosition.X * (currentWidth / _serverScreenWidth),
                currentPosition.Y * (currentHeight / _serverScreenHeight));
        }

        private Point GetMouseCooridinates()
        {
            var position = CursorHelper.GetCursorPos();
            var width = Screen.PrimaryScreen.Bounds.Width;
            var height = Screen.PrimaryScreen.Bounds.Height;
            var mousePosition = Mouse.GetPosition(ScreenImage);

            var mouse = ScaleScreenCoordinates(mousePosition, ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            return new Point(mouse.X, mouse.Y);
        }

        private void BorderControls_OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#A0AEC0");
            BorderControls.Background = brush;
            GridControls.Visibility = Visibility.Visible;
        }

        private void BorderControls_OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#00FFFFFF");
            BorderControls.Background = brush;
            GridControls.Visibility = Visibility.Hidden;
        }

        private void MeetingWindow_OnClosed(object? sender, EventArgs e)
        {
            Dispose();
            _mainWindow.Show();
            Environment.Exit(0);
        }

        public void Dispose()
        {
            UnSubscribe();
            DisconnectAudio();
        }

        private void DisconnectAudio()
        {
            _player?.Dispose();
            _audioSender?.Dispose();
            _selectedCodec?.Dispose();
        }

        public void UnSubscribe()
        {
            _globalHook.MouseMove -= GlobalHookMouseMove;
            _globalHook.MouseDownExt -= GlobalHookMouseDownExt;
            _globalHook.MouseUpExt -= GlobalHookMouseUpExt;
            _globalHook.KeyPress -= GlobalHookKeyPress;

            _globalHook.Dispose();
        }

        private async void SendBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var chat = new Chat { Message = MessageTxtBox.Text, Name = _userName, Time = DateTime.Now };
            await _rCSocket.SendMessage(chat);
            MessageTxtBox.Text = String.Empty;
        }


        private void BtnConnectAudio_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isAudioConnected)
            {
                _isAudioConnected = false;
                MicController.Kind = PackIconKind.MicOff;
                _audioSender.MuteAudio();
            }
            else
            {
                _isAudioConnected = true;
                MicController.Kind = PackIconKind.Microphone;
                _audioSender.UnMuteAudio();
            }
        }
    }
}
