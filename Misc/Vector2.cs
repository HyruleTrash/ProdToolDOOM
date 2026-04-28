namespace ProdToolDOOM;

public class Vector2
{
    public static Vector2 Zero => new(0f, 0f);
    
    public float x;
    public float y;

    public Vector2() : this(0, 0) { }
    public Vector2(float x, float y) { this.x = x; this.y = y; }
    public Vector2(Vector2 vector) : this(vector.x, vector.y) { }
    public Vector2(Microsoft.Xna.Framework.Point vector) : this(vector.X, vector.Y) { }
    
    public float Magnitude => (float)Math.Sqrt(this.x * this.x + this.y * this.y);

    public Vector2 Normalized
    {
        get
        {
            float mag = this.Magnitude;
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
        float mag = this.Magnitude;
        if (!(mag > 0f)) return;
        this.x /= mag;
        this.y /= mag;
    }

    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);
    public static Vector2 operator *(Vector2 a, float d) => new(a.x * d, a.y * d);
    public static Vector2 operator *(float d, Vector2 a) => new(a.x * d, a.y * d);
    public static Vector2 operator /(Vector2 a, float d) => new(a.x / d, a.y / d);
    public static Vector2 operator -(Vector2 a) => new(-a.x, -a.y);

    // Equality check
    public static bool operator ==(Vector2 lhs, Vector2 rhs) => lhs.x == rhs.x && lhs.y == rhs.y;
    public static bool operator !=(Vector2 lhs, Vector2 rhs) => !(lhs == rhs);

    public override string ToString() => $"[{this.x},{this.y}]";

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
    
    protected bool Equals(Vector2 other)
    {
        return this.x.Equals(other.x) && this.y.Equals(other.y);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Vector2)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.x, this.y);
    }

    public static float GetDistance(Vector2 a, Vector2 b)
    {
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    public static Vector2 GetDirection(Vector2 a, Vector2 b)
    {
        return (b - a).Normalized;
    }
}