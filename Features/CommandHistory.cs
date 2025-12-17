using System.Windows.Input;

namespace ProdToolDOOM;

public class CommandHistory
{
    private Stack<ICommand> history = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();
}