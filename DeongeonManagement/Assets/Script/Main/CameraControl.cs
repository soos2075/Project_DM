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


    UI_Management UI_Main;

    void Start()
    {
        UI_Main = FindAnyObjectByType<UI_Management>();

        mainCam = GetComponent<Camera>();
        pixelCam = GetComponent<PixelPerfectCamera>();
        Move = true;
        doubleDelay = 0.3f;
    }


    private void LateUpdate()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != SceneName._2_Management.ToString())
        {
            return;
        }

        if (UserData.Instance.GameMode == Define.GameMode.Stop) return;

        if (Cor_CameraChasing != null) return;

        //? esc키로 액션취소, 팝업 닫기 등을 할 수 있어야함(ui가 있을 땐 타임스케일이 0이니까 어쩔 수 없긴 한데 일단은)
        //? 그리고 그거 구현하고나면 타임스케일 리턴 위로 위치 옮겨야함. 일단은 pause만 구현
        Key_Esc();



        if (Time.timeScale == 0) return;







        limit_left = -150 / (mainCam.orthographicSize * mainCam.orthographicSize);
        limit_right = 150 / (mainCam.orthographicSize * mainCam.orthographicSize);




        ClickAction();
        PixelPerfection_Zoom();
        PPU_Zoom_Keyboard();
        KeyboardMove();


        // 팝업 ui 있을땐 더블클릭 안되게
        if (Managers.UI._popupStack.Count > 0) return;
        if (EventSystem.current.currentSelectedGameObject) // 선택된 ui가 있을경우 리턴 - 이경우엔 버튼같은거 클릭을 의미함
        {
            //Debug.Log(EventSystem.current.currentSelectedGameObject);
            //Debug.Log(EventSystem.current.currentSelectedGameObject.name + "@@name");
            return;
        }

        // 더블클릭
        FirstClickEvent();
    }


    public void LimitRefresh()
    {
        limit_down = Main.Instance.Floor[Main.Instance.ActiveFloor_Basement - 1].transform.position.y - 5;
    }


    #region DoubleClick
    // 짧은 딜레이 시간 / 일정범위 안에 2개의 클릭이 들어오는 경우를 더블클릭으로 인식(근데 범위는 솔직히 별 의미없는게 그렇게 멀리가기가 쉽지가 않음. 소모값만 더들고)
    float doubleDelay = 0.3f;
    Coroutine firstClick;

    void FirstClickEvent()
    {
        if (firstClick != null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            firstClick = StartCoroutine(SecondClickCheck(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
    }

    IEnumerator SecondClickCheck(Vector3 firstPos)
    {
        float count = 0;
        while (count < doubleDelay)
        {
            yield return null;
            count += Time.unscaledDeltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                var secondPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if ((firstPos - secondPos).magnitude < 0.3f)
                {
                    DoubleClickEvent(secondPos);
                }
                break;
            }
        }

        yield return new WaitForEndOfFrame();
        firstClick = null;
    }

    void DoubleClickEvent(Vector3 pos)
    {
        Debug.Log("더블클릭 이벤트");

        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        if (pixelCam.assetsPPU < 16)
        {
            pixelCam.assetsPPU = 25;
        }
        else if (pixelCam.assetsPPU >= 16)
        {
            pixelCam.assetsPPU = 10;
        }
        MouseLimit();
    }
    #endregion


    #region InputAction

    void PPU_Zoom_Keyboard()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Zoom(1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Zoom(-1);
        }
    }
    //void PPU_Zoom_Keyboard()
    //{
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        if (Hide_UI_Cor != null)
    //        {
    //            Zoom(1);
    //            timer = 0;
    //        }
    //        else
    //        {
    //            Hide_UI_Cor = StartCoroutine(Zoom_UI_Pixel(1));
    //        }
    //    }

    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        if (Hide_UI_Cor != null)
    //        {
    //            Zoom(-1);
    //            timer = 0;
    //        }
    //        else
    //        {
    //            Hide_UI_Cor = StartCoroutine(Zoom_UI_Pixel(-1));
    //        }
    //    }
    //}


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

        if (Mathf.Abs(scrollValue) > 0)
        {
            Zoom();

            //if (Hide_UI_Cor != null)
            //{
            //    Zoom();
            //    timer = 0;
            //}
            //else
            //{
            //    Hide_UI_Cor = StartCoroutine(Zoom_UI_Pixel());
            //}
        }
    }

    //Coroutine Hide_UI_Cor;
    //float timer = 0;
    //IEnumerator Zoom_UI_Pixel(int value = 0)
    //{
    //    //var mainCanvas = UI_Main.GetComponent<Canvas>();
    //    ////Managers.UI.HideCanvasAll();
    //    //mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

    //    yield return null;

    //    Zoom(value);

    //    yield return null;

    //    timer = 0;
    //    while (timer < 0.5f)
    //    {
    //        yield return null;
    //        timer += Time.unscaledDeltaTime;
    //    }

    //    //Managers.UI.ShowCanvasAll();
    //    //mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;

    //    Hide_UI_Cor = null;
    //}


    void Zoom(int value = 0)
    {
        if (value == 0)
        {
            pixelCam.assetsPPU += (int)scrollValue;
            scrollValue = 0;
        }
        else
        {
            pixelCam.assetsPPU += value;
        }

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




    Vector3 startMousePos;
    Vector3 startCameraPos;
    float dis_x;
    float dis_y;

    void ResetMousePos()
    {
        startMousePos = mainCam.ScreenToViewportPoint(Input.mousePosition);
        startCameraPos = transform.position;
        dis_y = 0;
        dis_x = 0;
    }
    void ClickAction()
    {
        //Debug.Log(startMousePos);
        if (Input.GetMouseButtonDown(0))
        {
            if (Cor_CameraChasing != null)
            {
                StopCoroutine(Cor_CameraChasing);
                Cor_CameraChasing = null;
            }

            startMousePos = mainCam.ScreenToViewportPoint(Input.mousePosition);
            startCameraPos = transform.position;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //startMousePos = Vector3.zero;
            //dis_y = 0;
            //dis_x = 0;
            ResetMousePos();
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
            transform.position += Vector3.right * moveX * Time.unscaledDeltaTime * 5;
            MouseLimit();
        }

        if (moveY != 0)
        {
            transform.position += Vector3.up * moveY * Time.unscaledDeltaTime * 5;
            MouseLimit();
        }
    }


    void Key_Esc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Managers.UI._popupStack.Count == 0)
            {
                Managers.UI.ShowPopUp<UI_Pause>();
            }
            else if (Managers.UI._popupStack.Peek().EscapeKeyAction())
            {
                Managers.UI.ClosePopUp();
            }
        }
    }


    #endregion


    #region Direction

    Coroutine Cor_CameraChasing;


    public void ChasingTarget(Vector3 target, float duration)
    {
        if (Cor_CameraChasing != null)
        {
            StopCoroutine(Cor_CameraChasing);
        }
        Cor_CameraChasing = StartCoroutine(ChasingLerp(target, duration));
    }
    public void ChasingTarget(Transform target, float duration)
    {
        if (Cor_CameraChasing != null)
        {
            StopCoroutine(Cor_CameraChasing);
        }
        Cor_CameraChasing = StartCoroutine(ChasingLerp(target.position, duration));
    }

    IEnumerator ChasingLerp(Vector3 targetPos, float duration)
    {
        pixelCam.assetsPPU = 20;

        //var targetPos = target.position;
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

        ResetMousePos();

        Cor_CameraChasing = null;
    }

    float Smoothstep(float t)
    {
        return 0.4f * Mathf.Log(t) + 1;
    }

    #endregion
}
