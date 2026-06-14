namespace DLLevelBuilder;

public class RemoveLevelCmd(Project project, int id, Level level, Action<int?, Level?> onChanged, bool isSilent = false) : ICommand
{
    public void Execute()
    {
        if (!project.levels.TryGetValue(id, out Level? data))
            return;
        Debug.Log($"removing level {data.LevelId}");
        
        project.RemoveLevel(level);
        onChanged?.Invoke(level.LevelId, null);
        
        if (project.CurrentLevel == id && !isSilent) project.SetLevelNearestId(); // silent removal is used for replacing, this func might create a required level 0, so preventing edge case here
    }

    public void Undo()
    {
        if (project.levels.TryGetValue(id, out Level? data))
        {
            Debug.Log($"Sad silent removal of new level {id}");
            new RemoveLevelCmd(project, id, data, (_, _) => { }, true).Execute();
        }
        
        Debug.Log("ReAdding level!");
        
        project.AddLevel(level);
        onChanged?.Invoke(level.LevelId, level);
    }
}