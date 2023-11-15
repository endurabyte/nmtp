﻿using System;
using System.Runtime.InteropServices;

namespace Nmtp.Structs;

public struct AlbumStruct
{
    public uint AlbumId { get; set; } = 0;
    public uint ParentId { get; set; } = 0;
    public uint StorageId { get; set; } = 0;
    public string? Name { get; set; } = null;
    public string? Artist { get; set; } = null;
    public string? Composer { get; set; } = null;
    public string? Genre { get; set; } = null;
    public uint[] Tracks { get; set; } = null!;

    internal AlbumStruct(AlbumNativeStruct albumNativeStruct) : this()
    {
        MapFromNative(ref albumNativeStruct);
    }

    private void MapFromNative(ref AlbumNativeStruct nativeStruct)
    {
        AlbumId = nativeStruct.album_id;
        ParentId = nativeStruct.parent_id;
        StorageId = nativeStruct.storage_id;
        Name = nativeStruct.name;
        Artist = nativeStruct.artist;
        Composer = nativeStruct.composer;
        Genre = nativeStruct.genre;
        Tracks = new uint[nativeStruct.no_tracks];
        for (int i = 0; i < nativeStruct.no_tracks; i++)
        {
            Tracks[i] = (uint)Marshal.ReadInt64(nativeStruct.tracks, i*sizeof(uint));
        }
    }

    internal void SendAlbum(IntPtr mptDeviceStructPointer, bool createNew)
    {
        GCHandle tracksHandle = GCHandle.Alloc(Tracks, GCHandleType.Pinned);
        try
        {
            IntPtr tracksPtr = tracksHandle.AddrOfPinnedObject();
            var albumNativeStruct = new AlbumNativeStruct()
            {
                album_id = AlbumId,
                parent_id = ParentId,
                storage_id = StorageId,
                name = Name,
                artist = Artist,
                composer = Composer,
                genre = Genre,
                no_tracks = (uint)Tracks.Length,
                tracks = tracksPtr
            };
            if (createNew)
                NativeAPI.LibMtpLibrary.CreateNewAlbum(mptDeviceStructPointer, ref albumNativeStruct);
            else
                NativeAPI.LibMtpLibrary.UpdateAlbum(mptDeviceStructPointer, ref albumNativeStruct);
            MapFromNative(ref albumNativeStruct);
        }
        finally
        {
            tracksHandle.Free();
        }
    }
}