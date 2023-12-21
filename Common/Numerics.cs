
public static class Numerics
{
    public static int Gcd(int a, int b)
    {
        return (int)Gcd((long)a, (long)b);
    }

    public static long Gcd(long a, long b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a | b;
    }

    public static long Lcm(long x, long y)
    {
        return x * y / Numerics.Gcd(x, y);
    }
}