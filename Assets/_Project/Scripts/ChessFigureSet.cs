using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chess Figure Set", menuName = "ScriptableObjects/ChessFigureSet")]
public class ChessFigureSet : ScriptableObject
{
    public Mesh[] Meshes;

    public Material[] PlayerMaterials;
}
