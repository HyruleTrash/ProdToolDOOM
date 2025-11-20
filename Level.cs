namespace ProdToolDOOM;

public class Level
{
    public List<Entity> Entities { get => entities; }
    private List<Entity> entities;

    public Level()
    {
        entities = new List<Entity>();
        entities.Add(new Entity());
    }
}