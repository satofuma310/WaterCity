using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

public class GizmoViwer : MonoBehaviour
{
    [SerializeField]
    private bool
        drawChunk, drawRoadSection;
    [SerializeField]
    private float sectionSphereRadius;
    void OnDrawGizmos()
    {
        try
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            if (drawChunk)
            {
                using var chunkQuery =
                  manager.CreateEntityQuery(typeof(LocalTransform), typeof(CityChunk));
                var chunkComponentArray = chunkQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                var chunkEntityArray = chunkQuery.ToEntityArray(Allocator.Temp);
                var chunkSetting = manager.CreateEntityQuery(typeof(CityChunkSetting)).GetSingleton<CityChunkSetting>();
                for (int i = 0; i < chunkComponentArray.Length; i++)
                {
                    Gizmos.color = Color.green;
                    var position = chunkComponentArray[i];
                    var entity = chunkEntityArray[i];
                    var linePosition = new Vector3[]{
                      position.Position,
                      (Vector3)position.Position + new Vector3(chunkSetting.width,0,0),
                      (Vector3)position.Position + new Vector3(chunkSetting.width,0,chunkSetting.height),
                      (Vector3)position.Position + new Vector3(0,0,chunkSetting.height),
                    };
                    Gizmos.DrawLineStrip(linePosition, true);
                    if (drawRoadSection)
                    {
                        Gizmos.color = Color.red;
                        var buffer = manager.GetBuffer<RoadSection>(entity);
                        foreach (var section in buffer)
                        {
                            var worldSectionPosition = (Vector3)position.Position + new Vector3(section.position.x, 0, section.position.y);
                            Gizmos.DrawSphere(worldSectionPosition, sectionSphereRadius);
                        }
                    }
                }
            }
        }
        catch
        {
            return;
        }
    }
}
