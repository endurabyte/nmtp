namespace Nmtp.Native;

/// <summary>
/// The callback type definition. Notice that a progress percentage ratio is easy to calculate by dividing
/// <code>sent</code> by <code>total</code>.
/// @param sent the number of bytes sent so far
/// @param total the total number of bytes to send
/// @param data a user-defined dereferencable pointer
/// @return if anything else than 0 is returned, the current transfer will be interrupted / cancelled.
/// </summary>
public delegate int ProgressFunction(ulong sent, ulong total, IntPtr data);

/// <summary>
/// Callback function for get by handler function
/// params the device parameters
/// @param priv a user-defined dereferencable pointer
/// @param wantlen the number of bytes wanted
/// @param data a buffer to write the data to
/// @param gotlen pointer to the number of bytes actually written to data
/// @return LIBMTP_HANDLER_RETURN_OK if successful,
/// LIBMTP_HANDLER_RETURN_ERROR on error or LIBMTP_HANDLER_RETURN_CANCEL to cancel the transfer
/// </summary>
public delegate ushort MtpDataGetFunction(IntPtr parameters, IntPtr priv,
    uint wantlen, IntPtr data, out uint gotlen);

/// <summary>
/// Callback function for put by handler function
/// @param params the device parameters
/// @param priv a user-defined dereferencable pointer
/// @param sendlen the number of bytes available
/// @param data a buffer to read the data from
/// @param putlen pointer to the number of bytes actually read from data
/// @return LIBMTP_HANDLER_RETURN_OK if successful,
/// LIBMTP_HANDLER_RETURN_ERROR on error or LIBMTP_HANDLER_RETURN_CANCEL to cancel the transfer
/// </summary>
public delegate ushort MtpDataPutFunction(IntPtr parameters, IntPtr priv,
    uint sendlen, IntPtr data, out uint putlen);
