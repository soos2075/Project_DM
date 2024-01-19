using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    public readonly float limet_up = 0;
    public readonly float limit_down = -10;

    public float cameraSpeed;

    public bool Move { get; set; }

    void Start()
    {
        Move = true;
    }


    void Update()
    {
        CheckCameraMove();

        if (!Move) return;

        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).y < 0.1f && Camera.main.ScreenToViewportPoint(Input.mousePosition).y > 0.0f)
        {
            SlowMove(-1);
        }

        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).y > 0.9f && Camera.main.ScreenToViewportPoint(Input.mousePosition).y < 1.0f)
        {
            SlowMove(1);
        }
    }


    void SlowMove(float dir)
    {
        transform.Translate(new Vector3(0, dir, 0) * Time.deltaTime * cameraSpeed);

        if (transform.position.y < limit_down)
        {
            transform.position = new Vector3(transform.position.x, limit_down, transform.position.z);
        }

        if (transform.position.y > limet_up)
        {
            transform.position = new Vector3(transform.position.x, limet_up, transform.position.z);
        }
    }


    void CheckCameraMove()
    {
        if (Managers.UI._popupStack.Count == 0)
        {
            Move = true;
            return;
        }

        var ui = Managers.UI._popupStack.Peek() as UI_Interface.IWorldSpace;
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
