using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VoxelLayerHandler : MonoBehaviour
{
    [SerializeField]
    private VoxelLayerHandler Next;

    public bool Handle(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset)
    {
        if (TryHandling(chunkData, x, y, z, groundHeight, terrainOffset))
            return true;
        if (Next != null)
            return Next.Handle(chunkData, x, y, z, groundHeight, terrainOffset);
        return false;
    }

    protected abstract bool TryHandling(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset);
}
