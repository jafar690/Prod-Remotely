﻿using System;
using NAudio.Wave;

namespace Silgred.ScreenCast.Win.Audio
{
    internal class UncompressedPcmChatCodec : INetworkChatCodec
    {
        public UncompressedPcmChatCodec()
        {
            RecordFormat = new WaveFormat(8000, 16, 1);
        }

        public string Name => "PCM 8kHz 16 bit uncompressed";

        public WaveFormat RecordFormat { get; }

        public byte[] Encode(byte[] data, int offset, int length)
        {
            var encoded = new byte[length];
            Array.Copy(data, offset, encoded, 0, length);
            return encoded;
        }

        public byte[] Decode(byte[] data, int offset, int length)
        {
            var decoded = new byte[length];
            Array.Copy(data, offset, decoded, 0, length);
            return decoded;
        }

        public int BitsPerSecond => RecordFormat.AverageBytesPerSecond * 8;

        public void Dispose()
        {
        }

        public bool IsAvailable => true;
    }
}