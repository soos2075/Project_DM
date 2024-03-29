using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class CameraControl : MonoBehaviour
{

    float limit_up = 0;
    float limit_down = -17;

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
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != SceneName._2_Management.ToString())
        {
            return;
        }

        if (Time.timeScale == 0) return;



        limit_left = -150 / (mainCam.orthographicSize * mainCam.orthographicSize);
        limit_right = 150 / (mainCam.orthographicSize * mainCam.orthographicSize);


        ClickAction();
        PixelPerfection_Zoom();
        KeyboardMove();
    }


    public void LimitRefresh()
    {
        limit_down = Main.Instance.Floor[Main.Instance.ActiveFloor_Basement - 1].transform.position.y - 5;
    }



    #region InputAction

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


    Vector3 startMousePos;
    Vector3 startCameraPos;
    float dis_x;
    float dis_y;
    void ClickAction()
    {
        //Debug.Log(startMousePos);
        if (Input.GetMouseButtonDown(0))
        {
            if (currentCor != null)
            {
                StopCoroutine(currentCor);
                currentCor = null;
            }

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


    void KeyboardMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0)
        {
            transform.position += Vector3.right * moveX * Time.deltaTime * 5;
            MouseLimit();
        }

        if (moveY != 0)
        {
            transform.position += Vector3.up * moveY * Time.deltaTime * 5;
            MouseLimit();
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

    #endregion


    #region Direction

    Coroutine currentCor;

    public void ChasingTarget(Transform target, float duration)
    {
        if (currentCor != null)
        {
            StopCoroutine(currentCor);
        }
        currentCor = StartCoroutine(ChasingLerp(target, duration));
    }

    IEnumerator ChasingLerp(Transform target, float duration)
    {
        var targetPos = target.position;
        //var direction = targetPos - transform.position;
        var startPos = transform.position;

        float timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / duration);

            var movePos2 = Vector3.Lerp(startPos, targetPos, Smoothstep(t));
            transform.position = new Vector3(movePos2.x, movePos2.y, transform.position.z);


            //if (Smoothstep(t) >= 1)
            //{
            //    Debug.Log(timer);
            //}
        }
        transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        currentCor = null;
    }

    float Smoothstep(float t)
    {
        return 0.4f * Mathf.Log(t) + 1;
    }

    #endregion
}
