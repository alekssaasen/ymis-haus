using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MeshGenerator mapGen = (MeshGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMesh(0);
        }
    }
}
