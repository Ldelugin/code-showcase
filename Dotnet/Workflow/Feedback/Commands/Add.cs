using System.CommandLine;
using Spectre.Console;
using Workflow.Feedback.Entities;
using Workflow.Feedback.Repositories;

namespace Workflow.Feedback.Commands;

public class Add : Command
{
    private readonly Argument<string> descriptionArgument = new(name: "description", description: "Description of the feedback");
    private readonly IFeedbackRepository feedbackRepository;
    
    public Add(IFeedbackRepository feedbackRepository) : base("add", "Add feedback")
    {
        this.feedbackRepository = feedbackRepository ?? throw new ArgumentNullException(nameof(feedbackRepository));
        this.AddArgument(this.descriptionArgument);
        this.SetHandler(this.HandleAsync, this.descriptionArgument);
    }
    
    public async Task<int> HandleAsync(string description)
    {
        var id = await this.feedbackRepository.GetNextIdAsync();
        var feedbackItem = new FeedbackItem
        {
            Description = description,
            CreatedAt = DateTime.Now,
            Id = id
        };
        await this.feedbackRepository.CreateFeedbackItemAsync(feedbackItem);
        AnsiConsole.MarkupLine($"[green]Feedback {id} created[/]");
        return 0;
    }
}