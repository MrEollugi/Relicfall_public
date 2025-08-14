using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightDarkMaskFollow : MonoBehaviour
{
    public Transform targetCamera;
    public Vector3 offset = new Vector3(0, -1f, 0); // ���� ��¦ ����

    void LateUpdate()
    {
        if (targetCamera != null)
            transform.position = targetCamera.position + offset;
    }
}
