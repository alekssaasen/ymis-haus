using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_NameChange : MonoBehaviour
{
    public TMP_Text requirementsForName;
    public GameObject saveButton;

    public void LoadName(TMP_InputField InputField)
    {
        string msg = "";
        InputField.text = FormatString(PlayerPrefs.GetString("PlayerName"), ref msg);
    }

    public void ModifyName(TMP_InputField InputField)
    {
        if (saveButton != null) { saveButton.SetActive(false); }
        if (requirementsForName == null) { return; }

        string msg = "";
        FormatString(InputField.text, ref msg);

        requirementsForName.text = msg;
    }

    public void ConfirmName(TMP_InputField InputField)
    {
        string msg = "";
        InputField.text = FormatString(InputField.text, ref msg);
        if (saveButton != null) { saveButton.SetActive(true); }
        SaveName(InputField.text);
    }

    public void SaveName(string PlayerName)
    {
        string msg = "";
        PlayerPrefs.SetString("PlayerName", FormatString(PlayerName, ref msg));
        PlayerPrefs.Save();
    }

    public static string FormatString(string String, ref string Message)
    {
        Message = "";
        String = String.Replace(" ", "_");
        Message += "Name cant contain any spaces\n";

        if (String.Length > 10)
        {
            String = String.Substring(0, 10);
            Message += "Name cannot be longer than 10 letters\n";
        }

        if (String.Length < 3)
        {
            String = "abc";
            Message += "Name cannot be shorter than 3 letters\n";
        }

        return String;
    }

    public static string GetName()
    {
        string msg = "";
        return FormatString(PlayerPrefs.GetString("PlayerName"), ref msg);
    }
}
