namespace ProdToolDOOM;

public class AddEntityCmd(Project project, Vector2 initialPosition) : ICommand
{
    private Entity? entity;
    private int? levelId;
    
    public void Execute()
    {
        if (project.entityDatas.Count == 0 || project.levels.Count == 0 ||
            project.CurrentLevel > project.levels.Count - 1)
            return;
        this.entity ??= new Entity(0, initialPosition);
        this.levelId ??= project.CurrentLevel;
        
        Debug.Log($"Adding entity to level {this.levelId}!");
        
        project.entityDatas[0].AddEntityRegistration(this.entity);
        project.levels[this.levelId.Value].Add(this.entity);
    }

    public void Undo()
    {
        if (this.entity == null || this.levelId == null)
            return;
        Debug.Log($"Removing entity from level {this.levelId}!");
        
        project.entityDatas[this.entity.DataId].RemoveEntityRegistration(this.entity);
        project.levels[this.levelId.Value].Remove(this.entity);
    }
}