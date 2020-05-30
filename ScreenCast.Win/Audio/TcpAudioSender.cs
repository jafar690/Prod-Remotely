using System.Net;
using System.Net.Sockets;

namespace Silgred.ScreenCast.Win.Audio
{
    public class TcpAudioSender : IAudioSender
    {
        private readonly bool _isConnected;
        private readonly TcpClient tcpSender;

        public TcpAudioSender(IPEndPoint endPoint)
        {
            tcpSender = new TcpClient();
            _isConnected = tcpSender.ConnectAsync(endPoint.Address, endPoint.Port).Wait(10000);
        }

        public void Send(byte[] payload)
        {
            if (_isConnected)
                tcpSender.Client.Send(payload);
        }

        public void Dispose()
        {
            tcpSender?.Close();
        }
    }
}