using Omaha.Feedback.Proto.Math;

using ProtoBuf;

namespace Omaha.Feedback.Proto.Extension
{
    [ProtoContract]
    public class PostedScreenshot
    {
        [ProtoMember(1, IsRequired=true)]
        public string MimeType { get; set; }

        [ProtoMember(2, IsRequired=true)]
        public Dimensions Dimensions { get; set; }

        [ProtoMember(3, IsRequired=false)]
        public string Base64Content { get; set; }

        [ProtoMember(4, IsRequired=false)]
        public byte[] BinaryContent { get; set; }
    }
}
