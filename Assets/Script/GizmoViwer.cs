using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

public class GizmoViwer : MonoBehaviour
{
    public bool
      drawChunk;
    void OnDrawGizmos()
    {
        try
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            if (drawChunk)
            {
                using var chunkQuery =
                  manager.CreateEntityQuery(typeof(LocalTransform), typeof(CityChunk))
                  .ToComponentDataArray<LocalTransform>(Allocator.Temp);
                var chunkSetting = manager.CreateEntityQuery(typeof(CityChunkSetting)).GetSingleton<CityChunkSetting>();
                Gizmos.color = Color.green;
                foreach (var position in chunkQuery)
                {
                    var linePosition = new Vector3[]{
            position.Position,
            (Vector3)position.Position + new Vector3(chunkSetting.width,0,0),
            (Vector3)position.Position + new Vector3(chunkSetting.width,0,chunkSetting.height),
            (Vector3)position.Position + new Vector3(0,0,chunkSetting.height),
         };
                    Gizmos.DrawLineStrip(linePosition, true);
                }
            }
        }
        catch
        {
            return;
        }
    }
}
