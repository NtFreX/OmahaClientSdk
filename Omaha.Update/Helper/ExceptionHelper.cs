using System.Runtime.InteropServices;
using Omaha.Update.Exceptions;

namespace Omaha.Update.Helper
{
    public static class ExceptionHelper
    {
        public static void ThrowAccordingException(int hresult)
        {
            if(hresult != 0) ThrowAccordingException(Marshal.GetExceptionForHR(hresult), string.Empty);
        }

        public static void ThrowAccordingException(System.Exception exception, string message)
        {
            if ((long)exception.HResult == OmahaConstants.GoopdateEAppUsingExternalUpdater)
                throw new UsingExternalUpdaterException(new System.Exception(message, exception));
            throw new System.Exception(message, exception);
        }
    }
}
