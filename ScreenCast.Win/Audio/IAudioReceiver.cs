using System;

namespace Silgred.ScreenCast.Win.Audio
{
    public interface IAudioReceiver : IDisposable
    {
        void OnReceived(Action<byte[]> handler);
    }
}