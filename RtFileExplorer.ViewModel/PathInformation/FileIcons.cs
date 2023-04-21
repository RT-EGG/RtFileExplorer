using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Utility;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    internal class FileIcons
    {
        public enum IconSize : uint
        {
            // 16x16
            Small = SHIL_SMALL,
            // 48x48
            Large = SHIL_LARGE,
            // 48x48
            ExtraLarge = SHIL_EXTRALARGE,
            // GetSystemMetrics(SM_CYSMICON)
            SysSmall = SHIL_SYSSMALL,
            // 256x256
            Jumbo = SHIL_JUMBO
        }

        private FileIcons()
        { }

        public static FileIcons Instance { get; } = new FileIcons();

        public async Task<BitmapSource?> GetAssociatedIcon(string inPath, CancellationToken? inCancellationToken = null)
            => await GetAssociatedIcon(inPath, IconSize.Small, inCancellationToken);

        public async Task<BitmapSource?> GetAssociatedIcon(string inPath, IconSize inSize, CancellationToken? inCancellationToken = null)
        {
            using (await _mutex.LockAsync())
            {
                if (_uniqueIcons.TryGetValue(inPath, out var result))
                    return result;

                result = null;
                var iconIndex = GetFileIconIndex(inPath);
                if (iconIndex != 0)
                {
                    // IconIndexが0である場合、既定のアイコンでない場合がある
                    // => 決まったアイコンでない場合があるので、毎回ファイルから読み出す
                    var key = new IconKey(iconIndex, inSize);
                    if (_icons.TryGetValue(key, out result))
                        return result;
                }

                await TargetApplicationBinder.Instance!.Application!.UiDispatcher.InvokeAsync(() =>
                {
                    result = CreateAssociateIconBitmap(inPath, inSize);
                },
                    System.Windows.Threading.DispatcherPriority.Background,
                    inCancellationToken ?? CancellationToken.None
                );

                return result;
            }
        }

        private BitmapSource CreateAssociateIconBitmap(string inPath, IconSize inSize)
        {
            if (!(File.Exists(inPath) || System.IO.Directory.Exists(inPath)))
                throw new FileNotFoundException();

            if (_uniqueIcons.TryGetValue(inPath, out var result))
                return result;

            var iconIndex = GetFileIconIndex(inPath);
            if (iconIndex == 0)
            {
                var icon = Icon.ExtractAssociatedIcon(inPath);
                if (icon is null)
                    throw new Exception($"Failed to extract associate icon from \"{inPath}\".");

                try
                {
                    var bitmap = icon.ToBitmap();

                    var size = GetIconDimension(inSize);
                    result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            icon.ToBitmap().GetHbitmap(),
                            IntPtr.Zero, new Int32Rect(0, 0, (int)(size.Width + 0.5), (int)(size.Height + 0.5)),
                            BitmapSizeOptions.FromEmptyOptions()
                        );
                }
                finally
                {
                    icon.Dispose();
                }

                _uniqueIcons.Add(inPath, result);
            }
            else
            {
                IntPtr source = IntPtr.Zero;
                IntPtr imageList = IntPtr.Zero;
                Icon? icon = null;
                try
                {
                    SHGetImageList((int)inSize, IID_IImageList, out imageList);
                    source = ImageList_GetIcon(imageList, iconIndex, 0);

                    if (source == IntPtr.Zero)
                        throw new Exception($"Failed to extract associate icon from \"{inPath}\".");

                    icon = Icon.FromHandle(source);
                    var bitmap = Icon.FromHandle(source).ToBitmap();


                    result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bitmap.GetHbitmap(),
                        IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
                    );
                    _icons.Add(new IconKey(iconIndex, inSize), result);
                }
                finally
                {
                    icon?.Dispose();
                    if (source != IntPtr.Zero)
                        DestroyIcon(source);
                    if (imageList != IntPtr.Zero)
                        ImageList_Destroy(imageList);
                }
            }
            return result;
        }

        private SHFILEINFO GetFileInfo(string inPath)
        {
            var attribute = File.Exists(inPath)
                ? FILE_ATTRIBUTE_NORMAL
                : System.IO.Directory.Exists(inPath)
                    ? FILE_ATTRIBUTE_DIRECTORY
                    : throw new FileNotFoundException(inPath);

            SHFILEINFO result = new SHFILEINFO();
            uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
            if (SHGetFileInfo(inPath, attribute, ref result, (uint)Marshal.SizeOf(result), flags) == IntPtr.Zero)
                throw new Exception("Failed to retrieve file information.");

            return result;
        }

        private int GetFileIconIndex(string inFilepath)
        {
            var info = GetFileInfo(inFilepath);
            try
            {
                return info.iIcon;
            }
            finally
            {
                if (info.hIcon != IntPtr.Zero)
                    DestroyIcon(info.hIcon);
            }
        }

        private System.Windows.Size GetIconDimension(IconSize inSize)
        {
            switch (inSize)
            {
                case IconSize.Small:
                    return new System.Windows.Size(16, 16);
                case IconSize.Large:
                case IconSize.ExtraLarge:
                    return new System.Windows.Size(48, 48);
                case IconSize.SysSmall:
                    const int SM_CYSMICON = 50;
                    var size = GetSystemMetrics(SM_CYSMICON);
                    return new System.Windows.Size(size, size);
                case IconSize.Jumbo:
                    return new System.Windows.Size(256, 256);
            }
            throw new Exception();
        }

        private Dictionary<IconKey, BitmapSource> _icons = new Dictionary<IconKey, BitmapSource>();
        private Dictionary<string, BitmapSource> _uniqueIcons = new Dictionary<string, BitmapSource>();
        private Dictionary<IconSize, BitmapSource> _directoryIcons = new Dictionary<IconSize, BitmapSource>();
        private AsyncLocker _mutex = new AsyncLocker();

        private class IconKey : IEquatable<IconKey>
        {
            public IconKey(int inIconIndex, IconSize inSize)
            {
                IconIndex = inIconIndex;
                IconSize = inSize;
            }

            public override int GetHashCode()
                => new
                {
                    IconIndex,
                    IconSize
                }.GetHashCode();

            public override bool Equals(object? obj)
                => obj is IconKey && Equals((IconKey)obj);

            public bool Equals(IconKey? other)
                => other is not null
                && IconIndex == other.IconIndex
                && IconSize == other.IconSize;

            private readonly int IconIndex;
            private readonly IconSize IconSize;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, string lpIconPath, out ushort lpiIcon);
        [DllImport("shell32.dll")]
        static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        [DllImport("Shell32.dll", PreserveSig = false)]
        static extern void SHGetImageList(int iImageList, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);
        [DllImport("Comctl32.dll")]
        static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, int flags);
        [DllImport("Comctl32.dll")]
        static extern bool ImageList_Destroy(IntPtr himl);
        [DllImport("User32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr hIcon);
        [DllImport("User32.dll")]
        static extern int GetSystemMetrics(int nIndex);

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_LARGEICON = 0x000000000;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        private const uint SHGFI_ICONLOCATION = 0x000001000;

        private const uint SHIL_LARGE = 0x0;
        private const uint SHIL_SMALL = 0x1;
        private const uint SHIL_EXTRALARGE = 0x2;
        private const uint SHIL_SYSSMALL = 0x3;
        private const uint SHIL_JUMBO = 0x4;

        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

        static readonly Guid IID_IImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
    }
}
