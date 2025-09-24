using UnityEngine;
using Unity.Entities;
public class CityChunkSettingAuthring : MonoBehaviour
{
    [field: SerializeField]
    public int width { get; private set; }
    [field: SerializeField]
    public int height { get; private set; }
    [field: SerializeField]
    public int loadingWidth { get; private set; }
    [field: SerializeField]
    public int loadingHeight { get; private set; }
    class Baker : Baker<CityChunkSettingAuthring>
    {
        public override void Bake(CityChunkSettingAuthring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new CityChunkSetting
            {
                width = authoring.width,
                height = authoring.height,
                loadingWidth = authoring.loadingWidth,
                loadingHeight = authoring.loadingHeight,
            });
        }
    }
}

