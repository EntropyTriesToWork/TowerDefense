using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoise(int size, float scale, int octaves, Vector2 offset, float persistance, float lacunarity, int seed)
    {
        if (scale <= 0f) { scale = 0.001f; }

        System.Random prandom = new System.Random(seed);
        Vector2[] offsetOctaves = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prandom.Next(-10000, 10000) + offset.x;
            float offsetY = prandom.Next(-10000, 10000) + offset.y;
            offsetOctaves[i] = new Vector2(offsetX, offsetY);
        }

        float[,] noiseMap = new float[size, size];
        float halfSize = size / 2f;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float amp = 1;
                float freq = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfSize) / scale * freq + offsetOctaves[i].x;
                    float sampleY = (y - halfSize) / scale * freq + offsetOctaves[i].y;
                    float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinNoise * amp;

                    amp *= persistance;
                    freq *= lacunarity;
                }
                //Debug.Log(noiseHeight);
                noiseMap[x, y] = noiseHeight;
            }
        }
        return noiseMap;
    }
}
