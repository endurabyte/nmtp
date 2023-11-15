namespace Nmtp
{
    public static class FileTypeExtensions
    {
        /// <summary>
        /// Audio filetype test.
        /// For filetypes that can be either audio or video, use <see cref="IsAudioOrVideo"/>
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool IsAudio(this FileType fileType)
        {
            return fileType == FileType.Wav ||
                   fileType == FileType.Mp3 ||
                   fileType == FileType.Mp2 ||
                   fileType == FileType.Wma ||
                   fileType == FileType.Ogg ||
                   fileType == FileType.Flac ||
                   fileType == FileType.Aac ||
                   fileType == FileType.M4A ||
                   fileType == FileType.Audible ||
                   fileType == FileType.UndefinedAudio;
        }

        /// <summary>
        /// Video filetype test.
        /// For filetypes that can be either audio or video, use <see cref="IsAudioOrVideo"/>
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool IsVideo(this FileType fileType)
        {
            return fileType == FileType.Wmv ||
                   fileType == FileType.Avi ||
                   fileType == FileType.Mpeg ||
                   fileType == FileType.UndefinedVideo;
        }

        /// <summary>
        /// Audio and/or video filetype test.
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool IsAudioOrVideo(this FileType fileType)
        {
            return fileType == FileType.Mp4 ||
                   fileType == FileType.Asf ||
                   fileType == FileType.Qt;
        }

        /// <summary>
        /// Test if filetype is a track.
        /// Use this to determine if the File API or Track API should be used to upload or download an object.
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool IsTrack(this FileType fileType)
        {
            return fileType.IsAudio() || fileType.IsVideo() || fileType.IsAudioOrVideo();
        }

        /// <summary>
        /// Image filetype test
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool IsImage(this FileType fileType)
        {
            return fileType == FileType.Jpeg ||
                   fileType == FileType.Jfif ||
                   fileType == FileType.Tiff ||
                   fileType == FileType.Bmp ||
                   fileType == FileType.Gif ||
                   fileType == FileType.Pict ||
                   fileType == FileType.Png ||
                   fileType == FileType.Jp2 ||
                   fileType == FileType.Jpx ||
                   fileType == FileType.WindowsImageFormat;
        }

        /// <summary>
        /// Address book and Business card filetype test
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool IsAddressBook(this FileType fileType)
        {
            return fileType == FileType.VCard2 ||
                   fileType == FileType.VCard3;
        }

        /// <summary>
        /// Audio and/or video filetype test.
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool IsCalendar(this FileType fileType)
        {
            return fileType == FileType.VCalendar1 ||
                   fileType == FileType.VCalendar2;
        }
    }
}