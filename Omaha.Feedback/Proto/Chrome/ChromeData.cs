using ProtoBuf;

namespace Omaha.Feedback.Proto.Chrome
{
    [ProtoContract]
    // Chrome Browser and Chrome OS specific data.
    public class ChromeData
    {
        [ProtoMember(1, IsRequired=false)]
        // What platform has a report been sent from.
        public ChromePlatform ChromePlatform { get; set; } //default=CHROME_OS

        [ProtoMember(2, IsRequired=false)]
        public ChromeOsData ChromeOsData { get; set; }

        [ProtoMember(3, IsRequired=false)]
        public ChromeBrowserData ChromeBrowserData { get; set; }
    }
}
