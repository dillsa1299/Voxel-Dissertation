using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Creates accessible menu in Unity editor for adding new voxel types

[CreateAssetMenu(fileName = "Voxel Data", menuName = "Data/Voxel Data")]
public class VoxelDataSO : ScriptableObject
{
    public float textureSizeX, textureSizeY; //Size of each tile within tilemap
    public List<TextureData> textureDataList;
}

[Serializable]
public class TextureData
{
    public VoxelType voxelType;
    public Vector2Int top, bottom, leftRight, frontBack;
    public bool isSolid = true;
    public bool generatesCollider = true;
}
