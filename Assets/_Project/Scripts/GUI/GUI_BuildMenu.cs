using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUI_BuildMenu : MonoBehaviour
{
    public GUI_BuildMenuTile[] buildMenuTileInfo;
    public GameObject buildMenuTilePrefab;
    public RectTransform contentReferance;

    public bool open;

    private void FixedUpdate()
    {
        int index = 0;

        List<GUI_BuildMenuTile> sorted = new List<GUI_BuildMenuTile>();

        for (int i = 0; i < buildMenuTileInfo.Length; i++)
        {
            if (buildMenuTileInfo[i].canBuy)
            {
                sorted.Insert(index, buildMenuTileInfo[i]);
                index++;
            }
            else
            {
                sorted.Add(buildMenuTileInfo[i]);
            }

            contentReferance.sizeDelta = new Vector2((i + 1) * 110 + 10, 120);
        }

        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(60 + (i * 110), 0);
        }

        if (open)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 65);
        }
        else
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -56);
        }
    }

    public void UpdateBool()
    {
        open = !open;
    }
}