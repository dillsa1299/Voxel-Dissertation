using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceLayerHandler : VoxelLayerHandler
{
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset, int waterLevel)
    {
        if(y == groundHeight)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            if (y <= waterLevel)
            {
                Chunk.SetVoxel(chunkData, pos, VoxelType.Sand);
                return true;
            }
            Chunk.SetVoxel(chunkData, pos, VoxelType.Grass);
            return true;
        }
        return false;
    }
}
