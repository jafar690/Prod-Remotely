﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Silgred.Shared.Models
{
    public class EventLog
    {
        [Key] public string ID { get; set; } = Guid.NewGuid().ToString();

        public EventType EventType { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string OrganizationID { get; set; }
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;

        [JsonIgnore] public Organization Organization { get; set; }
    }

    public enum EventType
    {
        Info = 0,
        Error = 1,
        Debug = 2,
        Warning = 3
    }
}