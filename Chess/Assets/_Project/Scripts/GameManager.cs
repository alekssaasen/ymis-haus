using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Main;
    [SyncVar] [SerializeField] public GameInfo gameInfo = null;

    private void Awake()
    {
        if (Main == null)
        {
            Main = this;
        }
        else
        {
            Destroy(this);
        }
    }
}

[System.Serializable]
public class GameInfo
{
    public bool matchCreated;
    public bool hostIsWhite;
    public bool whitesTurn;

    public int ID;

    public GameInfo()
    {
        matchCreated = false;
        hostIsWhite = true;
        whitesTurn = true;
    }
    public GameInfo(bool HostIsWhite, int NewID)
    {
        matchCreated = true;
        hostIsWhite = HostIsWhite;
        whitesTurn = true;

        ID = NewID;
    }
}
