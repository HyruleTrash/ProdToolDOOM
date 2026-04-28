namespace ProdToolDOOM;

public class AddLevelCmd(Project project) : ICommand
{
    private Level? level;
    
    public void Execute()
    {
        Debug.Log("Adding level!");
        this.level ??= new Level();
        project.levels.Add(this.level);
        project.CurrentLevel = project.levels.Count - 1;
    }

    public void Undo()
    {
        if (this.level is null)
            return;
        Debug.Log("removing level!");
        project.levels.Remove(this.level);
    }
}