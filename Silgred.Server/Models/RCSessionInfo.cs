﻿using System;
using Silgred.Shared.Enums;

namespace Silgred.Server.Models
{
    public class RCSessionInfo
    {
        public string AttendedSessionID { get; set; }
        public string DeviceID { get; set; }
        public string MachineName { get; set; }
        public RemoteControlMode Mode { get; set; }
        public string OrganizationID { get; set; }
        public string RCDeviceSocketID { get; set; }
        public string RequesterName { get; set; }
        public string RequesterSocketID { get; set; }
        public string RequesterUserName { get; set; }
        public string ServiceID { get; set; }
        public DateTimeOffset StartTime { get; set; }
    }
}