using System;

using ProtoBuf;

namespace Omaha.Feedback.Proto.Config
{
    [ProtoContract]
    // Product for which feedback can be sent: GMail, Writely etc.
    public class Product
    {
        [ProtoMember(1, IsRequired=true)]
        public Int32 Id { get; set; }
        [ProtoMember(2, IsRequired=true)]
        public string Name { get; set; }
        [ProtoMember(3, IsRequired=false)]
        public string[] Owners { get; set; }
    }
}
