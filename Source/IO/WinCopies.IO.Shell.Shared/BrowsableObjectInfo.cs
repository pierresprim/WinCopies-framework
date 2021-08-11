using System;
using System.Drawing;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

using WinCopies.GUI.Drawing;
using WinCopies.IO.ObjectModel;
using WinCopies.IO.ObjectModel.Reflection;

using static WinCopies.IO.Shell.ObjectModel.BrowsableObjectInfo;

namespace WinCopies.IO.Shell.ObjectModel
{
    public class BrowsableObjectInfoIconBitmapSources : BrowsableObjectInfoBitmapSources<Icon>
    {
        protected override BitmapSource SmallBitmapSourceOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource MediumBitmapSourceOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource LargeBitmapSourceOverride => TryGetBitmapSource(InnerObject);

        protected override BitmapSource ExtraLargeBitmapSourceOverride => TryGetBitmapSource(InnerObject);

        public BrowsableObjectInfoIconBitmapSources(in Icon icon) : base(icon) { /* Left empty. */ }
    }

    public static class BrowsableObjectInfo
    {
        private static void EmptyVoid() { /* Left empty. */ }

        public static Action RegisterDefaultBrowsabilityPaths { get; private set; } = () =>
        {
            ShellObjectInfo.BrowsabilityPathStack.Push(new MultiIconInfo.BrowsabilityPath());
            ShellObjectInfo.BrowsabilityPathStack.Push(new DotNetAssemblyInfo.BrowsabilityPath());

            RegisterDefaultBrowsabilityPaths = EmptyVoid;
        };

        public static Action RegisterDefaultProcessSelectors { get; private set; } = () =>
        {
            ShellObjectInfo.RegisterProcessSelectors();

            RegisterDefaultProcessSelectors = EmptyVoid;
        };

        public static Icon TryGetIcon(in Icon[] icons, in ushort size) => TryGetIcon(icons, new System.Drawing.Size(size, size));

        public static Icon TryGetIcon(in Icon icon, in ushort size) => TryGetIcon(icon, new System.Drawing.Size(size, size));

        public static Icon TryGetIcon(in Icon[] icons, in System.Drawing.Size size) => icons?.TryGetIcon(size, 32, true, true);

        public static Icon TryGetIcon(in Icon icon, in System.Drawing.Size size) => TryGetIcon(icon?.Split(), size);

        public static Icon TryGetIcon(in int iconIndex, in string dll, in System.Drawing.Size size) => TryGetIcon(new IconExtractor(WinCopies.IO.Path.GetRealPathFromEnvironmentVariables(IO.Path.System32Path + dll)).GetIcon(iconIndex), size);

        public static BitmapSource TryGetBitmapSource(in int iconIndex, in string dllName, in int size)
        {
            using
#if !CS8
            (
#endif
                Icon icon = TryGetIcon(iconIndex, dllName, new System.Drawing.Size(size, size))
#if CS8
            ;
#else
            )
#endif

            return TryGetBitmapSource(icon);
        }

        public static BitmapSource TryGetBitmapSource(in Icon icon) => icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }
}
