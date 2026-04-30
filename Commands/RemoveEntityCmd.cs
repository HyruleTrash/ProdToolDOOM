namespace ProdToolDOOM;

public class RemoveEntityCmd : RemoveLevelObjectCmd<Entity>
{
    public RemoveEntityCmd(Project proj, Entity tempEntity, Action? onExecuted = null) : base(proj, tempEntity, onExecuted) { }

    protected override void OnExecute()
    {
        Debug.Log($"Removing entity from level {this.levelObj.LevelId}!");
        this.projectRef.levels[this.levelObj.LevelId].Remove(this.levelObj);
        if (this.projectRef.entityDatas.TryGetValue(this.levelObj.DataId, out EntityData? value))
            value.RemoveEntityRegistration(this.levelObj);
        if (this.levelObj.icon != null) this.levelObj.Hide();
    }

    protected override void OnUndo()
    {
        Debug.Log($"Adding entity to level {this.levelObj.LevelId}!");
        this.projectRef.levels[this.levelObj.LevelId].Add(this.levelObj);
        if (this.projectRef.entityDatas.TryGetValue(this.levelObj.DataId, out EntityData? value))
            value.AddEntityRegistration(this.levelObj);
        if (this.levelObj.icon != null) this.levelObj.Show();
    }
}