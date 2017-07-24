using Omaha.Feedback.Proto.Dom;
using Omaha.Feedback.Proto.Math;

using ProtoBuf;

namespace Omaha.Feedback.Proto
{
    [ProtoContract]
    // An annotation drawn by the user on the screenshot of a web page.
    public class Annotation
    {
        [ProtoMember(1, IsRequired=true)]
        // A rectangular area covered by this annotation on annotated image.
        // The (0, 0) coordinate is placed in the top-left corner of the image.
        // One unit corresponds to one pixel.
        public Rectangle Rectangle { get; set; }

        [ProtoMember(2, IsRequired=false)]
        // A snippet of text displayed inside annotated portion of a web page.
        public string Snippet { get; set; }

        [ProtoMember(3, IsRequired=false)]
        // A path from root element of the document to the annotated element.
        public HtmlPath AnnotatedElementPath { get; set; }
    }
}
