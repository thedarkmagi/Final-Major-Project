using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise  {

    public enum NormaliseMode { Local, Global };


    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale)
    {
        float[,] noiseMap = new float[mapWidth,mapHeight];

        if(scale <=0)
        {
            scale = 0.0001f;
        }

        for (int y =0; y<mapHeight; y++)
        {
            for(int x =0;x<mapWidth;x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            }
        }
        
        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    public Noise.NormaliseMode normaliseMode;

    public float scale = 50;
    public int octaves = 6;
    
    [Range(0, 1)] // causes the value to be limited between 0 and 1 appears as a slider in editor 
    public float persistance =0.6f;
    public float lacunarity =2;

    public int seed;
    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }


}

