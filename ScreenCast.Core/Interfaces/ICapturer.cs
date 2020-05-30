using System;
using System.Drawing;

namespace Silgred.ScreenCast.Core.Interfaces
{
    public interface ICapturer : IDisposable
    {
        bool CaptureFullscreen { get; set; }
        Bitmap CurrentFrame { get; set; }
        Rectangle CurrentScreenBounds { get; }
        Bitmap PreviousFrame { get; set; }
        int SelectedScreen { get; }
        event EventHandler<Rectangle> ScreenChanged;
        void SetSelectedScreen(int screenNumber);
        int GetScreenCount();
        Rectangle GetVirtualScreenBounds();

        void GetNextFrame();
        void Init();
    }
}