using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_MenuSelection : MonoBehaviour
{
    public GameObject[] menuItems;

    private void Start()
    {
        SelectMenuItem(null);
    }

    public void SelectMenuItem(GameObject MenuItem)
    {
        bool state = true;
        for (int i = 0; i < menuItems.Length; i++)
        {
            if (menuItems[i] == MenuItem)
            {
                menuItems[i].SetActive(true);
                state = false;
            }
            else
            {
                menuItems[i].SetActive(false);
            }
        }

        if (state)
        {
            menuItems[0].SetActive(true);
        }
    }
}
