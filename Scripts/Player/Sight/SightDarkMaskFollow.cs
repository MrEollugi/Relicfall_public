using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightDarkMaskFollow : MonoBehaviour
{
    public Transform targetCamera;
    public Vector3 offset = new Vector3(0, -1f, 0); // À§¿¡ »ìÂ¦ ¶ç¿ì±â

    void LateUpdate()
    {
        if (targetCamera != null)
            transform.position = targetCamera.position + offset;
    }
}
