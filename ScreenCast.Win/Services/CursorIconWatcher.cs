﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using Silgred.Shared.Models;
using Silgred.Shared.Win32;
using Timer = System.Timers.Timer;

namespace Silgred.ScreenCast.Win.Services
{
    /// <summary>
    ///     A class that can be used to watch for cursor icon changes.
    /// </summary>
    public class CursorIconWatcher
    {
        private User32.CursorInfo cursorInfo;

        public CursorIconWatcher()
        {
            ChangeTimer = new Timer(25);
            ChangeTimer.Elapsed += ChangeTimer_Elapsed;
            ChangeTimer.Start();
        }

        private Timer ChangeTimer { get; }
        private string PreviousCursorHandle { get; set; }
        public event EventHandler<CursorInfo> OnChange;


        public CursorInfo GetCurrentCursor()
        {
            try
            {
                var ci = new User32.CursorInfo();
                ci.cbSize = Marshal.SizeOf(ci);
                User32.GetCursorInfo(out ci);
                if (ci.flags == User32.CURSOR_SHOWING)
                {
                    if (ci.hCursor.ToString() == Cursors.IBeam.Handle.ToString())
                        return new CursorInfo(new byte[0], Point.Empty, "text");

                    using (var icon = Icon.FromHandle(ci.hCursor))
                    {
                        using (var ms = new MemoryStream())
                        {
                            using (var cursor = new Cursor(ci.hCursor))
                            {
                                var hotspot = cursor.HotSpot;
                                icon.ToBitmap().Save(ms, ImageFormat.Png);
                                return new CursorInfo(ms.ToArray(), hotspot);
                            }
                        }
                    }
                }

                return new CursorInfo(new byte[0], Point.Empty, "default");
            }
            catch
            {
                return new CursorInfo(new byte[0], Point.Empty, "default");
            }
        }

        private void ChangeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (OnChange == null) return;
            try
            {
                cursorInfo = new User32.CursorInfo();
                cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);
                User32.GetCursorInfo(out cursorInfo);
                if (cursorInfo.flags == User32.CURSOR_SHOWING)
                {
                    var currentCursor = cursorInfo.hCursor.ToString();
                    if (currentCursor != PreviousCursorHandle)
                    {
                        if (currentCursor == Cursors.IBeam.Handle.ToString())
                            OnChange?.Invoke(this, new CursorInfo(new byte[0], Point.Empty, "text"));
                        else
                            using (var icon = Icon.FromHandle(cursorInfo.hCursor))
                            {
                                using (var ms = new MemoryStream())
                                {
                                    using (var cursor = new Cursor(cursorInfo.hCursor))
                                    {
                                        var hotspot = cursor.HotSpot;
                                        icon.ToBitmap().Save(ms, ImageFormat.Png);
                                        OnChange?.Invoke(this, new CursorInfo(ms.ToArray(), hotspot));
                                    }
                                }
                            }

                        PreviousCursorHandle = currentCursor;
                    }
                }
                else if (PreviousCursorHandle != "0")
                {
                    PreviousCursorHandle = "0";
                    OnChange?.Invoke(this, new CursorInfo(new byte[0], Point.Empty, "default"));
                }
            }
            catch
            {
                OnChange?.Invoke(this, new CursorInfo(new byte[0], Point.Empty, "default"));
            }
        }
    }
}