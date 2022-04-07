using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayerHandler : VoxelLayerHandler
{
    public int waterLevel = 8;
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset)
    {
        if(y > groundHeight && y <= waterLevel)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetVoxel(chunkData, pos, VoxelType.Water);
            if(y == groundHeight + 1)
            {
                pos.y = groundHeight;
                Chunk.SetVoxel(chunkData, pos, VoxelType.Sand);
            }
            return true;
        }
        return false;
    }
}
