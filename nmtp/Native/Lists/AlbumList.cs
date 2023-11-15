using System;
using Nmtp.Native.Structs;

namespace Nmtp.Native.Lists
{
    internal class AlbumList : UnmanagedList<NativeAlbum>
    {
        public AlbumList(nint mptDeviceStructPointer)
            : base(Native.LibMtp.GetAlbums(mptDeviceStructPointer))
        {
        }

        protected override nint GetPointerToNextItem(ref NativeAlbum item)
        {
            return item.next;
        }

        protected override void FreeItem(nint item)
        {
            Native.LibMtp.FreeAlbum(item);
        }
    }
}