using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class GUI_Select : MonoBehaviour, ISelectHandler //This Interface is required to receive OnDeselect callbacks.
{
    public void OnSelect(BaseEventData data)
    {
        Debug.Log("Selected");
    }
}