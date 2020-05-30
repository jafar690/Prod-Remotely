using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Silgred.ScreenCast.Core;
using Silgred.Shared.Models;
using Silgred.Win.Annotations;
using Silgred.Win.Interfaces;
using Silgred.Win.Services;
using Silgred.Win.UI_Garage;
using Silgred.Win.ViewModels;
using Silgred.Win.Windows;

namespace Silgred.Win.Pages
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StartSessionPage : Page, INotifyPropertyChanged
    {
        private IRCSocket _rcSocket;
        private StartSessionPageViewModel _viewModel;

        public StartSessionPage()
        {
            InitializeComponent();
            ViewModel = new StartSessionPageViewModel();
            Init();
            ViewModel.StartSession += ViewModel_StartSession;
        }

        public StartSessionPageViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Init()
        {
            await ViewModel.Init();
        }

        private async void ViewModel_StartSession()
        {
            GoToMeetingButton.IsEnabled = true;
            _rcSocket = (IRCSocket) ServiceContainer.Instance.GetService(typeof(IRCSocket)) ?? new RCSocket();
            await _rcSocket.Connect();
        }


        private void CopyUrlBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Barcode.Text))
                return;
            CopyText(Barcode.Text, sender);
        }

        private void HomeBtn_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void CopyIdBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SessionIdTxtBlock.Text))
                return;
            CopyText(SessionIdTxtBlock.Text, sender);
        }

        private async void CopyText(string text, object sender)
        {
            Clipboard.SetText(text);
            var tooltip = new ToolTip();
            tooltip.PlacementTarget = sender as Button;
            tooltip.Placement = PlacementMode.Bottom;
            tooltip.VerticalOffset = 5;
            tooltip.Content = "Copied to clipboard!";
            tooltip.HasDropShadow = true;
            tooltip.StaysOpen = false;
            tooltip.IsOpen = true;

            await Task.Delay(750);
            var animation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(750));
            tooltip.BeginAnimation(OpacityProperty, animation);
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

        private void GoToMeetingButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var meetingWindow = new Window1(StartSessionPageViewModel.Conductor,_rcSocket, window);
            meetingWindow.Show();

            if (window != null) window.Visibility = Visibility.Collapsed;
        }

        private void Settings_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new SettingsPage());
        }

        private void AcceptButton_OnClick(object sender, RoutedEventArgs e) =>
            ViewModel.AcceptViewer((ScreenCastRequest) (sender as Button)?.DataContext);

        private void DismissBtn_OnClick(object sender, RoutedEventArgs e) =>
            ViewModel.DismissViewer((ScreenCastRequest) (sender as Button)?.DataContext);
    }
}