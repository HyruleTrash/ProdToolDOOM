namespace ProdToolDOOM;

public class Level
{
    public List<Entity> Entities { get => entities;  set => entities = value; }
    private List<Entity> entities = [];
}