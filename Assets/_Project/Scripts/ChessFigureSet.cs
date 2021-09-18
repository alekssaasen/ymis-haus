using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chess Figure Set", menuName = "ScriptableObjects/ChessFigureSet")]
public class ChessFigureSet : ScriptableObject
{
    public Mesh KingMesh, QueenMesh, BishopMesh, KnightMesh, RookMesh, PawnMesh, FarmMesh, MineMesh, BarracksMesh;

    public Material[] MaterialsByID;

    public Mesh GetMesh(ChessFigure Figure)
    {
        switch (Figure)
        {
            case ChessFigure.Empty:
                return null;

            case ChessFigure.King:
                return KingMesh;

            case ChessFigure.Queen:
                return QueenMesh;

            case ChessFigure.Bishop:
                return BishopMesh;

            case ChessFigure.Knight:
                return KnightMesh;

            case ChessFigure.Rook:
                return RookMesh;

            case ChessFigure.Pawn:
                return PawnMesh;

            case ChessFigure.Farm:
                return FarmMesh;

            case ChessFigure.Mine:
                return MineMesh;

            case ChessFigure.Barracks:
                return BarracksMesh;

            default:
                return null;
        }
    }
}