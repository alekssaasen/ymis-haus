using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    public static float [,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, int seed, Vector2 offset, int tilesToFlatten,float flattenAmmount, float flattenOffset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        int yCenter = mapHeight / 2;
        int xCenter = mapWidth / 2;

        int flattenYStart = yCenter - tilesToFlatten;
        int flattenYEnd = yCenter + tilesToFlatten;
        int flattenXStart = xCenter - tilesToFlatten;
        int flattenXEnd = xCenter + tilesToFlatten;

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                float flatten = 1;
                float flattenOffsetValue = 0;
                if(tilesToFlatten != 0)
                {
                    if (y > flattenYStart && y < flattenYEnd &&
                        x > flattenXStart && x < flattenXEnd)
                    {
                        flatten = flattenAmmount;
                        flattenOffsetValue = flattenOffset;
                    }
                }

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.Abs((float)NoiseS3D.Noise(sampleX, sampleY)) * 2 -1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;

                }

                noiseHeight *= flatten;
                noiseHeight -= flattenOffsetValue;

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }


}
