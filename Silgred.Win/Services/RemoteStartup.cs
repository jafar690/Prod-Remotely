using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Silgred.ScreenCast.Core;
using Silgred.ScreenCast.Core.Interfaces;
using Silgred.ScreenCast.Core.Services;
using Silgred.ScreenCast.Win.Capture;
using Silgred.ScreenCast.Win.Services;
using Silgred.Win.Interfaces;

namespace Silgred.Win.Services
{
    public static class RemoteStartup
    {
        public static void BuildServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => { builder.AddConsole().AddEventLog(); });

            serviceCollection.AddSingleton<CursorIconWatcher>();
            serviceCollection.AddSingleton<IScreenCaster, WinScreenCaster>();
            serviceCollection.AddSingleton<IKeyboardMouseInput, WinInput>();
            serviceCollection.AddSingleton<IClipboardService, WinClipboardService>();
            serviceCollection.AddSingleton<IAudioCapturer, WinAudioCapturer>();
            serviceCollection.AddSingleton<IRCSocket, RCSocket>();
            serviceCollection.AddTransient<ICapturer>(provider =>
            {
                try
                {
                    var dxCapture = new DXCapture();
                    if (dxCapture.GetScreenCount() == Screen.AllScreens.Length) return dxCapture;

                    Logger.Write("DX screen count doesn't match.  Using CPU capturer instead.");
                    dxCapture.Dispose();
                    return new BitBltCapture();
                }
                catch
                {
                    return new BitBltCapture();
                }
            });


            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }
    }
}