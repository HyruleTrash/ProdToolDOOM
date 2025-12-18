namespace ProdToolDOOM;

public class AddLevelCmd(Project project) : ICommand
{
    private Level? level;
    
    public void Execute()
    {
        Debug.Log("Adding level!");
        level ??= new Level();
        project.levels.Add(level);
        project.currentLevel = project.levels.Count - 1;
    }

    public void Undo()
    {
        if (level is null)
            return;
        Debug.Log("removing level!");
        project.levels.Remove(level);
    }
}