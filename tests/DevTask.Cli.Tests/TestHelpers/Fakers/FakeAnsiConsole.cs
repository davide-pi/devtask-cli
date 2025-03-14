using Spectre.Console;
using Spectre.Console.Rendering;
using System;

namespace DevTask.Cli.Tests.TestHelpers.Fakers;

public class FakeAnsiConsole : IAnsiConsole
{
    public Profile Profile { get => throw new NotImplementedException(); }
    public IAnsiConsoleCursor Cursor { get => throw new NotImplementedException(); }
    public IAnsiConsoleInput Input { get => throw new NotImplementedException(); }
    public IExclusivityMode ExclusivityMode { get => throw new NotImplementedException(); }
    public RenderPipeline Pipeline { get => throw new NotImplementedException(); }

    public void Clear(bool home)
    {
        throw new NotImplementedException();
    }

    public void Write(IRenderable renderable)
    {
        throw new NotImplementedException();
    }
}
