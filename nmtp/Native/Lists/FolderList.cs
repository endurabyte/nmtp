using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Nmtp.Native;

namespace Nmtp.Native.Lists;

internal class FolderList : IEnumerable<Folder>, IDisposable
{
    private readonly nint _listPtr;

    public FolderList(nint mptDeviceStructPointer, uint? storageId = null)
    {
        _listPtr = storageId == null
            ? LibMtp.GetFolderList(mptDeviceStructPointer)
            : LibMtp.GetFolderListForStorage(mptDeviceStructPointer, storageId.Value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<Folder> GetEnumerator()
    {
        var currentItem = _listPtr;
        if (currentItem != nint.Zero)
        {
            foreach (var folder in EnumerateFolderTreeStartingFrom(_listPtr))
                yield return folder;
        }
    }

    private IEnumerable<Folder> EnumerateFolderTreeStartingFrom(nint folderPtr)
    {
        var folder = Marshal.PtrToStructure<Folder>(folderPtr);
        yield return folder;
        if (folder.Child != nint.Zero)
        {
            foreach (var childFolder in EnumerateFolderTreeStartingFrom(folder.Child))
                yield return childFolder;
        }

        if (folder.Sibling != nint.Zero)
        {
            foreach (var siblingFolder in EnumerateFolderTreeStartingFrom(folder.Sibling))
                yield return siblingFolder;
        }
    }

    private void ReleaseUnmanagedResources()
    {
        if (_listPtr != nint.Zero)
            LibMtp.DestroyFolder(_listPtr);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~FolderList()
    {
        ReleaseUnmanagedResources();
    }
}