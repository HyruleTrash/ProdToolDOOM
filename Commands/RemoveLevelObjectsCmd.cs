namespace DLLevelBuilder;

public class RemoveLevelObjectsCmd(Project project, Func<bool, LevelObject[]> getObjects) : ICommand
{
    private LevelObject[]? objects;
    private List<ICommand> actions = [];
    
    public void Execute()
    {
        this.objects ??= getObjects.Invoke(true);
        foreach (LevelObject obj in this.objects)
        {
            if (obj == null) continue;
            ICommand? cmd = obj switch
            {
                Line line => new RemoveLineCmd(project, line),
                Point point => new RemovePointCmd(project, point),
                Entity entity => new RemoveEntityCmd(project, entity),
                _ => null
            };

            if (cmd == null) continue;
            cmd.Execute();
            this.actions.Add(cmd);
        }
    }

    public void Undo()
    {
        foreach (ICommand action in this.actions) action.Undo();
    }
}