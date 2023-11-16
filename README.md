# nmtp

![nmtp logo](./logo.png "nmtp logo")

nmtp is a fork of [LibMtpSharp](https://github.com/shaosss/LibMtpSharp) and a wrapper of [libmtp](https://github.com/libmtp/libmtp) with a few [changes](https://github.com/endurabyte/libmtp).

## Use

To list available MTP devices, create a `Nmtp.RawDeviceList`.
For an object-oriented API, create a `Nmtp.Device` and use its instance methods.
If you need a low-level API, use `Nmtp.Native.LibMtp`. The methods are mostly 1-1 with the C API. See the source or libmtp documentation for details.

**Don't forget to dispose!**

## Platforms

The nuget packages includes native platform support for the following target frameworks:

- `win-x64`
- `linux-x64`
- `osx-x64`
- `osx-arm64`

## Example

```c#
using var list = new Nmtp.RawDeviceList();
foreach(Nmtp.RawDevice device in list)
{
  Console.WriteLine(device);
}

var rawDevice = list.First();
using var device = new Nmtp.Device()
bool isOpen = device.TryOpen(ref rawDevice, cached: true);
if (!isOpen) { return; }

IEnumerable<Nmtp.Folder> folders = device.GetFolderList(storage.Id);
var cameraDir = folders.FirstOrDefault(folder => folder.Name == "Camera"); // e.g. Android Camera directory
if (cameraDir.FolderId <= 0) { return; }

// Get all PNG files from the last 7 days
List<Nmtp.File> files = device
    .GetFiles(progress =>
    {
      Log.Info($"List files progress: {progress * 100:##.#}%");
      return true; // false: cancel, true: continue
    })
    .Where(file => file.ParentId == cameraDir.FolderId)
    .Where(file => file.FileName.EndsWith(".png"))
    .Where(file => DateTime.UnixEpoch + TimeSpan.FromSeconds(file.ModificationDate) > DateTime.UtcNow - TimeSpan.FromDays(7))
    .ToList();

List<byte[]> data = files
    .Select((Nmtp.File file) =>
    {
      using var ms = new MemoryStream();
      bool ok = device.GetFile(file.ItemId, progress =>
      {
        Log.Info($"Download progress {file.FileName} {progress * 100:##.#}%");
        return false; // false: continue, true: cancel
      }, ms);

      return ms.ToArray();
    })
    .ToList();
```

## Logo

Logo icon generated by DALL-E 3.
