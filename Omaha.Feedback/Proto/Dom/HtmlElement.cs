using ProtoBuf;

namespace Omaha.Feedback.Proto.Dom
{
    [ProtoContract]
    // Data captured from HTMLElement DOM object.
    public class HtmlElement
    {
        [ProtoMember(1, IsRequired=true)]
        // The value of element.tagName property.
        public string TagName { get; set; }

        [ProtoMember(2, IsRequired=false)]
        // The value of element.id property.
        public string Id { get; set; }

        [ProtoMember(3, IsRequired=false)]
        // The value of element.className property.
        public string ClassName { get; set; }

        [ProtoMember(4, IsRequired=true)]
        // A list of child elements.
        public HtmlElement[] ChildElements { get; set; }

        [ProtoMember(5, IsRequired=false)]
        // The value of frame.contentDocument property for FRAME and IFRAME elements.
        public HtmlDocument FrameContentDocument { get; set; }
    }
}
