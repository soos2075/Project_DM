using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class CameraControl : MonoBehaviour
{

    readonly float limit_up = 0;
    readonly float limit_down = -16;

    float limit_left;
    float limit_right;

    public float cameraSpeed;
    public float Clickmove;


    Camera mainCam;
    PixelPerfectCamera pixelCam;

    public bool Move { get; set; }

    void Start()
    {
        mainCam = GetComponent<Camera>();
        pixelCam = GetComponent<PixelPerfectCamera>();
        Move = true;
        
    }


    private void LateUpdate()
    {
        if (Time.timeScale == 0) return;

        limit_left = -81 / (mainCam.orthographicSize * mainCam.orthographicSize);
        limit_right = 81 / (mainCam.orthographicSize * mainCam.orthographicSize);


        ClickAction();
        PixelPerfection_Zoom();
    }


    float scrollValue;
    void PixelPerfection_Zoom()
    {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");

        if (scroll == 0)
        {
            return;
        }
        else
        {
            scrollValue += scroll * 10;
        }

        if (Mathf.Abs(scrollValue) > 1)
        {
            pixelCam.assetsPPU += (int)scrollValue;
            scrollValue = 0;

            if (pixelCam.assetsPPU < 10)
            {
                pixelCam.assetsPPU = 10;
            }
            if (pixelCam.assetsPPU > 25)
            {
                pixelCam.assetsPPU = 25;
            }
            MouseLimit();
        }
    }





    //void Zooming()
    //{
    //    float scroll = Input.GetAxis("Mouse ScrollWheel") * -3;

    //    if (scroll == 0)
    //    {
    //        return;
    //    }
    //    mainCam.orthographicSize += scroll;

    //    if (mainCam.orthographicSize > limit_size_max)
    //    {
    //        mainCam.orthographicSize = limit_size_max;
    //    }
    //    if (mainCam.orthographicSize < limit_size_min)
    //    {
    //        mainCam.orthographicSize = limit_size_min;
    //    }

    //    MouseLimit();
    //}



    Vector3 startMousePos;
    Vector3 startCameraPos;
    float dis_x;
    float dis_y;
    void ClickAction()
    {
        //Debug.Log(startMousePos);
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = mainCam.ScreenToViewportPoint(Input.mousePosition);
            startCameraPos = transform.position;
        }

        if (Input.GetMouseButtonUp(0))
        {
            startMousePos = Vector3.zero;
            dis_y = 0;
            dis_x = 0;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            dis_x = startMousePos.x - mainCam.ScreenToViewportPoint(Input.mousePosition).x;
            dis_y = startMousePos.y - mainCam.ScreenToViewportPoint(Input.mousePosition).y;
            transform.position = new Vector3(startCameraPos.x + dis_x * Clickmove, startCameraPos.y + dis_y * Clickmove, startCameraPos.z);
            MouseLimit();
        }
    }



    void MouseLimit()
    {
        if (transform.position.y < limit_down)
        {
            transform.position = new Vector3(transform.position.x, limit_down, transform.position.z);
        }

        if (transform.position.y > limit_up)
        {
            transform.position = new Vector3(transform.position.x, limit_up, transform.position.z);
        }

        if (transform.position.x < limit_left)
        {
            transform.position = new Vector3(limit_left, transform.position.y, transform.position.z);
        }

        if (transform.position.x > limit_right)
        {
            transform.position = new Vector3(limit_right, transform.position.y, transform.position.z);
        }
    }

    [System.Obsolete]
    void SlowMove(float dir)
    {
        transform.Translate(new Vector3(0, dir, 0) * Time.deltaTime * cameraSpeed);
        MouseLimit();
    }
    [System.Obsolete]
    void CheckCameraMove()
    {
        if (Managers.UI._popupStack.Count == 0)
        {
            Move = true;
            return;
        }

        var ui = Managers.UI._popupStack.Peek() as IWorldSpaceUI;
        if (ui == null)
        {
            Move = false;
        }
        else
        {
            Move = true;
        }
    }
}
