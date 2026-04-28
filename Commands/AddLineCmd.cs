namespace ProdToolDOOM;

public class AddLineCmd(Project projectRef, Point point1, Point point2) : ICommand, IDisposable
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
        this.line.Init();

        this.points[0].onDispose += this.line.EvaluateVisibility;
        this.points[0].onShowEvent += this.line.EvaluateVisibility;
        this.points[0].onHideEvent += this.line.EvaluateVisibility;
        this.points[1].onDispose += this.line.EvaluateVisibility;
        this.points[1].onShowEvent += this.line.EvaluateVisibility;
        this.points[1].onHideEvent += this.line.EvaluateVisibility;
        
        Debug.Log($"Adding line to level {levelId}: {this.points[0].LevelObjectId}, {this.points[1].LevelObjectId}!");
        projectRef.levels[levelId].Add(this.line);
    }

    public void Undo()
    {
        if (this.line == null || this.points == null || this.points.Length < 2)
            return;
        Debug.Log($"Removing line from level {this.line.LevelId}!");
        
        this.points[0].onDispose -= this.line.EvaluateVisibility;
        this.points[0].onShowEvent -= this.line.EvaluateVisibility;
        this.points[0].onHideEvent -= this.line.EvaluateVisibility;
        this.points[1].onDispose -= this.line.EvaluateVisibility;
        this.points[1].onShowEvent -= this.line.EvaluateVisibility;
        this.points[1].onHideEvent -= this.line.EvaluateVisibility;
        
        projectRef.levels[this.line.LevelId].Remove(this.line);
        if (this.line.icon != null) this.line.Hide();
    }
    
    public void Dispose()
    {
        if (this.line?.icon?.Visible == false) this.line?.Dispose();
    }
}