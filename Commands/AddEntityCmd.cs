using Microsoft.Xna.Framework.Graphics;

namespace ProdToolDOOM;

public class AddEntityCmd(Project projectRef, Vector2 initialPosition, Texture2D entityTexture, WindowInstance windowRef) : ICommand
{
    private Entity? entity;
    private int levelId;
    
    public void Execute()
    {
        if (projectRef.entityDatas.Count == 0 || projectRef.levels.Count == 0 ||
            projectRef.CurrentLevel > projectRef.levels.Count - 1)
            return;
        this.levelId = projectRef.CurrentLevel;
        this.entity ??= new Entity(this.levelId, entityTexture, windowRef, projectRef, projectRef.levels[this.levelId].levelObjectIdCounter++, initialPosition);
        this.entity.Init();
        
        Debug.Log($"Adding entity to level {this.levelId}!");
        
        if (projectRef.entityDatas.TryGetValue(this.entity.DataId, out EntityData? value))
            value.AddEntityRegistration(this.entity);
        projectRef.levels[this.levelId].Add(this.entity);
    }

    public void Undo()
    {
        if (this.entity == null)
            return;
        Debug.Log($"Removing entity from level {this.levelId}!");
        
        if (projectRef.entityDatas.TryGetValue(this.entity.DataId, out EntityData? value))
            value.RemoveEntityRegistration(this.entity);
        projectRef.levels[this.levelId].Remove(this.entity);
        
        if (this.entity.icon != null) this.entity.Hide();
    }
}