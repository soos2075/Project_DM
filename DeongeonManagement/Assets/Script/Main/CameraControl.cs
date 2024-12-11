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

    //public float KeyboardMoveSpeed;
    public float CameraSpeed;


    Camera mainCam;
    public PixelPerfectCamera pixelCam;
    public bool Move { get; set; }


    UI_Management UI_Main;
    UI_ClearPanel UI_Clear;

    void Start()
    {
        UI_Main = FindAnyObjectByType<UI_Management>();
        UI_Clear = FindAnyObjectByType<UI_ClearPanel>();

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

        if (Time.timeScale == 0 && FindObjectOfType<UI_Stop>() == null) return;

        limit_left = -1500 / (mainCam.orthographicSize * mainCam.orthographicSize);
        limit_right = 1500 / (mainCam.orthographicSize * mainCam.orthographicSize);


        PPU_Zoom_Keyboard();
        KeyboardMove();
        Keyboard_Shortcut();

        // PointerEventData를 생성하고 현재 마우스 위치를 설정
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            //Debug.Log("현재 마우스가 올라가 있는 UI: " + results[0].gameObject.name);
            //? 팝업 ui랑 메인 ui 위에선 작동안하도록 변경
            if (results[0].gameObject.GetComponentInParent<UI_Management>(true)) return;
            if (results[0].gameObject.GetComponentInParent<UI_PopUp>(true))
            {
                if (results[0].gameObject.GetComponentInParent<UI_DungeonPlacement>(true) == null)
                {
                    return;
                }
            }
        }


        ClickAction();
        PixelPerfection_Zoom();

        //// 팝업 ui 있을땐 더블클릭 안되게
        //if (Managers.UI._popupStack.Count > 0) return;
        //if (EventSystem.current.currentSelectedGameObject) // 선택된 ui가 있을경우 리턴 - 이경우엔 버튼같은거 클릭을 의미함
        //{
        //    //Debug.Log(EventSystem.current.currentSelectedGameObject);
        //    //Debug.Log(EventSystem.current.currentSelectedGameObject.name + "@@name");
        //    return;
        //}

        // 더블클릭
        FirstClickEvent();
    }


    public void LimitRefresh()
    {
        limit_down = Main.Instance.Floor[Main.Instance.ActiveFloor_Basement - 1].transform.position.y - 10;
    }

    public void View_Target(Vector3 targetPos)
    {
        AutoChasing = false;

        transform.position = new Vector3(targetPos.x, targetPos.y, -10);
        MouseLimit();
        ResetMousePos();
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

        transform.position = new Vector3(pos.x, pos.y, -10);

        if (pixelCam.assetsPPU < 16)
        {
            pixelCam.assetsPPU = 26;
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
        }
    }

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
        if (pixelCam.assetsPPU > 26)
        {
            pixelCam.assetsPPU = 26;
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
            AutoChasing = false;
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
            transform.position = new Vector3(startCameraPos.x + dis_x * CameraSpeed, startCameraPos.y + dis_y * CameraSpeed, -10);
            MouseLimit();
        }
    }


    void MouseLimit()
    {
        if (transform.position.y < limit_down)
        {
            transform.position = new Vector3(transform.position.x, limit_down, -10);
        }

        if (transform.position.y > limit_up)
        {
            transform.position = new Vector3(transform.position.x, limit_up, -10);
        }

        if (transform.position.x < limit_left)
        {
            transform.position = new Vector3(limit_left, transform.position.y, -10);
        }

        if (transform.position.x > limit_right)
        {
            transform.position = new Vector3(limit_right, transform.position.y, -10);
        }
    }


    void KeyboardMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0)
        {
            AutoChasing = false;
            transform.position += Vector3.right * moveX * Time.unscaledDeltaTime * CameraSpeed;
            MouseLimit();
        }

        if (moveY != 0)
        {
            AutoChasing = false;
            transform.position += Vector3.up * moveY * Time.unscaledDeltaTime * CameraSpeed;
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


    void Keyboard_Shortcut()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            View_Target(Main.Instance.Player.transform.position);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            View_Target(Main.Instance.Floor[1].transform.position);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            View_Target(Main.Instance.Floor[2].transform.position);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            View_Target(Main.Instance.Floor[3].transform.position);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && Main.Instance.ActiveFloor_Basement > 4)
        {
            View_Target(Main.Instance.Floor[4].transform.position);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && Main.Instance.ActiveFloor_Basement > 5)
        {
            View_Target(Main.Instance.Floor[5].transform.position);
        }

    }



    #endregion


    #region Direction

    public Coroutine Cor_CameraChasing;


    public void ChasingTarget(Vector3 target, float duration)
    {
        if (Cor_CameraChasing != null)
        {
            StopCoroutine(Cor_CameraChasing);
        }
        if (Chasing_Auto != null)
        {
            StopCoroutine(Chasing_Auto);
            Chasing_Auto = null;
        }
        Cor_CameraChasing = StartCoroutine(ChasingLerp(target, duration));
    }
    public void ChasingTarget(Transform target, float duration)
    {
        if (Cor_CameraChasing != null)
        {
            StopCoroutine(Cor_CameraChasing);
        }
        if (Chasing_Auto != null)
        {
            StopCoroutine(Chasing_Auto);
            Chasing_Auto = null;
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
            transform.position = new Vector3(movePos2.x, movePos2.y, -10);


            //if (Smoothstep(t) >= 1)
            //{
            //    Debug.Log(timer);
            //}
        }
        transform.position = new Vector3(targetPos.x, targetPos.y, -10);

        ResetMousePos();

        Cor_CameraChasing = null;
    }

    float Smoothstep(float t)
    {
        return 0.4f * Mathf.Log(t) + 1;
    }



    public bool AutoChasing { get; set; }
    Coroutine Chasing_Auto;

    public void ChasingTarget_Continue(Transform target)
    {
        if (Chasing_Auto != null)
        {
            StopCoroutine(Chasing_Auto);
        }

        Chasing_Auto = StartCoroutine(Chasing_Continue(target));
        AutoChasing = true;
    }

    IEnumerator Chasing_Continue(Transform target)
    {
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None && AutoChasing);

        if (Cor_CameraChasing != null)
        {
            StopCoroutine(Cor_CameraChasing);
            Cor_CameraChasing = null;
        }

        while (AutoChasing && target != null && target.GetComponentInChildren<SpriteRenderer>(true).enabled)
        {
            yield return null;
            yield return UserData.Instance.Wait_GamePlay;

            float t = 1 - Mathf.Exp(-Time.deltaTime * 2);

            var movePos2 = Vector3.Lerp(transform.position, target.position, t);
            transform.position = new Vector3(movePos2.x, movePos2.y, -10);
        }


        if (AutoChasing)
        {
            if (GameManager.NPC.Instance_EventNPC_List.Count > 0)
            {
                foreach (var item in GameManager.NPC.Instance_EventNPC_List)
                {
                    if (item.GetComponentInChildren<SpriteRenderer>(true).enabled)
                    {
                        Chasing_Auto = StartCoroutine(Chasing_Continue(item.transform));
                        yield break;
                    }
                }
            }

            if (GameManager.NPC.Instance_NPC_List.Count > 0)
            {
                foreach (var item in GameManager.NPC.Instance_NPC_List)
                {
                    if (item.GetComponentInChildren<SpriteRenderer>(true).enabled)
                    {
                        Chasing_Auto = StartCoroutine(Chasing_Continue(item.transform));
                        yield break;
                    }
                }
            }
        }
        else
        {
            //AutoChasing = false;
            Chasing_Auto = null;
        }
    }


    #endregion

    Vector3 CurrentCamPos;
    int CurrentPPU;

    public void SaveCurrentState()
    {
        CurrentCamPos = transform.position;
        CurrentPPU = pixelCam.assetsPPU;
    }
    public void SetOriginState()
    {
        transform.position = CurrentCamPos;
        pixelCam.assetsPPU = CurrentPPU;
    }

}
