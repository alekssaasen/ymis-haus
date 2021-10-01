using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameOver
{
    public static bool IsGameOver(int ID)
    {
        List<Vector2Int> totalMoves = FigureMovement.GetMoveableFigures(ID);

        if (totalMoves.Count == 0)
        {
            return true;
        }
        return false;
    }
}