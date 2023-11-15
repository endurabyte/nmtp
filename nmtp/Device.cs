using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Nmtp.Native.Structs;
using Nmtp.Native.Lists;
using Nmtp.Native;

namespace Nmtp
{
    public class Device : IDisposable
    {
        private readonly IntPtr _mptDeviceStructPointer;
        private readonly bool _cached;

        public Device(ref RawDevice rawDevice, bool cached)
        {
            _cached = cached;
            _mptDeviceStructPointer = _cached ? LibMtp.OpenRawDevice(ref rawDevice) 
                : LibMtp.OpenRawDeviceUncached(ref rawDevice);
            if (_mptDeviceStructPointer == IntPtr.Zero)
                throw new OpenDeviceException(rawDevice);
        }

        public string? GetManufacturerName()
        {
            return LibMtp.GetManufacturerName(_mptDeviceStructPointer);
        }

        public string? GetModelName()
        {
            return LibMtp.GetModelName(_mptDeviceStructPointer);
        }

        public string? GetSerialNumber()
        {
            return LibMtp.GetSerialNumber(_mptDeviceStructPointer);
        }

        public string? GetDeviceVersion()
        {
            return LibMtp.GetDeviceVersion(_mptDeviceStructPointer);
        }

        public string? GetFriendlyName()
        {
            return LibMtp.GetFriendlyName(_mptDeviceStructPointer);
        }
        
        public void SetFriendlyName(string name)
        {
            if (0 != LibMtp.SetFriendlyName(_mptDeviceStructPointer, name))
                throw new ApplicationException($"Failed to set '{name}' device name");
        }

        public IEnumerable<FileType> GetSupportedTypes()
        {
            using (var fileTypeList = new SupportedTypesList(_mptDeviceStructPointer))
            {
                foreach (var fileType in fileTypeList)
                    yield return fileType;
            }
        }

        public float GetBatteryLevel(bool throwException)
        {
            byte maxBattery = 255, currentBattery = 0; 
            var batterLevel = LibMtp.GetBatteryLevel(_mptDeviceStructPointer, ref maxBattery, ref currentBattery);
            if (batterLevel != 0)
                return throwException ? throw new ApplicationException() : 0.0f;
            return 100.0f * currentBattery / maxBattery;
        }

        private MtpDevice CurrentMtpDeviceStruct =>
            Marshal.PtrToStructure<MtpDevice>(_mptDeviceStructPointer);

        public void PopulateStorages()
        {
            if (LibMtp.PopulateStorages(_mptDeviceStructPointer) != 0)
                throw new PopulateStoragesException(this);
        }
        
        public IEnumerable<DeviceStorage> GetStorages()
        {
            var deviceStorage = Marshal.PtrToStructure<DeviceStorage>(CurrentMtpDeviceStruct.storage);
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
            using (var fileList = new FileAndFolderList(_mptDeviceStructPointer, storageId, 
                       folderId ?? LibMtp.LibmtpFilesAndFoldersRoot))
            {
                foreach (var file in fileList)
                    yield return file;
            }
        }

        public IEnumerable<File> GetFiles(Func<double, bool>? progressCallback)
        {
            using (var fileList = new FileList(_mptDeviceStructPointer, GetProgressFunction(progressCallback)))
            {
                foreach (var file in fileList)
                {
                    yield return file;
                }
            }
        }

        public IEnumerable<Album> GetAlbumList()
        {
            using (var albumList = new AlbumList(_mptDeviceStructPointer))
            {
                foreach (var album in albumList)
                    yield return new Album(album);
            }
        }
        
        public IEnumerable<Track> GetTrackList(Func<double, bool>? progressCallback)
        {
            using (var trackList = new TrackList(_mptDeviceStructPointer, GetProgressFunction(progressCallback)))
            {
                foreach (var track in trackList)
                    yield return track;
            }
        }

        public FileSampleData GetFileSampleDataForObject(uint objectId)
        {
            return new FileSampleData(_mptDeviceStructPointer, objectId);
        }

        public void SendRepresentativeDataForObject(uint objectId, ref FileSampleData dataStructStruct)
        {
            dataStructStruct.SendDataToDevice(_mptDeviceStructPointer, objectId);
        }

