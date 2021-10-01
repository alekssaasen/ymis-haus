using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        HeightMap, ColorMap, Mesh
    }
    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public int TextureScale;
    [Range(0,1)]
    public float TextureRandomness;

    public int tilesToFlatten;
    [Range(0,1)]
    public float flattenAmmount;
    [Range(-1f, 1f)]
    public float flattenOffset;

    public TerrainType[] regions;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, seed, offset,tilesToFlatten,flattenAmmount,flattenOffset);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y += TextureScale)
        {
            for (int x = 0; x < mapWidth; x += TextureScale)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        Color tileColor = regions[i].color;
                        float colorShift = Random.Range(0f, TextureRandomness);
                        tileColor = tileColor - new Color(colorShift, colorShift, colorShift);
                        for (int a = 0; a < TextureScale; a++)
                        {
                            for (int b = 0; b < TextureScale; b++)
                            {
                                colorMap[(y+a) * mapWidth + x + b] = tileColor;
                            }
                        }
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.HeightMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap,mapWidth,mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap,meshHeightMultiplier,meshHeightCurve), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }

    }

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
        if (TextureScale < 1)
        {
            TextureScale = 1;
        }
    }


}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}