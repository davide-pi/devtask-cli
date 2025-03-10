using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.HostedServices;

public sealed class CommandLine : IHostedService
{
    public CommandLine()
    {
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}
