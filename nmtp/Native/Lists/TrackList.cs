using System;
using Nmtp.Native;

namespace Nmtp.Native.Lists
{
    internal class TrackList : UnmanagedList<Track>
    {
        public TrackList(nint mptDeviceStructPointer, ProgressFunction? progressCallback)
            : base(LibMtp.GetTracks(mptDeviceStructPointer, progressCallback))
        {
        }

        protected override nint GetPointerToNextItem(ref Track item)
        {
            return item.next;
        }

        protected override void FreeItem(nint item)
        {
            LibMtp.FreeTrack(item);
        }
    }
}