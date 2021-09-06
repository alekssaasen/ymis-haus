using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkedPlayer : NetworkBehaviour
{
    private TMP_Text text;

    public override void OnStartClient()
    {
        text = GameObject.FindGameObjectWithTag("Text").GetComponent<TMP_Text>();

        if (isLocalPlayer && isServer && GameManager.GameInfo.matchCreated == false)
        {
            Debug.Log("Starting match");
            GameManager.GameInfo.hostIsWhite = Random.Range(0, 2) == 1;
            GameManager.GameInfo.ID = Random.Range(1, int.MaxValue);
            Debug.Log("ID: " + GameManager.GameInfo.ID);
        }
        else if (isLocalPlayer && GameManager.GameInfo.matchCreated == true)
        {
            Debug.Log("Joining match");
            Debug.Log("ID: " + GameManager.GameInfo.ID);
        }
    }

    public void Update()
    {
        if (isLocalPlayer)
        {
            text.text = GameManager.GameInfo.ID.ToString();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isServer)
                {
                    GameManager.GameInfo.ID = Random.Range(1, int.MaxValue);
                }
                else
                {
                    SetID(Random.Range(1, int.MaxValue));
                }
            } 
        }
    }

    [Command]
    public void SetID(int NewID)
    {
        GameManager.GameInfo.ID = NewID;
    }
}