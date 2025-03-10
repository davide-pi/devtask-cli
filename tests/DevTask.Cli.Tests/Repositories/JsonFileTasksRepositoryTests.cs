using DevTask.Cli.Models;
using DevTask.Cli.Repositories;
using DevTask.Cli.Repositories.Abstractions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
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
        _persistenceJsonFileForTests = $"devtask-cli-tasks.tests-{Guid.NewGuid()}.json";

        _repository = new JsonFileTasksRepository();

        _repository.GetType().GetField("_persistenceJsonFileName", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(_repository, _persistenceJsonFileForTests);
    }

    public void Dispose()
    {
        File.Delete(_persistenceJsonFileForTests);
    }

    [Trait("Category", "L0")]
    [Theory]
    [InlineData(typeof(ITasksRepository))]
    public void Should_InheritFrom(Type typeToInherit)
    {
        typeof(JsonFileTasksRepository)
            .Should()
            .BeAssignableTo(typeToInherit);
    }


    [Trait("Category", "L0")]
    [Fact]
    public void Should_DefineThePersistenceInTheJsonFile()
    {
        var expectedJsonFileName = "devtask-cli-tasks.json";

        var persistanceField = typeof(JsonFileTasksRepository)
            .GetField("_persistenceJsonFileName", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(new JsonFileTasksRepository())
            .Should()
            .Be(expectedJsonFileName);
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_InsertNewTask_When_InsertTaskIsInvoked()
    {
        var previousTasksList = new List<TaskItem>() { new(Guid.NewGuid(), "Already registered task") };
        await File.WriteAllTextAsync(_persistenceJsonFileForTests, JsonSerializer.Serialize(previousTasksList));

        var newTaskId = await _repository.InsertTaskAsync("Pass all tests!", CancellationToken.None);

        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileForTests);
        var tasksInFile = JsonSerializer.Deserialize<List<TaskItem>>(fileContent);

        tasksInFile
            .Should()
            .Satisfy(
                t => t.Id == previousTasksList[0].Id && t.Title == previousTasksList[0].Title,
                t => t.Id == newTaskId && t.Title == "Pass all tests!"
            );
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_RemoveTheTaskWithTheGuid_When_DeleteTaskIsInvoked()
    {
        var expectedTaskList = new List<TaskItem>() {
            new(Guid.NewGuid(), "Already registered task 1"),
            new(Guid.NewGuid(), "Already registered task 2"),
            new(Guid.NewGuid(), "Already registered task 3")
        };

        await File.WriteAllTextAsync(_persistenceJsonFileForTests, JsonSerializer.Serialize(expectedTaskList));

        await _repository.DeleteTaskAsync(expectedTaskList[1].Id, CancellationToken.None);

        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileForTests);
        var tasksInFile = JsonSerializer.Deserialize<List<TaskItem>>(fileContent);

        tasksInFile
            .Should()
            .Satisfy(
                t => expectedTaskList[0].Id == t.Id && expectedTaskList[0].Title == t.Title,
                t => expectedTaskList[2].Id == t.Id && expectedTaskList[2].Title == t.Title
            );
    }


    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_DoNothing_When_DeleteTaskIsInvokedIfTheTaskDoesNotExist()
    {
        var expectedTaskList = new List<TaskItem>() {
            new(Guid.NewGuid(), "Already registered task 1"),
            new(Guid.NewGuid(), "Already registered task 2"),
            new(Guid.NewGuid(), "Already registered task 3")
        };

        await File.WriteAllTextAsync(_persistenceJsonFileForTests, JsonSerializer.Serialize(expectedTaskList));

        await _repository.DeleteTaskAsync(Guid.NewGuid(), CancellationToken.None);

        var fileContent = await File.ReadAllTextAsync(_persistenceJsonFileForTests);
        var tasksInFile = JsonSerializer.Deserialize<List<TaskItem>>(fileContent);

        tasksInFile
            .Should()
            .Satisfy(
                t => expectedTaskList[0].Id == t.Id && expectedTaskList[0].Title == t.Title,
                t => expectedTaskList[1].Id == t.Id && expectedTaskList[1].Title == t.Title,
                t => expectedTaskList[2].Id == t.Id && expectedTaskList[2].Title == t.Title
            );
    }

    [Trait("Category", "L0")]
    [Fact]
    public async Task Should_ReturnAllTheRegisteredTasks_When_GetAllTasksIsInvoked()
    {
        var expectedTaskList = new List<TaskItem>() {
            new(Guid.NewGuid(), "Already registered task 1"),
            new(Guid.NewGuid(), "Already registered task 2"),
            new(Guid.NewGuid(), "Already registered task 3")
        };

        await File.WriteAllTextAsync(_persistenceJsonFileForTests, JsonSerializer.Serialize(expectedTaskList));

        var taskList = await _repository.GetAllTasksAsync(CancellationToken.None);

        taskList
            .Should()
            .Satisfy(
                t => expectedTaskList[0].Id == t.Id && expectedTaskList[0].Title == t.Title,
                t => expectedTaskList[1].Id == t.Id && expectedTaskList[1].Title == t.Title,
                t => expectedTaskList[2].Id == t.Id && expectedTaskList[2].Title == t.Title
            );
    }
}
