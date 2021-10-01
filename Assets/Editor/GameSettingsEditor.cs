using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameSettings))]
public class GameSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameSettings script = (GameSettings)target;

        script.hidegoldswitch = true;
        script.hidegoldvalue = !(!script.CanBuildBuildings && !script.CanSpawnFigures);

        DrawDefaultInspector();
    }
}
