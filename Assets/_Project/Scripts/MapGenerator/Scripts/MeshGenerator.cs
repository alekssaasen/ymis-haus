using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public Vector2Int mapSize;
    public int divisions;
    public AnimationCurve heightCurve;
    public float heightMultiply;
    public float noiseScale;
    public int borderTiles;

    public TerrainType[] regions;
    public Color blackColor;
    public Color whiteColor;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> triangles = new List<int>();

    private void Start()
    {
        mapSize = GameManager.GameSettingsInUse.BoardSetup.MapSize;
        GenerateMesh(System.DateTime.Now.Millisecond);
    }

    public void UpdateValues()
    {
        transform.position = new Vector3(-borderTiles - 0.5f, 0, -borderTiles - 0.5f);
        Noise.size = noiseScale;
        Noise.map = mapSize;
    }

    public void GenerateMesh(int Seed)
    {
        UpdateValues();

        NoiseS3D.seed = Seed;
        int index = 0;
        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();

        mapSize += Vector2Int.one * borderTiles * 2;
        float div = 1f / (float)(divisions + 1);
        Vector2Int size = mapSize * (divisions + 1);

        for (int x = 0; x < size.x + 1; x++)
        {
            for (int y = 0; y < size.y + 1; y++)
            {
                vertices.Add(new Vector3(x * div, Mathf.Clamp01(heightCurve.Evaluate(Noise.GetHeightNoise(x * div - borderTiles, y * div - borderTiles))) * heightMultiply, y * div));
                uvs.Add(new Vector2((1f / size.x) * x, (1f / size.y) * y));

                if (x < size.x && y < size.y)
                {
                    if (Random.Range(0, 2) > 0)
                    {
                        triangles.Add(index + 1);
                        triangles.Add(index + size.x + 2);
                        triangles.Add(index + size.x + 1);

                        triangles.Add(index + size.x + 1);
                        triangles.Add(index);
                        triangles.Add(index+ 1);
                    }
                    else
                    {
                        triangles.Add(index + size.x + 2);
                        triangles.Add(index + size.x + 1);
                        triangles.Add(index);

                        triangles.Add(index);
                        triangles.Add(index + 1);
                        triangles.Add(index + size.x + 2);
                    }
                }

                index++;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;

        UpdateTexture();

        mapSize -= Vector2Int.one * borderTiles * 2;
    }

    private void UpdateTexture()
    {
        Texture2D texture = new Texture2D(mapSize.x, mapSize.y);
        Color col = Color.black;
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                float noise = Noise.GetColorNoise(x - borderTiles, y - borderTiles);

                if (noise < 0)
                {
                    if (x % 2 > 0 && y % 2 > 0)
                    {
                        col = blackColor;
                    }
                    else if (x % 2 <= 0 && y % 2 <= 0)
                    {
                        col = blackColor;
                    }
                    else
                    {
                        col = whiteColor;
                    }
                }
                else
                {
                    for (int j = 0; j < regions.Length; j++)
                    {
                        if (noise <= regions[j].height)
                        {
                            col = regions[j].color;
                            break;
                        }
                    }
                }

                texture.SetPixel(x, y, col * Random.Range(0.9f, 1f));
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}