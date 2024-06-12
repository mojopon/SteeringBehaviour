using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    void Update()
    {
        var pointerPosition = (Vector3)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pointerPosition.z = 0;
        transform.position = pointerPosition;
    }
}
