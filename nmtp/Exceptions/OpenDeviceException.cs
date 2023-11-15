using System;
using Nmtp.Structs;

namespace Nmtp.Exceptions
{
    public class OpenDeviceException : ApplicationException
    {
        public OpenDeviceException(RawDevice rawDevice)
            :base($"Failed to open {rawDevice}") { }
    }
}