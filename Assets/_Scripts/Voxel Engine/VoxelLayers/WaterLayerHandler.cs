using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayerHandler : VoxelLayerHandler
{
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset, int waterLevel)
    {
        if(y > groundHeight && y <= waterLevel)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetVoxel(chunkData, pos, VoxelType.Water);
            return true;
        }
        return false;
    }
}
