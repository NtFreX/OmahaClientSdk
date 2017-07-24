using System;

using ProtoBuf;

namespace Omaha.Feedback.Proto
{
    [ProtoContract]
    // Data present in all kinds of feedbacks, regardless of source (Web, Android,
    // other).
    public class CommonData
    {
        [ProtoMember(1, IsRequired=false)]
        public Int64 GaiaId { get; set; }

        // Description of the problem entered by user.
        [ProtoMember(2, IsRequired=false)]
        public string Description { get; set; }
        [ProtoMember(4, IsRequired=false)]
        public string DescriptionTranslated { get; set; }
        [ProtoMember(5, IsRequired=false)]
        public string SourceDescriptionLanguage { get; set; } //default=en
        [ProtoMember(6, IsRequired=false)]
        public string UiLanguage { get; set; } //default=en_US

        [ProtoMember(3, IsRequired=false)]
        public string UserEmail { get; set; }

        // Unique identifier of feedback report. If set than only one report
        // with the same identifier is stored in the system.
        // If you are not sure how to use it leave it not set.
        [ProtoMember(7, IsRequired=false)]
        public string UniqueReportIdentifier { get; set; }
    }
}
