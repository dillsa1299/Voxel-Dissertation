using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int worldSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public GameObject chunkPrefab;

    public TerrainGenerator terrainGenerator;

    public Vector2Int terrainOffset;

    Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    public void GenerateWorld(int generationMode)
    {
        chunkDataDictionary.Clear(); //Destroy all chunks
        foreach (ChunkRenderer chunk in chunkDictionary.Values)
        {
            Destroy(chunk.gameObject); //Destroy all chunk game objects
        }
        chunkDictionary.Clear();

        for (int x = 0; x < worldSizeInChunks; x++)
        {
            for (int z = 0; z < worldSizeInChunks; z++)
            {
                ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                ChunkData newData = terrainGenerator.GenerateVoxelsPerlin(data, terrainOffset);
                if(generationMode == 0)
                {
                    newData = terrainGenerator.GenerateVoxelsPerlin(data, terrainOffset);
                }
                if(generationMode == 1)
                {
                    newData = terrainGenerator.GenerateVoxelsSphere(data);
                }
                if(generationMode == 2)
                {
                    newData = terrainGenerator.GenerateVoxelsGrid(data);
                }
                chunkDataDictionary.Add(newData.worldPos, newData);
            }
        }

        foreach (ChunkData data in chunkDataDictionary.Values)
        {
                MeshData meshData = Chunk.GetChunkMeshData(data);
                GameObject chunkObject = Instantiate(chunkPrefab, data.worldPos, Quaternion.identity);
                ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
                chunkDictionary.Add(data.worldPos, chunkRenderer);
                chunkRenderer.InitializeChunk(data);
                chunkRenderer.UpdateChunk(meshData);
        }
    }

    public void RegeneratePerlin()
    {
        GenerateWorld(0);
    }

    public void RegenerateSpheres()
    {
        GenerateWorld(1);
    }

    public void RegenerateGrid()
    {
        GenerateWorld(2);
    }

    internal VoxelType GetVoxelFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromVoxelCoords(this, x, y, z);
        ChunkData containerChunk = null;

        chunkDataDictionary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null)
            return VoxelType.None;
        Vector3Int voxelInChunkCoordinates = Chunk.GetVoxelInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetVoxelFromChunkCoordinates(containerChunk, voxelInChunkCoordinates);
    }

}
