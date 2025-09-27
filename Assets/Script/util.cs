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
    private static uint IntToNat(int z)
    {
        return z >= 0 ? (uint)z * 2 : (uint)(-z) * 2 - 1;
    }

    private static int NatToInt(uint n)
    {
        if (n % 2 == 0) return (int)(n / 2);
        else return -(int)((n + 1) / 2);
    }

    private static uint CantorPair(uint k, uint l)
    {
        return ((k + l) * (k + l + 1)) / 2 + l;
    }

    private static (uint, uint) CantorUnpair(uint z)
    {
        uint w = (uint)math.floor((math.sqrt(8 * (double)z + 1) - 1) / 2);
        uint t = (w * (w + 1)) / 2;
        uint l = z - t;
        uint k = w - l;
        return (k, l);
    }

    public static uint Encode(int n1, int n2)
    {
        uint g1 = IntToNat(n1);
        uint g2 = IntToNat(n2);
        return CantorPair(g1, g2);
    }

    public static (int, int) Decode(uint m)
    {
        var (g1, g2) = CantorUnpair(m);
        int n1 = NatToInt(g1);
        int n2 = NatToInt(g2);
        return (n1, n2);
    }
}
