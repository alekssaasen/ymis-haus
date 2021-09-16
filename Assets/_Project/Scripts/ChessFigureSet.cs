using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chess Figure Set", menuName = "ScriptableObjects/ChessFigureSet")]
public class ChessFigureSet : ScriptableObject
{
    public Mesh[] PlayerMeshes;

    public Mesh[] BuildingMeshes;

    public Material[] PlayerMaterials;

    public Material[] BuildingMaterials;

}
