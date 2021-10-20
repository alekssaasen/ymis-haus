using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureAnimation : MonoBehaviour
{
    private Vector3 newTarget;
    private Vector3 oldTarget;

    private float timeOffset;
    private float distance;

    float lerpvalue;
    Vector3 lerppos;

    public void Awake()
    {
        oldTarget = transform.position;
        newTarget = transform.position;
    }

    public void MoveFigure(Vector2Int NewPosition)
    {
        oldTarget = newTarget;
        newTarget = new Vector3(NewPosition.x, 0, NewPosition.y);

        distance = Vector3.Distance(oldTarget, newTarget);
        timeOffset = Time.time;
    }

    private void Update()
    {
        lerpvalue = CustomFunctions.Remap(Time.time - timeOffset, 0f, distance * 0.25f, 0f, 1f);
        lerppos = Vector3.Lerp(oldTarget, newTarget, lerpvalue);
        transform.position = lerppos;
    }
}
