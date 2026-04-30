namespace ProdToolDOOM;

public class MovePointCmd(Point pointRef, Vector2 oldPos, Vector2 newPos) : ICommand
{
    public void Execute()
    {
        pointRef.position = newPos;
        pointRef.UpdateVisualPosition(Program.instance.GetWindowSize());
    }

    public void Undo()
    {
        pointRef.position = oldPos;
        pointRef.UpdateVisualPosition(Program.instance.GetWindowSize());
    }
}