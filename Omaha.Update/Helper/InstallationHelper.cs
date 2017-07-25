using System;

namespace Omaha.Update.Helper
{
    public class InstallationHelper
    {
        public static bool IsPerUserInstall(string executable)
        {
            if (IsInSpecialFolder(executable, Environment.SpecialFolder.ProgramFilesX86))
                return false;
            if (IsInSpecialFolder(executable, Environment.SpecialFolder.ProgramFiles))
                return false;
            return true;
        }

        private static bool IsInSpecialFolder(string executable, Environment.SpecialFolder folder)
            => executable.ToLower().StartsWith(Environment.GetFolderPath(folder).ToLower());
    }
}
