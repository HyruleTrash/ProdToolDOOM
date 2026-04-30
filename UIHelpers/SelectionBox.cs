namespace ProdToolDOOM;

public class SelectionBox
{
    public Vector2 center;
    public Vector2 size;

    public SelectionBox()
    {
        this.center = Vector2.Zero;
        this.size = Vector2.Zero;
    }
    
    public SelectionBox(Vector2 center, Vector2 size)
    {
        this.center = center;
        this.size = size;
    }

    public SelectionBox(Vector2 center, float width, float height)
    {
        this.center = center;
        this.size = new Vector2(width, height);
    }

    public bool IsInsideBounds(Vector2 point)
    {
        float halfWidth = this.size.x * 0.5f;
        float halfHeight = this.size.y * 0.5f;

        bool insideWidth = point.x >= this.center.x - halfWidth && point.x <= this.center.x + halfWidth;
        bool insideHeight = point.y >= this.center.y - halfHeight && point.y <= this.center.y + halfHeight;
        
        return insideWidth && insideHeight;
    }
}