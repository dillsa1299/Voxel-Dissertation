using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    public VoxelType[] voxels; // Stores voxels within chunk
    
    public int size = 16; // 16*16
    public int height = 128; // 128 deep

    public World worldData;
    public Vector3Int worldPos; // Chunk position in world

    public bool modified = false; //Stores if chunk has been modified at runtime

    public ChunkData(int size, int height, World world, Vector3Int worldPos) //Chunk constructor 
    {
        this.size = size;
        this.height = height;
        this.worldData = world;
        this.worldPos = worldPos;
        voxels = new VoxelType[size * size * height];
    }
}
