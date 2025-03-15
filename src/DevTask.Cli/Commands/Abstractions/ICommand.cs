using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Commands.Abstractions;

public interface ICommand
{
    Task ExecuteAsync(string? commandArgument, CancellationToken cancellationToken);
}
