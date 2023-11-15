using System;

namespace Nmtp.Native.Lists
{
    internal class FileAndFolderList : UnmanagedList<File>
    {
        public FileAndFolderList(nint mptDeviceStructPointer, uint storageId, uint parentId)
            : base(Native.LibMtp.GetParentContent(mptDeviceStructPointer, storageId, parentId))
        {
        }

        protected override nint GetPointerToNextItem(ref File item)
        {
            return item.next;
        }

        protected override void FreeItem(nint item)
        {
            Native.LibMtp.DestroyFile(item);
        }
    }
}