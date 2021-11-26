using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "InfoBoxData", menuName = "ScriptableObjects/InfoBoxData")]
public class InfoBoxData : ScriptableObject
{
    public string title, description;
    public Sprite imageSprite;
}
