﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nmtp
{
    public class RawDeviceList : IEnumerable<RawDevice>, IDisposable
    {
        private readonly int _numberOfDevices;
        private readonly IntPtr _deviceListPointer;
        
        public RawDeviceList()
        {
            var error = Native.LibMtp.DetectRawDevices(ref _deviceListPointer, ref _numberOfDevices);
            if (error == Error.NoDeviceAttached)
                return;
            if (error != Error.None)
                throw new DetectDeviceException(error);
        }

        public IEnumerator<RawDevice> GetEnumerator()
        {
            for (int i = 0; i < _numberOfDevices; i++)
            {
                IntPtr offset = _deviceListPointer + i * Marshal.SizeOf(typeof(RawDevice));
                var deviceObject = Marshal.PtrToStructure(offset, typeof(RawDevice));
                yield return (RawDevice) deviceObject!;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        private void ReleaseUnmanagedResources()
        {
            if (_deviceListPointer != IntPtr.Zero)
                Native.LibMtp.Free(_deviceListPointer);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~RawDeviceList()
        {
            ReleaseUnmanagedResources();
        }
    }
}