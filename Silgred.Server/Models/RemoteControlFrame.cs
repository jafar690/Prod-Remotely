using System;

namespace Silgred.Server.Models
{
    public class RemoteControlFrame
    {
        public RemoteControlFrame(byte[] frameBytes, int left, int top, int width,
            int height, string viewerID, string machineName, DateTimeOffset startTime)
        {
            FrameBytes = frameBytes;
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            ViewerID = viewerID;
            MachineName = machineName;
            StartTime = startTime;
        }

        public byte[] FrameBytes { get; }
        public int Left { get; }
        public int Top { get; }
        public int Width { get; }
        public int Height { get; }
        public string ViewerID { get; }
        public string MachineName { get; }
        public DateTimeOffset StartTime { get; }
    }
}