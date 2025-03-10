using DevTask.Cli.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Repositories.Abstractions;

public interface ITasksRepository
{
    Task<Guid> InsertTaskAsync(string title, CancellationToken cancellationToken);
    Task DeleteTaskAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<TaskItem>> GetAllTasksAsync(CancellationToken cancellationToken);
}
