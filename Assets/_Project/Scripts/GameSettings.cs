using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Settings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettings : ScriptableObject
{
    public ChessGameModes CurrentGameMode = ChessGameModes.ChessEmpires;

    public int MaxMovePoints;

    public int KingMoveCost = 5;
    public int QueenMoveCost = 5;
    public int BishopMoveCost = 2;
    public int KnightMoveCost = 3;
    public int RookMoveCost = 2;
    public int PawnMoveCost = 1;

    public int GetMoveCost(ChessFigure Figure)
    {
        if (CurrentGameMode == ChessGameModes.ClassicChess)
        {
            return 5;
        }

        switch (Figure)
        {
            case ChessFigure.Empty:
                return 0;

            case ChessFigure.King:
                return KingMoveCost;

            case ChessFigure.Queen:
                return QueenMoveCost;

            case ChessFigure.Bishop:
                return BishopMoveCost;

            case ChessFigure.Knight:
                return KnightMoveCost;

            case ChessFigure.Rook:
                return RookMoveCost;

            case ChessFigure.Pawn:
                return PawnMoveCost;
        }
        return -1;
    }
}

public enum ChessGameModes
{
    ClassicChess, ChessEmpires
}
