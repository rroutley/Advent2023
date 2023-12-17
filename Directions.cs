using Point2D = (int x, int y);


enum Direction
{
    North, East, South, West
}


static class Directions
{
    public static readonly IReadOnlyDictionary<Direction, Point2D> Deltas = new Dictionary<Direction, Point2D>
    {
        [Direction.North] = (0, -1),
        [Direction.East] = (1, 0),
        [Direction.South] = (0, 1),
        [Direction.West] = (-1, 0),
    };


    public static Point2D From(this Direction direction, Point2D p)
    {
        return (p.x + Deltas[direction].x, p.y + Deltas[direction].y);
    }
}