using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour
{
    public RawImage image;

    public void GenerateMapDisplay(float[,] noiseMap, int mapSize)
    {
        image.texture = TextureFromHeightMap(noiseMap, mapSize);
    }
    public static Texture2D TextureFromColorMap(Color[] colorMap, int mapSize)
    {
        Texture2D texture = new Texture2D(mapSize, mapSize);
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
    public static Texture2D TextureFromHeightMap(float[,] heightMap, int mapSize)
    {
        Color[] colorMap = new Color[mapSize * mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                colorMap[y * mapSize + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }
        return TextureFromColorMap(colorMap, mapSize);
    }
}
