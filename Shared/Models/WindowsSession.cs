﻿namespace Silgred.Shared.Models
{
    public enum SessionType
    {
        Console,
        RDP
    }

    public class WindowsSession
    {
        public uint ID { get; internal set; }
        public string Name { get; internal set; }
        public SessionType Type { get; internal set; }
        public string Username { get; internal set; }
    }
}