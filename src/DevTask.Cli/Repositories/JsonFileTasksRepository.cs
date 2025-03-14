using DevTask.Cli.Models;
using DevTask.Cli.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Repositories;
public sealed class JsonFileTasksRepository : ITasksRepository
{
    private readonly string _persistenceJsonFileName = "devtask-cli-tasks.json";

    public JsonFileTasksRepository()
    {
        if (!File.Exists(_persistenceJsonFileName))
        {
            File.Create(_persistenceJsonFileName).Close();
        }
    }
    public async Task<Guid> InsertTaskAsync(string title, CancellationToken cancellationToken)
    {
        var registeredTasks = await ReadTasksFromFileAsync(cancellationToken);

        var newTask = new TaskItem(Guid.NewGuid(), title);
        registeredTasks.Add(newTask);

        await File.WriteAllTextAsync(_persistenceJsonFileName, JsonSerializer.Serialize(registeredTasks), cancellationToken);

        return newTask.Id;
    }

    public async Task DeleteTaskAsync(Guid id, CancellationToken cancellationToken)
    {
        var registeredTasks = await ReadTasksFromFileAsync(cancellationToken);

        var taskToRemove = registeredTasks.SingleOrDefault(t => t.Id == id);

        if (taskToRemove is not null)
        {
            registeredTasks.Remove(taskToRemove);
            await File.WriteAllTextAsync(_persistenceJsonFileName, JsonSerializer.Serialize(registeredTasks), cancellationToken);
        }
    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync(CancellationToken cancellationToken)
    {
        return await ReadTasksFromFileAsync(cancellationToken);
    }

    private async Task<List<TaskItem>> ReadTasksFromFileAsync(CancellationToken cancellationToken)
    {
        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileName, cancellationToken);

        if (string.IsNullOrWhiteSpace(fileContent))
        {
            return new();
        }

        return JsonSerializer.Deserialize<List<TaskItem>>(fileContent) ?? new();
    }
}
