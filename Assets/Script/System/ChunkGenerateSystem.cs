using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
public partial struct ChunkGenerateSystem : ISystem
{
    private Vector2Int prevPosition;
    public void OnCreate(ref SystemState state)
    {
        prevPosition = new(-1, -1);
    }
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (!SystemAPI.HasSingleton<CityChunkSetting>())
            return;
        var setting = SystemAPI.GetSingleton<CityChunkSetting>();
        if (!SystemAPI.HasSingleton<Player>())
            return;
        var player = SystemAPI.GetSingletonEntity<Player>();
        var playerPosition = SystemAPI.GetComponent<LocalTransform>(player).Position;
        var center = new Vector2Int(
            Mathf.FloorToInt(playerPosition.x / setting.width),
            Mathf.FloorToInt(playerPosition.z / setting.height));
        if (prevPosition.Equals(center))
            return;
        prevPosition = center;
        var chunkMap = new NativeHashMap<Vector2Int, Entity>(setting.width * setting.height, Allocator.Temp);
        var index = 0;
        foreach (var chunk in SystemAPI.Query<RefRW<CityChunk>>().WithEntityAccess())
        {
            chunk.Item1.ValueRW.unloadPending = true;
            chunkMap.Add(chunk.Item1.ValueRO.position, chunk.Item2);
            index++;
        }
        for (int y = center.y - setting.loadingHeight; y <= center.y + setting.loadingHeight; y++)
        {
            for (int x = center.x - setting.loadingWidth; x <= center.x + setting.loadingWidth; x++)
            {
                var mapPosition = new Vector2Int(x, y);
                if (chunkMap.ContainsKey(mapPosition))
                {
                    var data = entityManager.GetComponentData<CityChunk>(chunkMap[mapPosition]);
                    data.unloadPending = false;
                    entityManager.SetComponentData(chunkMap[mapPosition], data);
                    continue;
                }
                var worldChunkPosition = new Vector3(
                       x * setting.width,
                       0,
                       y * setting.height
                     );
                var chunkdata = new CityChunk
                {
                    position = mapPosition,
                    loaded = false,
                    unloadPending = false,
                };
                var chunkEntity = entityManager.CreateEntity(
                         typeof(LocalTransform),
                         typeof(CityChunk)
                     );
                entityManager.AddComponentData(chunkEntity, chunkdata);
                entityManager.SetName(chunkEntity, $"chunk:({worldChunkPosition.x},{worldChunkPosition.z})");
                entityManager.AddComponentData(chunkEntity, new LocalTransform
                {
                    Position = worldChunkPosition
                });
            }
        }
    }
    public void OnDestory(ref SystemState state)
    {
    }
}
