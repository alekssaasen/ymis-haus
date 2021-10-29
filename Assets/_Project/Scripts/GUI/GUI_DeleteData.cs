using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_DeleteData : MonoBehaviour
{
    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }
}
