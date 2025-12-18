namespace ProdToolDOOM;

public class AddEntityCmd(Project project) : ICommand
{
    public void Execute()
    {
        if (project.entityDatas.Count == 0 || project.levels.Count == 0 ||
            project.currentLevel > project.levels.Count - 1)
            return;
        Debug.Log($"Adding entity to level{project.currentLevel}!");
        project.levels[project.currentLevel].Entities.Add(new Entity(0));
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}