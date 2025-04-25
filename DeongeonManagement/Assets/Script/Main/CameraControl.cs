using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class CameraControl : MonoBehaviour
{

    float limit_up = 6;
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
        shake = GetComponent<CameraShake>();
    }



    private void LateUpdate()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != SceneName._2_Management.ToString())
        {
            return;
        }

        if (UserData.Instance.GameMode == Define.GameMode.Stop) return;

        if (Cor_CameraChasing != null) return;

        //? escŰ�� �׼����, �˾� �ݱ� ���� �� �� �־����(ui�� ���� �� Ÿ�ӽ������� 0�̴ϱ� ��¿ �� ���� �ѵ� �ϴ���)
        //? �׸��� �װ� �����ϰ��� Ÿ�ӽ����� ���� ���� ��ġ �Űܾ���. �ϴ��� pause�� ����
        Key_Esc();

        if (Time.timeScale == 0 && FindObjectOfType<UI_Stop>() == null) return;

        limit_left = -1200 / (mainCam.orthographicSize * mainCam.orthographicSize);
        limit_right = 1200 / (mainCam.orthographicSize * mainCam.orthographicSize);


        PPU_Zoom_Keyboard();
        KeyboardMove();
        Keyboard_Shortcut();
        Key_Tapkey();
        MenuHotKey_Management();

        // PointerEventData�� �����ϰ� ���� ���콺 ��ġ�� ����
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            //Debug.Log("���� ���콺�� �ö� �ִ� UI: " + results[0].gameObject.name);
            //? �˾� ui�� ���� ui ������ �۵����ϵ��� ����
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

        //// �˾� ui ������ ����Ŭ�� �ȵǰ�
        //if (Managers.UI._popupStack.Count > 0) return;
        //if (EventSystem.current.currentSelectedGameObject) // ���õ� ui�� ������� ���� - �̰�쿣 ��ư������ Ŭ���� �ǹ���
        //{
        //    //Debug.Log(EventSystem.current.currentSelectedGameObject);
        //    //Debug.Log(EventSystem.current.currentSelectedGameObject.name + "@@name");
        //    return;
        //}

        // ����Ŭ��
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
    // ª�� ������ �ð� / �������� �ȿ� 2���� Ŭ���� ������ ��츦 ����Ŭ������ �ν�(�ٵ� ������ ������ �� �ǹ̾��°� �׷��� �ָ����Ⱑ ������ ����. �Ҹ𰪸� �����)
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
        Debug.Log("����Ŭ�� �̺�Ʈ");

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
        //Debug.Log("Ŭ���׼� �޴���");
        //? ��ġ����
        if (Input.GetMouseButtonUp(1))
        {
            if (Managers.UI._paused != null)
            {
                SoundManager.Instance.ReplaceSound("RightClick");
                Main.Instance.ResetCurrentAction();
            }
        }


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

        if (target.GetComponent<Player>())
        {
            SpriteRenderer renderer = target.GetComponentInChildren<SpriteRenderer>();
            if (!renderer.enabled) renderer.enabled = true;
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


    public void AutoChasing_First(Transform target)
    {
        if (AutoChasing && Chasing_Auto == null)
        {
            ChasingTarget_Continue(target);
        }
    }

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

    void MenuHotKey_Management()
    {
        //if (!Main.Instance.Management) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            UI_Main.Button_Save();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            UI_Main.Button_Pedia();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            UI_Main.Button_DayLog();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            UI_Main.Button_Facility();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            UI_Main.Button_MonsterManage();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            UI_Main.Visit_Guild();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            UI_Main.Button_Quest();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            UI_Main.Button_DungeonEdit();
        }
    }


    void Key_Tapkey()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Change_Targeting();
        }
    }
    void Change_Targeting()
    {
        //if (Chasing_Auto == null || AutoChasing == false) return;

        List<Transform> targetList = new List<Transform>();

        if (GameManager.NPC.Instance_EventNPC_List.Count > 0)
        {
            foreach (var item in GameManager.NPC.Instance_EventNPC_List)
            {
                if (item.GetComponentInChildren<SpriteRenderer>(true).enabled)
                {
                    targetList.Add(item.transform);
                }
            }
        }

        if (GameManager.NPC.Instance_NPC_List.Count > 0)
        {
            foreach (var item in GameManager.NPC.Instance_NPC_List)
            {
                if (item.GetComponentInChildren<SpriteRenderer>(true).enabled)
                {
                    targetList.Add(item.transform);
                }
            }
        }

        Util.ListShuffle(targetList);
        if (targetList.Count > 0)
        {
            ChasingTarget_Continue(targetList[0]);
        }
    }


    #endregion


    #region CameraShake

    CameraShake shake;

    public void CameraShake(float duration, float power, float rotationPower = 0f, float fadeOutTime = -1f)
    {

        shake.StartShake(duration, power, rotationPower, fadeOutTime);


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
