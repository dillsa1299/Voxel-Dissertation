using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Chunk
{
    // Iterates through voxel array, performing method on each voxel
    public static void LoopThroughVoxels(ChunkData chunkData, Action<int, int, int> actionToPerform)
    {
        for (int index = 0; index < chunkData.voxels.Length; index++)
        {
            var position = GetPositionFromIndex(chunkData, index); //Gets 3D position from 1D array
            actionToPerform(position.x, position.y, position.z); //Performs method on each voxel
        }
    }

    // Gets 3D position from index in 1D voxel array
    private static Vector3Int GetPositionFromIndex(ChunkData chunkData, int index)
    {
        int x = index % chunkData.size;
        int y = (index / chunkData.size) % chunkData.height;
        int z = index / (chunkData.size * chunkData.height);
        return new Vector3Int(x, y, z);
    }

    // Checks local chunk coordinates are within range
    private static bool InRange(ChunkData chunkData, int axisCoordinate)
    {
        if (axisCoordinate < 0 || axisCoordinate >= chunkData.size)
            return false;
        return true;
    }

    // Checks local chunk coordinates are within range
    private static bool InRangeHeight(ChunkData chunkData, int yCoordinate)
    {
        if (yCoordinate < 0 || yCoordinate >= chunkData.height)
            return false;
        return true;
    }
    
    // Get voxel within chunk - accepts vector instead of separate coordinates
    public static VoxelType GetVoxelFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates)
    {
        return GetVoxelFromCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

    // Get voxel within chunk
    public static VoxelType GetVoxelFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        // Validating coordinate is within range
        if (InRange(chunkData, x) && InRangeHeight(chunkData, y) && InRange(chunkData, z))
        {
            int index = GetIndexFromPosition(chunkData, x, y, z);
            return chunkData.voxels[index];
        }
        // Coordinate is not within the passed chunk
        throw new Exception("Need to ask World for appropriate chunk");
    }

    // Set voxel within chunk
    public static void SetVoxel(ChunkData chunkData, Vector3Int localPos, VoxelType voxel)
    {
        // Validating coordinate is within range
        if (InRange(chunkData, localPos.x) && InRangeHeight(chunkData, localPos.y)
        && InRange(chunkData, localPos.z))
        {
            int index = GetIndexFromPosition(chunkData, localPos.x, localPos.y, localPos.z);
            chunkData.voxels[index] = voxel;
        }
        else
        {
            // Coordinate is not within the passed chunk
            throw new Exception("Need to ask world for appropriate chunk");
        }
    }

    // Gets 1D index from 3D position within chunk
    private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.size * y + chunkData.size * chunkData.height * z;
    }

    //Gets local chunk position from global world position 
    public static Vector3Int GetVoxelInChunkCoordinates(ChunkData chunkData, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = pos.x - chunkData.worldPos.x,
            y = pos.y - chunkData.worldPos.y,
            z = pos.z - chunkData.worldPos.z
        };
    }

    internal static MeshData GetChunkMeshData(ChunkData chunkData)
    {
        MeshData meshData = new MeshData();

        //Fill later

        return meshData;
    }
}
