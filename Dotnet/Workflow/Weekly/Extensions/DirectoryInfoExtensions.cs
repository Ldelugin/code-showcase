using Workflow.Extensions;
using Workflow.Weekly.Entities;
using Workflow.Weekly.Repositories.Models;

namespace Workflow.Weekly.Extensions;

public static class DirectoryInfoExtensions
{
    public static DateTime? IsDone(this IEnumerable<FileInfo> fileInfos) =>
        fileInfos.FirstOrDefault(x => x.Name == Constants.DoneFile)?.CreationTime;
    
    public static bool IsPlanned(this IEnumerable<FileInfo> fileInfos) =>
        fileInfos.FirstOrDefault(x => x.Name == Constants.PlannedFile) != null;
    
    public static async Task CreateDoneFileAsync(this DirectoryInfo directoryInfo) =>
        await directoryInfo.CreateFileAsync(Constants.DoneFile);
    
    public static async Task CreatePlannedFileAsync(this DirectoryInfo directoryInfo) =>
        await directoryInfo.CreateFileAsync(Constants.PlannedFile);

    public static async Task<IEnumerable<TaskItem>> ConvertToTaskItemsAsync(this IEnumerable<DirectoryInfo> directoryInfos, TaskFilter filter)
    {
        var list = new List<TaskItem>();
        foreach (var directoryInfo in directoryInfos)
        {
            var task = await directoryInfo.ConvertToTaskItemAsync(filter);
            if (!ShouldBeIncluded(task, filter))
            {
                continue;
            }
            list.Add(task);
        }
        return list;
    }
    
    private static bool ShouldBeIncluded(this TaskBase task, TaskFilter filter)
    {
        // Default
        return true;
        // if (!filter.Planned.HasValue && !filter.Done.HasValue && !filter.Deleted.HasValue)
        // {
        //     return !task.IsDeleted;
        // }
        
        
        return (!filter.Planned.HasValue || (task is TaskItem taskItem && filter.Planned.Value == taskItem.IsPlanned))
                 && (!filter.Done.HasValue || filter.Done.Value == task.IsDone)
                 && (!filter.Deleted.HasValue || filter.Deleted.Value != task.IsDeleted);
    }

    public static async Task<TaskItem> ConvertToTaskItemAsync(this DirectoryInfo directoryInfo, TaskFilter filter)
    {
        return await directoryInfo.ConvertToTaskItemAsync<TaskItem>(async (task, dir) =>
        {
            task.IsPlanned = dir.GetFiles().IsPlanned();
            var subTaskDirectories = dir.GetDirectories();
            var subTasks = await subTaskDirectories.ConvertToTaskItemsAsync<SubTaskItem>(filter);
            task.SubTasks.AddRange(subTasks);
        });
    }

    private static async Task<IEnumerable<T>> ConvertToTaskItemsAsync<T>(this IEnumerable<DirectoryInfo> directoryInfos, TaskFilter filter,
        Func<T, DirectoryInfo, Task>? additionalConvert = null) where T : TaskBase, new()
    {
        var list = new List<T>();
        foreach (var directoryInfo in directoryInfos)
        {
            var task = await directoryInfo.ConvertToTaskItemAsync(additionalConvert);
            if (!ShouldBeIncluded(task, filter))
            {
                continue;
            }
            list.Add(task);
        }
        return list;
    }
    
    private static async Task<T> ConvertToTaskItemAsync<T>(this DirectoryInfo directoryInfo,
        Func<T, DirectoryInfo, Task>? additionalConvert = null) where T : TaskBase, new()
    {
        var taskFiles = directoryInfo.GetFiles();
        var descriptionFile = taskFiles.GetDescriptionFile();
        var isDoneTimestamp = taskFiles.IsDone();
        var isDeletedTimestamp = taskFiles.IsDeleted();
        
        var task = new T
        {
            Id = int.Parse(directoryInfo.Name),
            Description = await File.ReadAllTextAsync(descriptionFile!.FullName),
            IsDone = isDoneTimestamp.HasValue,
            DoneAt = isDoneTimestamp ?? null,
            IsDeleted = isDeletedTimestamp.HasValue,
            DeletedAt = isDeletedTimestamp ?? null,
            CreatedAt = descriptionFile?.CreationTime ?? DateTime.Now
        };

        if (additionalConvert != null)
        {
            await additionalConvert.Invoke(task, directoryInfo);
        }
        return task;
    }
}