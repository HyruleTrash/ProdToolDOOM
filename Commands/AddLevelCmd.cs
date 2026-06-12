namespace DLLevelBuilder;

public class AddLevelCmd(Project project) : ICommand
{
    private Level? level;
    
    public void Execute()
    {
        Debug.Log("Adding level!");
        this.level ??= new Level(project.GetLowestUnusedLevelId());
        project.AddLevel(this.level);
        project.CurrentLevel = this.level.LevelId;
    }

    public void Undo()
    {
        if (this.level is null)
            return;
        Debug.Log("removing level!");
        project.RemoveLevel(this.level);
    }
}