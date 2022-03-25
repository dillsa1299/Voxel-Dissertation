using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int worldSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public int waterHeight = 50;
    public float noiseScale = 0.03f;
    public GameObject chunkPrefab;

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
                if(generationMode == 0) GenerateVoxelsPerlin(data);
                if(generationMode == 1) GenerateVoxelsSphere(data);
                if(generationMode == 2) GenerateVoxelsGrid(data);
                chunkDataDictionary.Add(data.worldPos, data);
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

    private void GenerateVoxelsPerlin(ChunkData data)
    {
        for (int x = 0; x < data.size; x++) //Loop x
        {
            for (int z = 0; z < data.size; z++) //Loop z
            {
                float noiseValue = Mathf.PerlinNoise((data.worldPos.x + x) * noiseScale, (data.worldPos.z + z) * noiseScale);
                int groundPosition = Mathf.RoundToInt(noiseValue * chunkHeight-1);

                for (int y = 0; y < chunkHeight; y++)
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
            }
        }
    }

    private void GenerateVoxelsSphere(ChunkData data)
    {
        int radius = (chunkSize-2)/2;

        for (int x = 0; x < data.size; x++) //Loop x
        {
            for (int z = 0; z < data.size; z++) //Loop z
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    VoxelType voxelType = VoxelType.Air;
                    if ((((x-radius)*(x-radius))+((z-radius)*(z-radius))+((y-radius)*(y-radius)))<(radius*radius))
                    {
                        voxelType = VoxelType.Standard;
                    }
                    Chunk.SetVoxel(data, new Vector3Int(x, y, z), voxelType);
                } 
            }
        }
    }

    private void GenerateVoxelsGrid(ChunkData data)
    {
        for (int x = 0; x < data.size; x++) //Loop x
        {
            for (int z = 0; z < data.size; z++) //Loop z
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    VoxelType voxelType = VoxelType.Air;
                    if ((x %2 == 0)&&(z %2 == 0)&&(y %2 == 0))
                    {
                        voxelType = VoxelType.Standard;
                    }
                    Chunk.SetVoxel(data, new Vector3Int(x, y, z), voxelType);
                } 
            }
        }
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
