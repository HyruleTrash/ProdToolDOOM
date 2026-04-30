namespace ProdToolDOOM;

/// <summary>
/// Used for the removal of a list of the same type of level object, this was made abstract to avoid duplicates
/// </summary>
/// <typeparam name="T">a level object, so a point, a line, or an entity</typeparam>
/// <typeparam name="TU">the command used for removing such a level object instance</typeparam>
public class RemoveLevelObjectArrayCmd<T, TU> : ICommand where T : Level.Object where TU : RemoveLevelObjectCmd<T>
{
    private T[]? levelObjects;
    private List<TU> actions = [];
    private readonly Project project;
    private readonly Func<bool, T[]> getLevelObjs;
    private readonly Func<Project, T, TU> cmdFactory;

    public RemoveLevelObjectArrayCmd(Project project, Func<bool, T[]> getLevelObjs, Func<Project, T, TU> cmdFactory)
    {
        this.project = project;
        this.getLevelObjs = getLevelObjs;
        this.cmdFactory = cmdFactory;
    }
    
    public void Execute()
    {
        this.levelObjects ??= this.getLevelObjs.Invoke(true);
        foreach (T obj in this.levelObjects)
        {
            TU cmd = this.cmdFactory.Invoke(this.project, obj);
            cmd.Execute();
            this.actions.Add(cmd);
        }
    }

    public void Undo()
    {
        foreach (TU action in this.actions) action.Undo();
    }
}