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
    public partial class DirectoryViewModel
    {
        private async Task CopyFilesToDirectory(IEnumerable<string> inFilepathes)
        {
            await Task.Yield();

            foreach (var filepath in inFilepathes)
            {
                var newFilepath = string.Join(Path.DirectorySeparatorChar,
                    Directory, Path.GetFileName(filepath)
                ).EnsureFileSystemPath();

                if (File.Exists(newFilepath))
                {
                    if (newFilepath == filepath.EnsureFileSystemPath())
                    {
                        // 移動先が同じフォルダである場合、末尾にコピーマークを付けて複製
                        const string CopyMark = " - Copy";
                        var extension = Path.GetExtension(filepath);
                        var newFileName = $"{Path.GetFileNameWithoutExtension(filepath)}{CopyMark}";

                        while (File.Exists(Path.Join(Directory, $"{newFileName}{extension}")))
                        {
                            newFileName = $"{newFileName}{CopyMark}";
                        }

                        newFilepath = Path.Join(Directory, $"{newFileName}{extension}");
                        File.Copy(filepath, newFilepath);
                    }
                    else
                    {
                        // 上書き確認ダイアログを出し、承認されれば上書きする
                        if (true)
                        {
                            File.Copy(filepath, newFilepath, true);
                        }
                    }
                }
                else
                {
                    File.Copy(filepath, newFilepath);
                }
            }
        }

        private async Task MoveFilesToDirectory(IEnumerable<string> inFilepathes)
        {
            await Task.Yield();

            foreach (var filepath in inFilepathes)
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
                        await ViewModel.CopyFilesToDirectory(files);
                        Clipboard.Clear();
                        break;

                    case DragDropEffects.Move:
                        await ViewModel.MoveFilesToDirectory(files);
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
