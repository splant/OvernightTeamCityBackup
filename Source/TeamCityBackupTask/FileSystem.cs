﻿using System.Collections.Generic;

namespace TeamCityBackupTask
{
    public interface FileSystem 
    {
        IEnumerable<string> GetFileNames(string directory);
    }
}