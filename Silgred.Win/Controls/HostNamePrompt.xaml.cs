using System.Windows;
using Syncfusion.Windows.Shared;

namespace Silgred.Win.Controls
{
    /// <summary>
    ///     Interaction logic for HostNamePrompt.xaml
    /// </summary>
    public partial class HostNamePrompt : ChromelessWindow
    {
        public HostNamePrompt()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}