using System.Diagnostics;
using System.Drawing;

namespace Silgred.Server.Models
{
    public class RecordingSessionState
    {
        public Bitmap CumulativeFrame { get; set; }
        public Process FfmpegProcess { get; set; }
    }
}