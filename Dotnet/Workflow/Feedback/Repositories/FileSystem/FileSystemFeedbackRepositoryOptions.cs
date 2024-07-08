using Workflow.Feedback;

namespace Workflow.Feedback.Repositories.FileSystem;

public class FileSystemFeedbackRepositoryOptions
{
    public string FeedbackFilePath { get; set; } = Constants.FeedbackDirectoryName;
    public string FeedbackItemsFilePath { get; set; } = string.Empty;
}