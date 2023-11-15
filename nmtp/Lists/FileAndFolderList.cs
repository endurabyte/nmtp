using System;
using Nmtp.Structs;

namespace Nmtp.Lists
{
    internal class FileAndFolderList : UnmanagedList<FileStruct>
    {
        public FileAndFolderList(IntPtr mptDeviceStructPointer, uint storageId, uint parentId)
            : base(NativeAPI.LibMtpLibrary.GetParentContent(mptDeviceStructPointer, storageId, parentId))
        {
        }

        protected override IntPtr GetPointerToNextItem(ref FileStruct item)
        {
            return item.next;
        }

        protected override void FreeItem(IntPtr item)
        {
            NativeAPI.LibMtpLibrary.DestroyFile(item);
        }
    }
}