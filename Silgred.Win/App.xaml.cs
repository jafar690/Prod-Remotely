using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Silgred.Win.Services;
using Syncfusion.Licensing;
using static Silgred.Win.Helpers.Constants;
using WinApplication= System.Windows.Forms.Application;

namespace Silgred.Win
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_LICENSE_KEY);
            RemoteStartup.BuildServices();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            WinApplication.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.ThrowException);
            WinApplication.ThreadException += (sender, args) =>
            {
                Crashes.TrackError(args.Exception);
            };

            //AppCenter.Start(APP_CENTER_APPSECRET, typeof(Analytics), typeof(Crashes));

            Crashes.ShouldAwaitUserConfirmation = () => false;
        }
    }
}