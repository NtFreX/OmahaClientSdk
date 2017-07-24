namespace Omaha.Feedback.Proto.Config
{
    // Granularity of notifications.
    public enum NotificationGranularity
    {
        // Send notification per each feedback.
        Feedback = 1,
        // Send notification per clustered group of similar feedbacks.
        Cluster = 2
    }
}
