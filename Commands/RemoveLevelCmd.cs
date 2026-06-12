namespace DLLevelBuilder;

public class RemoveLevelCmd(Project project, Level level, Action<int?, Level?> onChanged) : ICommand
{
    public void Execute()
    {
        Debug.Log("removing level!");
        project.levels.Remove(level);
    }

    public void Undo()
    {
        Debug.Log("Adding level!");
        project.AddLevel(level);
        project.CurrentLevel = project.levels.Count - 1;
    }
}