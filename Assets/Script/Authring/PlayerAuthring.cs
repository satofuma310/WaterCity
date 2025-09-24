using Unity.Entities;
using UnityEngine;

class PlayerAuthring : MonoBehaviour
{
}

class PlayerAuthringBaker : Baker<PlayerAuthring>
{
    public override void Bake(PlayerAuthring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
        AddComponent(entity, new Player { });
    }
}
