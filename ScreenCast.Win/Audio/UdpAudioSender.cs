using System.Net;
using System.Net.Sockets;

namespace Silgred.ScreenCast.Win.Audio
{
    public class UdpAudioSender : IAudioSender
    {
        private readonly UdpClient _udpSender;

        public UdpAudioSender(IPEndPoint endPoint)
        {
            _udpSender = new UdpClient();
            _udpSender.Connect(endPoint);
        }

        public void Send(byte[] payload)
        {
            _udpSender.Send(payload, payload.Length);
        }

        public void Dispose()
        {
            _udpSender?.Close();
        }
    }
}