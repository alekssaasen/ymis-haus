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

    public static List<Vector2Int> GetValidPositions(bool IsWhite, TileInfo FigureTile, Vector2Int Position)
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
                        else if (GameManager.Main.Board[Tile.x, Tile.y].figure != ChessFigure.Empty && GameManager.Main.Board[Tile.x, Tile.y].isWhite != IsWhite)
                        {
                            validpositions.Add(Tile);
                        }
                    }
                }

                break;

            case ChessFigure.Queen:
                //Queen movement, good

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
                                if (GameManager.Main.Board[Tile.x, Tile.y].isWhite != IsWhite)
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
                break;

            case ChessFigure.Bishop:
                //Bishop movement, good

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
                                if (GameManager.Main.Board[Tile.x, Tile.y].isWhite != IsWhite)
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
                break;

            case ChessFigure.Knight:

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
                            if (GameManager.Main.Board[Tile.x, Tile.y].isWhite != IsWhite)
                            {
                                validpositions.Add(Tile);
                            }
                        }
                    }
                }

                break;

            case ChessFigure.Rook:
                //Rook movement, good

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
                                if (GameManager.Main.Board[Tile.x, Tile.y].isWhite != IsWhite)
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
                break;

            //Pawn movement, works as expected. however can be redone to use a new list of pawn directions like the rook/bishop/queen/king 
            //but you will still have edge cases that require its own if statement that it is unlikely worth it
            case ChessFigure.Pawn:
                if (distancefromtop > 0 &&
                    IsWhite && GameManager.Main.Board[Position.x, Position.y + 1].figure == ChessFigure.Empty)
                {
                    validpositions.Add(Position + Vector2Int.up);

                    if (distancefromtop > 0 &&
                    IsWhite && GameManager.Main.Board[Position.x, Position.y + 2].figure == ChessFigure.Empty && !FigureTile.hasMoved)
                    {
                        validpositions.Add(Position + Vector2Int.up * 2);
                    }
                }
                if (distancefromtop > 0 && distancefromright > 0 &&
                    IsWhite && GameManager.Main.Board[Position.x + 1, Position.y + 1].figure != ChessFigure.Empty && !GameManager.Main.Board[Position.x + 1, Position.y + 1].isWhite)
                {
                    validpositions.Add(Position + Vector2Int.up + Vector2Int.right);
                }
                if (distancefromtop > 0 && distancefromleft > 0 &&
                    IsWhite && GameManager.Main.Board[Position.x - 1, Position.y + 1].figure != ChessFigure.Empty && !GameManager.Main.Board[Position.x - 1, Position.y + 1].isWhite)
                {
                    validpositions.Add(Position + Vector2Int.up + Vector2Int.left);
                }


                if (distancefrombottom > 0 &&
                    !IsWhite && GameManager.Main.Board[Position.x, Position.y - 1].figure == ChessFigure.Empty)
                {
                    validpositions.Add(Position + Vector2Int.down);

                    if (distancefrombottom > 0 &&
                        !IsWhite && GameManager.Main.Board[Position.x, Position.y - 2].figure == ChessFigure.Empty && !FigureTile.hasMoved)
                    {
                        validpositions.Add(Position + Vector2Int.down * 2);
                    }
                }
                if (distancefrombottom > 0 && distancefromright > 0 &&
                    !IsWhite && GameManager.Main.Board[Position.x + 1, Position.y - 1].figure != ChessFigure.Empty && GameManager.Main.Board[Position.x + 1, Position.y - 1].isWhite)
                {
                    validpositions.Add(Position + Vector2Int.down + Vector2Int.right);
                }
                if (distancefrombottom > 0 && distancefromleft > 0 &&
                    !IsWhite && GameManager.Main.Board[Position.x - 1, Position.y - 1].figure != ChessFigure.Empty && GameManager.Main.Board[Position.x - 1, Position.y - 1].isWhite)
                {
                    validpositions.Add(Position + Vector2Int.down + Vector2Int.left);
                }
                break;

            default:
                Debug.LogError("Trying to move air!");
                break;
        }

        return validpositions;
    }
}
