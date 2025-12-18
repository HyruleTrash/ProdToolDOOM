namespace ProdToolDOOM;

public struct Vector2
{
    public float x;
    public float y;

    public Vector2()
    {
        x = 0;
        y = 0;
    }
    
    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2(Vector2 vector)
    {
        x = vector.x;
        y = vector.y;
    }

    public override string ToString()
    {
        return $"[{x},{y}]";
    }

    public static Vector2 FromString(string vector)
    {
        // Handle null input
        if (vector == null)
            throw new ArgumentNullException(nameof(vector));

        // Remove brackets and split on comma
        string content = vector.Trim('[', ' ').Trim(']', ' ');
        string[] parts = content.Split(',');

        // Validate we have exactly two parts
        if (parts.Length != 2)
            throw new FormatException("Invalid Vector2 format. Expected [x,y]");

        // Parse
        if (float.TryParse(parts[0].Trim(), out float x))
        {
            if (float.TryParse(parts[1].Trim(), out float y))
                return new Vector2(x, y);
        }

        return new Vector2(0, 0);
    }
}