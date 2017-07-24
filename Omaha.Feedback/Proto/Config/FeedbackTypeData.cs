using System;

using ProtoBuf;

namespace Omaha.Feedback.Proto.Config
{
    [ProtoContract]
    public class FeedbackTypeData
    {
        [ProtoMember(1, IsRequired=true)]
        // index of feedback type as found in database
        public Int32 Id { get; set; }
        [ProtoMember(2, IsRequired=true)]
        // Specifies whether this feedback type is currently enabled and
        // feedback of this type can be submitted.
        public bool Enabled { get; set; }
        [ProtoMember(3, IsRequired=true)]
        // Problem name of this feedback type on Google Feedback pages.
        public string ProblemName { get; set; }
        [ProtoMember(4, IsRequired=false)]
        // Name of the product to which this feedback type belongs.
        public string ProductName { get; set; }

        // Tag 5 is used by some legacy data that is already in production db.

        [ProtoMember(6, IsRequired=true)]
        // matcher to execute against page
        public MatcherData Matcher { get; set; }
        [ProtoMember(7, IsRequired=true)]
        // Comma separated list of email addresses to which email notification
        // is sent upon each new feedback of this type.
        // No email is sent if this field is set to an empty string.
        public string NotificationEmail { get; set; }

        // Do not use tag 8, 9, 10. They were used by a legacy field.

        [ProtoMember(11, IsRequired=false)]
        // Kind of feedback type.
        public Kind Kind { get; set; } //default=PRODUCT
        [ProtoMember(12, IsRequired=false)]
        // Prefix to be added to summary of notification email sent for feedback of this
        // type.
        public string SummaryPrefix { get; set; }
        [ProtoMember(13, IsRequired=false)]
        // String template with which "Additional Info" field in extension
        // should be initially filled.
        public string Template { get; set; }
        [ProtoMember(14, IsRequired=false)]
        // ID of the product this feedback type belongs to.
        public Int32 ProductId { get; set; }
        [ProtoMember(15, IsRequired=false)]
        // Tag that is used for marking feedback types that require non-ordinary handling.
        // E.g: This field is equal:
        // "unclassified" for Unclassified feedback,
        // "android" for android feedback
        // "selenium" for selenium feedback
        public string Tag { get; set; }
        [ProtoMember(16, IsRequired=false)]
        // Problem description visible in feedback extension.
        public string ProblemDescription { get; set; }
        [ProtoMember(17, IsRequired=false)]
        // Specifies the visibility of this feedback type.
        public Visibility Visibility { get; set; } //default=INTERNAL

        // tag 18 was used by removed field

        // Specifies Buganizer fields
        // TODO(kaczmarek): enable once we migrated to new protos.
        // optional BuganizerSettings buganizer_settings = 19;

        [ProtoMember(20, IsRequired=false)]
        // Specifies channel via which notification about feedback of this type should be sent.
        public NotifyChannel NotifyChannel { get; set; } //default=EMAIL
        [ProtoMember(21, IsRequired=false)]
        // Specifies granularity of notifications send for feedbacks of this type.
        public NotificationGranularity NotificationGranularity { get; set; } //default=FEEDBACK
        [ProtoMember(22, IsRequired=false)]
        // Threshold for number of feedbacks in a cluster at which notification is sent.
        public Int32 ClusteringThreshold { get; set; } //default=5
    }
}
