using Ri.Interview.Interfaces;

namespace Ri.Interview.Services;

public class ConsoleWrapperService : IConsoleWrapperService
{
    public void WriteLine(string message) => Console.WriteLine(message);
    public void Write(string message) => Console.Write(message);
    public ConsoleKeyInfo ReadKey() => Console.ReadKey();
    public string ReadLine() => Console.ReadLine();
}