using System;

namespace Nmtp;

public class DetectDeviceException : ApplicationException
{
    public DetectDeviceException(Error error)
        : base($"Device detect error: {error}")
    {
    }
}