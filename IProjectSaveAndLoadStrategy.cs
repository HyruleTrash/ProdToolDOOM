namespace ProdToolDOOM;

public interface IProjectLoadStrategy
{
    public bool Load(string path);
}

public interface IProjectSaveStrategy
{
    public bool Save(string path);
}