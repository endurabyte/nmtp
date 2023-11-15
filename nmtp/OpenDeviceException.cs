using System;

namespace Nmtp
{
    public class OpenDeviceException : ApplicationException
    {
        public OpenDeviceException(RawDevice rawDevice)
            : base($"Failed to open {rawDevice}") { }
    }
}