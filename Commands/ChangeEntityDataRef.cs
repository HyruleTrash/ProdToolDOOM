namespace DLLevelBuilder;

public class ChangeEntityDataRef(Project projectRef, Entity tempEntity, int? newId) : ICommand
{
    int? oldId;
    
    public void Execute()
    {
        if (newId == null)
            return;
        this.oldId ??= tempEntity.DataId;
        Debug.Log($"Updating entity {tempEntity.DataId} to {newId}!");
        tempEntity.DataId = newId.Value;
        if (projectRef.EntityDatas.TryGetValue(newId.Value, out EntityData? value))
            value.AddEntityRegistration(tempEntity);
        if (projectRef.EntityDatas.TryGetValue(this.oldId.Value, out EntityData? value2))
            value2.RemoveEntityRegistration(tempEntity);
        tempEntity.UpdateName();
    }

    public void Undo()
    {
        if (newId == null || this.oldId == null)
            return;
        Debug.Log($"Updating entity back to {tempEntity.DataId} from {newId}!");
        tempEntity.DataId = this.oldId.Value;
        if (projectRef.EntityDatas.TryGetValue(newId.Value, out EntityData? value))
            value.RemoveEntityRegistration(tempEntity);
        if (projectRef.EntityDatas.TryGetValue(this.oldId.Value, out EntityData? value2))
            value2.AddEntityRegistration(tempEntity);
        tempEntity.UpdateName();
    }
}