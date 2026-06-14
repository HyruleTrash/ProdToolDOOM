using DLLevelBuilder.Window;
using Microsoft.Xna.Framework.Graphics;

namespace DLLevelBuilder;

public class AddEntityCmd(Project projectRef, Vector2 initialPosition, Texture2D entityTexture, WindowInstance windowRef) : ICommand
{
    private Entity? entity;
    private int levelId;
    private Level level;

    public void Execute()
    {
        if (projectRef.EntityDatas.Count == 0 || projectRef.levels.Count == 0 ||
            projectRef.CurrentLevel > projectRef.levels.Count - 1)
            return;
        this.levelId = projectRef.CurrentLevel;
        this.level = projectRef.levels[this.levelId];
        this.entity ??= new Entity(this.levelId, entityTexture, windowRef, projectRef, this.level, this.level.levelObjectIdCounter++, initialPosition);
        this.entity.Init();
        
        Debug.Log($"Adding entity to level {this.levelId}!");
        
        if (projectRef.EntityDatas.TryGetValue(this.entity.DataId, out EntityData? value))
            value.AddEntityRegistration(this.entity);
        this.level.Add(this.entity);
    }

    public void Undo()
    {
        if (this.entity == null)
            return;
        Debug.Log($"Removing entity from level {this.levelId}!");
        
        if (projectRef.EntityDatas.TryGetValue(this.entity.DataId, out EntityData? value))
            value.RemoveEntityRegistration(this.entity);
        this.level.Remove(this.entity);
        
        if (this.entity.icon != null) this.entity.Hide();
    }
}