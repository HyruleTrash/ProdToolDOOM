namespace ProdToolDOOM;

public class Entity : Level.Object
{
    public int DataId { get => this.dataDataId; set => this.dataDataId = value; }
    private int dataDataId;
    public Vector2 Position { get => this.position; set => this.position = value; }

    public Entity(int dataDataId = -1, Vector2? position = null)
    {
        this.dataDataId = dataDataId;
        if (position is not null) 
            this.position = position;
    }
    public Entity(Entity other)
    {
        this.dataDataId = other.DataId;
        this.position = other.position;
    }

    protected override void OnShow()
    {
        // throw new NotImplementedException();
    }

    protected override void OnHide()
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