namespace Omaha.Update.Enums
{
    public enum BasePathKey : long
    {
        PathStart = 0,

        DirCurrent,       // Current directory.
        DirExe,           // Directory containing FILE_EXE.
        DirModule,        // Directory containing FILE_MODULE.
        DirTemp,          // Temporary directory.
        DirHome,          // User's root home directory. On Windows this will look
                           // like "C:\Users\<user>"  which isn't necessarily a great
                           // place to put files.
        FileExe,          // Path and filename of the current executable.
        FileModule,       // Path and filename of the module containing the code for
                           // the PathService (which could differ from FILE_EXE if the
                           // PathService were compiled into a shared object, for
                           // example).
        DirSourceRoot,   // Returns the root of the source tree. This key is useful
                           // for tests that need to locate various resources. It
                           // should not be used outside of test code.
        DirUserDesktop,  // The current user's Desktop.

        DirTestData,     // Used only for testing.

        PathEnd,
        DirProgramFilesx86,
        DirProgramFiles
    }
}
