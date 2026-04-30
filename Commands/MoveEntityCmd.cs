namespace ProdToolDOOM;

public class MoveEntityCmd(Entity entityRef, Vector2 oldPos, Vector2 newPos) : ICommand
{
    public void Execute()
    {
        entityRef.position = newPos;
        entityRef.UpdateVisualPosition(Program.instance.GetWindowSize());
    }

    public void Undo()
    {
        entityRef.position = oldPos;
        entityRef.UpdateVisualPosition(Program.instance.GetWindowSize());
    }
}