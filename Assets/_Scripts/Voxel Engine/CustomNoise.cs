/*
    MULTI OCTAVE PERLIN NOISE GENERATOR
    Adapted from YouTube source:
    https://www.youtube.com/watch?v=nXGlbG3jcKM&list=PLcRSafycjWFesScBq3JgHMNd9Tidvk9hE&index=4

    Relevant information about octaves found here:
    https://adrianb.io/2014/08/09/perlinnoise.html
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomNoise
{
    public static float RemapValue(float value, float initialMin, float initialMax, float outputMin,
        float outputMax)
    {
        return outputMin + (value - initialMin) * (outputMax - outputMin) / (initialMax - initialMin);
    }

    public static float RemapValue01(float value, float outputMin, float outputMax)
    {
        return outputMin + (value - 0) * (outputMax - outputMin) / (1-0);
    }

    public static int RemapValue01ToInt(float value, float outputMin, float outputMax)
    {
        return (int)RemapValue01(value, outputMin, outputMax);
    }

    public static float Redistribution(float noise, NoiseData noiseData)
    {
        return Mathf.Pow(noise * noiseData.redistributionModifier, noiseData.exponent);
    }
    public static float OctavePerlin(float x, float z, NoiseData noiseData)
    {
        // x & z are voxel coordinates
        x *= noiseData.noiseZoom;
        z *= noiseData.noiseZoom;
        x += noiseData.noiseZoom;
        z += noiseData.noiseZoom;

        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float amplitudeSum = 0; //Used to normalise value

        for (int i = 0; i < noiseData.octaves; i++)
        {
            total += Mathf.PerlinNoise((noiseData.terrainOffset.x + noiseData.terrainOffset.x + x) * frequency,
                (noiseData.terrainOffset.y + noiseData.terrainOffset.y + z) * frequency) * amplitude;
            
            amplitudeSum += amplitude;

            amplitude *= noiseData.persistence;
            frequency *= 2;
        }
        return total / amplitudeSum;
    }
}
