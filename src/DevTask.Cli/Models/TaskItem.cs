using System;

namespace DevTask.Cli.Models;
public sealed class TaskItem
{
    public Guid Id { get; init; }
    public string Title { get; init; }

    public TaskItem(Guid id, string title)
    {
        if(string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentNullException(nameof(title));
        }

        Id = id;
        Title = title;
    }
}
