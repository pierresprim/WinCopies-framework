namespace WinCopies.Reflection.DotNetDocBuilder
{
    public struct PackageInfo
    {
        public string Header { get; }

        public int FrameworkId { get; }

        public string URL { get; }

        public PackageInfo(in string header, in int frameworkId, in string url)
        {
            Header = header;

            FrameworkId = frameworkId;

            URL = url;
        }
    }
}
