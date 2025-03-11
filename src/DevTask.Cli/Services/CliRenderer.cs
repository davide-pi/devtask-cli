using DevTask.Cli.Models;
using DevTask.Cli.Services.Abstractions;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Services;
public sealed class CliRenderer : ICliRenderer
{
    private readonly IAnsiConsole _console;

    public CliRenderer(IAnsiConsole console)
    {
        _console = console;
    }

    public Task RenderTaskListAsync(IEnumerable<TaskItem> tasks, CancellationToken cancellationToken)
    {
        var table = new Table();
        table.AddColumn(new TableColumn("Id"));
        table.AddColumn(new TableColumn("Title"));

        tasks.ToList().ForEach(t =>
        {
            table.AddRow(t.Id.ToString(), t.Title);
        });

        _console.Write(table);

        return Task.CompletedTask;
    }
}
