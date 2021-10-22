using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Tutorial : MonoBehaviour
{
    public int index = 0;
    public Sprite[] tutorialImages;
    public Image displayImage;
    public GameObject tutorialParent;

    private void Update()
    {
        displayImage.sprite = tutorialImages[index];
    }

    public void NextFrame()
    {
        index += 1;
        if (index >= tutorialImages.Length)
        {
            index = tutorialImages.Length - 1;
        }
    }

    public void PreviousFrame()
    {
        index -= 1;
        if (index < 0)
        {
            index = 0;
        }
    }

    public void Exit()
    {
        tutorialParent.SetActive(false);

    }
}
