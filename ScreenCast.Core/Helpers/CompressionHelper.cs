using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Silgred.ScreenCast.Core.Helpers
{
    public static class CompressionHelper
    {
        public static byte[] Compress(this byte[] b)
        {
            using MemoryStream ms = new MemoryStream();
            using (GZipStream z = new GZipStream(ms, CompressionMode.Compress, true))
                z.Write(b, 0, b.Length);
            return ms.ToArray();
        }
        public static byte[] Decompress(this byte[] b)
        {
            using var ms = new MemoryStream();
            using (var bs = new MemoryStream(b))
            using (var z = new GZipStream(bs, CompressionMode.Decompress))
                z.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
