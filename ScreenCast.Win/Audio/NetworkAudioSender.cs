using System;
using NAudio.Wave;

namespace Silgred.ScreenCast.Win.Audio
{
    public class NetworkAudioSender : IDisposable
    {
        private readonly IAudioSender audioSender;
        private readonly INetworkChatCodec codec;
        private readonly WaveIn waveIn;

        public NetworkAudioSender(INetworkChatCodec codec, int inputDeviceNumber, IAudioSender audioSender)
        {
            this.codec = codec;
            this.audioSender = audioSender;
            waveIn = new WaveIn
            {
                BufferMilliseconds = 50,
                DeviceNumber = inputDeviceNumber,
                WaveFormat = codec.RecordFormat
            };
            waveIn.DataAvailable += OnAudioCaptured;
            waveIn.StartRecording();
        }

        public void Dispose()
        {
            waveIn.DataAvailable -= OnAudioCaptured;
            waveIn.StopRecording();
            waveIn.Dispose();
            waveIn?.Dispose();
            audioSender?.Dispose();
        }

        private void OnAudioCaptured(object sender, WaveInEventArgs e)
        {
            var encoded = codec.Encode(e.Buffer, 0, e.BytesRecorded);
            audioSender.Send(encoded);
        }

        public void MuteAudio()
        {
            waveIn.StopRecording();
        }

        public void UnMuteAudio()
        {
            waveIn.StartRecording();
        }
    }
}