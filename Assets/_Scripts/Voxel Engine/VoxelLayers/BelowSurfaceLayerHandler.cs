using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BelowSurfaceLayerHandler : VoxelLayerHandler
{
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset, int waterLevel)
    {
        if(y < groundHeight)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetVoxel(chunkData, pos, VoxelType.Dirt);
            if (y < groundHeight-3)
            {
                Chunk.SetVoxel(chunkData, pos, VoxelType.Stone);
            }
            return true;
        }
        return false;
    }
}
