using ProtoBuf;

namespace Omaha.Feedback.Proto.Chrome
{
    [ProtoContract]
    public class ChromeOsData
    {
        [ProtoMember(1, IsRequired=false)]
        public ChromeOsCategory Category { get; set; } //default=OTHER
    }
}
