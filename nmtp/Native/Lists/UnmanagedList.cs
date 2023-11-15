using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nmtp.Native.Lists
{
    internal abstract class UnmanagedList<T> : IEnumerable<T>, IDisposable
    {
        private readonly nint _listPtr;

        public UnmanagedList(nint listPtr)
        {
            _listPtr = listPtr;
        }

        protected abstract nint GetPointerToNextItem(ref T item);
        protected abstract void FreeItem(nint item);

        public IEnumerator<T> GetEnumerator()
        {
            var currentItem = _listPtr;
            while (currentItem != nint.Zero)
            {
                var currentItemStruct = Marshal.PtrToStructure<T>(currentItem)!;
                yield return currentItemStruct;
                currentItem = GetPointerToNextItem(ref currentItemStruct);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void ReleaseUnmanagedResources()
        {
            var currentItem = _listPtr;
            while (currentItem != nint.Zero)
            {
                var currentItemStruct = Marshal.PtrToStructure<T>(currentItem)!;
                FreeItem(currentItem);
                currentItem = GetPointerToNextItem(ref currentItemStruct);
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~UnmanagedList()
        {
            ReleaseUnmanagedResources();
        }
    }
}