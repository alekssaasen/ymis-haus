using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Settings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Gamemode Main Settings")]
    public string GamemodeName = "ChessEmpires";
    public int MinPlayerCount = 2;
    public int MaxPlayerCount = 2;
    public bool CanSpawnFigures = true;
    public bool CanBuildBuildings = true;
    public bool ClassicCheckmate = false;
    public bool ClassicMovement = false;

    [Header("Gamemode Other Settings")]
    [ConditionalHide("CanSpawnFigures", false)] public bool FiguresCanMoveOnSpawn = false;
    [ConditionalHide("ClassicMovement", true)] public bool OneFigureMovePerTurn = true;
    public bool AlternativeMultiplayerFormula = false;
    public float[] CameraRotationOffsets = { 45, 225, 135, 315 };
    public Vector3[] CameraPositionOffsets = { new Vector3(1, 0, 1), new Vector3(12, 0, 12), new Vector3(1, 0, 12), new Vector3(12, 0, 1) };

    [Header("Movement costs")]
    [ConditionalHide("ClassicMovement", true)] public int MovePointsPerTurn = 5;
    [ConditionalHide("ClassicMovement", true)] public int KingMoveCost = 5;
    [ConditionalHide("ClassicMovement", true)] public int QueenMoveCost = 5;
    [ConditionalHide("ClassicMovement", true)] public int BishopMoveCost = 2;
    [ConditionalHide("ClassicMovement", true)] public int KnightMoveCost = 3;
    [ConditionalHide("ClassicMovement", true)] public int RookMoveCost = 2;
    [ConditionalHide("ClassicMovement", true)] public int PawnMoveCost = 1;

    [Header("Figure costs")]
    [ConditionalHide("CanSpawnFigures", false)] public int QueenSpawnCost = 75;
    [ConditionalHide("CanSpawnFigures", false)] public int BishopSpawnCost = 25;
    [ConditionalHide("CanSpawnFigures", false)] public int KnightSpawnCost = 40;
    [ConditionalHide("CanSpawnFigures", false)] public int RookSpawnCost = 25;
    [ConditionalHide("CanSpawnFigures", false)] public int PawnSpawnCost = 5;
    [ConditionalHide("CanSpawnFigures", false)] public float FigureCostMultiplayer = 1.1f;
    [ConditionalHide("CanSpawnFigures", false)] public int FigureSpawnTurnCost = 2;

    [Header("Building costs")]
    [ConditionalHide("CanBuildBuildings", false)] public int FarmCreationCost = 10;
    [ConditionalHide("CanBuildBuildings", false)] public int BarracksCreationCost = 25;
    [ConditionalHide("CanBuildBuildings", false)] public float BuildingCostMultiplayer = 1.5f;
    [ConditionalHide("CanBuildBuildings", false)] public int BuildingBuildTurnCost = 3;

    [Header("Money system")]
    [ConditionalHide("hidegoldvalue", false)] public int GoldPerTurn = 1;
    [ConditionalHide("hidegoldvalue", false)] public int StartingGold = 10;
    [ConditionalHide("CanBuildBuildings", false)] public int GoldPerFarm = 3;
    [ConditionalHide("CanBuildBuildings", false)] public int GoldPerGoldMine = 5;

    [Header("Board setup")]
    public BoardSettings BoardSetup;

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

    // Trash values
    [ConditionalHide("hidegoldswitch", true)] public bool hidegoldswitch = true;
    [ConditionalHide("hidegoldswitch", true)] public bool hidegoldvalue = true;

}