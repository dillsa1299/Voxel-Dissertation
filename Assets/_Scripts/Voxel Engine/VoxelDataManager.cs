using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is used to load all textures for the different voxels to a static class so that
    they can be accessed by other classes within the code.
*/

public class VoxelDataManager : MonoBehaviour
{
    public static float textureOffset = 0.001f; //Helps prevent texture artifacting
    public static float tileSizeX, tileSizeY; //Size of each tile within tilemap
    public static Dictionary<VoxelType, TextureData> voxelTextureDataDictionary = new
        Dictionary<VoxelType, TextureData>();
    public VoxelDataSO textureData;

    private void Awake()
    {
        foreach (var item in textureData.textureDataList)
        {
            if (voxelTextureDataDictionary.ContainsKey(item.voxelType) == false)
            {
                voxelTextureDataDictionary.Add(item.voxelType, item);
            }
        }
        tileSizeX = textureData.textureSizeX;
        tileSizeY = textureData.textureSizeY;
    }
}


