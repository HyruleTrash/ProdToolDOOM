namespace DLLevelBuilder;

public interface ICommand
{
    public void Execute();
    public void Undo();
}