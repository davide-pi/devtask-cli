using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.HostedServices;

public sealed class CommandLine : IHostedService
{
    private readonly ICliRenderer _renderer;
    private readonly ICommand _addTaskCommand;
    private readonly ICommand _deleteTaskCommand;
    private readonly ICommand _listAllTasksCommand;

    public CommandLine(
        ICliRenderer renderer,
        [FromKeyedServices("AddTask")] ICommand addTaskCommand,
        [FromKeyedServices("DeleteTask")] ICommand deleteTaskCommand,
        [FromKeyedServices("ListTasks")] ICommand listAllTasksCommand)
    {
        _renderer = renderer;
        _addTaskCommand = addTaskCommand;
        _deleteTaskCommand = deleteTaskCommand;
        _listAllTasksCommand = listAllTasksCommand;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var command = string.Empty;

        do
        {
            var userInput = await _renderer.AskUserForInputAsync(cancellationToken);
            var splittedUserInput = userInput?.Trim().Split(" ",2, System.StringSplitOptions.TrimEntries);

            command = splittedUserInput?[0].ToLower();
            var argument = string.Empty;
            switch (command)
            {
                case "exit":
                    break;
                case "add":
                    argument = splittedUserInput!.ElementAtOrDefault(1);
                    await _addTaskCommand.ExecuteAsync(argument, cancellationToken);
                    break;
                case "delete":
                    argument = splittedUserInput!.ElementAtOrDefault(1);
                    await _deleteTaskCommand.ExecuteAsync(argument, cancellationToken);
                    break;
                case "list":
                    await _listAllTasksCommand.ExecuteAsync(null, cancellationToken);
                    break;

                default:
                    await _renderer.RenderMessageAsync("Command not found", cancellationToken);
                    break;
            }

        } while (command != "exit");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}
