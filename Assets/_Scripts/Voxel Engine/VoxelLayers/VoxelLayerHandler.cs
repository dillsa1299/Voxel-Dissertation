using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VoxelLayerHandler : MonoBehaviour
{
    [SerializeField]
    private VoxelLayerHandler Next;

    public bool Handle(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset, int waterLevel)
    {
        if (TryHandling(chunkData, x, y, z, groundHeight, terrainOffset, waterLevel))
            return true;
        if (Next != null)
            return Next.Handle(chunkData, x, y, z, groundHeight, terrainOffset, waterLevel);
        return false;
    }

    protected abstract bool TryHandling(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset, int waterLevel);
}
