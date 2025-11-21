namespace ProdToolDOOM;

public class Entity
{
    public float XPosition { get => xPos; }
    public float YPosition { get => yPos; }
    public float Id { get => id; }
    
    private float xPos, yPos;
    private int id;

    public Entity(int id)
    {
        this.id = id;
    }
}