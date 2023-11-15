using System;
using System.Runtime.InteropServices;

namespace Nmtp.Native.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeFileSampleData
    {
        /// <summary>
        /// Width of sample if it is an image
        /// </summary>
        public uint width;
        /// <summary>
        /// Height of sample if it is an image
        /// </summary>
        public uint height;
        /// <summary>
        /// Duration in milliseconds if it is audio
        /// </summary>
        public uint duration;
        /// <summary>
        /// Filetype used for the sample
        /// </summary>
        public FileType filetype;
        /// <summary>
        /// Size of sample data in bytes
        /// </summary>
        public ulong size;
        /// <summary>
        /// Sample data
        /// </summary>
        public nint data;
    }
}