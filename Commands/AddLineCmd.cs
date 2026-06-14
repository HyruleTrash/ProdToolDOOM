using System;
using DLLevelBuilder.Window;

namespace DLLevelBuilder;

public class AddLineCmd(Project projectRef, WindowInstance windowRef, Point point1, Point point2) : ICommand, IDisposable
{
    private Point[]? points;
    private Line? line;
    private Level level;

    public void Execute()
    {
        int levelId = projectRef.CurrentLevel;
        this.level = projectRef.levels[levelId];
        this.points ??= [point1, point2];
        if (this.points == null || this.points.Length < 2)
            return;
        this.line ??= new Line(projectRef, windowRef, this.level, this.points[0].LevelObjectId, this.points[1].LevelObjectId, levelId);
        this.line.Init();
        
        Debug.Log($"Adding line to level {levelId}: {this.points[0].LevelObjectId}, {this.points[1].LevelObjectId}!");
        this.level.Add(this.line);
    }

    public void Undo()
    {
        if (this.line == null || this.points == null || this.points.Length < 2)
            return;
        Debug.Log($"Removing line from level {this.line.LevelId}!");
        
        this.level.Remove(this.line);
        if (this.line.icon != null) this.line.Hide();
    }
    
    public void Dispose()
    {
        if (this.line?.icon?.Visible == false) this.line?.Dispose();
    }
}