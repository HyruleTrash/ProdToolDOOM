namespace ProdToolDOOM;

public class RemoveEntityDataCmd(Project projectRef, int? id, Action<int?, EntityData?> onChanged) : ICommand, IDisposable
{
    private EntityData? entityData;

    public void Execute()
    {
        if (id == null || !projectRef.EntityDatas.TryGetValue(id.Value, out EntityData? data))
            return;

        Debug.Log($"Removing entity data {data.Name}");
        
        this.entityData ??= data;

        projectRef.RemoveEntityData(id.Value);
        this.entityData.SetEntityRegistration(-1);
        onChanged?.Invoke(id, null);
    }

    public void Undo()
    {
        if (id == null || this.entityData == null)
            return;
        
        Debug.Log($"Adding entity data {this.entityData.Name} back");

        projectRef.AddEntityData(id.Value, this.entityData);
        this.entityData.SetEntityRegistration(id.Value);
        onChanged?.Invoke(id, this.entityData);
    }

    public void Dispose() => this.entityData = null;
}