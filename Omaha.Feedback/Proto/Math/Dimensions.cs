using ProtoBuf;

namespace Omaha.Feedback.Proto.Math
{
    [ProtoContract]
    // 2D Dimensions.
    public class Dimensions
    {
        [ProtoMember(1, IsRequired=true)]
        public float Width { get; set; }
        
        [ProtoMember(2, IsRequired=true)]
        public float Height { get; set; }
    }
}
