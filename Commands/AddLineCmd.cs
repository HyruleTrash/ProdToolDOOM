namespace ProdToolDOOM;

public class AddLineCmd(Project projectRef, Point point1, Point point2) : ICommand
{
    private Point[]? points;
    private Line? line;
    
    public void Execute()
    {
        var levelId = projectRef.CurrentLevel;
        points ??= [point1, point2];
        if (points == null || points.Length < 2)
            return;
        line ??= new Line(projectRef, points[0].LevelObjectId, points[1].LevelObjectId, levelId);
        
        Debug.Log($"Adding line to level {levelId}: {points[0].LevelObjectId}, {points[1].LevelObjectId}!");
        projectRef.levels[levelId].Add(line);
    }

    public void Undo()
    {
        if (line == null || points == null || points.Length < 2)
            return;
        Debug.Log($"Removing line from level {line.LevelId}!");
        
        projectRef.levels[line.LevelId].Remove(line);
        // if (point.icon != null) point.icon.Visible = false;
    }
}