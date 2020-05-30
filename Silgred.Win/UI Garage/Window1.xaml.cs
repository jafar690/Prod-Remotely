using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using Silgred.Win.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Silgred.Event.Hookup;
using Silgred.ScreenCast.Core.Models;
using Silgred.Win.Helpers;
using Silgred.Shared.Models;
using Silgred.ScreenCast.Win.Audio;
using Silgred.ScreenCast.Win.Helpers;
using Silgred.Win.Annotations;
using Silgred.Win.Interfaces;
using Application = System.Windows.Application;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Silgred.ScreenCast.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Silgred.Win.UI_Garage
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class Window1 : Window, IDisposable, INotifyPropertyChanged
    {
        private const int _audioPort = 554;
        private NetworkAudioSender _audioSender;
        private bool _isAudioConnected;
        private NetworkAudioPlayer _player;
        private readonly IConductor _conductor;
        private readonly IRCSocket _rCSocket;
        private readonly INetworkChatCodec _selectedCodec;
        private SignalrAudioReceiver _signalrAudioReceiver;
        private SignalrAudioSender _signalrAudioSender;

        private ObservableCollection<Chat> _chats;
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

        public Window1(IConductor conductor,IRCSocket rCSocket,Window mainWindow)
        {
            InitializeComponent();
            _conductor = conductor;
            _rCSocket = rCSocket;
            _selectedCodec = new MicrosoftAdpcmChatCodec();

            _mainWindow = mainWindow;
            _rCSocket.MessageReceived += RCSocket_MessageReceived;
            Chats = new ObservableCollection<Chat>();
        }

        private void RCSocket_MessageReceived(object sender, Chat chat)
        {
            chat.ChatType = chat.Name == Config.GetConfig().Name ? ChatType.Sender : ChatType.Receiver;
            chat.Time = chat.Time.ToLocalTime();
            Application.Current.Dispatcher.Invoke(() => { Chats.Add(chat); });
        }

        public void Dispose()
        {
            DisconnectAudio();
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            _isAudioConnected = true;
            _signalrAudioReceiver = new SignalrAudioReceiver(_conductor.CasterSocket.Connection);
            _signalrAudioSender = new SignalrAudioSender(SendAudio);
            _audioSender = new NetworkAudioSender(_selectedCodec, 0, _signalrAudioSender);
            _player = new NetworkAudioPlayer(_selectedCodec, _signalrAudioReceiver);
        }
        private async Task SendAudio(byte[] buffer)
        {
            await _conductor.CasterSocket.SendAudioSample(buffer, _conductor.Viewers.Values.Select(x => x.ViewerConnectionID).ToList());
        }

        private void BtnAudioConnect_Click(object sender, RoutedEventArgs e)
        {
            if (_isAudioConnected)
            {
                _isAudioConnected = false;
                MicController.Kind = PackIconKind.MicrophoneOff;
                _audioSender.MuteAudio();
            }
            else
            {
                _isAudioConnected = true;
                MicController.Kind = PackIconKind.Microphone;
                _audioSender.UnMuteAudio();
            }
        }


        public BitmapImage GetBitmapImage(byte[] array)
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

        public byte[] GetPixels(int stride, int height, int bytesPerPixel)
        {
            var pixelByteArrayOfColors = new byte[stride * height];
            for (var pixel = 0; pixel < pixelByteArrayOfColors.Length; pixel += bytesPerPixel)
            {
                pixelByteArrayOfColors[pixel] = 0; // blue (depends normally on BitmapPalette)
                pixelByteArrayOfColors[pixel + 1] = 255; // green (depends normally on BitmapPalette)
                pixelByteArrayOfColors[pixel + 2] = 0; // red (depends normally on BitmapPalette)
                pixelByteArrayOfColors[pixel + 3] = 50; // alpha (depends normally on BitmapPalette)
            }

            return pixelByteArrayOfColors;
        }


        public void DisconnectAudio()
        {
            _player?.Dispose();
            _audioSender?.Dispose();
            _selectedCodec?.Dispose();
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
            var chat = new Chat { Message = MessageTxtBox.Text, Name = Config.GetConfig().Name, Time = DateTime.Now };
            await _rCSocket.SendMessage(chat);
            MessageTxtBox.Text = String.Empty;
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}