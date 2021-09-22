using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardMaster
{
    public static TileInfo[,] CreateClasicChessBoard()
    {
        // This creates a default Chess board with Figures (does not instantiate so you need to call "UpdateFigures()" later)
        TileInfo[,] newboard = new TileInfo[8, 8];

        for (int i = 0; i < 8; i++)
        {
            newboard[i, 1] = new TileInfo(0, ChessFigure.Pawn, null);
            newboard[i, 6] = new TileInfo(1, ChessFigure.Pawn, null);
        }

        newboard[0, 0] = new TileInfo(0, ChessFigure.Rook, null);
        newboard[1, 0] = new TileInfo(0, ChessFigure.Knight, null);
        newboard[2, 0] = new TileInfo(0, ChessFigure.Bishop, null);

        newboard[3, 0] = new TileInfo(0, ChessFigure.Queen, null);
        newboard[4, 0] = new TileInfo(0, ChessFigure.King, null);

        newboard[5, 0] = new TileInfo(0, ChessFigure.Bishop, null);
        newboard[6, 0] = new TileInfo(0, ChessFigure.Knight, null);
        newboard[7, 0] = new TileInfo(0, ChessFigure.Rook, null);

        newboard[0, 7] = new TileInfo(1, ChessFigure.Rook, null);
        newboard[1, 7] = new TileInfo(1, ChessFigure.Knight, null);
        newboard[2, 7] = new TileInfo(1, ChessFigure.Bishop, null);

        newboard[3, 7] = new TileInfo(1, ChessFigure.Queen, null);
        newboard[4, 7] = new TileInfo(1, ChessFigure.King, null);

        newboard[5, 7] = new TileInfo(1, ChessFigure.Bishop, null);
        newboard[6, 7] = new TileInfo(1, ChessFigure.Knight, null);
        newboard[7, 7] = new TileInfo(1, ChessFigure.Rook, null);

        return newboard;
    }

    public static TileInfo[,] CreateChessEmpiresBoard()
    {
        TileInfo[,] newboard = new TileInfo[14, 14];

        newboard[1, 1] = new TileInfo(0, ChessFigure.King, null);
        newboard[12, 12] = new TileInfo(1, ChessFigure.King, null);

        newboard[1, 2] = new TileInfo(0, ChessFigure.Pawn, null);
        newboard[12, 11] = new TileInfo(1, ChessFigure.Pawn, null);

        newboard[2, 11] = new TileInfo(-1, ChessBuiding.Mine, null);
        newboard[4, 8] = new TileInfo(-1, ChessBuiding.Mine, null);

        newboard[9, 5] = new TileInfo(-1, ChessBuiding.Mine, null);
        newboard[11, 2] = new TileInfo(-1, ChessBuiding.Mine, null);

        if (true)
        {
            newboard[2, 12] = new TileInfo(0, ChessFigure.Pawn, null);
            newboard[4, 9] = new TileInfo(1, ChessFigure.Pawn, null);

            newboard[9, 6] = new TileInfo(0, ChessFigure.Pawn, null);
            newboard[11, 3] = new TileInfo(1, ChessFigure.Pawn, null);
        }

        // Walls
        newboard[0, 2].wall = WallType.InteriorWall;
        newboard[0, 3].wall = WallType.ExteriorWall;
        newboard[1, 2].wall = WallType.InteriorGate;
        newboard[1, 3].wall = WallType.ExteriorGate;
        newboard[2, 2].wall = WallType.InteriorWall;
        newboard[2, 3].wall = WallType.ExteriorWall;
        newboard[3, 3].wall = WallType.ExteriorWall;
        newboard[3, 2].wall = WallType.ExteriorWall;
        newboard[2, 1].wall = WallType.InteriorGate;
        newboard[3, 1].wall = WallType.ExteriorGate;
        newboard[2, 0].wall = WallType.InteriorWall;
        newboard[3, 0].wall = WallType.ExteriorWall;

        int x = newboard.GetLength(0) - 1;
        int y = newboard.GetLength(1) - 1;

        newboard[x - 0, y - 2].wall = WallType.InteriorWall;
        newboard[x - 0, y - 3].wall = WallType.ExteriorWall;
        newboard[x - 1, y - 2].wall = WallType.InteriorGate;
        newboard[x - 1, y - 3].wall = WallType.ExteriorGate;
        newboard[x - 2, y - 2].wall = WallType.InteriorWall;
        newboard[x - 2, y - 3].wall = WallType.ExteriorWall;
        newboard[x - 3, y - 3].wall = WallType.ExteriorWall;
        newboard[x - 3, y - 2].wall = WallType.ExteriorWall;
        newboard[x - 2, y - 1].wall = WallType.InteriorGate;
        newboard[x - 3, y - 1].wall = WallType.ExteriorGate;
        newboard[x - 2, y - 0].wall = WallType.InteriorWall;
        newboard[x - 3, y - 0].wall = WallType.ExteriorWall;

        // Inside walls
        newboard[0, 1].wall = WallType.Inside;
        newboard[0, 1].ownerID = 0;
        newboard[1, 1].wall = WallType.Inside;
        newboard[1, 1].ownerID = 0;
        newboard[1, 0].wall = WallType.Inside;
        newboard[1, 0].ownerID = 0;
        newboard[0, 0].wall = WallType.Inside;
        newboard[0, 0].ownerID = 0;

        newboard[13, 12].wall = WallType.Inside;
        newboard[13, 12].ownerID = 1;
        newboard[12, 12].wall = WallType.Inside;
        newboard[12, 12].ownerID = 1;
        newboard[12, 13].wall = WallType.Inside;
        newboard[12, 13].ownerID = 1;
        newboard[13, 13].wall = WallType.Inside;
        newboard[13, 13].ownerID = 1;

        return newboard;
    }
}

[System.Serializable]
public struct TileInfo
{
    public int ownerID;
    public ChessFigure figure;
    public ChessBuiding building;
    public WallType wall;
    public Transform figureTransform;
    public Transform buildingTransform;
    public bool hasMoved;

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
    public TileInfo(int NewOwnerID, WallType Wall)
    {
        ownerID = NewOwnerID;
        figure = ChessFigure.Empty;
        building = ChessBuiding.Empty;
        wall = Wall;
        figureTransform = null;
        buildingTransform = null;
        hasMoved = false;
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
