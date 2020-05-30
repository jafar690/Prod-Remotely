using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Silgred.ScreenCast.Win.Audio
{
    public class TcpAudioReceiver : IAudioReceiver
    {
        private readonly TcpListener listener;
        private Action<byte[]> handler;
        private bool listening;

        public TcpAudioReceiver(int portNumber)
        {
            var endPoint = new IPEndPoint(IPAddress.Loopback, portNumber);
            listener = new TcpListener(endPoint);
            listener.Start();
            listening = true;
            ThreadPool.QueueUserWorkItem(ListenerThread, null);
        }

        public void OnReceived(Action<byte[]> onAudioReceivedAction)
        {
            handler = onAudioReceivedAction;
        }

        public void Dispose()
        {
            listening = false;
            listener?.Stop();
        }

        private void ListenerThread(object state)
        {
            var incomingBuffer = new byte[1024 * 16];
            try
            {
                while (listening)
                    using (var client = listener.AcceptTcpClient())
                    {
                        while (listening)
                        {
                            var received = client.Client.Receive(incomingBuffer);
                            var b = new byte[received];
                            Buffer.BlockCopy(incomingBuffer, 0, b, 0, received);
                            handler?.Invoke(b);
                        }
                    }
            }
            catch (SocketException)
            {
                // usually not a problem - just means we have disconnected
            }
        }
    }
}