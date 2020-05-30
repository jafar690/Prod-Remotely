using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Silgred.Win.Pages
{
    /// <summary>
    ///     Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
            SideBarListView.SelectedItem = Dashboard;
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
                case "Dashboard":
                    break;

                case "FileTransfer":
                    break;

                case "Account":
                    break;

                case "History":
                    break;
            }
        }

        private void StartSessionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(PageHelper.StartSessionPage);
        }

        private void JoinSessionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(PageHelper.JoinSessionPage);
        }

        private void ShareFilesBtn_OnClick(object sender, RoutedEventArgs e)
        {
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