using DevTask.Cli.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Services.Abstractions;

public interface ICliRenderer
{
    Task RenderTaskListAsync(IEnumerable<TaskItem> tasks, CancellationToken cancellationToken);
    Task<string?> AskUserForInputAsync(CancellationToken cancellationToken);
    Task RenderMessageAsync(string message, CancellationToken cancellationToken);
}
