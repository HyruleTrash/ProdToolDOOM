namespace ProdToolDOOM;

public abstract class RemoveLevelObjectCmd<T> : ICommand where T : Level.Object
{
    protected T? levelObj;
    protected readonly Project projectRef;
    private readonly T tempLevelObj;
    private readonly Action? onExecuted;

    protected RemoveLevelObjectCmd(Project projectRef, T tempLevelObj, Action? onExecuted = null)
    {
        this.projectRef = projectRef;
        this.tempLevelObj = tempLevelObj;
        this.onExecuted = onExecuted;
    }

    public void Execute()
    {
        if (this.tempLevelObj == null)
            return;
        this.levelObj ??= this.tempLevelObj;

        OnExecute();
        
        this.onExecuted?.Invoke();
    }

    protected abstract void OnExecute();

    public void Undo()
    {
        if (this.projectRef.levels.Count == 0 || this.projectRef.CurrentLevel > this.projectRef.levels.Count - 1)
            return;

        OnUndo();
    }
    
    protected abstract void OnUndo();
}