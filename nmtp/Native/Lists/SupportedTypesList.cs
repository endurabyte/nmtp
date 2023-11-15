using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nmtp.Native.Lists
{
    internal class SupportedTypesList : IEnumerable<FileType>, IDisposable
    {
        private readonly nint _listPointer;
        private readonly ushort _lengthOfList;

        public SupportedTypesList(nint mptDeviceStructPointer)
        {
            var error = Native.LibMtp.GetSupportedFiletypes(mptDeviceStructPointer, ref _listPointer,
                ref _lengthOfList);
            if (error != 0)
                throw new ApplicationException("Error while retrieving supported types");
        }

        public IEnumerator<FileType> GetEnumerator()
        {
            for (int i = 0; i < _lengthOfList; i++)
            {
                nint offset = _listPointer + i * Marshal.SizeOf(typeof(ushort));
                var deviceObject = Marshal.PtrToStructure(offset, typeof(ushort));
                yield return (FileType)deviceObject!;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void ReleaseUnmanagedResources()
        {
            Native.LibMtp.Free(_listPointer);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~SupportedTypesList()
        {
            ReleaseUnmanagedResources();
        }
    }
}