using System;
using System.Runtime.InteropServices;

namespace Nmtp.Native.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeAlbum
    {
        /// <summary>
        /// Unique playlist ID
        /// </summary>
        public uint album_id;
        /// <summary>
        /// ID of parent folder
        /// </summary>
        public uint parent_id;
        /// <summary>
        /// ID of storage holding this album
        /// </summary>
        public uint storage_id;
        /// <summary>
        /// Name of album
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? name;
        /// <summary>
        /// Name of album artist
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? artist;
        /// <summary>
        /// Name of recording composer
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? composer;
        /// <summary>
        ///  Genre of album
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? genre;
        /// <summary>
        /// The tracks in this album
        /// </summary>
        public nint tracks;
        /// <summary>
        /// The number of tracks in this album
        /// </summary>
        public uint no_tracks;
        /// <summary>
        /// Next album or NULL if last album
        /// </summary>
        internal nint next;
    }
}