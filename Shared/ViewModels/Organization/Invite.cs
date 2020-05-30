using System;

namespace Silgred.Shared.ViewModels.Organization
{
    public class Invite
    {
        public string ID { get; set; }
        public bool IsAdmin { get; set; }
        public DateTimeOffset DateSent { get; set; }
        public string InvitedUser { get; set; }
    }
}