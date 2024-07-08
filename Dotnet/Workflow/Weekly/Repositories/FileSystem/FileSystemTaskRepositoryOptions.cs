namespace Workflow.Weekly.Repositories.FileSystem;

public class FileSystemTaskRepositoryOptions
{
    public string WeeklyFilePath { get; set; } = Constants.WeeklyDirectoryName;
    public string TasksFilePath { get; set; } = string.Empty;
}