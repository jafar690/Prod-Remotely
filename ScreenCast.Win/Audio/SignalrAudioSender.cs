using System;
using System.Threading.Tasks;

namespace Silgred.ScreenCast.Win.Audio
{
    public class SignalrAudioSender : IAudioSender
    {
        private readonly Action<byte[]> _sendBytes;
        private readonly Func<byte[], Task> _sendFuncBytes;

        public SignalrAudioSender(Action<byte[]> sendBytes)
        {
            _sendBytes = sendBytes;
        }

        public SignalrAudioSender(Func<byte[], Task> sendBytes)
        {
            _sendFuncBytes = sendBytes;
        }

        public void Dispose()
        {
        }

        public void Send(byte[] payload)
        {
            if (_sendFuncBytes != null)
                _sendFuncBytes.Invoke(payload);
            else
                _sendBytes.Invoke(payload);
        }
    }
}