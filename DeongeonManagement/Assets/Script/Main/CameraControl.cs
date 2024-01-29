using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{

    readonly float limit_up = 0;
    readonly float limit_down = -16;

    float limit_left;  //? -15를 카메라 사이즈로 나눈값
    float limit_right; //? 15를 카메라 사이즈로 나눈값

    readonly float limit_size_min = 3;
    readonly float limit_size_max = 7;

    public float cameraSpeed;
    public float Clickmove;


    Camera mainCam;

    public bool Move { get; set; }

    void Start()
    {
        mainCam = GetComponent<Camera>();
        Move = true;

    }


    void Update()
    {
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    return;
        //}

        //CheckCameraMove();

        //if (!Move) return;

        //if (mainCam.ScreenToViewportPoint(Input.mousePosition).y < 0.1f && mainCam.ScreenToViewportPoint(Input.mousePosition).y > 0.0f)
        //{
        //    SlowMove(-1);
        //}

        //if (mainCam.ScreenToViewportPoint(Input.mousePosition).y > 0.9f && mainCam.ScreenToViewportPoint(Input.mousePosition).y < 1.0f)
        //{
        //    SlowMove(1);
        //}
    }

    private void LateUpdate()
    {
        limit_left = -15 / mainCam.orthographicSize;
        limit_right = 15 / mainCam.orthographicSize;


        ClickAction();
        Zooming();
    }



    void Zooming()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * -3;

        if (scroll == 0)
        {
            return;
        }


        mainCam.orthographicSize += scroll;



        if (mainCam.orthographicSize > limit_size_max)
        {
            mainCam.orthographicSize = limit_size_max;
        }
        if (mainCam.orthographicSize < limit_size_min)
        {
            mainCam.orthographicSize = limit_size_min;
        }

        MouseLimit();
    }



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
