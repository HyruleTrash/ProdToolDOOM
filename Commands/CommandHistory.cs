using System.Windows.Input;

namespace ProdToolDOOM;

public class CommandHistory
{
    private readonly Stack<ICommand> history = new();
    private readonly Stack<ICommand> redoStack = new();

    public void ApplyCmd(ICommand cmd, bool flushRedoStack = true)
    {
        cmd.Execute();
        this.history.Push(cmd);
        if (flushRedoStack) this.redoStack.Clear();
    }

    public void UndoCmd()
    {
        if (this.history.Count == 0) return;
        ICommand latest = this.history.Pop();
        latest.Undo();
        this.redoStack.Push(latest);
    }

    public void RedoCmd()
    {
        if (this.redoStack.Count == 0) return;
        ICommand latest = this.redoStack.Pop();
        ApplyCmd(latest, false);
    }

    public void Reset()
    {
        this.history.Clear();
        this.redoStack.Clear();
    }
}