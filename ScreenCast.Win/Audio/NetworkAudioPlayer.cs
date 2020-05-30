using System;
using NAudio.Wave;

namespace Silgred.ScreenCast.Win.Audio
{
    public class NetworkAudioPlayer : IDisposable
    {
        private readonly INetworkChatCodec codec;
        private readonly IAudioReceiver receiver;
        private readonly IWavePlayer waveOut;
        private readonly BufferedWaveProvider waveProvider;

        public NetworkAudioPlayer(INetworkChatCodec codec, IAudioReceiver receiver)
        {
            this.codec = codec;
            this.receiver = receiver;
            receiver.OnReceived(OnDataReceived);

            waveOut = new WaveOut();
            waveProvider = new BufferedWaveProvider(codec.RecordFormat)
            {
                DiscardOnBufferOverflow = true
            };
            waveOut.Init(waveProvider);
            waveOut.Play();
        }


        public void Dispose()
        {
            receiver?.Dispose();
            waveOut?.Dispose();
        }

        private void OnDataReceived(byte[] compressed)
        {
            var decoded = codec.Decode(compressed, 0, compressed.Length);
            waveProvider.AddSamples(decoded, 0, decoded.Length);
        }
    }
}