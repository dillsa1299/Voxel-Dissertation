using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour
{
    public int worldSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public int chunkRenderDistance = 4;
    private int worldType = 0;
    public GameObject chunkPrefab;
    public TerrainGenerator terrainGenerator;
    public Vector2Int terrainOffset;

    public WorldData worldData { get; private set; }

    public UnityEvent OnWorldCreated, OnNewChunksGenerated;

    private bool IsWorldCreated = false;

    public bool cullFaces = true;

    CancellationTokenSource taskTokenSource = new CancellationTokenSource();

    private void Awake()
    {
        worldData = new WorldData
        {
            chunkHeight = this.chunkHeight,
            chunkSize = this.chunkSize,
            chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>(),
            chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>()
        };
    }
    
    public async void GenerateWorld(int generationMode)
    {
        await GenerateWorld(Vector3Int.zero, generationMode);
    }
    private async Task GenerateWorld(Vector3Int position, int generationMode)
    {
        worldType = generationMode;
        WorldGenerationData worldGenerationData = await Task.Run(() =>GetPositionsVisibleToCamera(position), taskTokenSource.Token);

        foreach (var pos in worldGenerationData.chunkPositionsToRemove)
        {
            WorldDataHelper.RemoveChunk(this, pos);
        }

        foreach (var pos in worldGenerationData.chunkDataToRemove)
        {
            WorldDataHelper.RemoveChunk(this, pos);
        }

        //Concurrent Dictionary required for processing on separate thread
        ConcurrentDictionary<Vector3Int, ChunkData> dataDictionary = null;
        try
        {
            dataDictionary = await CalculateWorldChunkData(worldGenerationData.chunkDataPositionsToCreate);
        }
        catch(Exception)
        {
            Debug.Log("Task Cancelled");
            return;
        }
        
        foreach (var calculatedData in dataDictionary)
        {
            worldData.chunkDataDictionary.Add(calculatedData.Key, calculatedData.Value);
        }
        
        //Concurrent Dictionary required for processing on separate thread
        ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary = new ConcurrentDictionary<Vector3Int, MeshData>();
        
        List<ChunkData> dataToRender = worldData.chunkDataDictionary
            .Where(keyvaluepair => worldGenerationData.chunkPositionsToCreate.Contains(keyvaluepair.Key))
            .Select(keyvalpair => keyvalpair.Value)
            .ToList();

        try
        {
            meshDataDictionary = await CreateMeshDataAsync(dataToRender);
        }
        catch(Exception)
        {
            Debug.Log("Task Cancelled");
            return;
        }

        StartCoroutine(ChunkCreatorCoroutine(meshDataDictionary));
    }

    private Task<ConcurrentDictionary<Vector3Int, MeshData>> CreateMeshDataAsync(List<ChunkData> dataToRender)
    {
        ConcurrentDictionary<Vector3Int, MeshData> dictionary = new ConcurrentDictionary<Vector3Int, MeshData>();
        return Task.Run(() =>
        {
            foreach (ChunkData data in dataToRender)
            {
                if (taskTokenSource.Token.IsCancellationRequested)
                {
                    taskTokenSource.Token.ThrowIfCancellationRequested();
                }
                MeshData meshData = Chunk.GetChunkMeshData(data, cullFaces);
                dictionary.TryAdd(data.worldPos, meshData);
            }
            return dictionary;
        }, taskTokenSource.Token
        );
    }

    private Task<ConcurrentDictionary<Vector3Int, ChunkData>> CalculateWorldChunkData(List<Vector3Int> chunkDataPositionsToCreate)
    {
        ConcurrentDictionary<Vector3Int, ChunkData> dictionary = new ConcurrentDictionary<Vector3Int, ChunkData>();

        return Task.Run(() =>
        {
            foreach (Vector3Int pos in chunkDataPositionsToCreate)
            {
                if (taskTokenSource.Token.IsCancellationRequested)
                {
                    taskTokenSource.Token.ThrowIfCancellationRequested();
                }
                ChunkData data = new ChunkData(chunkSize, chunkHeight, this, pos);
                ChunkData newData = null;
                if(worldType == 0)
                {
                    newData = terrainGenerator.GenerateVoxelsPerlin(data, terrainOffset);
                }
                else if(worldType == 1)
                {
                    newData = terrainGenerator.GenerateVoxelsSphere(data);
                }
                else if(worldType == 2)
                {
                    newData = terrainGenerator.GenerateVoxelsGrid(data);
                }
                dictionary.TryAdd(pos, newData);
            }
            return dictionary;
        }, taskTokenSource.Token
        );
    }

    private WorldGenerationData GetPositionsVisibleToCamera(Vector3Int playerPos)
    {
        List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPos);
        List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPos);

        List<Vector3Int> chunkPositionsToCreate = WorldDataHelper.SelectPositionsToCreate(worldData, allChunkPositionsNeeded, playerPos);
        List<Vector3Int> chunkDataPositionsToCreate = WorldDataHelper.SelectDataPositionsToCreate(worldData, allChunkDataPositionsNeeded, playerPos);

        List<Vector3Int> chunkPositionsToRemove = WorldDataHelper.GetChunksToRemove(worldData, allChunkPositionsNeeded);
        List<Vector3Int> chunkDataToRemove = WorldDataHelper.GetDataToRemove(worldData, allChunkDataPositionsNeeded);

        WorldGenerationData data = new WorldGenerationData
        {
            chunkPositionsToCreate = chunkPositionsToCreate,
            chunkDataPositionsToCreate = chunkDataPositionsToCreate,
            chunkPositionsToRemove = chunkPositionsToRemove,
            chunkDataToRemove = chunkDataToRemove
        };
        return data;
    }

    internal async void LoadNextChunks(GameObject player)
    {
        await GenerateWorld(Vector3Int.RoundToInt(player.transform.position), worldType);
        OnNewChunksGenerated?.Invoke();
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

        worldData.chunkDataDictionary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null)
            return VoxelType.None;
        Vector3Int voxelInChunkCoordinates = Chunk.GetVoxelInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetVoxelFromChunkCoordinates(containerChunk, voxelInChunkCoordinates);
    }

    internal void RemoveChunk(ChunkRenderer chunk)
    {
        //chunk.gameObject.SetActive(false);
        Destroy(chunk.gameObject);
    }

    IEnumerator ChunkCreatorCoroutine(ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary)
    {
        foreach (var item in meshDataDictionary)
        {
            CreateChunk(worldData, item.Key, item.Value);
            yield return new WaitForEndOfFrame(); //Limits chunk rendering to 1 chunk per frame
        }                                         // Prevents lag spikes
        if (IsWorldCreated == false)
        {
            IsWorldCreated = true;
            OnWorldCreated?.Invoke();
        }
    }

    private void CreateChunk(WorldData worldData, Vector3Int pos, MeshData meshData)
    {
        GameObject chunkObject = Instantiate(chunkPrefab, pos, Quaternion.identity);
        ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
        worldData.chunkDictionary.Add(pos, chunkRenderer);
        chunkRenderer.InitializeChunk(worldData.chunkDataDictionary[pos]);
        chunkRenderer.UpdateChunk(meshData);
    }

    public struct WorldGenerationData
    {
        public List<Vector3Int> chunkPositionsToCreate;
        public List<Vector3Int> chunkDataPositionsToCreate;
        public List<Vector3Int> chunkPositionsToRemove;
        public List<Vector3Int> chunkDataToRemove;
    }

    public struct WorldData
    {
        public Dictionary<Vector3Int, ChunkData> chunkDataDictionary;
        public Dictionary<Vector3Int, ChunkRenderer> chunkDictionary;
        public int chunkSize;
        public int chunkHeight;
    }

}
