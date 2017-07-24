using ProtoBuf;

namespace Omaha.Feedback.Proto.Math
{
    [ProtoContract]
    // Axis-aligned rectangle in 2D space.
    public class Rectangle
    {
        [ProtoMember(1, IsRequired=true)]
        public float Left { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public float Top { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public float Width { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public float Height { get; set; }
    }
}
