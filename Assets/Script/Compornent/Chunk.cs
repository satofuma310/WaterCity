using Unity.Entities;
using UnityEngine;
public struct CityChunk : IComponentData
{
    public Vector2Int position;
    public bool loaded;
    public bool unloadPending;
}

public struct CityChunkSetting : IComponentData
{
    public int width, height;
    public int loadingWidth, loadingHeight;
}
