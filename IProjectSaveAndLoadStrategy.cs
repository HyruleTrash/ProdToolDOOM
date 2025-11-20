namespace ProdToolDOOM;

public interface IProjectSaveAndLoadStrategy
{
    public bool Load(string path);
    public bool Save(string path);
}