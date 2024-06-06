using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_SubCam : MonoBehaviour
{


    Camera subCam;
    void Start()
    {
        subCam = GetComponent<Camera>();
    }


    private void LateUpdate()
    {
        subCam.orthographicSize = Camera.main.orthographicSize;
        transform.position = Camera.main.transform.position;
    }

}
