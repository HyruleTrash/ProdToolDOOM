namespace ProdToolDOOM;

public class Entity : Level.Object
{
    public int Id { get => id; set => id = value; }
    private int id;

    public Entity(int id = -1, Vector2? position = null)
    {
        this.id = id;
        if (position is not null) 
            this.position = position;
    }
    public Entity(Entity other)
    {
        id = other.Id;
        position = other.Position;
    }
}