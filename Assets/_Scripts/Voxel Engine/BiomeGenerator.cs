using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public float noiseScale = 0.01f;
    public NoiseData noiseData;
    public VoxelLayerHandler startLayerHandler;
    public List<VoxelLayerHandler> additionalLayerHandlers;

    private int GetGroundHeightNoise(int x, int z, int height)
    {
        float groundHeight = CustomNoise.OctavePerlin(x, z, noiseData);
        groundHeight = CustomNoise.Redistribution(groundHeight, noiseData);
        int surfaceHeight = CustomNoise.RemapValue01ToInt(groundHeight, 0, height);
        return surfaceHeight;
    }
    public ChunkData GenerateColumnPerlin(ChunkData data, int x, int z, Vector2Int terrainOffset)
    {
        noiseData.terrainOffset = terrainOffset;
        int groundPosition = GetGroundHeightNoise(data.worldPos.x + x, data.worldPos.z + z, data.height);

        for (int y = 0; y < data.height-1; y++)
        {
            startLayerHandler.Handle(data, x, y, z, groundPosition, terrainOffset);
        }

        foreach(var layer in additionalLayerHandlers)
        {
            layer.Handle(data, x, data.worldPos.y, z, groundPosition, terrainOffset);
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
