using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class GUI_Deselect : MonoBehaviour, IDeselectHandler //This Interface is required to receive OnDeselect callbacks.
{
    public void OnDeselect(BaseEventData data)
    {
        Debug.Log("Deselected");
    }
}