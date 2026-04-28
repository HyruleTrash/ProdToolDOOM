namespace ProdToolDOOM;

public class AddLineCmd(Project projectRef, Point point1, Point point2) : ICommand
{
    private Point[]? points;
    private Line? line;
    
    public void Execute()
    {
        int levelId = projectRef.CurrentLevel;
        this.points ??= [point1, point2];
        if (this.points == null || this.points.Length < 2)
            return;
        this.line ??= new Line(projectRef, this.points[0].LevelObjectId, this.points[1].LevelObjectId, levelId);
        
        Debug.Log($"Adding line to level {levelId}: {this.points[0].LevelObjectId}, {this.points[1].LevelObjectId}!");
        projectRef.levels[levelId].Add(this.line);
    }

    public void Undo()
    {
        if (this.line == null || this.points == null || this.points.Length < 2)
            return;
        Debug.Log($"Removing line from level {this.line.LevelId}!");
        
        projectRef.levels[this.line.LevelId].Remove(this.line);
        // if (point.icon != null) point.icon.Visible = false;
    }
}