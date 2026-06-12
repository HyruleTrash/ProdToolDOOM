namespace DLLevelBuilder;

public class RemoveLevelCmd(Project project, int? id, Action<int?, Level?> onChanged) : ICommand
{
    private Level? level;
    
    public void Execute()
    {
        if (id == null || !project.levels.TryGetValue(id.Value, out Level? data))
            return;
        Debug.Log("removing level!");
        
        this.level ??= data;
        
        project.RemoveLevel(this.level);
        onChanged?.Invoke(this.level.LevelId, null);
    }

    public void Undo()
    { // TODO something is going wrong here
        if (id == null || this.level == null)
            return;
        
        Debug.Log("Adding level!");
        
        project.AddLevel(this.level);
        project.CurrentLevel = this.level.LevelId;
        onChanged?.Invoke(this.level.LevelId, this.level);
    }
}