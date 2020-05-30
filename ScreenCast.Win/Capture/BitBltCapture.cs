using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Silgred.ScreenCast.Core.Interfaces;
using Silgred.ScreenCast.Core.Services;
using Silgred.Shared.Win32;

namespace Silgred.ScreenCast.Win.Capture
{
    public class BitBltCapture : ICapturer
    {
        public BitBltCapture()
        {
            Init();
        }

        private Graphics Graphic { get; set; }

        public event EventHandler<Rectangle> ScreenChanged;

        public bool CaptureFullscreen { get; set; } = true;
        public Bitmap CurrentFrame { get; set; }
        public Rectangle CurrentScreenBounds { get; set; } = Screen.PrimaryScreen.Bounds;
        public Bitmap PreviousFrame { get; set; }
        public int SelectedScreen { get; private set; } = Screen.AllScreens.ToList().IndexOf(Screen.PrimaryScreen);

        public void Dispose()
        {
            Graphic.Dispose();
            CurrentFrame.Dispose();
            PreviousFrame.Dispose();
        }

        public void GetNextFrame()
        {
            try
            {
                Win32Interop.SwitchToInputDesktop();
                PreviousFrame.Dispose();
                PreviousFrame = (Bitmap) CurrentFrame.Clone();
                Graphic.CopyFromScreen(CurrentScreenBounds.Left, CurrentScreenBounds.Top, 0, 0,
                    new Size(CurrentScreenBounds.Width, CurrentScreenBounds.Height));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Logger.Write("Capturer error.  Trying to switch desktops in BitBltCapture.");
                if (Win32Interop.SwitchToInputDesktop())
                {
                    Win32Interop.GetCurrentDesktop(out var desktopName);
                    Logger.Write($"Switch to desktop {desktopName} after capture error in BitBltCapture.");
                }

                Init();
            }
        }

        public int GetScreenCount()
        {
            return Screen.AllScreens.Length;
        }

        public Rectangle GetVirtualScreenBounds()
        {
            return SystemInformation.VirtualScreen;
        }

        public void Init()
        {
            CurrentFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height,
                PixelFormat.Format32bppArgb);
            PreviousFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height,
                PixelFormat.Format32bppArgb);
            Graphic = Graphics.FromImage(CurrentFrame);
        }

        public void SetSelectedScreen(int screenNumber)
        {
            if (screenNumber == SelectedScreen) return;

            if (GetScreenCount() >= screenNumber + 1)
                SelectedScreen = screenNumber;
            else
                SelectedScreen = 0;
            CurrentScreenBounds = Screen.AllScreens[SelectedScreen].Bounds;
            CaptureFullscreen = true;
            Init();
            ScreenChanged?.Invoke(this, CurrentScreenBounds);
        }
    }
}