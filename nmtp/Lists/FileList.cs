using System;
using Nmtp.NativeAPI;
using Nmtp.Structs;

namespace Nmtp.Lists;

internal class FileList : UnmanagedList<FileStruct>
{
    public FileList(IntPtr mptDeviceStructPointer, ProgressFunction? progressCallback) 
        : base(LibMtpLibrary.GetFilelistingWithCallback(mptDeviceStructPointer, progressCallback))
    {
    }

    protected override IntPtr GetPointerToNextItem(ref FileStruct item)
    {
        return item.next;
    }

    protected override void FreeItem(IntPtr item)
    {
        LibMtpLibrary.DestroyFile(item);
    }
}