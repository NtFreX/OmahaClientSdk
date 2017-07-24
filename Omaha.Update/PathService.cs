using System.IO;
using System.Reflection;
using Omaha.Update.Enum;

namespace Omaha.Update
{
    public class PathService
    {
        public static bool Get(BasePathKey pathKey, out string path)
        {
            if (pathKey == BasePathKey.DirCurrent)
            {
                path = Directory.GetCurrentDirectory();
                return true;
            }
            if (pathKey == BasePathKey.DirExe)
            {
                path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return true;
            }

            path = string.Empty;
            return false;
        }
    }
}
