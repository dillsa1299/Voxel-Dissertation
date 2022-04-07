using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneLayer : VoxelLayerHandler
{
    [Range(0, 1)]
    public float stoneThreshold = 0.5f;

    [SerializeField]
    private NoiseData stoneNoiseData;

    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset)
    {
        if(chunkData.worldPos.y > groundHeight)
            return false;
        
        stoneNoiseData.terrainOffset = terrainOffset;
        float stoneNoise = CustomNoise.OctavePerlin(chunkData.worldPos.x + x, chunkData.worldPos.z + z, stoneNoiseData);

        int endPos = groundHeight;
        if (chunkData.worldPos.y < 0)
        {
            endPos = chunkData.worldPos.y + chunkData.height;
        }

        if (stoneNoise > stoneThreshold)
        {
            for (int i = chunkData.worldPos.y; i <= endPos; i++)
            {
                Vector3Int pos = new Vector3Int(x, i, z);
                Chunk.SetVoxel(chunkData, pos, VoxelType.Stone);
            }
            return true;
        }
        return false;
    }
}