        private void ReleaseUnmanagedResources()
        {
            if (_mptDeviceStructPointer != IntPtr.Zero)
                LibMtp.ReleaseDevice(_mptDeviceStructPointer);
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
            var result = LibMtp.SendTrackFromHandler(_mptDeviceStructPointer, GetDataFunction(dataProvider), 
                ref track, GetProgressFunction(progressCallback));
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
        public void GetFile(uint fileId, string filePath, Func<double, bool>? progressCallback)
        {
            var file = System.IO.File.Create(filePath);
            var result = LibMtp.GetFileToHandler(_mptDeviceStructPointer, fileId, PutDataFunction(
                d =>
                {
                    file.Write(d);
                    return false;
                }), GetProgressFunction(progressCallback));
            file.Close();
            if (result != 0)
                throw new ApplicationException($"Getting file Id: {fileId} to {filePath} failed");
        }

        public void CreateAlbum(ref Album albumStruct)
        {
            albumStruct.SendAlbum(_mptDeviceStructPointer, true);
        }
        
        public void UpdateAlbum(ref Album albumStruct)
        {
            albumStruct.SendAlbum(_mptDeviceStructPointer, false);
        }

        public uint CreateFolder(string name, uint parentFolderId, uint parentStorageId)
        {
            var newFolderId = LibMtp.CreateFolder(_mptDeviceStructPointer, name, parentFolderId, parentStorageId);
            if (newFolderId == 0)
                throw new FolderCreationException(name, parentFolderId, parentStorageId);
            return newFolderId;
        }

        public IEnumerable<Folder> GetFolderList(uint? storageId = null)
        {
            using (var folderList = new FolderList(_mptDeviceStructPointer, storageId))
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
            LibMtp.SendFile(_mptDeviceStructPointer, fileInfo.FullName, ref firmwareFile,
                GetProgressFunction(progressCallback), IntPtr.Zero);
        }

        public void DeleteObject(uint objectId)
        {
            if (0 != LibMtp.DeleteObject(_mptDeviceStructPointer, objectId)) 
                throw new ApplicationException($"Failed to delete the object with it {objectId}");
        }

        private MtpDataGetFunction GetDataFunction(Func<int, IList<byte>?> getData)
        {
            return (IntPtr _, IntPtr _, uint wantlen, IntPtr data, out uint gotlen) =>
            {
                var whereToWrite = data;
                long leftToRead = wantlen;
                gotlen = 0;
                do
                {
                    var howMuchToRead = leftToRead > int.MaxValue ? int.MaxValue : (int)leftToRead;

                    var readBytes = getData(howMuchToRead);
                    if (readBytes == null)
                        return (ushort)HandlerReturn.Cancel;
                    for (int i = 0; i < readBytes.Count; i++)
                    {
                        Marshal.WriteByte(whereToWrite, i, readBytes[i]);
                    }

                    whereToWrite = IntPtr.Add(whereToWrite, readBytes.Count);
                    leftToRead -= readBytes.Count;
                    gotlen += (uint)readBytes.Count;
                } while (leftToRead != 0);

                return (ushort)HandlerReturn.Ok;
            };
        }
        
        private MtpDataPutFunction PutDataFunction(Func<byte[], bool> putData)
        {
            return (IntPtr _, IntPtr _, uint sendlen, IntPtr data, out uint gotlen) =>
            {
                gotlen = sendlen > Array.MaxLength ? (uint)Array.MaxLength : sendlen;
                var readBytes = new byte[gotlen];
                for (int i = 0; i < gotlen; i++)
                {
                    readBytes[i] = Marshal.ReadByte(data, i);
                }

                if (putData(readBytes))
                    return (ushort)HandlerReturn.Cancel;

                return (ushort)HandlerReturn.Ok;
            };
        }
        
        private ProgressFunction? GetProgressFunction(Func<double, bool>? progressCallback)
        {
            if (progressCallback == null)
                return null;
            return (sent, total, _) =>
            {
                double progress = (double)sent / total;
                var isCancelled = progressCallback(progress);
                return isCancelled ? 1 : 0;
            };
        }
    }
}