using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float size;
    public static Vector2Int map;

    public static float GetHeightNoise(float x, float y)
    {
        if (x >= 0 && x < map.x && y >= 0 && y < map.y)
        {
            return 0;
        }
        else
        {
            if (x >= -1 && x < map.x + 1 && y >= -1 && y < map.y + 1)
            {
                float val1 = 0;
                float val2 = 0;

                if (x > 0)
                {
                    val1 = Mathf.Clamp01(1 - (map.x + 1 - x));
                }
                else
                {
                    val1 = Mathf.Clamp01(Mathf.Abs(x));
                }

                if (y > 0)
                {
                    val2 = Mathf.Clamp01(1 - (map.y + 1 - y));
                }
                else
                {
                    val2 = Mathf.Clamp01(Mathf.Abs(y));
                }

                return Mathf.Clamp01(Mathf.Max(val1, val2) * Mathf.Abs((float)NoiseS3D.NoiseCombinedOctaves(x / size, y / size)));
            }
            else
            {
                return Mathf.Clamp01(Mathf.Abs((float)NoiseS3D.NoiseCombinedOctaves(x / size, y / size)));
            }
        }
    }

    public static float GetColorNoise(float x, float y)
    {
        if (x >= 0 && x < map.x && y >= 0 && y < map.y)
        {
            return -1;
        }
        else
        {
            return Mathf.Clamp01(Mathf.Abs((float)NoiseS3D.NoiseCombinedOctaves(x / size, y / size)));
        }
    }
}
