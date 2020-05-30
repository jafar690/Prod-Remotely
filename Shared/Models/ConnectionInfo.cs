using System;

namespace Silgred.Shared.Models
{
    public class ConnectionInfo
    {
        private string host;
        public string DeviceID { get; set; } = Guid.NewGuid().ToString();

        public string Host
        {
            get => host;
            set
            {
                host = value.Trim();
                if (host.EndsWith("/")) host = host.Substring(0, host.LastIndexOf("/"));
            }
        }

        public string OrganizationID { get; set; }
        public string ServerVerificationToken { get; set; }
    }
}