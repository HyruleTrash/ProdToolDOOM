namespace ProdToolDOOM;

public class SelectionBox
{
    public Vector2 center;
    public Vector2 size;

    public SelectionBox()
    {
        center = Vector2.Zero;
        size = Vector2.Zero;
    }
    
    public SelectionBox(Vector2 center, Vector2 size)
    {
        this.center = center;
        this.size = size;
    }

    public SelectionBox(Vector2 center, float width, float height)
    {
        this.center = center;
        size = new Vector2(width, height);
    }

    public bool IsInsideBounds(Vector2 point)
    {
        float halfWidth = size.x * 0.5f;
        float halfHeight = size.y * 0.5f;

        bool insideWidth = point.x >= center.x - halfWidth && point.x <= center.x + halfWidth;
        bool insideHeight = point.y >= center.y - halfHeight && point.y <= center.y + halfHeight;
        
        return insideWidth && insideHeight;
    }
}