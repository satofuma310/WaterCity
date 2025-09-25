using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
public partial struct ChunkGenerateSystem : ISystem
{
    private int2 prevPosition;
    private EntityManager manager;
    public void OnCreate(ref SystemState state)
    {
        prevPosition = new(-1, -1);
        manager = state.EntityManager;
    }
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<CityChunkSetting>())
            return;
        var setting = SystemAPI.GetSingleton<CityChunkSetting>();
        if (!SystemAPI.HasSingleton<Player>())
            return;
        var player = SystemAPI.GetSingletonEntity<Player>();
        var playerPosition = SystemAPI.GetComponent<LocalTransform>(player).Position;
        var center = new int2(
            (int)math.floor(playerPosition.x / setting.width),
            (int)math.floor(playerPosition.z / setting.height));
        if (prevPosition.Equals(center))
            return;
        prevPosition = center;
        var chunkMap = new NativeHashMap<int2, Entity>(setting.width * setting.height, Allocator.Temp);
        var index = 0;
        foreach (var chunk in SystemAPI.Query<RefRW<CityChunk>>().WithEntityAccess())
        {
            chunk.Item1.ValueRW.unloadPending = true;
            chunkMap.Add(chunk.Item1.ValueRO.position, chunk.Item2);
            index++;
        }
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var chunkLookup = SystemAPI.GetComponentLookup<CityChunk>();
        var chunkEntityArchetype = manager.CreateArchetype(typeof(LocalTransform), typeof(CityChunk));
        for (int y = center.y - setting.loadingHeight; y <= center.y + setting.loadingHeight; y++)
        {
            for (int x = center.x - setting.loadingWidth; x <= center.x + setting.loadingWidth; x++)
            {
                var mapPosition = new int2(x, y);
                if (chunkMap.ContainsKey(mapPosition))
                {
                    var data = chunkLookup[chunkMap[mapPosition]];
                    data.unloadPending = false;
                    ecb.SetComponent(chunkMap[mapPosition], data);
                    continue;
                }
                var worldChunkPosition = new float3(
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
                var chunkEntity = ecb.CreateEntity(chunkEntityArchetype);
                ecb.AddComponent(chunkEntity, chunkdata);
                ecb.SetName(chunkEntity, $"chunk:({worldChunkPosition.x},{worldChunkPosition.z})");
                ecb.AddComponent(chunkEntity, new LocalTransform
                {
                    Position = worldChunkPosition
                });
                //道の分岐点作成
                var roadSectionBuffer = ecb.AddBuffer<RoadSection>(chunkEntity);
                var seed = (uint)Util.Encode(x, y);
                var poissonDiscSampling = new PoissonDiskSampling(setting.width, setting.height, setting.radius, seed);
                var sectionPositions = poissonDiscSampling.Generate();
                foreach (var section in sectionPositions)
                {
                    roadSectionBuffer.Add(new RoadSection { position = section });
                }
                poissonDiscSampling.Dispose();
            }
        }
        ecb.Playback(manager);
        ecb.Dispose();
    }
    public void OnDestory(ref SystemState state)
    {
    }
}
