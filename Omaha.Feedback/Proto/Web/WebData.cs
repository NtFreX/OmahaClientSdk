using Omaha.Feedback.Proto.Dom;

using ProtoBuf;

namespace Omaha.Feedback.Proto.Web
{
    [ProtoContract]
    // Data present in feedbacks sent from web extension.
    public class WebData
    {
        [ProtoMember(1, IsRequired=false)]
        // Data captured from DOM Navigator object.
        public Navigator Navigator { get; set; }

        [ProtoMember(2, IsRequired=false)]
        // Details of the extension from which this data was sent.
        public ExtensionDetails ExtensionDetails { get; set; }

        [ProtoMember(3, IsRequired=false)]
        // The URL of the document.
        // Useful when user opts out from sending html structure.
        public string Url { get; set; }

        [ProtoMember(4, IsRequired=true)]
        // A list of annotations.
        public Annotation[] Annotations { get; set; }

        [ProtoMember(5, IsRequired=false)]
        // The ID of the suggestion selected by the user.
        // Possible values:
        // - Not set if no suggestions were shown, either because the version of
        //   the client did not support suggestions, suggestions were disabled or
        //   no matching suggestions were found.
        // - NONE_OF_THE_ABOVE if the user has chosen "None of the above".
        // - Empty string if suggestions were shown but the user hasn't chosen
        //   any of them (and also she hasn't chosen "None of the above").
        // - Actual suggestion identifier as returned from the server.
        public string SuggestionId { get; set; }

        [ProtoMember(6, IsRequired=true)]
        public ProductSpecificData[] ProductSpecificDatas { get; set; }

        [ProtoMember(7, IsRequired=true)]
        // Name of the binary data stored. Replicated from
        // ProductSpecificBinaryData.name which is stored as a separate
        // column in Feedbacks3 megastore table.
        public string[] ProductSpecificBinaryDataNames { get; set; }
    }
}
