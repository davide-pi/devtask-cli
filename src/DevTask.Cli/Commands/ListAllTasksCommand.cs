using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Repositories.Abstractions;
using DevTask.Cli.Services.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Commands;
public sealed class ListAllTasksCommand : ICommand
{
    private readonly ITasksRepository _tasksRepository;
    private readonly ICliRenderer _renderer;

    public static readonly string Command = "list";
    public static readonly string Description = "List all the tasks";

    public ListAllTasksCommand(ITasksRepository tasksRepository, ICliRenderer renderer)
    {
        _tasksRepository = tasksRepository;
        _renderer = renderer;
    }

    public async Task ExecuteAsync(string? commandArgument, CancellationToken cancellationToken)
    {
        var tasks = await _tasksRepository.GetAllTasksAsync(cancellationToken);
        await _renderer.RenderTaskListAsync(tasks, cancellationToken);
    }
}
