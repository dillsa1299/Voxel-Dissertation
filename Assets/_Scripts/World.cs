using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int worldSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public int heightThreshold = 50;
    public float noiseScale = 0.03f;
    public GameObject chunkPrefab;

    public static bool cullFaces = false;

    Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    public void GenerateWorld()
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
                GenerateVoxels(data);
                chunkDataDictionary.Add(data.worldPos, data);
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
    }

    private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < data.size; x++) //Loop x
        {
            for (int z = 0; z < data.size; z++) //Loop z
            {
                float noiseValue = Mathf.PerlinNoise((data.worldPos.x + x) * noiseScale, (data.worldPos.z + z) * noiseScale);
                int groundPosition = Mathf.RoundToInt(noiseValue * chunkHeight);

                for (int y = 0; y < chunkHeight; y++)
                {
                    VoxelType voxelType = VoxelType.Standard;
                    if (y > groundPosition)
                    {
                        if (y < heightThreshold)
                        {
                            voxelType = VoxelType.Grey;
                        }
                        else
                        {
                            voxelType = VoxelType.Air;
                        }
                    }
                    else if (y == groundPosition)
                    {
                        voxelType = VoxelType.White;
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
