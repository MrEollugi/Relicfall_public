using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSync : MonoBehaviour
{
    public Camera fovCamera;
    public Camera mapCamera;

    void LateUpdate()
    {
        if (fovCamera != null && mapCamera != null)
        {
            mapCamera.transform.position = fovCamera.transform.position;
            mapCamera.transform.rotation = fovCamera.transform.rotation;
            // Orthographic 사이즈도 일치시키세요!
            // mapCamera.orthographicSize = fovCamera.orthographicSize;
        }
    }
}
