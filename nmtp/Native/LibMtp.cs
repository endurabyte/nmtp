using System;
using System.Runtime.InteropServices;

namespace Nmtp.Native
{
    /// <summary>
    /// Raw libmtp wrapper.
    /// </summary>
    public partial class LibMtp
    {
        private const string LibMtpName = "libmtp";

        [DllImport(LibMtpName)]
        private static extern void LIBMTP_Init();
        [DllImport(LibMtpName)]
        private static extern void LIBMTP_Init_MTPZ([MarshalAs(UnmanagedType.LPUTF8Str)] string publicExponent, 
            [MarshalAs(UnmanagedType.LPUTF8Str)] string hexenckey, [MarshalAs(UnmanagedType.LPUTF8Str)] string modulus, 
            [MarshalAs(UnmanagedType.LPUTF8Str)] string privateKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string hexcerts);

        static LibMtp()
        {
            LIBMTP_Init();
            LIBMTP_Init_MTPZ(MTPZ.publicExponent, MTPZ.encryptionKeyHex, MTPZ.modulus, MTPZ.privateKey, MTPZ.certificateHex);
        }
        
        [DllImport(LibMtpName)]
        private static extern void LIBMTP_FreeMemory(IntPtr ptrToFree);
        
        public static void Free(IntPtr ptrToFree)
        {
            LIBMTP_FreeMemory(ptrToFree);
        }
        
        [DllImport(LibMtpName)]
        private static extern void LIBMTP_Set_Debug(int level);

        public static void SetDebug(DebugLevel level)
        {
            LIBMTP_Set_Debug((int)level);
        }
    }
}