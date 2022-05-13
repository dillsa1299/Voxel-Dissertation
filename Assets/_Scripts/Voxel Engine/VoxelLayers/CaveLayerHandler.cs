using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    3D perlin noise for cave generation derived from:
    https://forum.unity.com/threads/tutorial-3d-voxel-terrain-caves.1151324/
*/

public class CaveLayerHandler : VoxelLayerHandler
{
    public float noiseScale = 0.05f;
    public float noiseMin = 0.5f;
    public float noiseMax = 0.65f;
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int groundHeight, Vector2Int terrainOffset, int waterLevel)
    {
        //x,y,z is local chunk xyz, need global
        if (y <= groundHeight)
        {
            if(Perlin3D(chunkData.worldPos.x + x,y,chunkData.worldPos.z + z)<noiseMax)
            {
                if(Perlin3D(chunkData.worldPos.x + x,y,chunkData.worldPos.z + z)>noiseMin)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    if (y == 0)
                    {
                        Chunk.SetVoxel(chunkData, pos, VoxelType.Stone);
                        return true;
                    }                    
                    Chunk.SetVoxel(chunkData, pos, VoxelType.Air);
                    return true;
                }
            }
        } 
        return false;
    }
    private float Perlin3D(int X, int Y, int Z)
    {
        float x =(float)X;
        float y =(float)Y;
        float z =(float)Z;
    
        x *= noiseScale;
        y *= noiseScale;
        z *= noiseScale;
    
        float xy = Mathf.PerlinNoise(x,y);
        float yz = Mathf.PerlinNoise(y,z);
        float xz = Mathf.PerlinNoise(x,z);

        float yx = Mathf.PerlinNoise(y,x);
        float zy = Mathf.PerlinNoise(z,y);
        float zx = Mathf.PerlinNoise(z,x);
    
        return (xy + yz + xz + yx + zy + zx) / 6;
    }
}


