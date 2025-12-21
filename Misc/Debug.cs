namespace ProdToolDOOM;

public static class Debug
{
    public static void Log(string message)
    {
#if DEBUG
        Console.WriteLine(message);
#endif
    }
    public static void Log(object? message)
    {
#if DEBUG
        Console.WriteLine(message == null ? "Null" : message.ToString());
#endif
    }
    
    public static void LogError(string message)
    {
#if DEBUG
        Console.WriteLine($"Error!!! {message}");
#endif
    }
    public static void LogError(object? message)
    {
#if DEBUG
        Console.WriteLine(message == null ? "Error!!! Null" : $"Error!!! {message}");
#endif
    }
}