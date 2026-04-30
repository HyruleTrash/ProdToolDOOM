namespace ProdToolDOOM;

public class RemoveLevelObjectArrayCmd<T, U> : ICommand where T : Level.Object where U : RemoveLevelObjectCmd<T>
{
    private T[]? levelObjects;
    private List<U> actions = [];
    private readonly Project project;
    private readonly Func<bool, T[]> getLevelObjs;
    private readonly Func<Project, T, U> cmdFactory;

    public RemoveLevelObjectArrayCmd(Project project, Func<bool, T[]> getLevelObjs, Func<Project, T, U> cmdFactory)
    {
        this.project = project;
        this.getLevelObjs = getLevelObjs;
        this.cmdFactory = cmdFactory;
    }

    // private static class RemoveCmdFactory
    // {
    //     public static Dictionary<Type, Func<Project, T, U>> functions = new()
    //     {
    //         [typeof(RemovePointCmd)] = (prj, obj) => new RemovePointCmd(prj, (Point)obj), // Cannot cast expression of type 'T' to type 'Point'
    //         [typeof(RemoveLineCmd)] = (prj, obj) => new RemoveLineCmd(prj, (Line)obj),
    //         [typeof(RemoveEntityCmd)] = (prj, obj) => new RemoveEntityCmd(prj, (Entity)obj)
    //     };
    // }
    
    public void Execute()
    {
        this.levelObjects ??= this.getLevelObjs.Invoke(true);
        foreach (T obj in this.levelObjects)
        {
            U cmd = this.cmdFactory.Invoke(this.project, obj);
            cmd.Execute();
            this.actions.Add(cmd);
        }
    }

    public void Undo()
    {
        foreach (U action in this.actions) action.Undo();
    }
}