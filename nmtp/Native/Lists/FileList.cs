using System;
using Nmtp.Native;

namespace Nmtp.Native.Lists;

internal class FileList : UnmanagedList<File>
{
    public FileList(nint mptDeviceStructPointer, ProgressFunction? progressCallback)
        : base(LibMtp.GetFilelistingWithCallback(mptDeviceStructPointer, progressCallback))
    {
    }

    protected override nint GetPointerToNextItem(ref File item)
    {
        return item.next;
    }

    protected override void FreeItem(nint item)
    {
        LibMtp.DestroyFile(item);
    }
}