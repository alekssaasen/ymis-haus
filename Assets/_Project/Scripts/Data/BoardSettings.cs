using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Board Settings", menuName = "ScriptableObjects/BoardSettings")]
public class BoardSettings : ScriptableObject
{
    public Vector2Int MapSize = new Vector2Int(14, 14);
    public GameObject Prefab;

    public FigureOverwrite[] Figures;
    public BuildingOverwrite[] Buildings;
    public WallOverwrite[] Walls;
}

[System.Serializable]
public class FigureOverwrite
{
    public int ID = -1;
    public Vector2Int Position = Vector2Int.zero;
    public ChessFigure Figure = ChessFigure.Empty;
    public CompassRotation RotationOffset;
}

[System.Serializable]
public class BuildingOverwrite
{
    public int ID = -1;
    public Vector2Int Position = Vector2Int.zero;
    public ChessBuiding Building = ChessBuiding.Empty;
    public CompassRotation RotationOffset;
}

[System.Serializable]
public class WallOverwrite
{
    public int ID = -1;
    public Vector2Int[] InteriorWalls, InteriorGates, ExteriorWalls, ExteriorGates, InsideTiles;
    public CompassRotation RotationOffset;
}

[System.Serializable]
public enum CompassRotation
{
    North, East, South, West
}