using System;
using Nmtp.Enums;

namespace Nmtp.Exceptions;

public class DetectDeviceException : ApplicationException
{
    public DetectDeviceException(ErrorEnum error)
        : base($"Device detect error: {error}")
    {
    }
}