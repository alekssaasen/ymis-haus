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

    public static List<Vector2Int> GetValidPositions(int ID, TileInfo FigureTile, Vector2Int Position)
    {
        List<Vector2Int> validpositions = new List<Vector2Int>();

        //Calculates distances from the map edge
        int distancefromtop = GameManager.Main.Board.GetLength(1) - Position.y - 1;
        int distancefromright = GameManager.Main.Board.GetLength(0) - Position.x - 1;
        int distancefrombottom = Position.y;
        int distancefromleft = Position.x;

        int distancefromtopright = Mathf.Min(distancefromtop, distancefromright);
        int distancefrombottomright = Mathf.Min(distancefrombottom, distancefromright);
        int distancefrombottomleft = Mathf.Min(distancefrombottom, distancefromleft);
        int distancefromtopleft = Mathf.Min(distancefromtop, distancefromleft);

        Debug.LogFormat("Distance from edges: (T={0}), (TR={1}), (R={2}), (BR={3}), (B={4}), (BL={5}), (L={6}), (TL={7})",
                        distancefromtop, distancefromtopright, distancefromright, distancefrombottomright,
                        distancefrombottom, distancefrombottomleft, distancefromleft, distancefromtopleft);
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





        foreach (Vector2Int item in checkingPieces)
        {
            Debug.Log("checking piece: " + item);
        }
        foreach (Vector2Int item in pinnedPieces)
        {
            Debug.Log("pinned piece: " + item);
        }
        foreach (Vector2Int item in pinnedCheckingPieces)
        {
            Debug.Log("pinned checking piece: " + item);
        }

        foreach (Vector2Int item in pinnableTiles)
        {
            Debug.Log("pinnable tile: " + item);
        }
        Debug.Log("checktype: " + checkType.ToString());

        foreach(List<Vector2Int[]> list in pinnedCheckedPair)
        {
            foreach(Vector2Int[]array in list)
            {
                Debug.LogError("checkpair: " + array[0] + array[1]);
            }
        }



        switch (FigureTile.figure)
        {
            case ChessFigure.King:
                //King movement, good

                for (int i = 0; i <= 7; i++)
                {
                    Vector2Int Tile = Position + allDirections[i];

                    if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                        Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                    {
                        if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                        {
                            validpositions.Add(Tile);
                        }
                        else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty && GameManager.Main.Board[Tile.x, Tile.y].ownerID != ID)
                        {
                            validpositions.Add(Tile);
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
                        for (int i = 1; i <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); i++)
                        {
                            Vector2Int Tile = Position + (allDirections[dir] * i);
                            if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                                Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                            {
                                if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                                {
                                    validpositions.Add(Tile);
                                }
                                else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty)
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
                        for (int i = 1; i <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); i++)
                        {
                            Vector2Int Tile = Position + (diagonalDirections[dir] * i);
                            if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                                Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                            {
                                if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                                {
                                    validpositions.Add(Tile);
                                }
                                else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty)
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
                            if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                            {
                                validpositions.Add(Tile);
                            }
                            else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty)
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
                        for (int i = 1; i <= Mathf.Max(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1)); i++)
                        {
                            Vector2Int Tile = Position + (straightDirections[dir] * i);
                            if (Tile.x < GameManager.Main.Board.GetLength(0) && Tile.x >= 0 &&
                                Tile.y < GameManager.Main.Board.GetLength(1) && Tile.y >= 0)
                            {
                                if (GameManager.Main.Board[Tile.x, Tile.y].figure == ChessFigure.Empty)
                                {
                                    validpositions.Add(Tile);
                                }
                                else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty)
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
                        }
                    }
                }
                break;

            //Pawn movement, works as expected. however can be redone to use a new list of pawn directions like the rook/bishop/queen/king 
            //but you will still have edge cases that require its own if statement that it is unlikely worth it
            case ChessFigure.Pawn:
                if (checkType != CheckType.DoubbleCheck)
                {
                    if (distancefromtop > 0 &&
                        (ID == GameManager.Main.localPlayerID) && GameManager.Main.Board[Position.x, Position.y + 1].figure == ChessFigure.Empty)
                    {
                        validpositions.Add(Position + Vector2Int.up);

                        if (distancefromtop > 0 &&
                        (ID == GameManager.Main.localPlayerID) && GameManager.Main.Board[Position.x, Position.y + 2].figure == ChessFigure.Empty && !FigureTile.hasMoved)
                        {
                            validpositions.Add(Position + Vector2Int.up * 2);
                        }
                    }
                    if (distancefromtop > 0 && distancefromright > 0 &&
                        (ID == GameManager.Main.localPlayerID) && GameManager.Main.Board[Position.x + 1, Position.y + 1].figure != ChessFigure.Empty && !(GameManager.Main.Board[Position.x + 1, Position.y + 1].ownerID == GameManager.Main.localPlayerID))
                    {
                        validpositions.Add(Position + Vector2Int.up + Vector2Int.right);
                    }
                    if (distancefromtop > 0 && distancefromleft > 0 &&
                        (ID == GameManager.Main.localPlayerID) && GameManager.Main.Board[Position.x - 1, Position.y + 1].figure != ChessFigure.Empty && !(GameManager.Main.Board[Position.x - 1, Position.y + 1].ownerID == GameManager.Main.localPlayerID))
                    {
                        validpositions.Add(Position + Vector2Int.up + Vector2Int.left);
                    }


                    if (distancefrombottom > 0 &&
                        !(ID == GameManager.Main.localPlayerID) && GameManager.Main.Board[Position.x, Position.y - 1].figure == ChessFigure.Empty)
                    {
                        validpositions.Add(Position + Vector2Int.down);

                        if (distancefrombottom > 0 &&
                            !(ID == GameManager.Main.localPlayerID) && GameManager.Main.Board[Position.x, Position.y - 2].figure == ChessFigure.Empty && !FigureTile.hasMoved)
                        {
                            validpositions.Add(Position + Vector2Int.down * 2);
                        }
                    }
                    if (distancefrombottom > 0 && distancefromright > 0 &&
                        !(ID == GameManager.Main.localPlayerID) && GameManager.Main.Board[Position.x + 1, Position.y - 1].figure != ChessFigure.Empty && GameManager.Main.Board[Position.x + 1, Position.y - 1].ownerID == GameManager.Main.localPlayerID)
                    {
                        validpositions.Add(Position + Vector2Int.down + Vector2Int.right);
                    }
                    if (distancefrombottom > 0 && distancefromleft > 0 &&
                        !(ID == GameManager.Main.localPlayerID) && GameManager.Main.Board[Position.x - 1, Position.y - 1].figure != ChessFigure.Empty && GameManager.Main.Board[Position.x - 1, Position.y - 1].ownerID == GameManager.Main.localPlayerID)
                    {
                        validpositions.Add(Position + Vector2Int.down + Vector2Int.left);
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
            Debug.Log("test: " + Position);
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
                    foreach (Vector2Int move in legalMoves)
                    {
                        Debug.Log("legalmove: " + move);
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
}

public enum CheckType
{
    DoubbleCheck,KnightCheck,SingleCheck,PinnedPiece,NoCheck
}