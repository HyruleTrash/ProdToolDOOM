namespace ProdToolDOOM;

public struct Vector2(float x, float y)
{
    public float x = x;
    public float y = y;

    public Vector2() : this(0, 0) { }
    public Vector2(Vector2 vector) : this(vector.x, vector.y) { }
    
    public float Magnitude => (float)Math.Sqrt(x * x + y * y);

    public Vector2 Normalized
    {
        get
        {
            float mag = Magnitude;
            if (mag > 0f)
                return new Vector2(this) / mag;
            return new Vector2(0f, 0f);
        }
    }

    /// <summary>
    /// Normalizes the current vector
    /// </summary>
    public void Normalize()
    {
        var mag = Magnitude;
        if (!(mag > 0f)) return;
        x /= mag;
        y /= mag;
    }

    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);
    public static Vector2 operator *(Vector2 a, float d) => new(a.x * d, a.y * d);
    public static Vector2 operator *(float d, Vector2 a) => new(a.x * d, a.y * d);
    public static Vector2 operator /(Vector2 a, float d) => new(a.x / d, a.y / d);

    // Equality check
    public static bool operator ==(Vector2 lhs, Vector2 rhs) => lhs.x == rhs.x && lhs.y == rhs.y;
    public static bool operator !=(Vector2 lhs, Vector2 rhs) => !(lhs == rhs);

    public override string ToString() => $"[{x},{y}]";

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