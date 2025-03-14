using DevTask.Cli.Models;
using DevTask.Cli.Repositories;
using DevTask.Cli.Repositories.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DevTask.Cli.Tests.Repositories;

public class JsonFileTasksRepositoryTests : IDisposable
{
    private readonly JsonFileTasksRepository _repository;
    private readonly string _persistenceJsonFileForTests;

    public JsonFileTasksRepositoryTests()
    {
        _repository = new ServiceCollection()
            .AddScoped<JsonFileTasksRepository>()
            .BuildServiceProvider()
            .GetRequiredService<JsonFileTasksRepository>();

        _persistenceJsonFileForTests = $"devtask-cli-tasks.tests-{Guid.NewGuid()}.json";
        _repository.GetType()
            .GetField("_persistenceJsonFileName", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(_repository, _persistenceJsonFileForTests);
    }

    public void Dispose()
    {
        File.Delete(_persistenceJsonFileForTests);
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_InheritFromICommand()
    {
        Assert.IsAssignableFrom<ITasksRepository>(_repository);
    }

    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineThePersistenceInTheJsonFile()
    {
        var expectedJsonFileName = "devtask-cli-tasks.json";

        var persistanceField = typeof(JsonFileTasksRepository)
            .GetField("_persistenceJsonFileName", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(new JsonFileTasksRepository());

        Assert.Equal(expectedJsonFileName, persistanceField);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_InsertNewTask_When_InsertTaskIsInvoked()
    {
        var previousTasksList = new List<TaskItem>() { new(Guid.NewGuid(), "Already registered task") };
        await File.WriteAllTextAsync(_persistenceJsonFileForTests, JsonSerializer.Serialize(previousTasksList));

        var newTaskId = await _repository.InsertTaskAsync("Pass all tests!", CancellationToken.None);

        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileForTests);
        var tasksInFile = JsonSerializer.Deserialize<List<TaskItem>>(fileContent)!;


        //Assert.Collection(tasksInFile,t => Assert.Multiple([
        //        () => Assert.Equal(previousTasksList[0].Id, t.Id),
        //        () => Assert.Equal(previousTasksList[0].Title, t.Title)
        //    ]),
        //    t => Assert.Multiple([
        //        () => Assert.Equal(previousTasksList[0].Id, t.Id),
        //        () => Assert.Equal(previousTasksList[0].Title, t.Title)
        //    ]));

        Assert.Equal(previousTasksList.Count + 1, tasksInFile.Count);

        Assert.Multiple([
            () => Assert.Equal(previousTasksList[0].Id, tasksInFile[0].Id),
            () => Assert.Equal(previousTasksList[0].Title, tasksInFile[0].Title),
            () => Assert.Equal(newTaskId, tasksInFile[1].Id),
            () => Assert.Equal( "Pass all tests!", tasksInFile[1].Title)
        ]);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_TreatEmptyFileAsEmptyList_When_InsertTaskIsInvoked()
    {
        await File.WriteAllTextAsync(_persistenceJsonFileForTests, string.Empty);

        var newTaskId = await _repository.InsertTaskAsync("Test task", CancellationToken.None);

        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileForTests);
        var tasksInFile = JsonSerializer.Deserialize<List<TaskItem>>(fileContent)!;

        Assert.Single(tasksInFile);

        Assert.Multiple([
            () => Assert.Equal(newTaskId, tasksInFile[0].Id),
            () => Assert.Equal( "Test task", tasksInFile[0].Title)
        ]);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_RemoveTheTaskWithTheGuid_When_DeleteTaskIsInvoked()
    {
        var previousTasksList = new List<TaskItem>() {
            new(Guid.NewGuid(), "Already registered task 1"),
            new(Guid.NewGuid(), "Already registered task 2"),
            new(Guid.NewGuid(), "Already registered task 3")
        };

        await File.WriteAllTextAsync(_persistenceJsonFileForTests, JsonSerializer.Serialize(previousTasksList));

        await _repository.DeleteTaskAsync(previousTasksList[1].Id, CancellationToken.None);

        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileForTests);
        var tasksInFile = JsonSerializer.Deserialize<List<TaskItem>>(fileContent)!;

        Assert.Equal(previousTasksList.Count - 1, tasksInFile.Count);

        Assert.Multiple([
            () => Assert.Equal(previousTasksList[0].Id, tasksInFile[0].Id),
            () => Assert.Equal(previousTasksList[0].Title, tasksInFile[0].Title),
            () => Assert.Equal(previousTasksList[2].Id, tasksInFile[1].Id),
            () => Assert.Equal(previousTasksList[2].Title, tasksInFile[1].Title)
        ]);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_DoNothing_When_DeleteTaskIsInvokedIfTheTaskDoesNotExist()
    {
        var previousTasksList = new List<TaskItem>() {
            new(Guid.NewGuid(), "Already registered task 1"),
            new(Guid.NewGuid(), "Already registered task 2"),
            new(Guid.NewGuid(), "Already registered task 3")
        };

        await File.WriteAllTextAsync(_persistenceJsonFileForTests, JsonSerializer.Serialize(previousTasksList));

        await _repository.DeleteTaskAsync(Guid.NewGuid(), CancellationToken.None);

        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileForTests);
        var tasksInFile = JsonSerializer.Deserialize<List<TaskItem>>(fileContent)!;

        Assert.Equal(previousTasksList.Count, tasksInFile.Count);

        Assert.Multiple([
            () => Assert.Equal(previousTasksList[0].Id, tasksInFile[0].Id),
            () => Assert.Equal(previousTasksList[0].Title, tasksInFile[0].Title),
            () => Assert.Equal(previousTasksList[1].Id, tasksInFile[1].Id),
            () => Assert.Equal(previousTasksList[1].Title, tasksInFile[1].Title),
            () => Assert.Equal(previousTasksList[2].Id, tasksInFile[2].Id),
            () => Assert.Equal(previousTasksList[2].Title, tasksInFile[2].Title)
        ]);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_TreatEmptyFileAsEmptyList_When_DeleteTaskIsInvoked()
    {
        await File.WriteAllTextAsync(_persistenceJsonFileForTests, string.Empty);

        await _repository.DeleteTaskAsync(Guid.NewGuid(), CancellationToken.None);

        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileForTests);

        Assert.Empty(fileContent);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_ReturnAllTheRegisteredTasks_When_GetAllTasksIsInvoked()
    {
        var previousTasksList = new List<TaskItem>() {
            new(Guid.NewGuid(), "Already registered task 1"),
            new(Guid.NewGuid(), "Already registered task 2"),
            new(Guid.NewGuid(), "Already registered task 3")
        };

        await File.WriteAllTextAsync(_persistenceJsonFileForTests, JsonSerializer.Serialize(previousTasksList));

        var tasksInFile = ( await _repository.GetAllTasksAsync(CancellationToken.None) ).ToList();

        Assert.Equal(previousTasksList.Count, tasksInFile.Count);

        Assert.Multiple([
            () => Assert.Equal(previousTasksList[0].Id, tasksInFile[0].Id),
            () => Assert.Equal(previousTasksList[0].Title, tasksInFile[0].Title),
            () => Assert.Equal(previousTasksList[1].Id, tasksInFile[1].Id),
            () => Assert.Equal(previousTasksList[1].Title, tasksInFile[1].Title),
            () => Assert.Equal(previousTasksList[2].Id, tasksInFile[2].Id),
            () => Assert.Equal(previousTasksList[2].Title, tasksInFile[2].Title)
        ]);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_TreatEmptyFileAsEmptyList_When_GetAllTasksIsInvoked()
    {
        await File.WriteAllTextAsync(_persistenceJsonFileForTests, string.Empty);

        var taskList = await _repository.GetAllTasksAsync(CancellationToken.None);

        Assert.Empty(taskList);
    }
}
