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

    private static MoveState CanMove(Vector2Int currentPosition, Vector2Int targetPosition, int id, ChessFigure figureType)
    {
        TileInfo[,] board = GameManager.Main.Board;
        bool forbidnext = false;
        bool pawnCapture = false;

        if (!InsideBoard(currentPosition) || !InsideBoard(targetPosition))
        {
            return MoveState.ForbidMove;
        }

        if (figureType == ChessFigure.King)
        {
            foreach (Vector2Int tile in GetCheckedTiles(id))
            {
                if (targetPosition == tile)
                {
                    return MoveState.ForbidMove;
                }
            }
        }


        if (board[targetPosition.x, targetPosition.y].figure != ChessFigure.Empty ||
            board[targetPosition.x, targetPosition.y].building != ChessBuiding.Empty)
        {
            if (board[targetPosition.x, targetPosition.y].ownerID == id)
            {
                return MoveState.ForbidMove;
            }
            else
            {
                forbidnext = true;
                if (figureType == ChessFigure.Knight)
                {
                    return MoveState.AllowMove;
                }
                if (figureType == ChessFigure.Pawn)
                {
                    pawnCapture = true;
                }
            }
        }
        if (figureType == ChessFigure.Knight)
        {
            return MoveState.AllowMove;
        }
        WallBlock wallBlock = CrossedWall(currentPosition,targetPosition);

        if (wallBlock == WallBlock.Block)
        {
            return MoveState.ForbidMove;
        }
        else if(wallBlock == WallBlock.Stop)
        {
            forbidnext = true;
        }
        if (forbidnext)
        {
            if (figureType == ChessFigure.Pawn &&
                pawnCapture)
            {
                return MoveState.PawnCapture;
            }
            return MoveState.ForbidNextMove;
        }
        else
        {
            if (figureType == ChessFigure.Pawn &&
                pawnCapture)
            {
                return MoveState.PawnCapture;
            }
            return MoveState.AllowMove;
        }
    }

    public static Vector2Int GetKing(int id)
    {
        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
            {
                if (GameManager.Main.Board[x, y].figure == ChessFigure.King &&
                    GameManager.Main.Board[x, y].ownerID == id)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one;
    }



    public static Vector2Int[] GetCheckedTiles (int id)
    {
        Vector2Int king = GetKing(id);

        List<Vector2Int> checkedTiles = new List<Vector2Int>();

        foreach (Vector2Int tilepos in allDirections)
        {
            Vector2Int tile = king + tilepos;

            for (int dir = 0; dir < straightDirections.Length; dir++)
            {
                Vector2Int firstTile = tile;
                List<Vector2Int> potentialyCheckedTiles = new List<Vector2Int>();
                potentialyCheckedTiles.Add(firstTile);
                for (int dist = 1; dist < Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); dist++)
                {
                    Vector2Int secondTile = tile + straightDirections[dir] * dist;
                    if (CanMove(firstTile, secondTile, id, ChessFigure.Empty) == MoveState.AllowMove)
                    {
                        potentialyCheckedTiles.Add(secondTile);
                        firstTile = secondTile;
                    }
                    else if (CanMove(firstTile, secondTile, id, ChessFigure.Empty) == MoveState.ForbidMove)
                    {
                        break;
                    }
                    else
                    {
                        if ((GameManager.Main.Board[secondTile.x,secondTile.y].figure == ChessFigure.Queen ||
                            GameManager.Main.Board[secondTile.x, secondTile.y].figure == ChessFigure.Rook) &&
                            GameManager.Main.Board[secondTile.x, secondTile.y].ownerID != id)
                        {
                            if (CrossedWall(firstTile,secondTile) == WallBlock.Pass)
                            {
                                checkedTiles.AddRange(potentialyCheckedTiles);
                            }
                            break;
                        }
                    }
                }
            }

            for (int dir = 0; dir < diagonalDirections.Length; dir++)
            {
                Vector2Int firstTile = tile;
                List<Vector2Int> potentialyCheckedTiles = new List<Vector2Int>();
                potentialyCheckedTiles.Add(firstTile);
                for (int dist = 1; dist < Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); dist++)
                {
                    Vector2Int secondTile = tile + diagonalDirections[dir] * dist;
                    if (CanMove(firstTile, secondTile, id, ChessFigure.Empty) == MoveState.AllowMove)
                    {
                        potentialyCheckedTiles.Add(secondTile);
                        firstTile = secondTile;
                    }
                    else if (CanMove(firstTile, secondTile, id, ChessFigure.Empty) == MoveState.ForbidMove)
                    {
                        break;
                    }
                    else
                    {
                        if ((GameManager.Main.Board[secondTile.x, secondTile.y].figure == ChessFigure.Queen ||
                            GameManager.Main.Board[secondTile.x, secondTile.y].figure == ChessFigure.Bishop) &&
                            GameManager.Main.Board[secondTile.x, secondTile.y].ownerID != id)
                        {
                            if (CrossedWall(firstTile,secondTile) == WallBlock.Pass)
                            {
                                checkedTiles.AddRange(potentialyCheckedTiles);
                            }
                            break;
                        }
                    }
                }
            }

            for (int dir = 0; dir < knightDirections.Length; dir++)
            {
                Vector2Int targetTile = tile + knightDirections[dir];
                if (!InsideBoard(targetTile))
                {
                    continue;
                }
                if (GameManager.Main.Board[targetTile.x, targetTile.y].figure == ChessFigure.Knight &&
                     GameManager.Main.Board[targetTile.x, targetTile.y].ownerID != id)
                {
                    if (CanMove(tile,targetTile,id,ChessFigure.Empty) != MoveState.ForbidMove)
                    {
                        checkedTiles.Add(targetTile);
                    }
                }
            }

            for (int dir = 0; dir < diagonalDirections.Length; dir++)
            {

                Vector2Int targetTile = tile + diagonalDirections[dir];
                if (!InsideBoard(targetTile))
                {
                    continue;
                }
                if (GameManager.Main.Board[targetTile.x, targetTile.y].figure == ChessFigure.Pawn &&
                     GameManager.Main.Board[targetTile.x, targetTile.y].ownerID != id)
                {
                    if (CanMove(tile,targetTile,id,ChessFigure.Empty)!=MoveState.ForbidMove)
                    {
                        checkedTiles.Add(targetTile);
                    }
                }
            }

            for (int dir = 0; dir < allDirections.Length; dir++)
            {
                Vector2Int targetTile = tile + allDirections[dir];
                if (!InsideBoard(targetTile))
                {
                    continue;
                }
                if (GameManager.Main.Board[targetTile.x, targetTile.y].figure == ChessFigure.King &&
                     GameManager.Main.Board[targetTile.x, targetTile.y].ownerID != id)
                {
                    if (CanMove(tile, targetTile, id, ChessFigure.Empty) != MoveState.ForbidMove)
                    {
                        checkedTiles.Add(targetTile);
                    }
                }
            }
        }
        return checkedTiles.ToArray();
    }

    public static bool GetChecksAndPins(int id, out List<PinnedPair>pinnedPairs, out List<Vector2Int>pinnables)
    {
        Vector2Int king = GetKing(id);
        pinnedPairs = new List<PinnedPair>();
        pinnables = new List<Vector2Int>();
        bool inCheck = false;

        foreach (Vector2Int dir in straightDirections)
        {
            Vector2Int firstTile = king;
            Vector2Int possiblePin = -Vector2Int.one;
            List<Vector2Int> possiblePinnables = new List<Vector2Int>();
            for (int dist = 1; dist < Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); dist++)
            {
                Vector2Int secondTile = king + (dir * dist);

                if (CanMove(firstTile, secondTile, id, ChessFigure.Empty) == MoveState.AllowMove)
                {
                    firstTile = secondTile;
                }
                else if (CanMove(firstTile, secondTile, id, ChessFigure.Empty) != MoveState.AllowMove)
                {
                    if (InsideBoard(secondTile))
                    {
                        if (CrossedWall(firstTile,secondTile)==WallBlock.Block)
                        {
                            break;
                        }
                        if (CrossedWall(firstTile, secondTile) == WallBlock.Stop)
                        {
                            if (firstTile != king)
                            {
                                break;
                            }
                        }
                        if (GameManager.Main.Board[secondTile.x,secondTile.y].building != ChessBuiding.Empty)
                        {
                            break;
                        }
                        else if (GameManager.Main.Board[secondTile.x, secondTile.y].figure != ChessFigure.Empty)
                        {
                            if (GameManager.Main.Board[secondTile.x, secondTile.y].ownerID == id)
                            {
                                if (possiblePin == -Vector2Int.one)
                                {
                                    possiblePin = secondTile;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else if (GameManager.Main.Board[secondTile.x, secondTile.y].figure == ChessFigure.Queen ||
                                        GameManager.Main.Board[secondTile.x, secondTile.y].figure == ChessFigure.Rook)
                            {
                                if (possiblePin != -Vector2Int.one)
                                {
                                    PinnedPair pinnedPair = new PinnedPair("wish i was in C# 10");
                                    pinnedPair.pinnedPeice = possiblePin;
                                    pinnedPair.directions[0] = dir;
                                    pinnedPair.directions[1] = -dir;
                                    pinnedPairs.Add(pinnedPair);
                                }
                                else
                                {
                                    possiblePinnables.Add(king + (dir * dist));
                                    inCheck = true;
                                    pinnables.AddRange(possiblePinnables);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        possiblePinnables.Add(king + (dir * dist));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        foreach (Vector2Int dir in diagonalDirections)
        {
            Vector2Int firstTile = king;
            Vector2Int possiblePin = -Vector2Int.one;
            List<Vector2Int> possiblePinnables = new List<Vector2Int>();
            for (int dist = 1; dist < Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); dist++)
            {
                Vector2Int secondTile = king + (dir * dist);

                if (CanMove(firstTile, secondTile, id, ChessFigure.Empty) == MoveState.AllowMove)
                {
                    firstTile = secondTile;
                }
                else if (CanMove(firstTile, secondTile, id, ChessFigure.Empty) != MoveState.AllowMove)
                {
                    if (InsideBoard(secondTile))
                    {
                        if (CrossedWall(firstTile, secondTile) == WallBlock.Block)
                        {
                            break;
                        }
                        if (CrossedWall(firstTile, secondTile) == WallBlock.Stop)
                        {
                            if (firstTile != king)
                            {
                                break;
                            }
                        }
                        if (GameManager.Main.Board[secondTile.x, secondTile.y].building != ChessBuiding.Empty)
                        {
                            break;
                        }
                        else if (GameManager.Main.Board[secondTile.x, secondTile.y].figure != ChessFigure.Empty)
                        {
                            if (GameManager.Main.Board[secondTile.x, secondTile.y].ownerID == id)
                            {
                                if (possiblePin == -Vector2Int.one)
                                {
                                    possiblePin = secondTile;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else if (GameManager.Main.Board[secondTile.x, secondTile.y].figure == ChessFigure.Queen ||
                                        GameManager.Main.Board[secondTile.x, secondTile.y].figure == ChessFigure.Bishop)
                            {
                                if (possiblePin != -Vector2Int.one)
                                {
                                    PinnedPair pinnedPair = new PinnedPair("wish i was in C# 10");
                                    pinnedPair.pinnedPeice = possiblePin;
                                    pinnedPair.directions[0] = dir;
                                    pinnedPair.directions[1] = -dir;
                                    pinnedPairs.Add(pinnedPair);
                                }
                                else
                                {
                                    inCheck = true;
                                    possiblePinnables.Add(king + (dir * dist));
                                    pinnables.AddRange(possiblePinnables);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        possiblePinnables.Add(king + (dir * dist));
                    }
                    else
                    {
                        break;
                    }
                }
            }

        }

        return inCheck;
    }
    
    public static List<Vector2Int> GetValidPositions(int id, TileInfo FigureTile, Vector2Int Position, out CheckType TypeOfCheck)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();


        switch (FigureTile.figure)
        {
            case ChessFigure.Empty:
                break;
            case ChessFigure.King:
                foreach (Vector2Int dir in allDirections)
                {
                    if (CanMove(Position,Position+dir,id,ChessFigure.King) != MoveState.ForbidMove)
                    {
                        validPositions.Add(Position + dir);
                    }
                }

                break;
            case ChessFigure.Queen:
                foreach (Vector2Int dir in allDirections)
                {
                    Vector2Int firstPos = Position;
                    for (int dist = 1; dist < Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); dist++)
                    {
                        Vector2Int secondPos = Position + dir * dist;
                        if (CanMove(firstPos,secondPos,id,ChessFigure.Queen) == MoveState.AllowMove)
                        {
                            validPositions.Add(Position + (dir * dist));
                            firstPos = secondPos;
                        }
                        else if (CanMove(firstPos, secondPos, id, ChessFigure.Queen) == MoveState.ForbidNextMove)
                        {
                            validPositions.Add(Position + (dir * dist));
                            break;
                        }
                        else if (CanMove(firstPos, secondPos, id, ChessFigure.Queen) == MoveState.ForbidMove)
                        {
                            break;
                        }
                    }
                }
                break;
            case ChessFigure.Bishop:
                foreach (Vector2Int dir in diagonalDirections)
                {
                    Vector2Int firstPos = Position;
                    for (int dist = 1; dist < Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); dist++)
                    {
                        Vector2Int secondPos = Position + dir * dist;
                        if (CanMove(firstPos, secondPos, id, ChessFigure.Bishop) == MoveState.AllowMove)
                        {
                            validPositions.Add(Position + (dir * dist));
                            firstPos = secondPos;
                        }
                        else if (CanMove(firstPos, secondPos, id, ChessFigure.Bishop) == MoveState.ForbidNextMove)
                        {
                            validPositions.Add(Position + (dir * dist));
                            break;
                        }
                        else if (CanMove(firstPos, secondPos, id, ChessFigure.Bishop) == MoveState.ForbidMove)
                        {
                            break;
                        }
                    }
                }
                break;
            case ChessFigure.Knight:
                foreach (Vector2Int dir in knightDirections)
                {
                    if (CanMove(Position, Position + dir, id, ChessFigure.Knight) != MoveState.ForbidMove)
                    {
                        validPositions.Add(Position + dir);
                    }
                }
                break;
            case ChessFigure.Rook:
                foreach (Vector2Int dir in straightDirections)
                {
                    Vector2Int firstPos = Position;
                    for (int dist = 1; dist < Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); dist++)
                    {
                        Vector2Int secondPos = Position + dir * dist;
                        if (CanMove(firstPos, secondPos, id, ChessFigure.Rook) == MoveState.AllowMove)
                        {
                            validPositions.Add(Position + (dir * dist));
                            firstPos = secondPos;
                        }
                        else if (CanMove(firstPos, secondPos, id, ChessFigure.Rook) == MoveState.ForbidNextMove)
                        {
                            validPositions.Add(Position + (dir * dist));
                            break;
                        }
                        else if (CanMove(firstPos, secondPos, id, ChessFigure.Rook) == MoveState.ForbidMove)
                        {
                            break;
                        }
                    }
                }
                break;
            case ChessFigure.Pawn:
                foreach (Vector2Int dir in diagonalDirections)
                {
                    if (CanMove(Position, Position + dir, id, ChessFigure.Pawn) == MoveState.PawnCapture)
                    {
                        validPositions.Add(Position + dir);
                    }
                }
                foreach (Vector2Int dir in straightDirections)
                {
                    if (CanMove(Position, Position + dir, id, ChessFigure.Pawn) != MoveState.ForbidMove &&
                        CanMove(Position, Position + dir, id, ChessFigure.Pawn) != MoveState.PawnCapture)
                    {
                        validPositions.Add(Position + dir);
                    }
                }
                break;
            default:
                break;
        }

        List<PinnedPair> pairs;
        List<Vector2Int> pinnables;
        bool check = GetChecksAndPins(id, out pairs, out pinnables);
        List<Vector2Int> pinnedValidTiles = new List<Vector2Int>();

        foreach (PinnedPair pair in pairs)
        {
            if (pair.pinnedPeice == Position)
            {
                for (int dist = 1; dist <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); dist++)
                {
                    if (InsideBoard(Position + (pair.directions[0] * dist)))
                    {
                        pinnedValidTiles.Add(Position + (pair.directions[0] * dist));
                    }
                    if (InsideBoard(Position + (pair.directions[1] * dist)))
                    {
                        pinnedValidTiles.Add(Position + (pair.directions[1] * dist));
                    }
                }
                break;
            }
        }

        List<Vector2Int> newValidTiles = new List<Vector2Int>();


        if (pinnedValidTiles.Count > 0)
        {
            foreach (Vector2Int tile in validPositions)
            {
                foreach (Vector2Int pTile in pinnedValidTiles)
                {
                    if (tile == pTile)
                    {
                        newValidTiles.Add(pTile);
                    }
                }
            }
            validPositions = newValidTiles;
        }

        newValidTiles = new List<Vector2Int>();
        if (check)
        {
            if (FigureTile.figure != ChessFigure.King)
            {
                foreach (Vector2Int tile in validPositions)
                {
                    foreach (Vector2Int pTile in pinnables)
                    {
                        if (tile == pTile)
                        {
                            newValidTiles.Add(pTile);
                        }
                    }
                }
                validPositions = newValidTiles;
            }
        }



        TypeOfCheck = CheckType.NoCheck;

        return validPositions;
    }
    
    
    



    /*
    public static List<Vector2Int> GetValidPositions(int ID, TileInfo FigureTile, Vector2Int Position,out CheckType TypeOfCheck)
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
            WallBlock block1 = WallBlock.Pass;
            WallBlock block2 = WallBlock.Pass;


            if (InsideBoard(straightDirections[dir]))
            {
                block1 = CrossedWall(OurKing, straightDirections[dir]);
                if (InsideBoard(straightDirections[dir]*2))
                {
                    block2 = CrossedWall(straightDirections[dir],straightDirections[dir]*2);
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
            if (block1 != WallBlock.Pass &&
                block2 != WallBlock.Stop &&
                (block1 != WallBlock.Block ||
                block2 != WallBlock.Block))
            {
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
                                if (GameManager.Main.Board[Tile.x, Tile.y].building != ChessBuiding.Empty)
                                {
                                    break;
                                }

                                if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Rook ||
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
                                            Vector2Int[] tilearray = new Vector2Int[2] { possiblePin, tile };
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

        TypeOfCheck = checkType;

        
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
                                        Vector2Int wall1check = Tile;
                                        if (!InsideBoard(Tile + straightDirections[dir]))
                                        {
                                            break;
                                        }
                                        WallBlock firstCheck = CrossedWall(wall1check, Tile + (straightDirections[dir]));
                                        WallBlock secondCheck = WallBlock.Pass;
                                        for (int x = 1; x <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); x++)
                                        {
                                            Vector2Int tileToCheck = Tile + (straightDirections[dir] * x);
                                            if (InsideBoard(tileToCheck))
                                            {
                                                Vector2Int wall2check = tileToCheck;
                                                if (CrossedWall(wall1check, wall2check) == WallBlock.Pass ||
                                                    CrossedWall(wall1check, wall2check) == WallBlock.Stop)
                                                {
                                                    WallBlock blocktype = CrossedWall(wall1check, wall2check);
                                                    wall1check = wall2check;
                                                    
                                                    if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure != ChessFigure.Empty &&
                                                        GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID != ID ||
                                                        GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.King &&
                                                        GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID == ID)
                                                    {
                                                        if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Rook ||
                                                            GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Queen)
                                                        {
                                                            if (firstCheck == WallBlock.Pass &&
                                                                secondCheck != WallBlock.Block)
                                                            {
                                                                check = true;
                                                                //directionsToSkip.Add(allDirectionsInverted[i]);
                                                            }
                                                            else if (firstCheck == WallBlock.Stop)
                                                            {
                                                                check = true;
                                                            }
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
                                                    else if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].building != ChessBuiding.Empty)
                                                    {
                                                        break;
                                                    }
                                                    else if (blocktype == WallBlock.Stop)
                                                    {
                                                        secondCheck = WallBlock.Stop;
                                                    }
                                                    else if (secondCheck == WallBlock.Stop)
                                                    {
                                                        break;
                                                    }
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
                                    for (int dir = 0; dir <= 3; dir++)
                                    {
                                        Vector2Int wall1check = Tile;
                                        if (!InsideBoard(Tile + diagonalDirections[dir]))
                                        {
                                            break;
                                        }
                                        WallBlock firstCheck = CrossedWall(wall1check, Tile + (diagonalDirections[dir]));
                                        WallBlock secondCheck = WallBlock.Pass;
                                        for (int x = 1; x <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); x++)
                                        {
                                            Vector2Int tileToCheck = Tile + (diagonalDirections[dir] * x);
                                            if (InsideBoard(tileToCheck))
                                            {
                                                Vector2Int wall2check = tileToCheck;
                                                if (CrossedWall(wall1check, wall2check) == WallBlock.Pass ||
                                                    CrossedWall(wall1check, wall2check) == WallBlock.Stop)
                                                {
                                                    WallBlock blocktype = CrossedWall(wall1check, wall2check);
                                                    wall1check = wall2check;

                                                    if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure != ChessFigure.Empty &&
                                                        GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID != ID ||
                                                        GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.King &&
                                                        GameManager.Main.Board[tileToCheck.x, tileToCheck.y].ownerID == ID)
                                                    {
                                                        if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Bishop ||
                                                            GameManager.Main.Board[tileToCheck.x, tileToCheck.y].figure == ChessFigure.Queen)
                                                        {
                                                            if (firstCheck == WallBlock.Pass &&
                                                                secondCheck == WallBlock.Pass)
                                                            {
                                                                check = true;
                                                                //directionsToSkip.Add(allDirectionsInverted[i]);
                                                            }
                                                            else if (firstCheck == WallBlock.Stop)
                                                            {
                                                                check = true;
                                                            }
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
                                                    else if (GameManager.Main.Board[tileToCheck.x, tileToCheck.y].building != ChessBuiding.Empty)
                                                    {
                                                        break;
                                                    }
                                                    else if (blocktype == WallBlock.Stop)
                                                    {
                                                        secondCheck = WallBlock.Stop;
                                                    }
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
                                    if (GameManager.GameSettingsInUse.ClassicMovement)
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
                    if (GameManager.GameSettingsInUse.ClassicMovement)
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
    */
    public static List<Vector2Int> GetMoveableFigures(int ID, bool CheckMovementCost)
    {
        List<Vector2Int> figuresThatCanMove = new List<Vector2Int>();

        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
            {
                if (GameManager.Main.Board[x, y].figure != ChessFigure.Empty && GameManager.Main.Board[x, y].ownerID == ID && GetValidPositions(GameManager.Main.localPlayerID, GameManager.Main.Board[x, y], new Vector2Int(x, y),out CheckType discard).Count > 0)
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

public enum MoveState
{
    AllowMove, ForbidMove, ForbidNextMove,PawnCapture
}

public struct PinnedPair
{
    public Vector2Int pinnedPeice;
    public Vector2Int[] directions;

    public PinnedPair(string anything)
    {
        directions = new Vector2Int[2];
        pinnedPeice = -Vector2Int.one;
    }
}