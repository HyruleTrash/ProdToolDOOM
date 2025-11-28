namespace ProdToolDOOM;

public class Entity
{
    public Vector2 Position { get => position; set  => position = value; }
    public int Id { get => id; set => id = value; }
    
    private Vector2 position;
    private int id;

    public Entity() {}
    public Entity(int id)
    {
        this.id = id;
    }
    public Entity(Entity other)
    {
        id = other.Id;
        position = other.Position;
    }
}