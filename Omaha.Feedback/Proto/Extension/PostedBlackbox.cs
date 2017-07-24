using ProtoBuf;

namespace Omaha.Feedback.Proto.Extension
{
    [ProtoContract]
    public class PostedBlackbox
    {
        [ProtoMember(1, IsRequired=false)]
        // mime_type of data
        public string MimeType { get; set; }

        [ProtoMember(2, IsRequired=true)]
        // raw data
        public byte[] Data { get; set; }
    }
}
