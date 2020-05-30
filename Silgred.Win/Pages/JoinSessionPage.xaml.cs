using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Silgred.ScreenCast.Core;
using Silgred.Shared.Enums;
using Silgred.Win.Interfaces;
using Silgred.Win.Services;
using Silgred.Win.UI_Garage;
using Silgred.Win.Windows;

namespace Silgred.Win.Pages
{
    /// <summary>
    ///     Interaction logic for JoinSessionPage.xaml
    /// </summary>
    public partial class JoinSessionPage
    {
        private IRCSocket _rcSocket;

        public JoinSessionPage()
        {
            InitializeComponent();
            _rcSocket = (IRCSocket) ServiceContainer.Instance.GetService(typeof(IRCSocket));
        }

        private async void JoinSessionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var sessionId = Regex.Replace(SessionIdTxtBox.Text, @"\s+", "");
            var name = NameTxtBox.Text;
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(name)) return;
            await _rcSocket.SendScreenCastRequestToDevice(sessionId, name, (int) RemoteControlMode.Normal);
            var config = Config.GetConfig();
            config.Name = name;
            config.Save();

            OnMachineNameReceived();
        }

        private void OnMachineNameReceived()
        {
            _rcSocket.Connection.On("ReceiveMachineName", (Action<string>) (machineName =>
            {
                //Subscribe to events only when connection has been established
                try
                {
                    var window = Window.GetWindow(this);
                    var meetingWindow = new UIWindow(_rcSocket, Config.GetConfig().Name, window, machineName);
                    meetingWindow.Show();

                    if (window != null) window.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }));
        }

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private async void JoinSessionPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            _rcSocket ??= new RCSocket();
            await _rcSocket.Connect();
        }

        private void SessionIdTxtBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (SessionIdTxtBox.Text == "Session ID")
                SessionIdTxtBox.Text = "";
        }

        private void SessionIdTxtBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SessionIdTxtBox.Text))
                SessionIdTxtBox.Text = "Session ID";
        }

        private void NameTxtBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (NameTxtBox.Text == "Your Name")
                NameTxtBox.Text = "";
        }

        private void NameTxtBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameTxtBox.Text))
                NameTxtBox.Text = "Your Name";
        }

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            if (win != null) win.WindowState = WindowState.Minimized;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Settings_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new SettingsPage());
        }
    }
}