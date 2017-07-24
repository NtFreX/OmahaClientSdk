using System;

using Omaha.Feedback.Proto.Chrome;
using Omaha.Feedback.Proto.Dom;
using Omaha.Feedback.Proto.Web;

using ProtoBuf;

namespace Omaha.Feedback.Proto.Extension
{
    [ProtoContract]
    // A query for suggestions, sent when the user hits the preview button.
    public class SuggestQuery
    {
        [ProtoMember(1, IsRequired=true)]
        public CommonData CommonData { get; set; }

        [ProtoMember(2, IsRequired=true)]
        public WebData WebData { get; set; }

        [ProtoMember(3, IsRequired=true)]
        public Int32 TypeId { get; set; }

        [ProtoMember(4, IsRequired=false)]
        public HtmlDocument HtmlDocumentStructure { get; set; }

        [ProtoMember(5, IsRequired=false)]
        public ChromeData ChromeData { get; set; }
    }
}
