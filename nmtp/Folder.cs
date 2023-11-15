using System;
using System.Runtime.InteropServices;

namespace Nmtp
{
    /// <summary>
    /// MTP Folder structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Folder
    {
        /// <summary>
        /// Unique folder ID
        /// </summary>
        public uint FolderId;

        /// <summary>
        /// ID of parent folder
        /// </summary>
        public uint ParentId;

        /// <summary>
        /// ID of storage holding this folder
        /// </summary>
        public uint StorageId;

        /// <summary>
        /// Name of folder
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string Name;

        /// <summary>
        /// Next folder at same level or NULL if no more
        /// </summary>
        internal nint Sibling;

        /// <summary>
        /// Child folder or NULL if no children
        /// </summary>
        internal nint Child;
    }
}