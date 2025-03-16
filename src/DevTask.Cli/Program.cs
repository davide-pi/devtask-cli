﻿using DevTask.Cli.Commands;
using DevTask.Cli.Commands.Abstractions;
using DevTask.Cli.HostedServices;
using DevTask.Cli.Repositories;
using DevTask.Cli.Repositories.Abstractions;
using DevTask.Cli.Services;
using DevTask.Cli.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using System.Threading.Tasks;

namespace DevTask.Cli;

public class Program
{
    static async Task Main(string[] args)
    {
        var hostBuilder = CreateHostBuilder(args);

        var host = hostBuilder.Build();

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices(ConfigureServices);

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton(AnsiConsole.Console);

        services.AddSingleton<ITasksRepository, JsonFileTasksRepository>();
        services.AddSingleton<ICliRenderer, CliRenderer>();
        services.AddKeyedSingleton<ICommand, AddTaskCommand>("AddTask");
        services.AddKeyedSingleton<ICommand, DeleteTaskCommand>("DeleteTask");
        services.AddKeyedSingleton<ICommand, ListAllTasksCommand>("ListTasks");
        services.AddKeyedSingleton<ICommand, ClearCommand>("Clear");

        services.AddHostedService<CommandLine>();
    }
}
