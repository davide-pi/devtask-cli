using DevTask.Cli.Models;
using DevTask.Cli.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Repositories;
public sealed class JsonFileTasksRepository : ITasksRepository
{
    private readonly string _persistenceJsonFileName = "devtask-cli-tasks.json";

    public async Task DeleteTaskAsync(Guid id, CancellationToken cancellationToken)
    {
        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileName);
        var registeredTasks = JsonSerializer.Deserialize<List<TaskItem>>(fileContent) ?? new();

        var taskToRemove = registeredTasks.SingleOrDefault(t => t.Id == id);

        if (taskToRemove is not null)
        {
            registeredTasks.Remove(taskToRemove);
            await File.WriteAllTextAsync(_persistenceJsonFileName, JsonSerializer.Serialize(registeredTasks));
        }

    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync(CancellationToken cancellationToken)
    {
        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileName);
        var registeredTasks = JsonSerializer.Deserialize<List<TaskItem>>(fileContent) ?? new();

        return registeredTasks;
    }

    public async Task<Guid> InsertTaskAsync(string title, CancellationToken cancellationToken)
    {
        var newTask = new TaskItem(Guid.NewGuid(), title);

        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileName);
        var registeredTasks = JsonSerializer.Deserialize<List<TaskItem>>(fileContent) ?? new();

        registeredTasks.Add(newTask);

        await File.WriteAllTextAsync(_persistenceJsonFileName, JsonSerializer.Serialize(registeredTasks));

        return newTask.Id;
    }
}
