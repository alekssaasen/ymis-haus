using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager script = (GameManager)target;

        if (GameManager.Main != null)
        {
            GameManager.MapTexture2D = new Texture2D(GameManager.Main.Board.GetLength(0), GameManager.Main.Board.GetLength(1));
            GameManager.MapTexture2D.filterMode = FilterMode.Point;

            for (int x = 0; x < GameManager.MapTexture2D.width; x++)
            {
                for (int y = 0; y < GameManager.MapTexture2D.height; y++)
                {
                    GameManager.MapTexture2D.SetPixel(x, y, GameManager.ChessFigureSetInUse.ColorsByID[GameManager.Main.Board[x, y].ownerID + 1]);
                }
            }
            GameManager.MapTexture2D.Apply();

            EditorGUI.DrawPreviewTexture(new Rect(0, 0, Screen.width / 2, Screen.width / 2), GameManager.MapTexture2D);
            EditorGUILayout.Space(Screen.width / 2);
        }

        DrawDefaultInspector();
    }
}
