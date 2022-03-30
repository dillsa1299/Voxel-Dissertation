using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Converts data from chunk data and passess to mesh data for rendering

    Modified from:
    https://github.com/SunnyValleyStudio/Unity-2020-Voxel-World-Tutorial-Voxel-Engine-members/blob/main/Section_1_Scripts/BlockHelper.cs
*/

public static class VoxelHelper
{
    //public bool cullFaces = true;
    private static Direction[] directions =
    {
        Direction.backwards,
        Direction.down,
        Direction.forward,
        Direction.left,
        Direction.right,
        Direction.up
    };
    
    public static MeshData GetMeshData (ChunkData chunk, int x, int y, int z, MeshData meshData, VoxelType voxelType)
    {
        if (voxelType == VoxelType.Air || voxelType == VoxelType.None) //If air or nothing, don't add to mesh
            return meshData;

        foreach (Direction direction in directions)
        {
            var neighbourVoxelCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
            var neighbourVoxelType = Chunk.GetVoxelFromChunkCoordinates(chunk, neighbourVoxelCoordinates);

            bool cullFaces = GameObject.Find("World").GetComponent<MainController>().cullFaces;
            //This if statement controls if naive or not
            if (neighbourVoxelType != VoxelType.None && VoxelDataManager.voxelTextureDataDictionary
                [neighbourVoxelType].isSolid == false)
            {
                if (voxelType == VoxelType.Water)
                {
                    if (neighbourVoxelType == VoxelType.Air)
                    {
                        meshData.waterMesh = GetFaceDataIn(direction, chunk, x, y, z, meshData.waterMesh, voxelType);
                    }
                }
                else
                {
                    meshData = GetFaceDataIn(direction, chunk, x, y, z, meshData, voxelType);
                }
            }
            if (!cullFaces)
            {
                meshData = GetFaceDataIn(direction, chunk, x, y, z, meshData, voxelType);
            }
        }
        return meshData;
    }
    
    public static Vector2[] FaceUVs(Direction direction, VoxelType voxelType)
    {
        Vector2[] UVs = new Vector2[4];
        var tilePos = TexturePosition(direction, voxelType);

        UVs[0] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX -
            VoxelDataManager.textureOffset,
                VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset);

        UVs[1] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX -
            VoxelDataManager.textureOffset,
                VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeY - VoxelDataManager.textureOffset);

        UVs[2] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset,
                VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeY - VoxelDataManager.textureOffset);

        UVs[3] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset,
                VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset);

        return UVs;
    }

    public static MeshData GetFaceDataIn(Direction direction, ChunkData chunk, int x, int y, int z,
        MeshData meshData, VoxelType voxelType)
    {
        GetFaceVertices(direction, x, y, z, meshData, voxelType);
        meshData.AddQuadTriangles(VoxelDataManager.voxelTextureDataDictionary[voxelType].generatesCollider);
        meshData.uv.AddRange(FaceUVs(direction, voxelType));

        return meshData;
    }
    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, VoxelType voxelType)
    {
        var generatesCollider = VoxelDataManager.voxelTextureDataDictionary[voxelType].generatesCollider;
        //order of vertices matters for the normals and how we render the mesh
        switch (direction)
        {
            case Direction.backwards:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;
            case Direction.forward:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.left:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;

            case Direction.right:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                break;
            default:
                break;
        }
    }

    public static Vector2Int TexturePosition(Direction direction, VoxelType voxelType)
    {
        return direction switch //Selects texture based on side of voxel
        {
            Direction.up => VoxelDataManager.voxelTextureDataDictionary[voxelType].top,
            Direction.down => VoxelDataManager.voxelTextureDataDictionary[voxelType].bottom,
            Direction.left => VoxelDataManager.voxelTextureDataDictionary[voxelType].leftRight,
            Direction.right => VoxelDataManager.voxelTextureDataDictionary[voxelType].leftRight,
            _ => VoxelDataManager.voxelTextureDataDictionary[voxelType].frontBack
        };
    }
}
