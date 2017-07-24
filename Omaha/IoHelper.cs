using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omaha
{
    public class IoHelper
    {
        public static void CreateDirectoryIfNotExists(string directory)
        {
            var folders = directory.Split(Path.DirectorySeparatorChar);
            var currentPath = string.Empty;
            foreach (string folder in folders)
            {
                currentPath += folder + Path.DirectorySeparatorChar;
                if (!Directory.Exists(currentPath))
                    Directory.CreateDirectory(currentPath); //throws IOException; UnauthorizedAccessException; ArgumentException; PathTooLongException; DirectoryNotFoundException; NotSupportedException
            }
        }
    }
}
