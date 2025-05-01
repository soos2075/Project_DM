using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public TileMapData_Guild mapData;
    public Tilemap tilemap;

    public float speed;
    public float h;
    public float v;

    Rigidbody2D rig;
    Animator anim;
    SpriteRenderer sprite;

    float playerSize;

    private void Awake()
    {
        gameObject.name = "Player";
    }
    void Start()
    {
        transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Start).position;

        GuildManager.Instance.GuildEnter();

        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        playerSize = transform.localScale.y;

        mainUI = FindAnyObjectByType<UI_Guild_Info>();
    }

   
    void Update()
    {
        if (Time.timeScale == 0 || Managers.UI._popupStack.Count > 0 || UserData.Instance.GameMode == Define.GameMode.Stop)
        {
            return;
        }

        KeyboardEvent();
        MouseEvent();
    }

    

    private void LateUpdate()
    {
        Key_Esc();

        if (Time.timeScale == 0 || Managers.UI._popupStack.Count > 0 || UserData.Instance.GameMode == Define.GameMode.Stop)
        {
            return;
        }

        if (rig.bodyType != RigidbodyType2D.Dynamic)
        {
            rig.bodyType = RigidbodyType2D.Dynamic;
        }
        if (sprite.flipX || sprite.flipY)
        {
            sprite.flipX = false;
            sprite.flipY = false;
        }

        RunningAnimation();
    }
    void RunningAnimation()
    {
        if (rig.velocity.magnitude > 0)
        {
            anim.Play(Define.ANIM_Running);
            if (rig.velocity.x < 0)
            {
                //transform.localScale = new Vector3(-1, 1, 1) * playerSize;
                transform.GetChild(0).localScale = new Vector3(-1, 1, 1) * playerSize;
            }
            else if (rig.velocity.x > 0)
            {
                //transform.localScale = Vector3.one * playerSize;
                transform.GetChild(0).localScale = Vector3.one * playerSize;
            }
        }
        else
        {
            anim.Play(Define.ANIM_Idle);
        }
    }
    void KeyboardEvent()
    {
        Interaction_Guild current = null;
        if (CurrentInteraction(out current) && Input.GetKeyDown(KeyCode.E))
        {
            if (MoveCor_A != null)
            {
                StopCoroutine(MoveCor_A);
                MoveCor_A = null;
            }
            anim.Play(Define.ANIM_Idle);
            rig.velocity = Vector2.zero;
            current.StartDialogue();
            return;
        }

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(h, v).normalized;
        rig.velocity = move * speed;

        if (move.magnitude > 0 && MoveCor_A != null)
        {
            StopCoroutine(MoveCor_A);
            MoveCor_A = null;
        }
    }


    void MouseEvent()
    {
        if (Managers.UI._popupStack.Count > 0)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Move_AStar();
            NPC_Interaction();
        }
    }

    TileMapData_Guild.GuildTile targetPoint;
    TileMapData_Guild.GuildTile startPoint;

    Coroutine MoveCor_A;


    readonly float moveOffset = 1f / 7f;

    void Move_AStar()
    {
        // 마우스 위치를 스크린 좌표에서 월드 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 월드 좌표를 타일맵의 그리드 좌표로 변환
        var target = (Vector2Int)tilemap.WorldToCell(mouseWorldPos) - new Vector2Int(tilemap.cellBounds.xMin, tilemap.cellBounds.yMin);

        if (mapData.guildTileMap.ContainsKey(target) == false) return;

        targetPoint = mapData.guildTileMap[target];


        Vector3 roundPos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);

        var _player = (Vector2Int)tilemap.WorldToCell(roundPos) - new Vector2Int(tilemap.cellBounds.xMin, tilemap.cellBounds.yMin);
        startPoint = mapData.guildTileMap[_player];

        //Debug.Log($"월드! 플레이어 좌표 : {startPoint.worldPosition}\n도착지 좌표 : {targetPoint.worldPosition}");
        //Debug.Log($"인덱스! 플레이어 좌표 : {startPoint.index}\n도착지 좌표 : {targetPoint.index}");

        var check = targetPoint.isPath;
        //Debug.Log(check);
        if (check)
        {
            var pathfinding = mapData.PathFinding(startPoint, targetPoint);
            if (MoveCor_A != null)
            {
                StopCoroutine(MoveCor_A);
            }
            MoveCor_A = StartCoroutine(PlayerMove(pathfinding));
        }
    }


    IEnumerator PlayerMove(List<TileMapData_Guild.GuildTile> moveList)
    {
        float dis;
        float moveValue;
        Vector3 dir;
        float timer = 0;
        float duration = moveOffset;
        foreach (var item in moveList)
        {
            dir = item.worldPosition - transform.position;
            dis = Vector3.Distance(transform.position, item.worldPosition);
            duration = moveOffset * dis;
            moveValue = dis / duration;
            timer = 0;
            while (timer < duration)
            {
                yield return null;
                timer += Time.deltaTime;
                //transform.position += dir.normalized * moveValue * Time.deltaTime;
                rig.velocity = dir.normalized * speed;
            }

            //transform.position = new Vector3(item.worldPosition.x, item.worldPosition.y, 0);
        }

        MoveCor_A = null;
    }


    void NPC_Interaction()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Interaction_Guild current = null;
        if (CurrentInteraction(out current))
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == current.gameObject)
            {
                if (MoveCor_A != null)
                {
                    StopCoroutine(MoveCor_A);
                    MoveCor_A = null;
                }
                //Debug.Log("충돌한 객체: " + hit.collider.gameObject.name);
                anim.Play(Define.ANIM_Idle);
                rig.velocity = Vector2.zero;
                current.StartDialogue();
                return;
            }
        }
    }



    HashSet<Collider2D> currentContact = new HashSet<Collider2D>();

    bool CurrentInteraction(out Interaction_Guild current)
    {
        if (currentContact.Count == 0)
        {
            current = null;
            return false;
        }

        List<Interaction_Guild> list = new List<Interaction_Guild>();
        foreach (var item in currentContact)
        {
            list.Add(item.GetComponent<Interaction_Guild>());
        }

        current = list[list.Count - 1];
        return true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"Enter : {collision.name}");
        if (collision.CompareTag("Guild_Exit"))
        {
            var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
            ui.SetText(UserData.Instance.LocaleText("Confirm_Return"), () => Exit_Action());
            return;
        }

        currentContact.Add(collision);
        collision.GetComponent<Interaction_Guild>()?.Contact();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log($"Exit : {collision.name}");

        currentContact.Remove(collision);
        collision.GetComponent<Interaction_Guild>()?.ContactOff();
    }


    void Exit_Action()
    {
        //Debug.Log("각종 데이터 처리");
        Managers.Scene.AddLoadAction_OneTime(() => Main.Instance.Default_Init());
        Managers.Scene.AddLoadAction_OneTime(() => Managers.Data.LoadGame_ToGuild("Temp_GuildSave"));

        if (GuildManager.Instance.DungeonBackAction != null)
        {
            Managers.Scene.AddLoadAction_OneTime(GuildManager.Instance.DungeonBackAction);
            Managers.Scene.AddLoadAction_OneTime(() => { GuildManager.Instance.DungeonBackAction = null; });
        }

        Managers.Scene.AddLoadAction_OneTime(() => FindObjectOfType<UI_Management>().Texts_Refresh());
        //Managers.Scene.AddLoadAction_OneTime(() => Debug.Log("유명도 오르는거 실행하는 순서인데 잘 하고 있냐?"));

        //? 시간 원래대로 되돌리기
        Managers.Scene.AddLoadAction_OneTime(() => UserData.Instance.GamePlay(UserData.Instance.GameSpeed_GuildReturn));
        Managers.Scene.LoadSceneAsync(SceneName._2_Management);
    }



    bool mainUI_Hide = false;
    UI_Guild_Info mainUI;

    void Key_Esc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if (Managers.UI._popupStack.Count == 0)
            //{
            //    Managers.UI.ShowPopUp<UI_Pause>();
            //}
            //else if (Managers.UI._popupStack.Peek().EscapeKeyAction())
            //{
            //    Managers.UI.ClosePopUp();
            //}

            if (Managers.UI._popupStack.Count > 0)
            {
                return;
            }

            if (mainUI_Hide == false)
            {
                mainUI_Hide = true;
                mainUI.gameObject.SetActive(false);
            }
            else
            {
                mainUI_Hide = false;
                mainUI.gameObject.SetActive(true);
            }
        }
    }
}
