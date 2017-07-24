using ProtoBuf;

namespace Omaha.Feedback.Proto.Config
{
    // Used to detect content relevant to particular type of feedback.
    [ProtoContract]
    public class MatcherData
    {
        [ProtoMember(1, IsRequired=true)]
        // XPATH expression to match against page.
        public string ContentMatcher { get; set; }
        [ProtoMember(2, IsRequired=true)]
        // Regexp matching page URL.
        public string UrlMatcher { get; set; }
        [ProtoMember(3, IsRequired=false)]
        // Approval by feedback admins
        public bool UrlMatcherApproved { get; set; } //default=true
    }
}
