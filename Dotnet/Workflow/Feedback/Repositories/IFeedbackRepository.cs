using Workflow.Feedback.Entities;

namespace Workflow.Feedback.Repositories;

public interface IFeedbackRepository
{
    Task<IEnumerable<FeedbackItem>> GetFeedbackItemsAsync(CancellationToken cancellationToken = default);
    Task<FeedbackItem> GetFeedbackItemAsync(int id, CancellationToken cancellationToken = default);
    Task<FeedbackItem> CreateFeedbackItemAsync(FeedbackItem feedbackItem, CancellationToken cancellationToken = default);
    Task DeleteFeedbackItemAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetNextIdAsync(CancellationToken cancellationToken = default);
}