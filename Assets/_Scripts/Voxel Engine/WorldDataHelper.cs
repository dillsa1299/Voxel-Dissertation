using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldDataHelper : MonoBehaviour
{
    public static Vector3Int ChunkPositionFromVoxelCoords(World world, Vector3Int position)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(position.x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(position.y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(position.z / (float)world.chunkSize) * world.chunkSize
        };
    }

    internal static List<Vector3Int> GetChunkPositionsAroundPlayer(World world, Vector3Int playerPos)
    {
        //Sets start and end positions to iterate through chunks around the player
        int startX = playerPos.x - (world.chunkRenderDistance) * world.chunkSize;
        int startZ = playerPos.z - (world.chunkRenderDistance) * world.chunkSize;
        int endX = playerPos.x + (world.chunkRenderDistance) * world.chunkSize;
        int endZ = playerPos.z + (world.chunkRenderDistance) * world.chunkSize;

        List<Vector3Int> chunkPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, 0, z));
                chunkPositionsToCreate.Add(chunkPos);
                // if (x >= playerPos.x - world.chunkSize
                //     && x <= playerPos.x + world.chunkSize
                //     && z >= playerPos.z - world.chunkSize
                //     && z <= playerPos.z + world.chunkSize)
                // {
                //     for (int y = -world.chunkHeight; y >= playerPos.y - world.chunkHeight * 2; y -= world.chunkHeight)
                //     {
                //         chunkPos = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, y, z));
                //         chunkPositionsToCreate.Add(chunkPos);
                //     }
                // }
            }
        }
        return chunkPositionsToCreate;
    }

    internal static List<Vector3Int> GetDataPositionsAroundPlayer(World world, Vector3Int playerPos)
    {
        //Sets start and end positions to iterate through chunks around the player
        int startX = playerPos.x - (world.chunkRenderDistance+1) * world.chunkSize;
        int startZ = playerPos.z - (world.chunkRenderDistance+1) * world.chunkSize;
        int endX = playerPos.x + (world.chunkRenderDistance+1) * world.chunkSize;
        int endZ = playerPos.z + (world.chunkRenderDistance+1) * world.chunkSize;
        //+1 so that chunks are loaded just outside render distance

        List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, 0, z));
                chunkDataPositionsToCreate.Add(chunkPos);
                // if (x >= playerPos.x - world.chunkSize
                //     && x <= playerPos.x + world.chunkSize
                //     && z >= playerPos.z - world.chunkSize
                //     && z <= playerPos.z + world.chunkSize)
                // {
                //     for (int y = -world.chunkHeight; y >= playerPos.y - world.chunkHeight * 2; y -= world.chunkHeight)
                //     {
                //         chunkPos = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, y, z));
                //         chunkDataPositionsToCreate.Add(chunkPos);
                //     }
                // }
            }
        }
        return chunkDataPositionsToCreate;
    }

    internal static List<Vector3Int> SelectPositionsToCreate(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPos)
    {
        return allChunkPositionsNeeded
            .Where(pos => worldData.chunkDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPos, pos))
            .ToList();               
    }

    internal static List<Vector3Int> GetChunksToRemove(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded)
    {
        List<Vector3Int> chunkPosToRemove = new List<Vector3Int>();
        foreach (var pos in worldData.chunkDictionary.Keys
            .Where(pos => allChunkPositionsNeeded.Contains(pos) == false))
        {
            if (worldData.chunkDictionary.ContainsKey(pos))
            {
                chunkPosToRemove.Add(pos);
            }
        }
        return chunkPosToRemove;
    }

    internal static List<Vector3Int> GetDataToRemove(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded)
    {
        return worldData.chunkDataDictionary.Keys
            .Where(pos => allChunkDataPositionsNeeded.Contains(pos) == false)
            .ToList();

    }

    internal static void RemoveChunkData(World world, Vector3Int pos)
    {
        world.worldData.chunkDataDictionary.Remove(pos);
    }

    internal static void RemoveChunk(World world, Vector3Int pos)
    {
        ChunkRenderer chunk = null;
        if (world.worldData.chunkDictionary.TryGetValue(pos, out chunk))
        {
            world.RemoveChunk(chunk);
            world.worldData.chunkDictionary.Remove(pos);
        }
    }

    internal static List<Vector3Int> SelectDataPositionsToCreate(World.WorldData worldData, List<Vector3Int> allChunkDataPositionsNeeded, Vector3Int playerPos)
    {
        return allChunkDataPositionsNeeded
            .Where(pos => worldData.chunkDataDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPos, pos))
            .ToList();               
    }
}
