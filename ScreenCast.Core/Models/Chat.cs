using System;

namespace Silgred.ScreenCast.Core.Models
{
    public enum ChatType
    {
        Sender,
        Receiver
    }

    public class Chat
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }

        public string TimeDisplay => $"{Time:MM/dd}  {Time:t}";

        public ChatType ChatType { get; set; }
    }
}