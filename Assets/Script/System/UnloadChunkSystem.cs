using Unity.Burst;
using Unity.Entities;
using Unity.Collections;

partial struct UnloadChunkSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entitymanager = state.EntityManager;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var chunk in SystemAPI.Query<CityChunk>().WithEntityAccess())
        {
            if (!chunk.Item1.unloadPending)
                continue;
            ecb.DestroyEntity(chunk.Item2);
        }
        ecb.Playback(entitymanager);
        ecb.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}
