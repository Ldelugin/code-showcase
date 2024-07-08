using System.CommandLine;
using Workflow.Feedback.Repositories;

namespace Workflow.Feedback.Commands;

public class List : Command
{
    private readonly IFeedbackRepository feedbackRepository;
    
    public List(IFeedbackRepository feedbackRepository) : base("list", "List feedback")
    {
        this.feedbackRepository = feedbackRepository ?? throw new ArgumentNullException(nameof(feedbackRepository));
    }
}