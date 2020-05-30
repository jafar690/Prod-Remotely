using System;
using System.Windows;
using System.Windows.Input;
using Silgred.Win.Pages;
using Silgred.Win.ViewModels;

namespace Silgred.Win.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel = new MainWindowViewModel();
        }

        public MainWindowViewModel MainWindowViewModel { get; set; }


        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.Navigate(PageHelper.StartPage);
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}