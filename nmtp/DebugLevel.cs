﻿namespace Nmtp;

public enum DebugLevel
{
    None = 0x00,
    Ptp = 0x01,
    Plst = 0x02,
    Usb = 0x04,
    Data = 0x08,
    All = 0xFF
}