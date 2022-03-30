using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public int waterHeight = 50;
    public float noiseScale = 0.03f;
    public ChunkData GenerateColumnPerlin(ChunkData data, int x, int z, Vector2Int terrainOffset)
    {
        float noiseValue = Mathf.PerlinNoise((terrainOffset.x + data.worldPos.x + x) * noiseScale,
            (terrainOffset.y + data.worldPos.z + z) * noiseScale);
        int groundPosition = Mathf.RoundToInt(noiseValue * data.height-1);

        for (int y = 0; y < data.height; y++)
        {
            VoxelType voxelType = VoxelType.Dirt;
            if (y > groundPosition)
            {
                if (y < waterHeight)
                {
                    voxelType = VoxelType.Water;
                }
                else
                {
                    voxelType = VoxelType.Air;
                }
            }
            else if (y == groundPosition)
            {
                if(y < waterHeight)
                {
                    voxelType = VoxelType.Sand;
                }
                else
                {
                    voxelType = VoxelType.Grass;
                }
            }
            Chunk.SetVoxel(data, new Vector3Int(x, y, z), voxelType);
        }
        return data;
    }

    public ChunkData GenerateColumnSphere(ChunkData data, int x, int z)
    {
        int radius = (data.size-2)/2;
        for (int y = 0; y < data.height; y++)
        {
            VoxelType voxelType = VoxelType.Air;
            if ((((x-radius)*(x-radius))+((z-radius)*(z-radius))+((y-radius)*(y-radius)))<(radius*radius))
            {
                voxelType = VoxelType.Standard;
            }
            Chunk.SetVoxel(data, new Vector3Int(x, y, z), voxelType);
        }
        return data;
    }

    public ChunkData GenerateColumnGrid(ChunkData data, int x, int z)
    {
        for (int y = 0; y < data.height; y++)
        {
            VoxelType voxelType = VoxelType.Air;
            if ((x %2 == 0)&&(z %2 == 0)&&(y %2 == 0))
            {
                voxelType = VoxelType.Standard;
            }
            Chunk.SetVoxel(data, new Vector3Int(x, y, z), voxelType);
        }
        return data;
    }
}
