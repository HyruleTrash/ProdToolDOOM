namespace ProdToolDOOM;

public class Entity : Level.Object
{
    public int Id { get => id; set => id = value; }
    private int id;

    public Entity(int id = -1)
    {
        this.id = id;
    }
    public Entity(Entity other)
    {
        id = other.Id;
        position = other.Position;
    }
}