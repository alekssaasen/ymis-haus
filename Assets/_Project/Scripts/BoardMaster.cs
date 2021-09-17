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

        newboard[1, 2] = new TileInfo(0, ChessFigure.Pawn, null, false);
        newboard[12, 11] = new TileInfo(1, ChessFigure.Pawn, null, false);

        newboard[2, 11] = new TileInfo(-1, ChessBuilding.Mine, null);
        newboard[4, 8] = new TileInfo(-1, ChessBuilding.Mine, null);

        newboard[9, 5] = new TileInfo(-1, ChessBuilding.Mine, null);
        newboard[11, 2] = new TileInfo(-1, ChessBuilding.Mine, null);

        if (true)
        {
            newboard[2, 12] = new TileInfo(0, ChessFigure.Pawn, null);
            newboard[4, 9] = new TileInfo(1, ChessFigure.Pawn, null);

            newboard[9, 6] = new TileInfo(0, ChessFigure.Pawn, null);
            newboard[11, 3] = new TileInfo(1, ChessFigure.Pawn, null);
        }

        return newboard;
    }
}

[System.Serializable]
public struct TileInfo
{
    public int ownerID;
    public ChessFigure figure;
    public ChessBuilding building;
    public Transform figureTransform;
    public Transform buildingTransform;
    public bool hasMoved;

    public TileInfo(int NewOwnerID, ChessFigure Figure, Transform Object)
    {
        ownerID = NewOwnerID;
        figure = Figure;
        building = ChessBuilding.Empty;
        figureTransform = Object;
        buildingTransform = null;
        hasMoved = false;
    }
    public TileInfo(int NewOwnerID, ChessFigure Figure, Transform Object, bool HasMoved)
    {
        ownerID = NewOwnerID;
        figure = Figure;
        building = ChessBuilding.Empty;
        figureTransform = Object;
        buildingTransform = null;
        hasMoved = HasMoved;
    }
    public TileInfo(int NewOwnerID, ChessBuilding Building, Transform Object)
    {
        ownerID = NewOwnerID;
        figure = ChessFigure.Empty;
        building = Building;
        figureTransform = null;
        buildingTransform = Object;
        hasMoved = false;
    }
}

[System.Serializable]
public enum ChessFigure
{
    Empty, King, Queen, Bishop, Knight, Rook, Pawn,
}

[System.Serializable]
public enum ChessBuilding
{
    Empty, Mine, Farm, Barracks,
}
