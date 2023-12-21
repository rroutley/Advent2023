using System.Data;
using Point2d = (int x, int y);

readonly struct Vector2d(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;



    public static Vector2d operator *(Vector2d lhs, int scaleFacor)
    {
        return new Vector2d(lhs.X * scaleFacor, lhs.Y * scaleFacor);
    }

    public static Point2d operator +(Point2d lhs, Vector2d vector)
    {
        return new Point2d(lhs.x + vector.X, lhs.y + vector.Y);
    }

}