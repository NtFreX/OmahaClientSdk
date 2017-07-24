using ProtoBuf;

namespace Omaha.Feedback.Proto.Dom
{
    [ProtoContract]
    // Data captured from HTMLDocument DOM object.
    public class HtmlDocument
    {
        [ProtoMember(1, IsRequired=true)]
        // The value of document.URL property.
        public string Url { get; set; }

        [ProtoMember(2, IsRequired=false)]
        // The value of document.title property.
        public string Title { get; set; }

        [ProtoMember(3, IsRequired=false)]
        // The value of document.documentElement property.
        public HtmlElement DocumentElement { get; set; }
    }
}
