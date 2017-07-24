namespace Omaha.Feedback.Proto.Config
{
    // Channel via which notification about feedback should be send
    public enum NotifyChannel
    {
        // Send email notification.
        Email = 1,
        // File a bug in buganizer.
        Buganizer = 2,
        // File a bug in issue tracker.
        IssueTracker = 3
    }
}
