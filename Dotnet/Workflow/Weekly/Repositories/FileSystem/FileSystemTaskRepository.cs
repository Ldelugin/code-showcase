using Workflow.Extensions;
using Workflow.Weekly.Entities;
using Workflow.Weekly.Extensions;
using Workflow.Weekly.Repositories.Models;

namespace Workflow.Weekly.Repositories.FileSystem;

public class FileSystemTaskRepository : ITaskRepository
{
    private readonly FileSystemTaskRepositoryOptions options;
    private readonly DirectoryInfo tasksDirectory;

    public FileSystemTaskRepository(FileSystemTaskRepositoryOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.tasksDirectory = this.options.TasksFilePath.GetOrCreateDirectory();
    }

    public async Task<IEnumerable<TaskItem>> GetTasksAsync(TaskFilter filter, CancellationToken cancellationToken = default)
    {
        var weekDirectory = this.GetWeekDirectory(filter.Week);
        var taskDirectories = weekDirectory.GetDirectories();
        return await taskDirectories.ConvertToTaskItemsAsync(filter);
    }

    public async Task<TaskItem> GetTaskAsync(int id, int week, CancellationToken cancellationToken = default)
    {
        var taskDirectory = GetTaskDirectory(this.GetWeekDirectory(week), id);
        return await taskDirectory.ConvertToTaskItemAsync(TaskFilter.IncludeDefault());
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task, int week, CancellationToken cancellationToken = default)
    {
        var taskDirectory = GetTaskDirectory(this.GetWeekDirectory(week), task.Id);
        await taskDirectory.CreateDescriptionFileAsync(task.Description);
        if (task.IsPlanned)
        {
            await taskDirectory.CreatePlannedFileAsync();
        }
        return await taskDirectory.ConvertToTaskItemAsync(TaskFilter.IncludeDefault());
    }

    public async Task<TaskItem> MarkTaskAsDoneAsync(int id, int week, CancellationToken cancellationToken = default)
    {
        var taskDirectory = GetTaskDirectory(this.GetWeekDirectory(week), id);
        await taskDirectory.CreateDoneFileAsync();
        var task = await taskDirectory.ConvertToTaskItemAsync(TaskFilter.IncludeDefault());
        return task;
    }

    public async Task DeleteTaskAsync(int id, int week, CancellationToken cancellationToken = default)
    {
        var taskDirectory = GetTaskDirectory(this.GetWeekDirectory(week), id);
        await taskDirectory.CreateDeletedFileAsync();
    }
    
    public async Task<int> GetNextIdAsync(int week, CancellationToken cancellationToken = default)
    {
        var weekDirectory = this.GetWeekDirectory(week);
        return await Task.FromResult(weekDirectory.GetNextId());
    }
    
    private DirectoryInfo GetWeekDirectory(int week) =>
        this.tasksDirectory.GetOrCreateSubDirectory(week.ToString());

    private static DirectoryInfo GetTaskDirectory(DirectoryInfo weekDirectory, int taskId) =>
        weekDirectory.GetOrCreateSubDirectory(taskId.ToString());
}