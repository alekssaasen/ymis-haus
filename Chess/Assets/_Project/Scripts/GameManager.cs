using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager GameInfo;

    [SyncVar] public bool matchCreated;
    [SyncVar] public bool hostIsWhite;
    [SyncVar] public bool whitesTurn;

    [SyncVar] public int ID;

    private void Awake()
    {
        if (GameInfo == null)
        {
            GameInfo = this;
        }
        else
        {
            Destroy(this);
            Debug.LogWarning("There can only be one GameManager!");
        }
    }
}
