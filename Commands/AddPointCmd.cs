using System;
using DLLevelBuilder.Window;
using Microsoft.Xna.Framework.Graphics;

namespace DLLevelBuilder;

public class AddPointCmd(Project projectRef, Vector2 initialPosition, Texture2D pointTexture, WindowInstance windowRef) : ICommand, IDisposable
{
    private Point? point;
    private Level level;

    public void Execute()
    {
        if (projectRef.levels.Count == 0 || projectRef.CurrentLevel > projectRef.levels.Count - 1)
            return;
        int levelId = projectRef.CurrentLevel;
        this.level = projectRef.levels[levelId];
        this.point ??= new Point(initialPosition, pointTexture, this.level.levelObjectIdCounter++, levelId, windowRef, projectRef, this.level);
        this.point.Init();
        
        Debug.Log($"Adding point to level {levelId} {this.point.position}!");
        this.level.Add(this.point);
    }

    public void Undo()
    {
        if (this.point == null)
            return;
        Debug.Log($"Removing point from level {this.point.LevelId}!");
        
        this.level.Remove(this.point);
        if (this.point.icon != null) this.point.Hide();
    }

    public void Dispose()
    {
        if (this.point?.icon?.Visible == false) this.point?.Dispose();
    }
}