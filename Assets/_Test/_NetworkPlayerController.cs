using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class _NetworkPlayerController : MonoBehaviour
{
    public TMP_Text text;
    public int ID = 0;

    private void Awake()
    {
        //PhotonView.Get(this).RPC("SyncValue", RpcTarget.AllBuffered, 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PhotonView.Get(this).RPC("UpdateValue", RpcTarget.AllBuffered, 1);
        }
    }

    [PunRPC]
    private void UpdateValue(int Change)
    {
        ID += Change;
        text.text = "ID: " + ID.ToString();
    }
}
