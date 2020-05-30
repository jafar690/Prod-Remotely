using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Silgred.ScreenCast.Core;
using Silgred.ScreenCast.Core.Capture;
using Silgred.ScreenCast.Core.Interfaces;
using Silgred.ScreenCast.Core.Services;
using Silgred.Shared.Models;
using Silgred.Shared.Win32;

namespace Silgred.ScreenCast.Win.Services
{
    public class WinScreenCaster : ScreenCasterBase, IScreenCaster
    {
        public WinScreenCaster(CursorIconWatcher cursorIconWatcher)
        {
            CursorIconWatcher = cursorIconWatcher;
        }

        public CursorIconWatcher CursorIconWatcher { get; }

        public async Task BeginScreenCasting(ScreenCastRequest screenCastRequest)
        {
            if (Win32Interop.GetCurrentDesktop(out var currentDesktopName))
            {
                Logger.Write($"Setting desktop to {currentDesktopName} before screen casting.");
                Win32Interop.SwitchToInputDesktop();
            }
            else
            {
                Logger.Write("Failed to get current desktop before screen casting.");
            }

            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            await conductor.CasterSocket.SendCursorChange(CursorIconWatcher.GetCurrentCursor(),
                new List<string> {screenCastRequest.ViewerID});
            _ = BeginScreenCasting(screenCastRequest.ViewerID, screenCastRequest.RequesterName,
                ServiceContainer.Instance.GetRequiredService<ICapturer>());
        }
    }
}