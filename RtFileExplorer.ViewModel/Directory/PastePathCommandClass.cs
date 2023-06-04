using RtFileExplorer.ViewModel.Wpf.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility;

namespace RtFileExplorer.ViewModel.Wpf.Directory
{
    using IODirectory = System.IO.Directory;

    public partial class DirectoryViewModel
    {
        private class PathPaster
        {
            public async Task Copy(string inDestination, IEnumerable<string> inPathes)
            {
                var pathes = new List<MovedFile>(LookupPathes(inDestination, inPathes));

            }

            private IEnumerable<MovedFile> LookupPathes(string inDestination, IEnumerable<string> inPathes)
            {
                foreach (var path in inPathes)
                {
                    if (File.Exists(path))
                    {
                        yield return new MovedFile(
                            path,
                            GetMovedPath(inDestination, path)
                        );
                    }
                    else if (IODirectory.Exists(path))
                    {
                        var directory = GetMovedPath(inDestination, path);
                        foreach (var result in LookupPathes(directory, IODirectory.EnumerateFiles(path).Concat(IODirectory.EnumerateDirectories(path))))
                        {
                            yield return result;
                        }
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }

                yield break;
            }

            private class MovedFile
            {
                public MovedFile(string inSource, string inDestination)
                {
                    SourcePath = inSource;
                    DestinationPath = inDestination;

                    Overwritten = File.Exists(inDestination);
                }

                public readonly string SourcePath;
                public readonly string DestinationPath;
                public bool Overwritten { get; set; }
            }

            private static string GetMovedPath(string inDirectoryPath, string inSourcePath)
                => Path.Join(inDirectoryPath, Path.GetFileName(inSourcePath));
        }
        private async Task CopyPathesToDirectory(IEnumerable<string> inPathes)
        {
            // TODO show progress
            IDisposable? closer = null;
            try
            {
                var progress = new ProgressViewModel()
                {
                    Title = "ファイルのコピー",
                };

                closer = TargetApplicationBinder.Instance!.Application!.Dialogs.ShowProgressDialog(progress);
                await CopyPathesToDirectory__(inPathes, progress);

                await Task.Run(() =>
                {

                });
            }
            finally
            {
                closer?.Dispose();
            }
        }

        private async Task CopyPathesToDirectory__(IEnumerable<string> inPathes, ProgressViewModel inProgress)
        {
            foreach (var path in inPathes)
            {
                var newPath = string.Join(Path.DirectorySeparatorChar,
                    Directory, Path.GetFileName(path)
                ).EnsureFileSystemPath();

                if (File.Exists(path))
                {
                    if (File.Exists(newPath))
                    {
                        if (newPath == path.EnsureFileSystemPath())
                        {
                            // 移動先が同じフォルダである場合、末尾にコピーマークを付けて複製
                            const string CopyMark = " - Copy";
                            var extension = Path.GetExtension(path);
                            var newFileName = $"{Path.GetFileNameWithoutExtension(path)}{CopyMark}";

                            while (File.Exists(Path.Join(Directory, $"{newFileName}{extension}")))
                            {
                                newFileName = $"{newFileName}{CopyMark}";
                            }

                            newPath = Path.Join(Directory, $"{newFileName}{extension}");
                            File.Copy(path, newPath);
                        }
                        else
                        {
                            //TODO 上書き確認ダイアログを出し、承認されれば上書きする
                            if (true)
                            {
                                File.Copy(path, newPath, true);
                            }
                        }
                    }
                    else
                    {
                        File.Copy(path, newPath);
                    }
                }
                else if (System.IO.Directory.Exists(path))
                {
                    if (!System.IO.Directory.Exists(newPath))
                        System.IO.Directory.CreateDirectory(newPath);


                }
                else
                {
                    throw new ArgumentException(path, nameof(path));
                }
            }
        }

        private async Task CopyFileToToDirectory(string inSrc, string inDst)
        {
            if (!File.Exists(inSrc))
            {
                throw new FileNotFoundException(inSrc);
            }

            if (File.Exists(inDst))
            {
                if (inDst == inSrc.EnsureFileSystemPath())
                {
                    // 移動先が同じフォルダである場合、末尾にコピーマークを付けて複製
                    const string CopyMark = " - Copy";
                    var extension = Path.GetExtension(inSrc);
                    var newFileName = $"{Path.GetFileNameWithoutExtension(inSrc)}{CopyMark}";

                    while (File.Exists(Path.Join(Directory, $"{newFileName}{extension}")))
                    {
                        newFileName = $"{newFileName}{CopyMark}";
                    }

                    inDst = Path.Join(Directory, $"{newFileName}{extension}");
                    File.Copy(inSrc, inDst);
                }
                else
                {
                    //TODO 上書き確認ダイアログを出し、承認されれば上書きする
                    if (true)
                    {
                        File.Copy(inSrc, inDst, true);
                    }
                }
            }
            else
            {
                File.Copy(inSrc, inDst);
            }
        }

        private async Task MovePathesToDirectory(IEnumerable<string> inPathes)
        {
            await Task.Yield();

            foreach (var filepath in inPathes)
            {
                var newFilepath = string.Join(Path.DirectorySeparatorChar,
                    Directory, Path.GetFileName(filepath)
                ).EnsureFileSystemPath();

                if (File.Exists(newFilepath))
                {
                    if (filepath.EnsureFileSystemPath() == newFilepath)
                    {
                        // 移動先が同じフォルダである場合、移動の必要なし
                        continue;
                    }
                    else
                    {
                        // 上書き確認ダイアログを出し、承認されれば上書きする
                        if (true)
                        {
                            File.Move(filepath, newFilepath, true);
                        }
                    }
                }
                else
                {
                    File.Move(filepath, newFilepath);
                }
            }
        }

        class PastePathCommandClass : AsyncCommandBase<DirectoryViewModel>
        {
            public PastePathCommandClass(DirectoryViewModel inViewModel)
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => Clipboard.ContainsFileDropList();

            protected override async Task ExecuteAsync(object? parameter)
            {
                if (!Clipboard.ContainsFileDropList())
                    throw new InvalidProgramException();

                var data = Clipboard.GetDataObject();
                var files = (string[])data.GetData(DataFormats.FileDrop);
                var effect = GetPreferredDropEffect(data);

                switch (effect)
                {
                    case DragDropEffects.Copy | DragDropEffects.Link:
                        await ViewModel.CopyPathesToDirectory(files);
                        Clipboard.Clear();
                        break;

                    case DragDropEffects.Move:
                        await ViewModel.MovePathesToDirectory(files);
                        Clipboard.Clear();
                        break;
                }
            }

            private static DragDropEffects GetPreferredDropEffect(IDataObject inData)
            {
                if (inData is not null)
                {
                    if (inData.GetData("Preferred DropEffect") is MemoryStream stream)
                    {
                        var effect = (DragDropEffects)stream.ReadByte();

                        return effect;
                    }
                }

                return DragDropEffects.None;
            }
        }
    }
}
