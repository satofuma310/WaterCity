using Unity.Mathematics;
using Unity.Burst;
[BurstCompile]
public class Util
{
    public static float Distance(float2 a, float2 b)
    {
        float2 diff = a - b;
        return math.sqrt(math.dot(diff, diff));
    }
    public static int Int2ToInt(int x, int y, int width)
    {
        return x + y * width;
    }
    private static long IntToNat(int z)
    {
        return z >= 0 ? (long)z * 2 : (long)(-z) * 2 - 1;
    }

    private static int NatToInt(long n)
    {
        if (n % 2 == 0) return (int)(n / 2);
        else return -(int)((n + 1) / 2);
    }

    private static long CantorPair(long k, long l)
    {
        return ((k + l) * (k + l + 1)) / 2 + l;
    }

    private static (long, long) CantorUnpair(long z)
    {
        long w = (long)math.floor((math.sqrt(8 * (double)z + 1) - 1) / 2);
        long t = (w * (w + 1)) / 2;
        long l = z - t;
        long k = w - l;
        return (k, l);
    }

    public static long Encode(int n1, int n2)
    {
        long g1 = IntToNat(n1);
        long g2 = IntToNat(n2);
        return CantorPair(g1, g2);
    }

    public static (int, int) Decode(long m)
    {
        var (g1, g2) = CantorUnpair(m);
        int n1 = NatToInt(g1);
        int n2 = NatToInt(g2);
        return (n1, n2);
    }
}
