using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLayerHandler : VoxelLayerHandler
{
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset)
    {
        if(y > groundHeight)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetVoxel(chunkData, pos, VoxelType.Air);
            return true;
        }
        return false;
    }
}
