using System;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Silgred.ScreenCast.Core.Interfaces;
using Silgred.ScreenCast.Core.Services;
using Silgred.Shared.Win32;
using Timer = System.Timers.Timer;

namespace Silgred.ScreenCast.Win.Services
{
    public class WinClipboardService : IClipboardService
    {
        private string ClipboardText { get; set; }

        private Timer ClipboardWatcher { get; set; }
        public event EventHandler<string> ClipboardTextChanged;

        public void BeginWatching()
        {
            try
            {
                if (ClipboardWatcher?.Enabled == true) ClipboardWatcher.Stop();

                if (Clipboard.ContainsText())
                {
                    ClipboardText = Clipboard.GetText();
                    if (ClipboardTextChanged == null)
                        return;
                    ClipboardTextChanged.Invoke(this, ClipboardText);
                }

                ClipboardWatcher = new Timer(500);
            }
            catch
            {
                return;
            }

            ClipboardWatcher.Elapsed += ClipboardWatcher_Elapsed;
            ClipboardWatcher.Start();
        }

        public void SetText(string clipboardText)
        {
            try
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(clipboardText))
                            Clipboard.Clear();
                        else
                            Clipboard.SetText(clipboardText);
                    }
                    catch
                    {
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void ClipboardWatcher_Elapsed(object sender, ElapsedEventArgs e)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    Win32Interop.SwitchToInputDesktop();


                    if (Clipboard.ContainsText() && Clipboard.GetText() != ClipboardText)
                    {
                        ClipboardText = Clipboard.GetText();
                        ClipboardTextChanged.Invoke(this, ClipboardText);
                    }
                }
                catch
                {
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void StopWatching()
        {
            ClipboardWatcher?.Stop();
        }
    }
}