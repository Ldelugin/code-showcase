using System.CommandLine;
using Workflow.Feedback.Repositories;

namespace Workflow.Feedback.Commands;

public class Delete : Command
{
    private readonly Argument<int> taskIdArgument = new (name: "id", description: "The id of the feedback to delete");
    private readonly IFeedbackRepository feedbackRepository;
    
    public Delete(IFeedbackRepository feedbackRepository) : base("delete", "Delete feedback")
    {
        this.feedbackRepository = feedbackRepository ?? throw new ArgumentNullException(nameof(feedbackRepository));
    }
}