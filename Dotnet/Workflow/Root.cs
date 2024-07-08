using System.CommandLine;
using Workflow.Extensions;
using Workflow.Feedback.Commands;
using Workflow.Feedback.Repositories.FileSystem;
using Workflow.Howto.Commands;
using Workflow.Weekly.Commands;
using Workflow.Weekly.Repositories.FileSystem;

namespace Workflow;

public class Root : RootCommand
{
    private readonly RootOptions rootOptions = new();
    
    public Root() : base("Workflow")
    {
        _ = rootOptions.RootPath.GetOrCreateDirectory();
        
        this.AddCommand(this.GetWeeklyCommand());
        this.AddCommand(this.GetFeedbackCommand());
        this.AddCommand(this.GetHowtoCommand());
    }

    private WeeklyCommand GetWeeklyCommand()
    {
        var fileSystemTaskRepositoryOptions = new FileSystemTaskRepositoryOptions();
        fileSystemTaskRepositoryOptions.TasksFilePath = Path.Join(rootOptions.RootPath, fileSystemTaskRepositoryOptions.WeeklyFilePath);
        var taskRepository = new FileSystemTaskRepository(fileSystemTaskRepositoryOptions);
        return new WeeklyCommand(taskRepository);
    }

    private FeedbackCommand GetFeedbackCommand()
    {
        var feedbackRepositoryOptions = new FileSystemFeedbackRepositoryOptions();
        feedbackRepositoryOptions.FeedbackItemsFilePath = Path.Join(rootOptions.RootPath, feedbackRepositoryOptions.FeedbackFilePath);
        var feedbackRepository = new FileSystemFeedbackRepository(feedbackRepositoryOptions);
        return new FeedbackCommand(feedbackRepository);
    }
    
    private HowtoCommand GetHowtoCommand()
    {
        return new HowtoCommand();
    }
}