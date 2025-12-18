namespace ProdToolDOOM;

public class AddLevelCmd(Project project) : ICommand
{
    public void Execute()
    {
        Debug.Log("Adding level!");
        project.levels.Add(new Level());
        project.currentLevel = project.levels.Count - 1;
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}