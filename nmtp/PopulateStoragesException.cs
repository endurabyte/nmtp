using System;

namespace Nmtp
{
    public class PopulateStoragesException : ApplicationException
    {
        public Device Device { get; }

        public PopulateStoragesException(Device device)
            : base($"Failed to populate storages for {device}")
        {
            Device = device;
        }
    }
}