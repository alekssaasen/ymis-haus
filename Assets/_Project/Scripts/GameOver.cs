using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameOver
{
    public static bool DidThePlayerLose(int ID)
    {
        List<Vector2Int> totalMoves = FigureMovement.GetMoveableFigures(ID, false);

        if (totalMoves.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool DidThePlayerWin(int ID)
    {
        for (int x = 0; x < GameManager.Main.Board.GetLength(0); x++)
        {
            for (int y = 0; y < GameManager.Main.Board.GetLength(1); y++)
            {
                if (GameManager.Main.Board[x, y].ownerID != ID && (GameManager.Main.Board[x, y].figure != ChessFigure.Empty || GameManager.Main.Board[x, y].building != ChessBuiding.Empty))
                {
                    return false;
                }
            }
        }
        return true;
    }
}