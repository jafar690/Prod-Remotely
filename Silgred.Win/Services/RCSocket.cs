using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Silgred.ScreenCast.Core.Helpers;
using Silgred.ScreenCast.Core.Models;
using Silgred.ScreenCast.Core.Services;
using Silgred.Win.Interfaces;

namespace Silgred.Win.Services
{
    public class RCSocket : IRCSocket
    {
        public HubConnection Connection { get; private set; }
        public HubConnection MessagingHub { get; private set; }
        public event EventHandler<Chat> MessageReceived;

        public async Task Connect()
        {
            var host = Config.GetConfig().Host;
            if (Connection != null)
            {
                await Connection.StopAsync();
                await Connection.DisposeAsync();
            }

            if (MessagingHub != null)
            {
                await MessagingHub.StopAsync();
                await MessagingHub.DisposeAsync();
            }

            Connection = new HubConnectionBuilder()
                .WithUrl($"{host}/RCBrowserHub")
                .AddMessagePackProtocol()
                .WithAutomaticReconnect()
                .Build();

            MessagingHub = new HubConnectionBuilder()
                .WithUrl($"{host}/MessagingHub")
                .AddMessagePackProtocol()
                .WithAutomaticReconnect()
                .Build();

            await Connection.StartAsync();
            await MessagingHub.StartAsync();


            OnConnectionFailed();
            OnReceiveMessage();
            ApplyConnectionHandlers();
        }


        public async Task SendIceCandidate()
        {
            await Connection.InvokeAsync("SendIceCandidateToAgent", "", 0, "");
        }

        public async Task SendRtcAnswer(string sessionDescription)
        {
            await Connection.InvokeAsync("SendRtcAnswerToAgent", sessionDescription);
        }

        public async Task SendScreenCastRequestToDevice(string screenCasterID, string requesterName,
            int remoteControlMode)
        {
            await Connection.InvokeAsync("SendScreenCastRequestToDevice", screenCasterID, requesterName,
                remoteControlMode);
        }

        public async Task SendMessage(Chat chat)
        {
            await MessagingHub.InvokeAsync("SendMessage", chat.Message, chat.Name, chat.Time);
        }

        public async Task SendLatencyUpdate(DateTime sentTime, int bytesRecieved)
        {
            await Connection.InvokeAsync("SendLatencyUpdate", sentTime, bytesRecieved);
        }

        public async Task SendMouseMove(double percentX, double percentY)
        {
            await Connection.InvokeAsync("MouseMove", percentX, percentY);
        }

        public async Task SendMouseDown(int button, double percentX, double percentY)
        {
            await Connection.InvokeAsync("MouseDown", button, percentX, percentY);
        }

        public async Task SendMouseUp(int button, double percentX, double percentY)
        {
            await Connection.InvokeAsync("MouseUp", button, percentX, percentY);
        }

        public async Task KeyDown(string key)
        {
            await Connection.InvokeAsync("KeyDown", key);
        }

        public async Task KeyUp(string key)
        {
            await Connection.InvokeAsync("KeyUp", key);
        }

        public async Task KeyPress(string key)
        {
            await Connection.InvokeAsync("KeyPress", key);
        }

        public async Task SendCtrlAltDel()
        {
            await Connection.InvokeAsync("CtrlAltDel");
        }
        public async Task SendAudioEndpoint(byte[] endPoint)
        {
            await Connection.SendAsync("SendAudioEndpoint", endPoint.Compress());
        }
        public async Task SendAudioSample(byte[] buffer)
        {
            await Connection.SendAsync("SendAudioSample", buffer);
        }

        private void OnConnectionFailed()
        {
            Connection.On("ConnectionFailed", async () =>
            {
                await Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() => { MessageBox.Show("Connection failed"); }));
                await Connection.StopAsync();
            });
        }

        private void ApplyConnectionHandlers()
        {
            Connection.Closed += ex =>
            {
                Logger.Write($"Connection closed.  Error: {ex?.Message}");
                return Task.CompletedTask;
            };
            MessagingHub.On<string, string, DateTime>("ReceiveMessage", (senderName, message, time) =>
            {
                var chat = new Chat {Message = message, Name = senderName, Time = time};
                MessageReceived?.Invoke(null, chat);
            });
        }


        private void OnReceiveMessage()
        {
            Connection.On<string>("ShowMessage", async message =>
            {
                await Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() => { MessageBox.Show(message); }));
                await Connection.StopAsync();
            });
        }
    }
}