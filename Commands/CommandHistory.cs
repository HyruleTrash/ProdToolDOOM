using System.Windows.Input;

namespace ProdToolDOOM;

public class CommandHistory
{
    private Stack<ICommand> history = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    public void ApplyCmd(ICommand cmd)
    {
        cmd.Execute();
        history.Push(cmd);
    }

    public void UndoCmd()
    {
        ICommand latest = history.Pop();
        latest.Undo();
        redoStack.Push(latest);
    }

    public void RedoCmd()
    {
        ICommand latest = redoStack.Pop();
        ApplyCmd(latest);
    }
}