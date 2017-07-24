using System;

using ProtoBuf;

namespace Omaha.Feedback.Proto.Dom
{
    [ProtoContract]
    // A path in the HTML document between two elements, which are in the
    // ancestor-descendant relationship.
    public class HtmlPath
    {
        [ProtoMember(1, IsRequired=true)]
        // Ordered list of zero-based indices.
        // Empty path selects root element.
        // Non-negative index N selects (N+1)-th child.
        // Index -1 selects root element from frame content document.
        public Int32 Index { get; set; }
    }
}
