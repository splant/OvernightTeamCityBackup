using System.Collections.Generic;
using System.IO;

namespace TeamCityBackupTask
{
    public class WindowsFileSystem : FileSystem 
    {
        public IEnumerable<string> GetFileNames(string directory)
        {
            return Directory.GetFiles(directory);
        }

        public void CutFile(string sourceFile, string destinationFile)
        {
            File.Move(sourceFile, destinationFile);
        }
    }
}