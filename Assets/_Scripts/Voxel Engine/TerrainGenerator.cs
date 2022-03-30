using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public BiomeGenerator biomeGenerator;
    public ChunkData GenerateVoxelsPerlin(ChunkData data, Vector2Int terrainOffset)
    {
        for (int x = 0; x < data.size; x++) //Loop x
        {
            for (int z = 0; z < data.size; z++) //Loop z
            {
                data = biomeGenerator.GenerateColumnPerlin(data, x, z, terrainOffset);
            }
        }
        return data;
    }

    public ChunkData GenerateVoxelsSphere(ChunkData data)
    {
        for (int x = 0; x < data.size; x++) //Loop x
        {
            for (int z = 0; z < data.size; z++) //Loop z
            {
                data = biomeGenerator.GenerateColumnSphere(data, x, z);
            }
        }
        return data;
    }

    public ChunkData GenerateVoxelsGrid(ChunkData data)
    {
        for (int x = 0; x < data.size; x++) //Loop x
        {
            for (int z = 0; z < data.size; z++) //Loop z
            {
                data = biomeGenerator.GenerateColumnGrid(data, x, z);
            }
        }
        return data;
    }
}
