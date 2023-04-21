using System.Diagnostics;
using System.IO;

namespace RtFileExplorer.ViewModel.Wpf.PathInformation
{
    internal class FileExecutor
    {
        private FileExecutor()
        { }

        public static FileExecutor Instance
        { get; } = new FileExecutor();

        public void Execute(string inFilepath)
        {
            if (!File.Exists(inFilepath))
                throw new FileNotFoundException();

            Process process = new Process();
            process.StartInfo.UseShellExecute = true;

            process.StartInfo.FileName = inFilepath;

            process.Start();
        }
    }
}
