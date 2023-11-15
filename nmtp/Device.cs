using System.Runtime.InteropServices;
using Nmtp.Native.Structs;
using Nmtp.Native.Lists;
using Nmtp.Native;

namespace Nmtp;

public class Device : IDisposable
{
    private MtpDevice _Device => Marshal.PtrToStructure<MtpDevice>(_device);

    private IntPtr _device;
    private bool _cached;

    public bool TryOpen(ref RawDevice rawDevice, bool cached)
    {
        _cached = cached;

        IntPtr ptr = cached
            ? LibMtp.OpenRawDevice(ref rawDevice)
            : LibMtp.OpenRawDeviceUncached(ref rawDevice);

        _device = ptr;
        return ptr != IntPtr.Zero;
    }

    public string? GetManufacturerName() => LibMtp.GetManufacturerName(_device);
    public string? GetModelName() => LibMtp.GetModelName(_device);
    public string? GetSerialNumber() => LibMtp.GetSerialNumber(_device);
    public string? GetDeviceVersion() => LibMtp.GetDeviceVersion(_device);
    public string? GetFriendlyName() => LibMtp.GetFriendlyName(_device);

    public bool SetFriendlyName(string name) => 0 == LibMtp.SetFriendlyName(_device, name);

    public IEnumerable<FileType> GetSupportedTypes()
    {
        using var fileTypeList = new SupportedTypesList(_device);
        foreach (var fileType in fileTypeList)
            yield return fileType;
    }

    public float GetBatteryLevel(bool throwException)
    {
        byte maxBattery = 255, currentBattery = 0;
        var batterLevel = LibMtp.GetBatteryLevel(_device, ref maxBattery, ref currentBattery);
        if (batterLevel != 0)
            return throwException ? throw new ApplicationException() : 0.0f;
        return 100.0f * currentBattery / maxBattery;
    }

    public bool PopulateStorages() => LibMtp.PopulateStorages(_device) == 0;

    public IEnumerable<DeviceStorage> GetStorages()
    {
        var deviceStorage = Marshal.PtrToStructure<DeviceStorage>(_Device.storage);
        yield return deviceStorage;
        while (deviceStorage.next != IntPtr.Zero)
        {
            deviceStorage = Marshal.PtrToStructure<DeviceStorage>(deviceStorage.next);
            yield return deviceStorage;
        }
    }

    public IEnumerable<File> GetFolderContent(uint storageId, uint? folderId)
    {
        if (_cached)
            throw new ApplicationException(
                "GetFolderContent cannot be called on cached device. Open device with cached: false");
        using var fileList = new FileAndFolderList(_device, storageId,
                   folderId ?? LibMtp.LibmtpFilesAndFoldersRoot);
        foreach (var file in fileList)
            yield return file;
    }

    public IEnumerable<File> GetFiles(Func<double, bool>? progressCallback)
    {
        using var fileList = new FileList(_device, DeviceFunction.GetProgress(progressCallback));
        foreach (var file in fileList)
        {
            yield return file;
        }
    }

    public IEnumerable<Album> GetAlbumList()
    {
        using var albumList = new AlbumList(_device);
        foreach (var album in albumList)
            yield return new Album(album);
    }

    public IEnumerable<Track> GetTrackList(Func<double, bool>? progressCallback)
    {
        using var trackList = new TrackList(_device, DeviceFunction.GetProgress(progressCallback));
        foreach (var track in trackList)
            yield return track;
    }

    public FileSampleData GetFileSampleDataForObject(uint objectId)
    {
        return new FileSampleData(_device, objectId);
    }

    public void SendRepresentativeDataForObject(uint objectId, ref FileSampleData dataStructStruct)
    {
        dataStructStruct.SendDataToDevice(_device, objectId);
    }

    private void ReleaseUnmanagedResources()
    {
        if (_device != IntPtr.Zero)
            LibMtp.ReleaseDevice(_device);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Device()
    {
        ReleaseUnmanagedResources();
    }

    /// <summary>
    /// Sends the track with passed metadata to the device
    /// </summary>
    /// <param name="track">Track metadata structure to send to device</param>
    /// <param name="dataProvider">Requests data. If data is null - operation considered to be cancelled</param>
    /// <param name="progressCallback">Reports a progress and returns boolean indication if the operation was cancelled</param>
    /// <exception cref="Exception"></exception>
    public void SendTrack(ref Track track, Func<int, IList<byte>> dataProvider,
        Func<double, bool>? progressCallback)
    {
        var result = LibMtp.SendTrackFromHandler(_device, DeviceFunction.GetData(dataProvider),
            ref track, DeviceFunction.GetProgress(progressCallback));
        if (result != 0)
            throw new ApplicationException("Sending file failed");
    }

    /// <summary>
    /// Gets file with specified id to the file on specified path
    /// </summary>
    /// <param name="fileId">id of the file to retrieve</param>
    /// <param name="filePath">file path where to write</param>
    /// <param name="progressCallback">callback for progress reporting</param>
    /// <exception cref="ApplicationException">throws exception if getting the file failed</exception>
    public bool GetFile(uint fileId, string filePath, Func<double, bool>? progressCallback)
    {
        using Stream file = System.IO.File.Create(filePath);
        return GetFile(fileId, progressCallback, file);
    }

    public bool GetFile(uint fileId, Func<double, bool>? progressCallback, Stream file)
    {
        return 0 == LibMtp.GetFileToHandler(_device, fileId, DeviceFunction.PutData(
            d =>
            {
                file.Write(d);
                return false;
            }), DeviceFunction.GetProgress(progressCallback));
    }

    public void CreateAlbum(ref Album albumStruct)
    {
        albumStruct.SendAlbum(_device, true);
    }

    public void UpdateAlbum(ref Album albumStruct)
    {
        albumStruct.SendAlbum(_device, false);
    }

    public uint CreateFolder(string name, uint parentFolderId, uint parentStorageId)
    {
        return LibMtp.CreateFolder(_device, name, parentFolderId, parentStorageId);
    }

    public IEnumerable<Folder> GetFolderList(uint? storageId = null)
    {
        using var folderList = new FolderList(_device, storageId);
        foreach (var folder in folderList)
        {
            yield return folder;
        }
    }

    public void SendFirmwareFile(FileInfo fileInfo, Func<double, bool>? progressCallback)
    {
        var firmwareFile = new File
        {
            FileSize = (ulong)fileInfo.Length,
            FileName = fileInfo.Name,
            Filetype = FileType.Firmware,
            ParentId = 0,
            StorageId = 0
        };

        LibMtp.SendFile(_device, fileInfo.FullName, ref firmwareFile,
            DeviceFunction.GetProgress(progressCallback), IntPtr.Zero);
    }

    public void DeleteObject(uint objectId)
    {
        if (0 != LibMtp.DeleteObject(_device, objectId))
            throw new ApplicationException($"Failed to delete the object with it {objectId}");
    }
}