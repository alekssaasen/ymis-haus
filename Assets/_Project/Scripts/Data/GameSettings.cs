using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Settings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Gamemode Settings")]
    public string GamemodeName = "ChessEmpires";
    public int MinPlayerCount = 2;
    public int MaxPlayerCount = 2;
    public Vector2Int MapSize = new Vector2Int(14, 14);
    public bool CanBuildBuildings = true;
    public bool CanSpawnFigures = true;
    public bool ClassicCheckmate = false;
    public bool ClassicMovement = false;
    public float[] CameraRotationOffsets = { 45, 225 };
    public Vector3[] CameraPositionOffsets = { new Vector3(1, 0, 1), new Vector3(12, 0, 12) };

    [Header("Movement costs")]
    public int MovePointsPerTurn = 5;
    [Space(3)]
    public int KingMoveCost = 5;
    public int QueenMoveCost = 5;
    public int BishopMoveCost = 2;
    public int KnightMoveCost = 3;
    public int RookMoveCost = 2;
    public int PawnMoveCost = 1;

    [Header("Figure costs")]
    public int QueenSpawnCost = 75;
    public int BishopSpawnCost = 25;
    public int KnightSpawnCost = 40;
    public int RookSpawnCost = 25;
    public int PawnSpawnCost = 5;

    [Header("Building costs")]
    public int FarmCreationCost = 10;
    public int BarracksCreationCost = 25;

    [Header("Building costs")]
    public int GoldPerTurn = 1;
    public int StartingGold = 10;
    public int GoldPerFarm = 3;
    public int GoldPerGoldMine = 5;

    public int GetMoveCost(ChessFigure Figure)
    {
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