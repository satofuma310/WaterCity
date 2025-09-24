using Unity.Mathematics;

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
}
