# nmtp

nmtp is a fork of [LibMtpSharp](https://github.com/shaosss/LibMtpSharp). 
nmtp is a wrapper of [libmtp](https://github.com/libmtp/libmtp) with a few [changes](https://github.com/endurabyte/libmtp)

## Use

See the libmtp documentation at the link above.

## Examples

To list available MTP devices, create a `RawDeviceList`.

To connect to a device, create a `OpenedMtpDevice`. It contains the methods to communicate with device.

**Don't forget to dispose!**

```c#
using var list = new RawDeviceList();
foreach(var device in list)
{
  Console.WriteLine(device);
}

var rawDevice = list.FirstOrDefault();
if (list == null) { return; }
using var connectedDevice = new OpenedMtpDevice(ref rawDevice, false);
```

