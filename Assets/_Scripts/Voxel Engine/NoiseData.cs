using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "noiseData", menuName = "Data/NoiseData")]
public class NoiseData : ScriptableObject
{
    public float noiseZoom;
    public int octaves;
    public Vector2Int terrainOffset;
    public float persistance;
    public float redistributionModifier;
    public float exponent;
}
