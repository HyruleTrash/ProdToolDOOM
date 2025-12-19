namespace ProdToolDOOM;

public class AddPointCmd(Project project, Vector2 initialPosition) : ICommand
{
    private Point? point;
    private int? levelId;
    
    public void Execute()
    {
        if (project.levels.Count == 0 || project.currentLevel > project.levels.Count - 1)
            return;
        levelId ??= project.currentLevel;
        point ??= new Point(initialPosition);
        
        Debug.Log($"Adding point to level {levelId} {point.Position}!");
        project.levels[levelId.Value].Add(point);
    }

    public void Undo()
    {
        if (point == null || levelId == null)
            return;
        Debug.Log($"Removing point from level {levelId}!");
        
        project.levels[levelId.Value].Remove(point);
    }
}