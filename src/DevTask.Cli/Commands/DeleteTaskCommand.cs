using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Repositories.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Commands;
public sealed class DeleteTaskCommand : ICommand
{
    private readonly ITasksRepository _tasksRepository;
    
    public static readonly string Command = "delete";
    public static readonly string Description = "Delete an existing task by its ID";

    public DeleteTaskCommand(ITasksRepository tasksRepository)
    {
        _tasksRepository = tasksRepository;
    }

    public async Task ExecuteAsync(string commandArgument, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(commandArgument))
        {
            throw new ArgumentNullException(nameof(commandArgument));
        }

        if(!Guid.TryParse(commandArgument, out Guid taskId))
        {
            throw new ArgumentException("Argument should be a task ID", nameof(commandArgument));
        }

        await _tasksRepository.DeleteTaskAsync(taskId, cancellationToken);
    }
}
