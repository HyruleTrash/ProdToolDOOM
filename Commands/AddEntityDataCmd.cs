namespace ProdToolDOOM;

public class AddEntityDataCmd(Project project) : ICommand
{
    public void Execute()
    {
        Debug.Log("Adding entity data!");
        project.entityDatas.Add(project.idCounter, new EntityData());
        project.idCounter++;
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}