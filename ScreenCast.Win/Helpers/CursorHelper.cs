using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Silgred.ScreenCast.Win.Helpers
{
    public class CursorHelper
    {
        private const int CURSOR_SHOWING = 0x0001;
        private const int DI_NORMAL = 0x0003;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyHeight,
            int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);
        //With all these hacks i keep needing to do I need to learn C++ and go back to COM that microdoft kicked us all off with .NET about 15 years ago and still uses at its core

        public static Point GetCursorPos()
        {
            //get the mouse position and show on the TextBlock
            return Cursor.Position;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(out CURSORINFO pci);

        public static Color CaptureCursor(ref int X, ref int Y, Graphics theShot, int ScreenServerX, int ScreenServerY)
        {
            //We return a color so that it can be embeded in a bitmap to be returned to the client program
            var C = Cursors.Arrow.Handle;
            CURSORINFO pci;
            pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
            if (GetCursorInfo(out pci))
            {
                X = pci.ptScreenPos.x;
                Y = pci.ptScreenPos.y;
                if (pci.hCursor == Cursors.Default.Handle) return Color.Red;
                if (pci.hCursor == Cursors.WaitCursor.Handle) return Color.Green;
                if (pci.hCursor == Cursors.Arrow.Handle) return Color.Blue;
                if (pci.hCursor == Cursors.IBeam.Handle) return Color.White;
                if (pci.hCursor == Cursors.Hand.Handle) return Color.Violet;
                if (pci.hCursor == Cursors.SizeNS.Handle) return Color.Yellow;
                if (pci.hCursor == Cursors.SizeWE.Handle) return Color.Orange;
                if (pci.hCursor == Cursors.SizeNESW.Handle) return Color.Aqua;
                if (pci.hCursor == Cursors.SizeNWSE.Handle) return Color.Pink;
                if (pci.hCursor == Cursors.PanEast.Handle) return Color.BlueViolet;
                if (pci.hCursor == Cursors.HSplit.Handle) return Color.Cyan;
                if (pci.hCursor == Cursors.VSplit.Handle) return Color.DarkGray;
                if (pci.hCursor == Cursors.Help.Handle) return Color.DarkGreen;
                if (pci.hCursor == Cursors.AppStarting.Handle) return Color.SlateGray;
                if (pci.flags == CURSOR_SHOWING)
                {
                    //We need to caputer the mouse image and embed tha in the screen shot of the desktop because it's a custom mouse cursor
                    var XReal = pci.ptScreenPos.x * (float) ScreenServerX / Screen.PrimaryScreen.Bounds.Width - 11;
                    var YReal = pci.ptScreenPos.y * (float) ScreenServerY / Screen.PrimaryScreen.Bounds.Height - 11;
                    var x = Screen.PrimaryScreen.Bounds.X;
                    var hdc = theShot.GetHdc();
                    DrawIconEx(hdc, (int) XReal, (int) YReal, pci.hCursor, 0, 0, 0, IntPtr.Zero, DI_NORMAL);
                    theShot.ReleaseHdc();
                }

                return Color.Black;
            }

            X = 0;
            Y = 0;
            return Color.Black;
        }

        public static Cursor ColorToCursor(Color C)
        {
            //Code for the client that pulls a pixel from the picture and converts it to a cursor
            if (C.ToArgb() == Color.Red.ToArgb()) return Cursors.Default;
            if (C.ToArgb() == Color.Green.ToArgb()) return Cursors.WaitCursor;
            if (C.ToArgb() == Color.Blue.ToArgb()) return Cursors.Arrow;
            if (C.ToArgb() == Color.White.ToArgb()) return Cursors.IBeam;
            if (C.ToArgb() == Color.Violet.ToArgb()) return Cursors.Hand;
            if (C.ToArgb() == Color.Yellow.ToArgb()) return Cursors.SizeNS;
            if (C.ToArgb() == Color.Orange.ToArgb()) return Cursors.SizeWE;
            if (C.ToArgb() == Color.Aqua.ToArgb()) return Cursors.SizeNESW;
            if (C.ToArgb() == Color.Pink.ToArgb() || C.B == 206) return Cursors.SizeNWSE;
            if (C.ToArgb() == Color.BlueViolet.ToArgb()) return Cursors.PanEast;
            if (C.ToArgb() == Color.Cyan.ToArgb()) return Cursors.HSplit;
            if (C.ToArgb() == Color.DarkGray.ToArgb()) return Cursors.VSplit;
            if (C.ToArgb() == Color.DarkGreen.ToArgb()) return Cursors.Help;
            if (C.ToArgb() == Color.SlateGray.ToArgb()) return Cursors.AppStarting;
            if (C.ToArgb() == Color.Fuchsia.ToArgb()) return Cursors.No;
            return Cursors.Default;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CURSORINFO
        {
            public int cbSize;
            public readonly int flags;
            public readonly IntPtr hCursor;
            public readonly POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTAPI
        {
            public readonly int x;
            public readonly int y;
        }
    }
}