namespace Omaha.Update.Exception
{
    public class UsingExternalUpdaterException : System.Exception
    {
        private const string ErrorMessage = "There is allready an external updater running. Multiple access to the com interface is not supported.";

        public UsingExternalUpdaterException()
            : base(ErrorMessage) { }

        public UsingExternalUpdaterException(System.Exception innerException)
            : base(ErrorMessage, innerException) { }
    }
}
