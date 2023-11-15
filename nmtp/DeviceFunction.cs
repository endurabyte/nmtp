using System.Runtime.InteropServices;
using Nmtp.Native;

namespace Nmtp;

public static class DeviceFunction
{ 
    public static MtpDataGetFunction GetData(Func<int, IList<byte>?> getData)
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

    public static MtpDataPutFunction PutData(Func<byte[], bool> putData)
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

    public static ProgressFunction? GetProgress(Func<double, bool>? progressCallback)
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