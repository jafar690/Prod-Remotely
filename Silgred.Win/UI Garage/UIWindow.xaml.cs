using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.SignalR.Client;
using Silgred.Event.Hookup;
using Silgred.ScreenCast.Core.Helpers;
using Silgred.ScreenCast.Core.Models;
using Silgred.Shared.Models;
using Silgred.ScreenCast.Win.Audio;
using Silgred.Win.Annotations;
using Silgred.Win.Interfaces;
using Application = System.Windows.Application;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Silgred.Win.UI_Garage
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class UIWindow : IDisposable, INotifyPropertyChanged
    {
        private IRCSocket _rcSocket;
        private WriteableBitmap _writeableBitmap;
        private WriteableBitmap _cursorBitmap;
        private int _serverScreenHeight;
        private int _serverScreenWidth;
        private BitmapImage _cursor;
        private IKeyboardMouseEvents _globalHook;
        private bool _isAudioConnected;
        private INetworkChatCodec _selectedCodec;
        private NetworkAudioPlayer _player;
        private NetworkAudioSender _audioSender;
        private SignalrAudioReceiver _signalrAudioReceiver;
        private SignalrAudioSender _signalrAudioSender;

        private ObservableCollection<Chat> _chats;
        private readonly string _userName;
        private readonly Window _mainWindow;

        public ObservableCollection<Chat> Chats
        {
            get => _chats;
            set
            {
                _chats = value;
                OnPropertyChanged(nameof(Chats));
            }
        }

        public void Subscribe()
        {
            _globalHook = Hook.AppEvents();
            _isAudioConnected = true;
            _globalHook.MouseMove += GlobalHookMouseMove;
            _globalHook.MouseDownExt += GlobalHookMouseDownExt;
            _globalHook.MouseUpExt += GlobalHookMouseUpExt;
            _globalHook.KeyPress += GlobalHookKeyPress;
        }

        private async void GlobalHookKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            await _rcSocket.KeyPress(e.KeyChar.ToString());
        }

        private async void GlobalHookMouseUpExt(object sender, MouseEventExtArgs e)
        {
            var button = e.Button;
            var position = GetMouseCooridinates();
            await _rcSocket.SendMouseUp(button == System.Windows.Forms.MouseButtons.Left ? 0 : 2, position.X, position.Y);
        }

        private async void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            var button = e.Button;
            var position = GetMouseCooridinates();
            await _rcSocket.SendMouseDown(button == System.Windows.Forms.MouseButtons.Left ? 0 : 2, position.X, position.Y);
        }

        public void UnSubscribe()
        {
            _globalHook.MouseMove -= GlobalHookMouseMove;
            _globalHook.MouseDownExt -= GlobalHookMouseDownExt;
            _globalHook.MouseUpExt -= GlobalHookMouseUpExt;
            _globalHook.KeyPress -= GlobalHookKeyPress;

            _globalHook.Dispose();
        }

        private async void GlobalHookMouseMove(object sender, MouseEventArgs e)
        {
            var mouse = GetMouseCooridinates();
            await _rcSocket.SendMouseMove(mouse.X, mouse.Y);
        }

        public UIWindow(IRCSocket rcSocket, string userName, Window mainWindow, string machineName)
        {
            InitializeComponent();
            _rcSocket = rcSocket;
            _selectedCodec = new MicrosoftAdpcmChatCodec();

            _userName = userName;
            _mainWindow = mainWindow;
            _rcSocket.MessageReceived += RCSocket_MessageReceived;
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
                Grid.SetColumn(BorderControls, 0);
                Grid.SetColumnSpan(BorderControls, 2);

                SidePanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                Grid.SetColumn(ScreenImage, 1);
                Grid.SetColumn(BorderControls, 1);

                SidePanel.Visibility = Visibility.Visible;
            }
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            OnScreenSizeReceived();
            OnMachineNameReceived();
        }

        private Point GetMouseCooridinates()
        {
            var mousePosition = Mouse.GetPosition(ScreenImage);

            var mouse = Scale(mousePosition, _writeableBitmap.PixelWidth, _writeableBitmap.PixelHeight);
            return new Point(mouse.X, mouse.Y);
        }
        public Point ScaleScreenCoordinates(Point currentPosition, double currentWidth, double currentHeight)
        {
            if (_serverScreenHeight == 0 && _serverScreenWidth == 0)
                return new Point(0, 0);
            return new Point(currentPosition.X * (currentWidth / _serverScreenWidth), currentPosition.Y * (currentHeight / _serverScreenHeight));
        }
        private Point Scale(Point currentPosition, double screenWidth, double screenHeight)
        {
            if (_serverScreenHeight == 0 && _serverScreenWidth == 0)
                return new Point(0, 0);
            var difference = Math.Abs(_serverScreenWidth - screenWidth);
            var factor = _serverScreenWidth / difference;
            var newHeight = _serverScreenHeight - (_serverScreenHeight / factor);

            return new Point(currentPosition.X * (screenWidth / _serverScreenWidth), currentPosition.Y * (newHeight / _serverScreenHeight));
        }

        private void OnCursorChange()
        {
            _rcSocket.Connection.On<CursorInfo>("CursorChange", cursorInfo =>
            {
               cursorInfo.ImageBytes = cursorInfo.ImageBytes.Decompress();
                if (cursorInfo != null)
                {
                    if (cursorInfo.ImageBytes.Length > 0)
                    {
                        _cursor = GetBitmapImage(cursorInfo.ImageBytes);
                    }

                    if (_cursor == null)
                        return; //prevent exeption when cursor pointer image has not been sent

                    Int32Rect rect = new Int32Rect(cursorInfo.HotSpot.X, cursorInfo.HotSpot.Y, _cursor.PixelWidth, _cursor.PixelHeight);
                    int bytesPerPixel = (_cursor.Format.BitsPerPixel + 7) / 8;
                    int stride = bytesPerPixel * _cursor.PixelWidth;
                    byte[] pixels = new byte[_cursor.PixelHeight * stride];
                    _cursor.CopyPixels(pixels, stride, 0);
                    if (cursorInfo.ImageBytes.Length != 0)
                    {
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
                }
            });
        }
        private void OnScreenCapture()
        {
            _rcSocket.Connection.On<byte[], int, int, int, int, DateTime>("ScreenCapture", (captureBytes, left, top, width, height, captureTime) =>
            {
                captureBytes = captureBytes.Decompress();
                _rcSocket.SendLatencyUpdate(captureTime, captureBytes.Length);
                var image = GetBitmapImage(captureBytes);
                Int32Rect rect = new Int32Rect(left, top, width, height);
                int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8; // general formula
                int stride = bytesPerPixel * width;
                byte[] pixels = new byte[height * stride];
                image.CopyPixels(pixels, stride, 0);
                if (pixels.Length != 0)
                {
                    _writeableBitmap.WritePixels(rect, pixels, stride, 0);
                    ScreenImage.Source = _writeableBitmap;
                }
            });
        }
        private void OnMachineNameReceived()
        {
            _rcSocket.Connection.On("ReceiveMachineName", (Action<string>)(async machineName =>
            {
                Title = $"{machineName} - Remotely Session";
            }));
        }

        private void SubscribeToRemoteEvents()
        {
            OnScreenCapture();
            OnCursorChange();
            Subscribe();
            _signalrAudioReceiver = new SignalrAudioReceiver(_rcSocket.Connection);
            _signalrAudioSender = new SignalrAudioSender(_rcSocket.SendAudioSample);
            _audioSender = new NetworkAudioSender(_selectedCodec, 0, _signalrAudioSender);
            _player = new NetworkAudioPlayer(_selectedCodec, _signalrAudioReceiver);
        }

        private void BtnAudioConnect_Click(object sender, RoutedEventArgs e)
        {
            if (_isAudioConnected)
            {
                _isAudioConnected = false;
                MicController.Kind = PackIconKind.Microphone;
                _audioSender.MuteAudio();
            }
            else
            {
                _isAudioConnected = true;
                MicController.Kind = PackIconKind.MicrophoneOff;
                _audioSender.UnMuteAudio();

            }
        }


        private void OnScreenSizeReceived()
        {
            _rcSocket.Connection.On<int, int>("ScreenSize", (width, height) =>
            {
                _serverScreenHeight = height;
                _serverScreenWidth = width;

                _writeableBitmap = new WriteableBitmap(width, height, 120, 120, PixelFormats.Bgra32, null);
                _cursorBitmap = new WriteableBitmap(width, height, 120, 120, PixelFormats.Bgra32, null);
                CursorImage.Stretch = Stretch.Uniform;
                ScreenImage.Stretch = Stretch.Uniform;

                //Subscribe to events only when connection has been established
                try
                {

                    SubscribeToRemoteEvents();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        private (double dpiX, double dpiY) GetScreenDPI()
        {
            var dpi_scale = VisualTreeHelper.GetDpi(this);
            double dpiX = dpi_scale.PixelsPerInchX;
            double dpiY = dpi_scale.PixelsPerInchY;
            return (dpiX, dpiY);
        }


        public BitmapImage GetBitmapImage(byte[] array)
        {
            if (array.Length != 0)
            {
                using (var ms = new System.IO.MemoryStream(array))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
            }
            return null;
        }
        public byte[] GetPixels(int stride, int height, int bytesPerPixel)
        {
            var pixelByteArrayOfColors = new byte[stride * height];
            for (int pixel = 0; pixel < pixelByteArrayOfColors.Length; pixel += bytesPerPixel)
            {
                pixelByteArrayOfColors[pixel] = 0;        // blue (depends normally on BitmapPalette)
                pixelByteArrayOfColors[pixel + 1] = 255;  // green (depends normally on BitmapPalette)
                pixelByteArrayOfColors[pixel + 2] = 0;    // red (depends normally on BitmapPalette)
                pixelByteArrayOfColors[pixel + 3] = 50;   // alpha (depends normally on BitmapPalette)
            }
            return pixelByteArrayOfColors;
        }


        private void UIWindow_OnClosed(object? sender, EventArgs e)
        {
            Dispose();
            _mainWindow.Show();
            Environment.Exit(0);
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
        private async void SendBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var chat = new Chat { Message = MessageTxtBox.Text, Name = _userName, Time = DateTime.Now };
            await _rcSocket.SendMessage(chat);
            MessageTxtBox.Text = String.Empty;
        }
        public void Dispose()
        {
            UnSubscribe();
            DisconnectAudio();
        }
        public void DisconnectAudio()
        {
            _player?.Dispose();
            _audioSender?.Dispose();
            _selectedCodec?.Dispose();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}