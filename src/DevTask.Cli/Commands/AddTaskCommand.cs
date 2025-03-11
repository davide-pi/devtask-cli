using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Repositories.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Commands;
public sealed class AddTaskCommand : ICommand
{
    public static readonly string Description = "Add a new task to the list";
    private readonly ITasksRepository _tasksRepository;

    public AddTaskCommand(ITasksRepository tasksRepository)
    {
        _tasksRepository = tasksRepository;
    }

    public async Task ExecuteAsync(string commandArgument, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(commandArgument))
        {
            throw new ArgumentNullException(nameof(commandArgument));
        }

        var newTaskId = await _tasksRepository.InsertTaskAsync(commandArgument, cancellationToken);
    }
}
