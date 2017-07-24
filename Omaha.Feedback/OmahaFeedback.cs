namespace Omaha.Feedback
{
    public class OmahaFeedback
    {
        public string Description { get; set; }
        public string Email { get; set; }
        public InternetMedia[] AdditionalFile { get; set; }
        public string SystemInfoJson { get; set; }
        public OmahaScreenshot Screenshot { get; set; }

        public OmahaFeedback()
        {
            Screenshot = new OmahaScreenshot();
            AdditionalFile = new InternetMedia[0];
        }
    }
}
