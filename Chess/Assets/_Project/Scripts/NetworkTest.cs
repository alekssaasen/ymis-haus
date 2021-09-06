using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkTest : NetworkBehaviour
{
    private TMP_Text text;

    public override void OnStartClient()
    {
        text = GameObject.FindGameObjectWithTag("Text").GetComponent<TMP_Text>();

        if (isLocalPlayer && isServer && GameManager.Main.gameInfo.matchCreated == false)
        {
            Debug.Log("Starting match");
            GameManager.Main.gameInfo = new GameInfo(Random.Range(0, 2) == 1, Random.Range(1, int.MaxValue));
            Debug.Log("ID: " + GameManager.Main.gameInfo.ID);
        }
        else if (isLocalPlayer && GameManager.Main.gameInfo.matchCreated == true)
        {
            Debug.Log("Joining match");
            Debug.Log("ID: " + GameManager.Main.gameInfo.ID);
        }
    }

    public void Update()
    {
        if (isLocalPlayer)
        {
            text.text = GameManager.Main.gameInfo.ID.ToString();
        }
    }
}