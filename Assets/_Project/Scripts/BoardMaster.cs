using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardMaster
{
    public static TileInfo[,] InitializeBoard(BoardSettings Settings)
    {
        TileInfo[,] newboard = new TileInfo[Settings.MapSize.x, Settings.MapSize.y];

        for (int x = 0; x < Settings.MapSize.x; x++)
        {
            for (int y = 0; y < Settings.MapSize.y; y++)
            {
                newboard[x, y] = new TileInfo(-1);
            }
        }

        Vector2Int pos;

        for (int i = 0; i < Settings.Figures.Length; i++)
        {
            pos = RotateAroundVector2Int(Settings.Figures[i].RotationOffset, Settings.MapSize, Settings.Figures[i].Position);
            newboard[pos.x, pos.y].ownerID = Settings.Figures[i].ID;
            newboard[pos.x, pos.y].figure = Settings.Figures[i].Figure;
        }

        for (int i = 0; i < Settings.Buildings.Length; i++)
        {
            pos = RotateAroundVector2Int(Settings.Buildings[i].RotationOffset, Settings.MapSize, Settings.Buildings[i].Position);
            newboard[pos.x, pos.y].ownerID = Settings.Buildings[i].ID;
            newboard[pos.x, pos.y].building = Settings.Buildings[i].Building;
        }

        for (int i = 0; i < Settings.Walls.Length; i++)
        {
            for (int j = 0; j < Settings.Walls[i].InteriorWalls.Length; j++)
            {
                pos = RotateAroundVector2Int(Settings.Walls[i].RotationOffset, Settings.MapSize, Settings.Walls[i].InteriorWalls[j]);
                newboard[pos.x, pos.y].wall = WallType.InteriorWall;
                newboard[pos.x, pos.y].ownerID = Settings.Walls[i].ID;
            }
            for (int j = 0; j < Settings.Walls[i].InteriorGates.Length; j++)
            {
                pos = RotateAroundVector2Int(Settings.Walls[i].RotationOffset, Settings.MapSize, Settings.Walls[i].InteriorGates[j]);
                newboard[pos.x, pos.y].wall = WallType.InteriorGate;
                newboard[pos.x, pos.y].ownerID = Settings.Walls[i].ID;
            }
            for (int j = 0; j < Settings.Walls[i].ExteriorWalls.Length; j++)
            {
                pos = RotateAroundVector2Int(Settings.Walls[i].RotationOffset, Settings.MapSize, Settings.Walls[i].ExteriorWalls[j]);
                newboard[pos.x, pos.y].wall = WallType.ExteriorWall;
                newboard[pos.x, pos.y].ownerID = Settings.Walls[i].ID;
            }
            for (int j = 0; j < Settings.Walls[i].ExteriorGates.Length; j++)
            {
                pos = RotateAroundVector2Int(Settings.Walls[i].RotationOffset, Settings.MapSize, Settings.Walls[i].ExteriorGates[j]);
                newboard[pos.x, pos.y].wall = WallType.ExteriorGate;
                newboard[pos.x, pos.y].ownerID = Settings.Walls[i].ID;
            }
            for (int j = 0; j < Settings.Walls[i].InsideTiles.Length; j++)
            {
                pos = RotateAroundVector2Int(Settings.Walls[i].RotationOffset, Settings.MapSize, Settings.Walls[i].InsideTiles[j]);
                newboard[pos.x, pos.y].wall = WallType.Inside;
                newboard[pos.x, pos.y].ownerID = Settings.Walls[i].ID;
            }
        }

        return newboard;
    }

    public static Vector2Int RotateAroundVector2Int(CompassRotation Rotation, Vector2Int BoardSize, Vector2Int pos)
    {
        Vector2Int newpos = Vector2Int.RoundToInt(Quaternion.AngleAxis((int)Rotation * 90, Vector3.back) * new Vector3(pos.x, pos.y, 0));
        switch (Rotation)
        {
            case CompassRotation.North:
                return newpos;

            case CompassRotation.East:
                return newpos + Vector2Int.up * (BoardSize.y - 1);

            case CompassRotation.South:
                return newpos + (Vector2Int.up * (BoardSize.y - 1) + Vector2Int.right * (BoardSize.x - 1));

            case CompassRotation.West:
                return newpos + Vector2Int.right * (BoardSize.x - 1);

            default:
                return Vector2Int.zero;
        }
    }
}

[System.Serializable]
public struct TileInfo
{
    public int ownerID;
    public ChessFigure figure;
    public ChessBuiding building;
    public WallType wall;
    [HideInInspector] public Transform figureTransform;
    [HideInInspector] public Transform buildingTransform;
    public bool hasMoved;

    public TileInfo(int NewOwnerID)
    {
        ownerID = NewOwnerID;
        figure = ChessFigure.Empty;
        building = ChessBuiding.Empty;
        wall = WallType.None;
        figureTransform = null;
        buildingTransform = null;
        hasMoved = false;
    }

    public TileInfo(int NewOwnerID, ChessFigure Figure, Transform Object)
    {
        ownerID = NewOwnerID;
        figure = Figure;
        building = ChessBuiding.Empty;
        wall = WallType.None;
        figureTransform = Object;
        buildingTransform = null;
        hasMoved = false;
    }
    public TileInfo(int NewOwnerID, ChessBuiding Building, Transform Object)
    {
        ownerID = NewOwnerID;
        figure = ChessFigure.Empty;
        building = Building;
        wall = WallType.None;
        figureTransform = null;
        buildingTransform = Object;
        hasMoved = false;
    }
    public TileInfo(int NewOwnerID, TileInfo OldTile, WallType Wall)
    {
        ownerID = NewOwnerID;
        figure = OldTile.figure;
        building = OldTile.building;
        wall = Wall;
        figureTransform = OldTile.figureTransform;
        buildingTransform = OldTile.buildingTransform;
        hasMoved = OldTile.hasMoved;
    }
}

[System.Serializable]
public enum ChessFigure
{
    Empty, King, Queen, Bishop, Knight, Rook, Pawn,
}

[System.Serializable]
public enum ChessBuiding
{
    Empty, Farm, Mine, Barracks,
}

[System.Serializable]
public enum WallType
{
    None, InteriorWall, InteriorGate, ExteriorWall, ExteriorGate, Inside
}