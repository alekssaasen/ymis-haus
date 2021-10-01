using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chess Figure Set", menuName = "ScriptableObjects/ChessFigureSet")]
public class ChessFigureSet : ScriptableObject
{
    public Mesh KingMesh, QueenMesh, BishopMesh, KnightMesh, RookMesh, PawnMesh, FarmMesh, MineMesh, BarracksMesh;

    public Material[] MaterialsByID;

    public Color[] ColorsByID;

    public Mesh GetMesh(ChessFigure Figure, ChessBuiding Building)
    {
        switch (Figure)
        {
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
        }
        switch (Building)
        {
            case ChessBuiding.Farm:
                return FarmMesh;

            case ChessBuiding.Mine:
                return MineMesh;

            case ChessBuiding.Barracks:
                return BarracksMesh;

            default:
                return null;
        }
    }
}