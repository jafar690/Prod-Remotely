using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Silgred.ScreenCast.Core.Models;
using Silgred.ScreenCast.Win.Audio;
using Silgred.Win.Annotations;
using Silgred.Win.Interfaces;
using Silgred.Win.Services;

namespace Silgred.Win.Windows
{
    /// <summary>
    ///     Interaction logic for GoToMeetingWindow.xaml
    /// </summary>
    public partial class GoToMeetingWindow : Window, INotifyPropertyChanged
    {
        private const int AUDIO_PORT = 554;

        private readonly Window _mainWindow;


        private readonly IRCSocket _rcSocket;
        private readonly INetworkChatCodec _selectedCodec;
        private NetworkAudioSender _audioSender;
        private ObservableCollection<Chat> _chats;
        private bool _isAudioConnected;
        private NetworkAudioPlayer _player;
        private SignalrAudioReceiver _signalrAudioReceiver;
        private SignalrAudioSender _signalrAudioSender;
        private UdpAudioReceiver _tcpAudioReceiver;

        public GoToMeetingWindow(IRCSocket rcSocket, Window mainWindow)
        {
            InitializeComponent();
            _rcSocket = rcSocket;
            _mainWindow = mainWindow;
            _rcSocket.MessageReceived += _rcSocket_MessageReceived;
            _selectedCodec = new MicrosoftAdpcmChatCodec();
            _isAudioConnected = true;

            Chats = new ObservableCollection<Chat>();
        }

        public ObservableCollection<Chat> Chats
        {
            get => _chats;
            set
            {
                _chats = value;
                OnPropertyChanged(nameof(Chats));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void _rcSocket_MessageReceived(object sender, Chat chat)
        {
            chat.ChatType = chat.Name == Config.GetConfig().Name ? ChatType.Sender : ChatType.Receiver;
            chat.Time = chat.Time.ToLocalTime();
            Application.Current.Dispatcher.Invoke(() => { Chats.Add(chat); });
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

        private async void SendMessageBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageTxtBox.Text == "Type a message") return;
            var chat = new Chat {Message = MessageTxtBox.Text, Name = Config.GetConfig().Name, Time = DateTime.Now};
            await _rcSocket.SendMessage(chat);
            MessageTxtBox.Text = "";
            Keyboard.ClearFocus();
        }

        private void MessageTxtBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MessageTxtBox.Text))
                MessageTxtBox.Text = "Type a message";
        }

        private void MessageTxtBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (MessageTxtBox.Text == "Type a message")
                MessageTxtBox.Text = "";
        }

        private void GoToMeetingWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _tcpAudioReceiver = new UdpAudioReceiver(AUDIO_PORT); //start listening
            _player = new NetworkAudioPlayer(_selectedCodec, _tcpAudioReceiver);
            SubscribeToRemoteEvents();
        }

        private void SubscribeToRemoteEvents()
        {
            _signalrAudioReceiver = new SignalrAudioReceiver(_rcSocket.Connection);
            _signalrAudioSender = new SignalrAudioSender(_rcSocket.SendAudioSample);
            _audioSender = new NetworkAudioSender(_selectedCodec, 0, _signalrAudioSender);
            _player = new NetworkAudioPlayer(_selectedCodec, _signalrAudioReceiver);
        }

        private void Dispose()
        {
            DisconnectAudio();
        }

        private void DisconnectAudio()
        {
            _player?.Dispose();
            _audioSender?.Dispose();
            _selectedCodec?.Dispose();
        }

        private void GoToMeetingWindow_OnClosed(object sender, EventArgs e)
        {
            Dispose();
            Environment.Exit(0);
        }
    }
}