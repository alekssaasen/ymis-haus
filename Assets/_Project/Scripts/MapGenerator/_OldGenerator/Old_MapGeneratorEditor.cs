using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Old_MapGenerator))]
public class Old_MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Old_MapGenerator mapGen = (Old_MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }

    }
}
