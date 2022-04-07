using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceLayerHandler : VoxelLayerHandler
{
    public VoxelType surfaceVoxelType; //Can set surface voxel to different voxels for different biomes
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset)
    {
        if(y == groundHeight)
        {
            Vector3Int pos = new Vector3Int(x, y, z);
            Chunk.SetVoxel(chunkData, pos, surfaceVoxelType);
            return true;
        }
        return false;
    }
}
