using Unity.Entities;
using Unity.Mathematics;
public struct CityChunk : IComponentData
{
    public int2 position;
    public bool loaded;
    public bool unloadPending;
}
[InternalBufferCapacity(0)]
public struct RoadSection : IBufferElementData
{
    public float2 position;
}

public struct CityChunkSetting : IComponentData
{
    public int width, height;
    public float radius;
    public int loadingWidth, loadingHeight;
}
