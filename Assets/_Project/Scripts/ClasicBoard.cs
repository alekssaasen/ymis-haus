using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClasicBoard : MonoBehaviour
{
    void OnEnable()
    {
        transform.localScale = new Vector3(GameManager.GameSettingsInUse.BoardSetup.MapSize.x,
            (GameManager.GameSettingsInUse.BoardSetup.MapSize.x + GameManager.GameSettingsInUse.BoardSetup.MapSize.y) / 2f,
            GameManager.GameSettingsInUse.BoardSetup.MapSize.y);
        transform.position = transform.localScale * 0.5f;
        transform.position = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
        transform.position -= new Vector3(0.5f, 0, 0.5f);

        gameObject.GetComponent<MeshRenderer>().materials[1].mainTextureScale = (Vector2)GameManager.GameSettingsInUse.BoardSetup.MapSize * 0.5f;
    }
}
