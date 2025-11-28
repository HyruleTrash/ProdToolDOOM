namespace ProdToolDOOM;

public class Entity
{
    public float XPosition { get => xPos; set => xPos = value; }
    public float YPosition { get => yPos; set  => yPos = value; }
    public int Id { get => id; set => id = value; }
    
    private float xPos, yPos;
    private int id;

    public Entity() {}
    public Entity(int id)
    {
        this.id = id;
    }
    public Entity(Entity other)
    {
        id = other.Id;
        xPos = other.XPosition;
        yPos = other.YPosition;
    }
}