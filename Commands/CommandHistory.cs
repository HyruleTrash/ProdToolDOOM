using System.Windows.Input;

namespace ProdToolDOOM;

public class CommandHistory
{
    private readonly Stack<ICommand> history = new();
    private readonly Stack<ICommand> redoStack = new();

    public void ApplyCmd(ICommand cmd, bool flushRedoStack = true)
    {
        cmd.Execute();
        history.Push(cmd);
        if (flushRedoStack)
            redoStack.Clear();
    }

    public void UndoCmd()
    {
        if (history.Count == 0) return;
        var latest = history.Pop();
        latest.Undo();
        redoStack.Push(latest);
    }

    public void RedoCmd()
    {
        if (redoStack.Count == 0) return;
        var latest = redoStack.Pop();
        ApplyCmd(latest, false);
    }
}