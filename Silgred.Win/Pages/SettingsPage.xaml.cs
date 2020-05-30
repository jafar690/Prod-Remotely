using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Silgred.Win.Annotations;
using Silgred.Win.Services;

namespace Silgred.Win.Pages
{
    /// <summary>
    ///     Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private Config _config;

        public SettingsPage()
        {
            InitializeComponent();
            SideBarListView.SelectedItem = Account;
            Config = Config.GetConfig();
        }

        public Config Config
        {
            get => _config;
            set
            {
                _config = value;
                OnPropertyChanged(nameof(Config));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void SideBarListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
                if (e.RemovedItems[0] is DockPanel remItem)
                    remItem.Opacity = .7;

            var item = (DockPanel) SideBarListView.SelectedItem;
            item.Opacity = 1;

            switch (item.Name)
            {
                case "Account":
                    break;

                case "Theme":
                    break;

                case "Media":
                    break;

                case "Help":
                    break;
            }
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

        private void SaveHostBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var result = HostNameTxtBox.Text.TrimEnd('/');

            if (!string.IsNullOrEmpty(result) && !result.StartsWith("https://") && !result.StartsWith("http://"))
                result = $"https://{result}";


            if (!IsValidUrl(result))
            {
                ShowToolTip(sender, "Invalid Host Url");
                HostNameTxtBox.Text = Config.Host;
                return;
            }

            if (result == Config.Host)
                return;

            Config.Host = result;
            Config.Save();
            ShowToolTip(sender, "Host Updated");
            PageHelper.Init();
        }

        private bool IsValidUrl(string url)
        {
            var Pattern = @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
            var Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Rgx.IsMatch(url);
        }

        private async void ShowToolTip(object sender, string message)
        {
            var tooltip = new ToolTip();
            tooltip.PlacementTarget = sender as Button;
            tooltip.Placement = PlacementMode.Bottom;
            tooltip.VerticalOffset = 5;
            tooltip.Content = message;
            tooltip.HasDropShadow = true;
            tooltip.StaysOpen = false;
            tooltip.IsOpen = true;

            await Task.Delay(750);
            var animation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(750));
            tooltip.BeginAnimation(OpacityProperty, animation);
        }

        private void HostNameTxtBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (HostNameTxtBox.Text == Config.Host)
                HostNameTxtBox.Text = "https://";
        }

        private void HostNameTxtBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(HostNameTxtBox.Text))
                HostNameTxtBox.Text = Config.Host;
        }
    }
}