using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FigureBuilding
{
    private static readonly Vector2Int[] Directions = new Vector2Int[8]
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

    public static List<Vector2Int> GetValidFoundations(int ID)
    {
        List<Vector2Int> validfoundations = new List<Vector2Int>();
        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
            {
                if (EconomySystem.CanBuyBuilding() && GameManager.Main.Board[x, y].figure == ChessFigure.Pawn && GameManager.Main.Board[x, y].ownerID == ID)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (FigureMovement.InsideBoard(new Vector2Int(x + Directions[i].x, y + Directions[i].y)) &&
                            GameManager.Main.Board[x + Directions[i].x, y + Directions[i].y].building == ChessBuiding.Empty)
                        {
                            if (!validfoundations.Contains(new Vector2Int(x, y) + Directions[i]))
                            {
                                validfoundations.Add(new Vector2Int(x, y) + Directions[i]);
                            }
                        }
                    }
                }
                if (EconomySystem.CanBuyBuilding() && GameManager.Main.Board[x, y].wall != WallType.None && GameManager.Main.Board[x, y].ownerID == ID)
                {
                    if (!validfoundations.Contains(new Vector2Int(x, y)))
                    {
                        validfoundations.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        return validfoundations;
    }

    public static List<Vector2Int> GetValidSpawnpoints(int ID)
    {
        List<Vector2Int> validspawnpoints = new List<Vector2Int>();
        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
            {
                if (EconomySystem.CanBuyFigure() && GameManager.Main.Board[x, y].building == ChessBuiding.Barracks && GameManager.Main.Board[x, y].ownerID == ID)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (GameManager.Main.Board[x + Directions[i].x, y + Directions[i].y].figure == ChessFigure.Empty)
                        {
                            if (!validspawnpoints.Contains(new Vector2Int(x, y) + Directions[i]))
                            {
                                validspawnpoints.Add(new Vector2Int(x, y) + Directions[i]);
                            }
                        }
                    }
                }
                if (EconomySystem.CanBuyFigure() && (GameManager.Main.Board[x, y].wall != WallType.None &&
                    GameManager.Main.Board[x, y].wall != WallType.ExteriorWall &&
                    GameManager.Main.Board[x, y].wall != WallType.ExteriorGate) && GameManager.Main.Board[x, y].ownerID == ID)
                {
                    if (!validspawnpoints.Contains(new Vector2Int(x, y)))
                    {
                        validspawnpoints.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        return validspawnpoints;
    }
}
