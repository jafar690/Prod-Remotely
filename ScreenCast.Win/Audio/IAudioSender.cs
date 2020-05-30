using System;

namespace Silgred.ScreenCast.Win.Audio
{
    public interface IAudioSender : IDisposable
    {
        void Send(byte[] payload);
    }
}