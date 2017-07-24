using ProtoBuf;

namespace Omaha.Feedback.Proto.Web
{
    [ProtoContract]
    public class ExtensionDetails
    {
        [ProtoMember(1, IsRequired=true)]
        // Indicates browser and mpm release.
        public string ExtensionVersion { get; set; }

        [ProtoMember(2, IsRequired=true)]
        public string ProtocolVersion { get; set; }
    }
}
