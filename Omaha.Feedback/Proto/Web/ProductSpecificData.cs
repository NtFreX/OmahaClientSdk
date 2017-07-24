using ProtoBuf;

namespace Omaha.Feedback.Proto.Web
{
    [ProtoContract]
    // Product specific data. Contains one key/value pair that is specific to the
    // product for which feedback is submitted.
    public class ProductSpecificData
    {
        [ProtoMember(1, IsRequired=true)]
        public string Key { get; set; }

        [ProtoMember(2, IsRequired=false)]
        public string Value { get; set; }
    }
}
