using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InstallScript.Extensions
{
    public static class StreamExtension
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
