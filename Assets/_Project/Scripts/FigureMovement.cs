using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FigureMovement
{
    private static readonly Vector2Int[] allDirections = new Vector2Int[8]
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
    };
    private static readonly int[] allDirectionsInverted = new int[] {4,5,6,7,0,1,2,3};

    private static readonly Vector2Int[] straightDirections = new Vector2Int[4]
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
    };
    private static readonly Vector2Int[] diagonalDirections = new Vector2Int[4]
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1),
    };
    private static readonly Vector2Int[] knightDirections = new Vector2Int[8]
    {
        new Vector2Int(1, 2),
        new Vector2Int(2, 1),
        new Vector2Int(2, -1),
        new Vector2Int(1, -2),
        new Vector2Int(-1, -2),
        new Vector2Int(-2, -1),
        new Vector2Int(-2, 1),
        new Vector2Int(-1, 2),
    };

    public static bool InsideBoard(Vector2Int Tile)
    {
        if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
            Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
        {
            return true;
        }
        return false;
    }

    private static WallBlock CrossedWall(Vector2Int Tile1, Vector2Int Tile2)
    {
        WallType Wall1 = GameManager.Main.Board[Tile1.x, Tile1.y].wall;
        WallType Wall2 = GameManager.Main.Board[Tile2.x, Tile2.y].wall;

        if (Wall1 != WallType.None && Wall2 != WallType.None)
        {
            if (((Wall1 == WallType.InteriorWall || Wall1 == WallType.InteriorGate) &&
                (Wall2 == WallType.ExteriorWall || Wall2 == WallType.ExteriorGate))||
                ((Wall1 == WallType.ExteriorWall || Wall1 == WallType.ExteriorGate) &&
                (Wall2 == WallType.InteriorWall || Wall2 == WallType.InteriorGate)))
            {
                if (Wall1 == WallType.InteriorGate || Wall1 == WallType.ExteriorGate ||
                    Wall2 == WallType.InteriorGate || Wall2 == WallType.ExteriorGate)
                {
                    return WallBlock.Stop;
                }
                else
                {
                    return WallBlock.Block;
                }
            }
        }
        return WallBlock.Pass;
    }



    public static List<Vector2Int> GetValidPositions(int ID, TileInfo FigureTile, Vector2Int Position)
    {
        List<Vector2Int> validpositions = new List<Vector2Int>();
        //Check-for-checks
        //Find our king
        Vector2Int OurKing = new Vector2Int();

        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
            {
                if (GameManager.Main.Board[x,y].figure == ChessFigure.King && GameManager.Main.Board[x, y].ownerID == ID)
                {
                    OurKing = new Vector2Int(x,y);
                }
            }
        }

        //Find checking/pinned pieces
        List<Vector2Int> checkingPieces = new List<Vector2Int>();
        List<Vector2Int> pinnedPieces = new List<Vector2Int>();
        List<Vector2Int> pinnableTiles = new List<Vector2Int>();
        List<Vector2Int> pinnedCheckingPieces = new List<Vector2Int>();
        List<Vector2Int> pinnedPinnableTiles = new List<Vector2Int>();
        List<List<Vector2Int[]>> pinnedCheckedPair = new List<List<Vector2Int[]>> ();

        bool knightCheck = false;

        for (int dir = 0; dir <= 3; dir++)
        {
            Vector2Int possiblePin = new Vector2Int(-1, -1);
            List<Vector2Int> possiblePinnableTiles = new List<Vector2Int>();
            for (int i = 1; i <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); i++)
            {
                Vector2Int Tile = OurKing + (straightDirections[dir] * i);
                if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                    Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                {
                    if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty)
                    {
                        if (GameManager.Main.Board[Tile.x, Tile.y].ownerID == ID)
                        {
                            if (possiblePin != new Vector2Int(-1, -1))
                            {
                                break;
                            }
                            possiblePin = Tile;
                        }
                        else
                        {
                            if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Rook ||
                                GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Queen)
                            {
                                if (possiblePin != new Vector2Int(-1,-1))
                                {
                                    pinnedPieces.Add(possiblePin);
                                    pinnedCheckingPieces.Add(Tile);
                                    possiblePinnableTiles.Add(Tile);
                                    pinnedPinnableTiles.AddRange(possiblePinnableTiles);
                                    List<Vector2Int[]> arrayList = new List<Vector2Int[]>();
                                    foreach (Vector2Int tile in pinnedPinnableTiles)
                                    {
                                        Vector2Int[] tilearray = new Vector2Int[2] {possiblePin,tile};
                                        arrayList.Add(tilearray);
                                    }
                                    pinnedCheckedPair.Add(arrayList);
                                }
                                else
                                {
                                    pinnableTiles.AddRange(possiblePinnableTiles);
                                    checkingPieces.Add(Tile);
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        possiblePinnableTiles.Add(Tile);
                    }
                }
            }
        }

        for (int dir = 0; dir <= 3; dir++)
        {
            Vector2Int possiblePin = new Vector2Int(-1, -1);
            List<Vector2Int> possiblePinnableTiles = new List<Vector2Int>();
            for (int i = 1; i <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); i++)
            {
                Vector2Int Tile = OurKing + (diagonalDirections[dir] * i);
                if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                    Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                {
                    if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty)
                    {
                        if (GameManager.Main.Board[Tile.x, Tile.y].ownerID == ID)
                        {
                            if (possiblePin != new Vector2Int(-1, -1))
                            {
                                break;
                            }
                            possiblePin = Tile;
                        }
                        else
                        {
                            if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Bishop ||
                                GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Queen)
                            {
                                if (possiblePin != new Vector2Int(-1, -1))
                                {
                                    pinnedPieces.Add(possiblePin);
                                    pinnedCheckingPieces.Add(Tile);
                                    possiblePinnableTiles.Add(Tile);
                                    pinnedPinnableTiles.AddRange(possiblePinnableTiles);
                                    List<Vector2Int[]> arrayList = new List<Vector2Int[]>();
                                    foreach (Vector2Int tile in pinnedPinnableTiles)
                                    {
                                        Vector2Int[] tilearray = new Vector2Int[2] { possiblePin, tile};
                                        arrayList.Add(tilearray);
                                    }
                                    pinnedCheckedPair.Add(arrayList);
                                }
                                else
                                {
                                    pinnableTiles.AddRange(possiblePinnableTiles);
                                    checkingPieces.Add(Tile);
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        possiblePinnableTiles.Add(Tile);
                    }
                }
            }
        }

        for (int dir = 0; dir <= 7; dir++)
        {
            Vector2Int Tile = OurKing + (knightDirections[dir]);
            if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
            {
                if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Knight && GameManager.Main.Board[Tile.x, Tile.y].ownerID != ID)
                {
                    checkingPieces.Add(Tile);
                    knightCheck = true;
                }
            }
        }


        CheckType checkType = CheckType.NoCheck;
        List<Vector2Int> onlyPossibleTiles = new List<Vector2Int>();

        if (checkingPieces.Count > 1)
        {
            checkType = CheckType.DoubbleCheck;
        }
        else if (knightCheck == true)
        {
            checkType = CheckType.KnightCheck;
            onlyPossibleTiles.AddRange(checkingPieces);
        }
        else if (checkingPieces.Count == 1)
        {
            checkType = CheckType.SingleCheck;
            onlyPossibleTiles.AddRange(pinnableTiles);
            onlyPossibleTiles.AddRange(checkingPieces);
        }
        else if (pinnedPieces.Count > 0)
        {
            checkType = CheckType.PinnedPiece;
        }
        else
        {
            checkType = CheckType.NoCheck;
        }


        
        switch (FigureTile.figure)
        {
            case ChessFigure.King:
                //King movement

                List<int> directionsToSkip = new List<int>();

                for (int i = 0; i <= 7; i++)
                {
                    Vector2Int wall1 = Position;
                    if (directionsToSkip.Contains(i) == false)
                    {
                        Vector2Int Tile = Position + allDirections[i];

                        if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                            Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                        {
                            Vector2Int wall2 = Tile;
                            if ((GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty ||
                                (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty && ID != GameManager.Main.Board[Tile.x, Tile.y].ownerID))&&
                                (GameManager.Main.Board[Tile.x, Tile.y].building == ChessBuiding.Empty ||
                                 GameManager.Main.Board[Tile.x, Tile.y].building != ChessBuiding.Empty && ID != GameManager.Main.Board[Tile.x, Tile.y].ownerID))
                            {
                                if(CrossedWall(wall1,wall2) == WallBlock.Pass||
                                    CrossedWall(wall1, wall2) == WallBlock.Stop)
                                {
                                    bool check = false;
                                    for (int dir = 0; dir <= 3; dir++)
                                    {
                                        for (int x = 1; x <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); x++)
                                        {
                                            Vector2Int tileToCheck = Tile + (straightDirections[dir] * x);
                                            if (InsideBoard(tileToCheck))
                                            {
                                                if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure != ChessFigure.Empty &&
                                                    GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID != ID)
                                                {
                                                    if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Rook ||
                                                        GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Queen)
                                                    {
                                                        check = true;
                                                        directionsToSkip.Add(allDirectionsInverted[i]);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }
                                                else if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure != ChessFigure.Empty &&
                                                    GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID == ID)
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    for (int dir = 0; dir <= 3; dir++)
                                    {
                                        for (int x = 1; x <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); x++)
                                        {
                                            Vector2Int tileToCheck = Tile + (diagonalDirections[dir] * x);
                                            if (InsideBoard(tileToCheck))
                                            {
                                                if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure != ChessFigure.Empty)
                                                {
                                                    if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID != ID)
                                                    {
                                                        if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Bishop ||
                                                            GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Queen)
                                                        {
                                                            check = true;
                                                            directionsToSkip.Add(allDirectionsInverted[i]);
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    for (int dir = 0; dir <= 7; dir++)
                                    {
                                        Vector2Int tileToCheck = Tile + knightDirections[dir];
                                        if (InsideBoard(tileToCheck))
                                        {
                                            if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure != ChessFigure.Empty)
                                            {
                                                if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID != ID)
                                                {
                                                    if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Knight)
                                                    {
                                                        check = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //placeholder gamemode statement
                                    if (GameManager.GameSettingsInUse.ClassicCheckmate)
                                    {
                                        int invert;
                                        if (ID == 0)
                                        {
                                            invert = 1;
                                        }
                                        else
                                        {
                                            invert = -1;
                                        }
                                        Vector2Int tileToCheck;
                                        tileToCheck = Tile + new Vector2Int(-1, 1 * invert);
                                        //check left
                                        if (InsideBoard(tileToCheck) && GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Pawn &&
                                            GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID != ID)
                                        {
                                            check = true;
                                        }
                                        //check right
                                        tileToCheck = Tile + new Vector2Int(1, 1 * invert);
                                        if (InsideBoard(tileToCheck) && GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Pawn &&
                                            GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID != ID)
                                        {
                                            check = true;
                                        }
                                    }
                                    else
                                    {
                                        for (int dir = 0; dir <= 3; dir++)
                                        {
                                            Vector2Int tileToCheck = Tile + diagonalDirections[dir];
                                            if (InsideBoard(tileToCheck) && GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Pawn &&
                                                GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID != ID)
                                            {
                                                check = true;
                                            }
                                        }
                                    }
                                    if (check == false)
                                    {
                                        validpositions.Add(Tile);
                                    }
                                }
                            }
                        }
                    }
                }

                break;
            case ChessFigure.Queen:
                //Queen movement, good
                if (checkType != CheckType.DoubbleCheck)
                {
                    for (int dir = 0; dir <= 7; dir++)
                    {
                        Vector2Int wall1 = Position;
                        for (int i = 1; i <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); i++)
                        {
                            Vector2Int Tile = Position + (allDirections[dir] * i);
                            if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                                Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                            {
                                Vector2Int wall2 = Tile;
                                if (CrossedWall(wall1, wall2) == WallBlock.Pass)
                                {
                                    if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty &&
                                        GameManager.Main.Board[Tile.x, Tile.y].building == ChessBuiding.Empty)
                                    {
                                        validpositions.Add(Tile);
                                        wall1 = wall2;
                                    }
                                    else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty ||
                                             GameManager.Main.Board[Tile.x, Tile.y].building != ChessBuiding.Empty)
                                    {
                                        if (GameManager.Main.Board[Tile.x, Tile.y].ownerID != ID)
                                        {
                                            validpositions.Add(Tile);
                                            break;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else if (CrossedWall(wall1,wall2) == WallBlock.Stop)
                                {
                                    if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty &&
                                        GameManager.Main.Board[Tile.x, Tile.y].building == ChessBuiding.Empty)
                                    {
                                        validpositions.Add(Tile);
                                        break;
                                    }
                                    else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty ||
                                             GameManager.Main.Board[Tile.x, Tile.y].building != ChessBuiding.Empty)
                                    {
                                        if (GameManager.Main.Board[Tile.x, Tile.y].ownerID != ID)
                                        {
                                            validpositions.Add(Tile);
                                            break;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;

            case ChessFigure.Bishop:
                //Bishop movement, good
                if (checkType != CheckType.DoubbleCheck)
                {
                    for (int dir = 0; dir <= 3; dir++)
                    {
                        Vector2Int wall1 = Position;
                        for (int i = 1; i <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); i++)
                        {
                            Vector2Int Tile = Position + (diagonalDirections[dir] * i);
                            if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                                Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                            {
                                Vector2Int wall2 = Tile;
                                if (CrossedWall(wall1, wall2) == WallBlock.Pass)
                                {
                                    if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty &&
                                        GameManager.Main.Board[Tile.x, Tile.y].building == ChessBuiding.Empty)
                                    {
                                        validpositions.Add(Tile);
                                        wall1 = wall2;
                                    }
                                    else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty ||
                                             GameManager.Main.Board[Tile.x, Tile.y].building != ChessBuiding.Empty)
                                    {
                                        if (GameManager.Main.Board[Tile.x, Tile.y].ownerID != ID)
                                        {
                                            validpositions.Add(Tile);
                                            break;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else if (CrossedWall(wall1, wall2) == WallBlock.Stop)
                                {
                                    if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty &&
                                        GameManager.Main.Board[Tile.x, Tile.y].building == ChessBuiding.Empty)
                                    {
                                        validpositions.Add(Tile);
                                        break;
                                    }
                                    else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty ||
                                             GameManager.Main.Board[Tile.x, Tile.y].building != ChessBuiding.Empty)
                                    {
                                        if (GameManager.Main.Board[Tile.x, Tile.y].ownerID != ID)
                                        {
                                            validpositions.Add(Tile);
                                            break;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;

            case ChessFigure.Knight:
                if (checkType != CheckType.DoubbleCheck)
                {
                    for (int i = 0; i < knightDirections.Length; i++)
                    {
                        Vector2Int Tile = Position + knightDirections[i];
                        if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                            Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                        {
                            if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty &&
                                    GameManager.Main.Board[Tile.x, Tile.y].building == ChessBuiding.Empty)
                            {
                                validpositions.Add(Tile);
                            }
                            else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty ||
                                     GameManager.Main.Board[Tile.x, Tile.y].building != ChessBuiding.Empty)
                            {
                                if (GameManager.Main.Board[Tile.x, Tile.y].ownerID != ID)
                                {
                                    validpositions.Add(Tile);
                                }
                            }
                        }
                    }
                }
                break;

            case ChessFigure.Rook:
                //Rook movement, good
                if (checkType != CheckType.DoubbleCheck)
                {
                    for (int dir = 0; dir <= 3; dir++)
                    {
                        Vector2Int wall1 = Position;
                        for (int i = 1; i <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); i++)
                        {
                            Vector2Int Tile = Position + (straightDirections[dir] * i);

                            if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                                Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                            {
                                Vector2Int wall2 = Tile;
                                if (CrossedWall(wall1,wall2) == WallBlock.Pass)
                                {
                                    if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty &&
                                        GameManager.Main.Board[Tile.x, Tile.y].building == ChessBuiding.Empty)
                                    {
                                        validpositions.Add(Tile);
                                        wall1 = wall2;
                                    }
                                    else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty ||
                                             GameManager.Main.Board[Tile.x, Tile.y].building != ChessBuiding.Empty)
                                    {
                                        if (GameManager.Main.Board[Tile.x, Tile.y].ownerID != ID)
                                        {
                                            validpositions.Add(Tile);
                                            break;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else if (CrossedWall(wall1,wall2) == WallBlock.Stop)
                                {
                                    if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty &&
                                        GameManager.Main.Board[Tile.x, Tile.y].building == ChessBuiding.Empty)
                                    {
                                        validpositions.Add(Tile);
                                        break;
                                    }
                                    else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty ||
                                             GameManager.Main.Board[Tile.x, Tile.y].building != ChessBuiding.Empty)
                                    {
                                        if (GameManager.Main.Board[Tile.x, Tile.y].ownerID != ID)
                                        {
                                            validpositions.Add(Tile);
                                            break;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;

            //Pawn movement, works as expected. however can be redone to use a new list of pawn directions like the rook/bishop/queen/king 
            //but you will still have edge cases that require its own if statement that it is unlikely worth it
            case ChessFigure.Pawn:
                if (checkType != CheckType.DoubbleCheck)
                {
                    //gamemode placeholder statement
                    if (GameManager.GameSettingsInUse.ClassicCheckmate)
                    {
                        //wheter or not to invert the direction, will always be multiplied by this number but when its 1 no change will occur
                        int invert;
                        Vector2Int Tile;
                        if (ID == 0)
                        {
                            invert = 1;
                        }
                        else
                        {
                            invert = -1;
                        }
                        //Forward and twice forward
                        Tile = new Vector2Int(Position.x, Position.y + (1 * invert));
                        if (InsideBoard(Tile) && GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                        {
                            validpositions.Add(Tile);
                            if (FigureTile.hasMoved == false)
                            {
                                Tile = new Vector2Int(Position.x, Position.y + (2 * invert));
                                if (InsideBoard(Tile) && GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                                {
                                    validpositions.Add(Tile);
                                }
                            }
                        }
                        //these could be a for loop where the offset on x is taken from a list but why would you do that i dont even think you save more than 2 lines doing that
                        //Left capture
                        Tile = new Vector2Int(Position.x - 1, Position.y + (1 * invert));
                        if (InsideBoard(Tile) && GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty && ID != GameManager.Main.Board[Tile.x, Tile.y].ownerID)
                        {
                            validpositions.Add(Tile);
                        }
                        //Right capture
                        Tile = new Vector2Int(Position.x + 1, Position.y + (1 * invert));
                        if (InsideBoard(Tile) && GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty && ID != GameManager.Main.Board[Tile.x, Tile.y].ownerID)
                        {
                            validpositions.Add(Tile);
                        }

                    }
                    else
                    {
                        Vector2Int Tile;
                        //Straigh movement
                        for (int dir = 0; dir <= 3; dir++)
                        {
                            Vector2Int wall1 = Position;
                            Tile = Position + straightDirections[dir];
                            if (InsideBoard(Tile) && GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty &&
                                                     GameManager.Main.Board[Tile.x, Tile.y].building == ChessBuiding.Empty)
                            {
                                Vector2Int wall2 = Tile;
                                if (CrossedWall(wall1, wall2) == WallBlock.Pass ||
                                    CrossedWall(wall1, wall2) == WallBlock.Stop)
                                {
                                    validpositions.Add(Tile);
                                }
                            }
                        }
                        //Diagonal capture
                        for (int dir = 0; dir <= 3; dir++)
                        {
                            Vector2Int wall1 = Position;
                            Tile = Position + diagonalDirections[dir];
                            if (InsideBoard(Tile) && GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty && ID != GameManager.Main.Board[Tile.x, Tile.y].ownerID ||
                                InsideBoard(Tile) && GameManager.Main.Board[Tile.x, Tile.y].building != ChessBuiding.Empty && ID != GameManager.Main.Board[Tile.x, Tile.y].ownerID)
                            {
                                Vector2Int wall2 = Tile;
                                if (CrossedWall(wall1, wall2) == WallBlock.Pass ||
                                    CrossedWall(wall1, wall2) == WallBlock.Stop)
                                {
                                    validpositions.Add(Tile);
                                }
                            }
                        }
                    }
                }
                break;

            default:
                Debug.LogError("Trying to move air!");
                break;
        }
        //calculate wheter or not a move will put the king in check
        if (checkType != CheckType.DoubbleCheck && checkType != CheckType.NoCheck)
        {
            List<Vector2Int> legalMoves = new List<Vector2Int>();
            foreach (Vector2Int piece in pinnedPieces)
            {
                if (piece.GetHashCode() == Position.GetHashCode())
                {
                    foreach (List<Vector2Int[]> list in pinnedCheckedPair)
                    {
                        if (list[0][0] == Position)
                        {
                            foreach (Vector2Int[] array in list)
                            {
                                legalMoves.Add(array[1]);
                            }
                        }
                    }

                    List<Vector2Int> newvalidpositions = new List<Vector2Int>();

                    foreach (Vector2Int item1 in validpositions)
                    {
                        foreach (Vector2Int item2 in legalMoves)
                        {
                            if (item2 == item1)
                            {
                                newvalidpositions.Add(item1);
                            }
                        }
                    }

                    validpositions = newvalidpositions;
                    break;
                }
            }
        }
        if(GameManager.Main.Board[Position.x, Position.y].figure != ChessFigure.King)
        {
            if(checkType == CheckType.KnightCheck || checkType == CheckType.SingleCheck)
            {
                List<Vector2Int> newvalidpositions = new List<Vector2Int>();

                foreach (Vector2Int item1 in validpositions)
                {
                    foreach (Vector2Int item2 in onlyPossibleTiles)
                    {
                        if (item2 == item1)
                        {
                            newvalidpositions.Add(item1);
                        }
                    }
                }

                validpositions = newvalidpositions;
            }
        }

        return validpositions;
    }

    public static List<Vector2Int> GetMoveableFigures(int ID, bool CheckMovementCost)
    {
        List<Vector2Int> figuresThatCanMove = new List<Vector2Int>();

        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
            {
                if (GameManager.Main.Board[x, y].figure != ChessFigure.Empty && GameManager.Main.Board[x, y].ownerID == ID && GetValidPositions(GameManager.Main.localPlayerID, GameManager.Main.Board[x, y], new Vector2Int(x, y)).Count > 0)
                {
                    if (CheckMovementCost && GameManager.GameSettingsInUse.GetMoveCost(GameManager.Main.Board[x, y].figure) <= GameManager.Main.turnPointsLeft)
                    {
                        figuresThatCanMove.Add(new Vector2Int(x, y));
                    }
                    else if (!CheckMovementCost)
                    {
                        figuresThatCanMove.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        return figuresThatCanMove;
    }

}

public enum CheckType
{
    DoubbleCheck, KnightCheck, SingleCheck, PinnedPiece, NoCheck
}

public enum WallBlock
{
    Pass, Stop, Block
}