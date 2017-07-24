namespace Omaha.Feedback
{
    public class OmahaScreenshot
    {
        public InternetMedia Image { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public OmahaScreenshot()
        {
            Height = 0;
            Width = 0;
        }
    }
}
