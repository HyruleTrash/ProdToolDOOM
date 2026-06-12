namespace DLLevelBuilder;

public class RemoveLevelCmd(Project project, Level level, Action<int?, Level?> onChanged) : ICommand
{
    public void Execute()
    {
        Debug.Log("removing level!");
        project.levels.Remove(level.LevelId);
    }

    public void Undo()
    {
        Debug.Log("Adding level!");
        project.AddLevel(level);
        project.CurrentLevel = level.LevelId;
    }
}