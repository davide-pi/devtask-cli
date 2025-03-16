using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.Services.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Commands;
public sealed class ClearCommand : ICommand
{
    private readonly ICliRenderer _renderer;

    public static readonly string Command = "clear";
    public static readonly string Description = "Clean the CLI";

    public ClearCommand(ICliRenderer renderer)
    {
        _renderer = renderer;
    }

    public async Task ExecuteAsync(string? commandArgument, CancellationToken cancellationToken)
    {
        await _renderer.ClearAsync(cancellationToken);
    }
}
