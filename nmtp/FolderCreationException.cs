using System;

namespace Nmtp
{
    public class FolderCreationException : ApplicationException
    {
        public FolderCreationException(string name, uint parentFolderItemId, uint parentStorageId)
            : base($"Folder creation failed! Name {name}, parentId: {parentFolderItemId}, storageId: {parentStorageId}")
        { }
    }
}