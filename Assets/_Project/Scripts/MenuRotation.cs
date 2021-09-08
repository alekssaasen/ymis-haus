using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRotation : MonoBehaviour
{
    public float speed = 30f;
    float rotation;

    private void Update()
    {
        rotation += Time.deltaTime * speed;
        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }
}
