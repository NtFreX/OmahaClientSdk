using ProtoBuf;

namespace Omaha.Feedback.Proto.Web
{
    [ProtoContract]
    public class ProductSpecificBinaryData
    {
        [ProtoMember(1, IsRequired=true)]
        public string Name { get; set; }

        [ProtoMember(2, IsRequired=false)]
        // mime_type of data
        public string MimeType { get; set; }

        [ProtoMember(3, IsRequired=false)]
        // raw data
        public byte[] Data { get; set; }
    }
}
