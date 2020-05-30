using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Silgred.Server.Services
{
    public class MessagingSocketHub : Hub
    {
        public async Task SendMessage(string message, string senderName, DateTime time)
        {
            await Clients.All.SendAsync("ReceiveMessage", senderName, message, time);
        }
    }
}