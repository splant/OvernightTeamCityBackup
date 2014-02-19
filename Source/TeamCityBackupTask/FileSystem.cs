using System.Collections.Generic;

namespace TeamCityBackupTask
{
    public interface FileSystem 
    {
        IEnumerable<string> GetFileNames(string directory);
        void CutFile(string sourceFile, string destinationFile);
        void RemoveFile(string fileName);
    }
}