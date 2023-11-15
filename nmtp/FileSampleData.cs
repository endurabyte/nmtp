using System;
using System.Runtime.InteropServices;
using Nmtp.Native.Structs;
using Nmtp.Native;

namespace Nmtp
{
    public struct FileSampleData
    {
        public FileType FileType { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint Duration { get; set; }
        public byte[]? Data { get; set; }

        internal FileSampleData(nint devicePtr, uint objectId)
        {
            var fileSampleDataPtr = LibMtp.CreateFileSampleData();
            if (fileSampleDataPtr == nint.Zero)
                throw new ApplicationException("Fails to allocate FileSampleData in LibMtp native layer");
            try
            {
                if (0 != LibMtp.GetRepresentativeSample(devicePtr, objectId, fileSampleDataPtr))
                {
                    LibMtp.DumpErrorStack(devicePtr);
                    throw new ApplicationException($"Cannot retrieve representative sample for object id {objectId}");
                }
                var fileSampleDataStruct = Marshal.PtrToStructure<NativeFileSampleData>(fileSampleDataPtr);
                FileType = fileSampleDataStruct.filetype;
                Width = fileSampleDataStruct.width;
                Height = fileSampleDataStruct.height;
                Duration = fileSampleDataStruct.duration;
                Data = fileSampleDataStruct.size == 0 ? null : new byte[fileSampleDataStruct.size];
                var dataPtr = fileSampleDataStruct.data;
                for (ulong i = 0; i < fileSampleDataStruct.size; i++)
                {
                    var insideBlockOffset = (int)(i % int.MaxValue);
                    if (insideBlockOffset == 0 && i != 0)
                        dataPtr = nint.Add(dataPtr, int.MaxValue);
                    Data![i] = Marshal.ReadByte(dataPtr, insideBlockOffset);
                }
            }
            finally
            {
                LibMtp.FreeFileSampleData(fileSampleDataPtr);
            }
        }

        internal void SendDataToDevice(nint mptDeviceStructPointer, uint objectId)
        {
            GCHandle handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
            try
            {
                nint dataPtr = handle.AddrOfPinnedObject();
                var size = Data?.Length ?? 0;
                var fileSampleData = new NativeFileSampleData()
                {
                    filetype = FileType,
                    height = Height,
                    width = Width,
                    duration = Duration,
                    data = dataPtr,
                    size = (ulong)size
                };
                LibMtp.SendRepresentativeSample(mptDeviceStructPointer, objectId, ref fileSampleData);
            }
            finally
            {
                handle.Free();
            }
        }
    }
}