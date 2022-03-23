namespace WinCopies.IO.Shell
{
    public static class Extensions
    {
        public static Microsoft.WindowsAPICodePack.PortableDevices.ClientVersion ToPortableDeviceClientVersion(this ClientVersion clientVersion) => new Microsoft.WindowsAPICodePack.PortableDevices.ClientVersion(clientVersion.ClientName, clientVersion.MajorVersion, clientVersion.MinorVersion, clientVersion.Revision);

        public static bool IsFolder(this FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Folder:
                case FileType.KnownFolder:
                case FileType.Drive:
                case FileType.Library:

                    return true;
            }

            return false;
        }

        public static bool IsFile(this FileType fileType)
        {
            switch (fileType)
            {
                case FileType.File:
                case FileType.Archive:

                    return true;
            }

            return false;
        }
    }
}
