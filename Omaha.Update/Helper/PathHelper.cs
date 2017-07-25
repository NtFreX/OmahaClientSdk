using System.IO;
using System.Reflection;
using Omaha.Update.Enums;

namespace Omaha.Update.Helper
{
    public class PathHelper
    {
        public static bool Get(BasePathKey pathKey, out string path)
        {
            switch (pathKey)
            {
                case BasePathKey.DirCurrent:
                    path = Directory.GetCurrentDirectory();
                    return true;
                case BasePathKey.DirExe:
                    path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    return true;
                default:
                    // TODO: implement
                    path = string.Empty;
                    return false;
            }
        }
    }
}
