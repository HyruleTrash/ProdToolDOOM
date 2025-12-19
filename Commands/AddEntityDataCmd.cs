namespace ProdToolDOOM;

public class AddEntityDataCmd(Project project) : ICommand
{
    private EntityData? entityData;
    private int? id;
    
    public void Execute()
    {
        Debug.Log("Adding entity data!");
        
        entityData ??= new EntityData();
        id ??= project.idCounter++;
        
        project.entityDatas.Add(id.Value, entityData);
        entityData.SetEntityRegistration(id.Value);
    }

    public void Undo()
    {
        if (entityData == null || id == null)
            return;
        
        project.entityDatas.Remove(id.Value);
        entityData.SetEntityRegistration(-1);
    }
}