using Workflow.Extensions;
using Workflow.Feedback.Entities;

namespace Workflow.Feedback.Extensions;

public static class DirectoryInfoExtensions
{
    public static async Task<IEnumerable<FeedbackItem>> ConvertToFeedbackItemsAsync(this IEnumerable<DirectoryInfo> feedbackItemDirectories)
    {
        var feedbackItems = new List<FeedbackItem>();
        foreach (var feedbackItemDirectory in feedbackItemDirectories)
        {
            var feedbackItem = await feedbackItemDirectory.ConvertToFeedbackItemAsync();
            feedbackItems.Add(feedbackItem);
        }
        return feedbackItems;
    }
    
    public static async Task<FeedbackItem> ConvertToFeedbackItemAsync(this DirectoryInfo feedbackItemDirectory)
    {
        var feedbackItemFiles = feedbackItemDirectory.GetFiles();
        var descriptionFile = feedbackItemFiles.GetDescriptionFile();
        var isDeletedTimestamp = feedbackItemFiles.IsDeleted();
        
        var feedbackItem = new FeedbackItem
        {
            Id = int.Parse(feedbackItemDirectory.Name),
            Description = await File.ReadAllTextAsync(descriptionFile!.FullName),
            IsDeleted = isDeletedTimestamp != null,
            DeletedAt = isDeletedTimestamp ?? null,
        };

        return feedbackItem;
    }
}