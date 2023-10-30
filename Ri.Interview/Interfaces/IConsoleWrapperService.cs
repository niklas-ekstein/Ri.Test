namespace Ri.Interview.Interfaces;

public interface IConsoleWrapperService
{
    void WriteLine(string message);
    void Write(string message);
    ConsoleKeyInfo ReadKey();
    string ReadLine();
}