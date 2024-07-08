using Workflow.Extensions;
using Workflow.Feedback.Entities;
using Workflow.Feedback.Extensions;

namespace Workflow.Feedback.Repositories.FileSystem;

public class FileSystemFeedbackRepository : IFeedbackRepository
{
    private readonly FileSystemFeedbackRepositoryOptions options;
    private readonly DirectoryInfo feedbackItemsDirectory;

    public FileSystemFeedbackRepository(FileSystemFeedbackRepositoryOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.feedbackItemsDirectory = this.options.FeedbackItemsFilePath.GetOrCreateDirectory();
    }
    
    public async Task<IEnumerable<FeedbackItem>> GetFeedbackItemsAsync(CancellationToken cancellationToken = default)
    {
        var feedbackItemDirectories = this.feedbackItemsDirectory.GetDirectories();
        return await feedbackItemDirectories.ConvertToFeedbackItemsAsync();
    }

    public async Task<FeedbackItem> GetFeedbackItemAsync(int id, CancellationToken cancellationToken = default)
    {
        var feedbackItemDirectory = GetFeedbackItemDirectory(this.feedbackItemsDirectory, id);
        return await feedbackItemDirectory.ConvertToFeedbackItemAsync();
    }

    public async Task<FeedbackItem> CreateFeedbackItemAsync(FeedbackItem feedbackItem, CancellationToken cancellationToken = default)
    {
        var feedbackItemDirectory = GetFeedbackItemDirectory(this.feedbackItemsDirectory, feedbackItem.Id);
        await feedbackItemDirectory.CreateDescriptionFileAsync(feedbackItem.Description);
        return await feedbackItemDirectory.ConvertToFeedbackItemAsync();
    }

    public async Task DeleteFeedbackItemAsync(int id, CancellationToken cancellationToken = default)
    {
        var feedbackItemDirectory = GetFeedbackItemDirectory(this.feedbackItemsDirectory, id);
        await feedbackItemDirectory.CreateDeletedFileAsync();
    }

    public async Task<int> GetNextIdAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(this.feedbackItemsDirectory.GetNextId());
    }
    
    private static DirectoryInfo GetFeedbackItemDirectory(DirectoryInfo feedbackItemsDirectory, int id) =>
        feedbackItemsDirectory.GetOrCreateSubDirectory(id.ToString());
}