
public class PortableBitmap<T>
{
    private readonly T[,] canvas;
    private readonly T maxValue;
    private readonly string comment;
    private readonly int height;
    private readonly int width;

    public PortableBitmap(T[,] canvas, T maxValue, string commment)
    {
        this.canvas = canvas;
        this.maxValue = maxValue;
        this.comment = commment;

        this.height = canvas.GetLength(1);
        this.width = canvas.GetLength(0);
    }


    public void SaveAsPbm(string fileName)
    {
        fileName = Path.ChangeExtension(fileName, ".pbm");
        using var writer = File.CreateText(fileName);
        writer.NewLine = "\n";

        writer.WriteLine("P1");
        writer.WriteLine("# " + this.comment);
        writer.WriteLine($"{width} {height}");
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                writer.Write(canvas[c, r]);
            }
            writer.WriteLine();
        }
    }

    public void SaveAsPgm(string fileName)
    {
        fileName = Path.ChangeExtension(fileName, ".pgm");
        using var writer = File.CreateText(fileName);
        writer.NewLine = "\n";

        writer.WriteLine("P2");
        writer.WriteLine("# " + this.comment);
        writer.WriteLine($"{width} {height}");
        writer.WriteLine(maxValue); // Maximum greyscale value
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                writer.Write(canvas[c, r]);
                writer.Write(" ");
            }
            writer.WriteLine();
        }
    }
}