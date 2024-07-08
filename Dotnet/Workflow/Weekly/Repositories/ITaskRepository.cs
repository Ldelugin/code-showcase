using Workflow.Weekly.Entities;
using Workflow.Weekly.Repositories.Models;

namespace Workflow.Weekly.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetTasksAsync(TaskFilter filter, CancellationToken cancellationToken = default);
    Task<TaskItem> GetTaskAsync(int id, int week, CancellationToken cancellationToken = default);
    Task<TaskItem> CreateTaskAsync(TaskItem task, int week, CancellationToken cancellationToken = default);
    Task<TaskItem> MarkTaskAsDoneAsync(int id, int week, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(int id, int week, CancellationToken cancellationToken = default);
    Task<int> GetNextIdAsync(int week, CancellationToken cancellationToken = default);
}