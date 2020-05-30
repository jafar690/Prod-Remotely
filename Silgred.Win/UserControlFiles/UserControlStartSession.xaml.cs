using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Silgred.Win.ViewModels;

namespace Silgred.Win.UserControlFiles
{
    /// <summary>
    ///     Interaction logic for UserControlStartSession.xaml
    /// </summary>
    public partial class UserControlStartSession : UserControl
    {
        private MainWindowViewModel _viewModel = new MainWindowViewModel();

        public UserControlStartSession()
        {
            InitializeComponent();
        }

        private async void CopyLinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (Barcode.Text != null) Clipboard.SetText(Barcode.Text);
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
    }

    public class Tst
    {
        public string Name { get; set; }
        public bool HasControl { get; set; }
    }

    public class tstdt
    {
        public List<Tst> tstdta = new List<Tst>
        {
            new Tst {HasControl = true, Name = "Sam"},
            new Tst {HasControl = true, Name = "James"},
            new Tst {HasControl = false, Name = "Max"},
            new Tst {HasControl = false, Name = "Tim"}
        };
    }
}