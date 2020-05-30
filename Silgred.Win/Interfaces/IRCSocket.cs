using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Silgred.ScreenCast.Core.Models;

namespace Silgred.Win.Interfaces
{
    public interface IRCSocket
    {
        HubConnection Connection { get; }
        HubConnection MessagingHub { get; }

        event EventHandler<Chat> MessageReceived;

        Task Connect();
        Task KeyDown(string key);
        Task KeyPress(string key);
        Task KeyUp(string key);
        Task SendCtrlAltDel();
        Task SendIceCandidate();
        Task SendLatencyUpdate(DateTime sentTime, int bytesRecieved);
        Task SendMouseDown(int button, double percentX, double percentY);
        Task SendMouseMove(double percentX, double percentY);
        Task SendMouseUp(int button, double percentX, double percentY);
        Task SendRtcAnswer(string sessionDescription);
        Task SendAudioEndpoint(byte[] endPoint);
        Task SendScreenCastRequestToDevice(string screenCasterID, string requesterName, int remoteControlMode);
        Task SendMessage(Chat chat);
        Task SendAudioSample(byte[] buffer);
    }
}