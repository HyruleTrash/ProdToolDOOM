namespace ProdToolDOOM;

public class AddEntityDataCmd(Project project) : ICommand
{
    private EntityData? entityData;
    private int? id;
    
    public void Execute()
    {
        Debug.Log("Adding entity data!");

        this.entityData ??= new EntityData();
        this.id ??= project.entityDataIdCounter++;
        
        project.entityDatas.Add(this.id.Value, this.entityData);
        this.entityData.SetEntityRegistration(this.id.Value);
    }

    public void Undo()
    {
        if (this.entityData == null || this.id == null)
            return;
        
        project.entityDatas.Remove(this.id.Value);
        this.entityData.SetEntityRegistration(-1);
    }
}