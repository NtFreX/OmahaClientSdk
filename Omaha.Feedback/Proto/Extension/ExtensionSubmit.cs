using System;

using Omaha.Feedback.Proto.Chrome;
using Omaha.Feedback.Proto.Web;

using ProtoBuf;

namespace Omaha.Feedback.Proto.Extension
{
    [ProtoContract]
    // Sent when user hits final submit button.
    public class ExtensionSubmit
    {
        [ProtoMember(1, IsRequired=true)]
        public CommonData CommonData { get; set; }

        [ProtoMember(2, IsRequired=true)]
        public WebData WebData { get; set; }

        [ProtoMember(3, IsRequired=true)]
        public Int32 TypeId { get; set; }

        [ProtoMember(4, IsRequired=false)]
        public PostedScreenshot Scrennshot { get; set; }

        [ProtoMember(5, IsRequired=false)]
        public PostedBlackbox Blackbox { get; set; }

        [ProtoMember(14, IsRequired=false)]
        public ChromeData ChromeData { get; set; }

        [ProtoMember(15, IsRequired=true)]
        public ProductSpecificBinaryData[] ProductSpecificBinaryDatas { get; set; }

        [ProtoMember(16, IsRequired=false)]
        public string CategoryTag { get; set; }

        [ProtoMember(17, IsRequired=false)]
        public Int32 ProductId { get; set; }

        [ProtoMember(18, IsRequired=false)]
        public string Bucket { get; set; }
    }
}
