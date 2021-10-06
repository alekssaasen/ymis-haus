using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_MainMessage : MonoBehaviour
{
    public static GUI_MainMessage Main;
    public Animator messageAnimator;
    public TMP_Text messageText;

    void Awake()
    {
        if (Main == null)
        {
            Main = this;
        }
        else
        {
            Main = this;
            Debug.LogWarning("There can only be one GUI_MainMessage!");
        }
    }

    public static void SendNewMessage(string NewMessage)
    {
        Main.messageAnimator.Play("Message", -1, 0f);
        Main.messageText.text = NewMessage;

    }
}
