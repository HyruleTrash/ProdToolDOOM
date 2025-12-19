namespace ProdToolDOOM;

public class AddEntityCmd(Project project) : ICommand
{
    private Entity? entity;
    private int? levelId;
    
    public void Execute()
    {
        if (project.entityDatas.Count == 0 || project.levels.Count == 0 ||
            project.currentLevel > project.levels.Count - 1)
            return;
        entity ??= new Entity(0);
        levelId ??= project.currentLevel;
        
        Debug.Log($"Adding entity to level {levelId}!");
        
        project.entityDatas[0].AddEntityRegistration(entity);
        project.levels[levelId.Value].Add(entity);
    }

    public void Undo()
    {
        if (entity == null || levelId == null)
            return;
        Debug.Log($"Removing entity from level {levelId}!");
        
        project.entityDatas[entity.Id].RemoveEntityRegistration(entity);
        project.levels[levelId.Value].Remove(entity);
    }
}