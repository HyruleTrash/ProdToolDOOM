namespace ProdToolDOOM;

public class Entity : Level.Object
{
    public int DataId { get => dataDataId; set => dataDataId = value; }
    private int dataDataId;
    public Vector2 Position { get => position; set => position = value; }

    public Entity(int dataDataId = -1, Vector2? position = null)
    {
        this.dataDataId = dataDataId;
        if (position is not null) 
            this.position = position;
    }
    public Entity(Entity other)
    {
        dataDataId = other.DataId;
        position = other.position;
    }

    public override void Hide()
    {
        // throw new NotImplementedException();
    }

    public override void ShowSelectionVisual()
    {
        // throw new NotImplementedException();
    }

    public override void HideSelectionVisual()
    {
        // throw new NotImplementedException();
    }
}