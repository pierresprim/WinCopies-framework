namespace WinCopies.IO.Shell
{
    public static class Extensions
    {
        public static Microsoft.WindowsAPICodePack.PortableDevices.ClientVersion ToPortableDeviceClientVersion(this ClientVersion clientVersion) => new Microsoft.WindowsAPICodePack.PortableDevices.ClientVersion(clientVersion.ClientName, clientVersion.MajorVersion, clientVersion.MinorVersion, clientVersion.Revision);
    }
}
