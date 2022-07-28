using System;

namespace WinCopies.IO
{
    public struct BrowsableObjectInfoURL : IEquatable<BrowsableObjectInfoURL>
    {
        public string Path { get; }

        public string URI { get; }

        public BrowsableObjectInfoURL(in string path, in string uri)
        {
            Path = path;

            URI = uri;
        }

        public BrowsableObjectInfoURL(in string path) => this = new
#if !CS9
            BrowsableObjectInfoURL
#endif
            (path, path);

        public bool Equals(BrowsableObjectInfoURL other) => other.URI == URI;

        public static bool operator ==(BrowsableObjectInfoURL x, BrowsableObjectInfoURL y) => x.Equals(y);

        public static bool operator !=(BrowsableObjectInfoURL x, BrowsableObjectInfoURL y) => !(x == y);
    }

    public struct BrowsableObjectInfoURL2 : IEquatable<BrowsableObjectInfoURL2>
    {
        public BrowsableObjectInfoURL URL { get; }

        public string Protocol { get; }

        public BrowsableObjectInfoURL2(in BrowsableObjectInfoURL url, in string protocol)
        {
            URL = url;

            Protocol = protocol;
        }

        public BrowsableObjectInfoURL2(in string path)
        {
            int index = path.IndexOf("://");

            this = index >= 0 ? new BrowsableObjectInfoURL2(new BrowsableObjectInfoURL(path.Substring(index + 3)), path.Substring(0, index)) : new BrowsableObjectInfoURL2(new BrowsableObjectInfoURL(path), "file");
        }

        public bool Equals(BrowsableObjectInfoURL2 other) => other.Protocol == Protocol && other.URL == URL;

        public static bool operator ==(BrowsableObjectInfoURL2 x, BrowsableObjectInfoURL2 y) => x.Equals(y);

        public static bool operator !=(BrowsableObjectInfoURL2 x, BrowsableObjectInfoURL2 y) => !(x == y);
    }

    public struct BrowsableObjectInfoURL3
    {
        public BrowsableObjectInfoURL2 URL { get; }

        public ClientVersion ClientVersion { get; }

        public BrowsableObjectInfoURL3(in BrowsableObjectInfoURL2 url, in ClientVersion clientVersion)
        {
            URL = url;

            ClientVersion = clientVersion;
        }
    }
}
