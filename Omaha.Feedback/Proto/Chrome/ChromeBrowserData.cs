using ProtoBuf;

namespace Omaha.Feedback.Proto.Chrome
{
    [ProtoContract]
    public class ChromeBrowserData
    {
        [ProtoMember(1, IsRequired=false)]
        public ChromeBrowserCategory Category { get; set; } //default=OTHER
    }
}
