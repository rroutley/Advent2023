
readonly  record struct Vector3d(double X, double Y, double Z)
{
    public static Vector3d operator *(Vector3d lhs, double scaleFacor)
    {
        return new Vector3d(lhs.X * scaleFacor, lhs.Y * scaleFacor, lhs.Z * scaleFacor);
    }

    public static Vector3d operator +(Vector3d lhs, Vector3d vector)
    {
        return new Vector3d(lhs.X + vector.X, lhs.Y + vector.Y, lhs.Z + vector.Z);
    }

    internal static Vector3d Parse(string pos)
    {
        var (x, y, z) = pos.Split(',', 3).Select(long.Parse).ToArray();
        return new Vector3d(x, y, z);
    }
}