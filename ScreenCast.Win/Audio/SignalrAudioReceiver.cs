using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Silgred.ScreenCast.Win.Audio
{
    public class SignalrAudioReceiver : IAudioReceiver
    {
        private readonly HubConnection _hubConnection;
        private Action<byte[]> _handler;
        private bool _listening;

        public SignalrAudioReceiver(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
            _listening = true;
            ListenerThread();
        }

        public void Dispose()
        {
            _hubConnection.DisposeAsync();
        }

        public void OnReceived(Action<byte[]> handler)
        {
            _handler = handler;
        }

        private void ListenerThread()
        {
            try
            {
                _hubConnection.On<byte[]>("AudioSample", buffer => { _handler?.Invoke(buffer); });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while reading audio");
                // usually not a problem - just means we have disconnected
            }
        }
    }
}