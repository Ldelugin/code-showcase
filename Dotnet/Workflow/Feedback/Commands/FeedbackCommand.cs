using System.CommandLine;
using Workflow.Feedback.Repositories;

namespace Workflow.Feedback.Commands;

public class FeedbackCommand : Command
{
    private readonly IFeedbackRepository feedbackRepository;
    
    public FeedbackCommand(IFeedbackRepository feedbackRepository) : base("feedback", "Manage feedback")
    {
        this.feedbackRepository = feedbackRepository ?? throw new ArgumentNullException(nameof(feedbackRepository));
        this.AddCommand(new Add(this.feedbackRepository));
        this.AddCommand(new Delete(this.feedbackRepository));
        this.AddCommand(new List(this.feedbackRepository));
    }
}