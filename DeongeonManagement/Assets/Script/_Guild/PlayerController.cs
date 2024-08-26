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

    //GuildManager guildManager;
    //Tilemap tile_borderline;

    float playerSize;
    void Start()
    {
        GuildManager.Instance.GuildEnter();

        rig = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        //guildManager = FindAnyObjectByType<GuildManager>();
        //tile_borderline = FindAnyObjectByType<TilemapCollider2D>().GetComponent<Tilemap>();

        playerSize = transform.localScale.y;
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
        RunningAnimation();

        if (MoveCor_A == null)
        {
            rig.bodyType = RigidbodyType2D.Dynamic;
        }
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
            else
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
        if (current_NPC != null && Input.GetKeyDown(KeyCode.E))
        {
            if (MoveCor_A != null)
            {
                StopCoroutine(MoveCor_A);
                MoveCor_A = null;
            }
            current_NPC.StartDialogue();
            rig.velocity = Vector2.zero;
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
            //MouseClick();
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
        // ���콺 ��ġ�� ��ũ�� ��ǥ���� ���� ��ǥ�� ��ȯ
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //var targetCheck = (Vector2Int)tilemap.WorldToCell(mouseWorldPos);
        //Debug.Log($"{targetCheck}");

        // ���� ��ǥ�� Ÿ�ϸ��� �׸��� ��ǥ�� ��ȯ
        var target = (Vector2Int)tilemap.WorldToCell(mouseWorldPos) - new Vector2Int(tilemap.cellBounds.xMin, tilemap.cellBounds.yMin);
        targetPoint = mapData.guildTileMap[target];


        Vector3 roundPos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);

        var _player = (Vector2Int)tilemap.WorldToCell(roundPos) - new Vector2Int(tilemap.cellBounds.xMin, tilemap.cellBounds.yMin);
        startPoint = mapData.guildTileMap[_player];

        //Debug.Log($"����! �÷��̾� ��ǥ : {startPoint.worldPosition}\n������ ��ǥ : {targetPoint.worldPosition}");
        //Debug.Log($"�ε���! �÷��̾� ��ǥ : {startPoint.index}\n������ ��ǥ : {targetPoint.index}");

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
        var rig = GetComponent<Rigidbody2D>();
        rig.bodyType = RigidbodyType2D.Kinematic;

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

        rig.bodyType = RigidbodyType2D.Dynamic;
        MoveCor_A = null;
    }


    void NPC_Interaction()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (current_NPC != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == current_NPC.gameObject)
            {
                if (MoveCor_A != null)
                {
                    StopCoroutine(MoveCor_A);
                    MoveCor_A = null;
                }
                //Debug.Log("�浹�� ��ü: " + hit.collider.gameObject.name);
                current_NPC.StartDialogue();
                rig.velocity = Vector2.zero;
                return;
            }
        }
    }



    //Coroutine moveCor;
    void MouseClick()
    {
        //��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //if (current_NPC != null)
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        //    if (hit.collider != null && hit.collider.gameObject == current_NPC.gameObject)
        //    {
        //        if (moveCor != null)
        //        {
        //            StopCoroutine(moveCor);
        //        }
        //        //Debug.Log("�浹�� ��ü: " + hit.collider.gameObject.name);
        //        current_NPC.StartDialogue();
        //        return;
        //    }
        //}


        //if (moveCor != null)
        //{
        //    StopCoroutine(moveCor);
        //}
        //moveCor = StartCoroutine(PlayerMouseMove(worldPosition));
    }

    //IEnumerator PlayerMouseMove(Vector3 _movePoint)
    //{
    //    Vector3 direction = _movePoint - transform.position;
    //    Vector2 move = new Vector2(direction.x, direction.y).normalized;

    //    float distance = Vector3.Distance(transform.position, _movePoint);
    //    if (distance < 0.5f)
    //    {
    //        StopCoroutine(moveCor);
    //        moveCor = null;
    //    }
    //    float timer = 0;
    //    while (timer < 0.1f)
    //    {
    //        yield return null;
    //        timer += Time.deltaTime;
    //        rig.velocity = move * speed;
    //    }

    //    while (distance > 0.5f && isContact == null)
    //    {
    //        yield return null;
    //        rig.velocity = move * speed;
    //        distance = Vector2.Distance(transform.position, _movePoint);
    //    }

    //    moveCor = null;
    //}

    Collision2D isContact;


    Interaction_Guild current_NPC;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"Enter : {collision.name}");
        if (collision.CompareTag("Guild_Exit"))
        {
            var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
            ui.SetText(UserData.Instance.LocaleText("Confirm_Return"), () => Exit_Action());
            return;
        }

        current_NPC = collision.GetComponent<Interaction_Guild>();
        current_NPC.Contact();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log($"Exit : {collision.name}");
        Interaction_Guild interact;
        if (collision.TryGetComponent(out interact))
        {
            interact.ContactOff();
        }
        current_NPC = null;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isContact = collision;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isContact = null;
    }



    void Exit_Action()
    {
        //Debug.Log("���� ������ ó��");
        Managers.Scene.AddLoadAction_OneTime(() => Main.Instance.Default_Init());
        Managers.Scene.AddLoadAction_OneTime(() => Managers.Data.LoadGame_ToGuild("Temp_GuildSave"));

        if (GuildManager.Instance.DungeonBackAction != null)
        {
            Managers.Scene.AddLoadAction_OneTime(GuildManager.Instance.DungeonBackAction);
            Managers.Scene.AddLoadAction_OneTime(() => { GuildManager.Instance.DungeonBackAction = null; });
        }

        Managers.Scene.AddLoadAction_OneTime(() => FindObjectOfType<UI_Management>().Texts_Refresh());
        //Managers.Scene.AddLoadAction_OneTime(() => Debug.Log("���� �����°� �����ϴ� �����ε� �� �ϰ� �ֳ�?"));

        //? �ð� ������� �ǵ�����
        Managers.Scene.AddLoadAction_OneTime(() => UserData.Instance.GamePlay(UserData.Instance.GameSpeed_GuildReturn));
        Managers.Scene.LoadSceneAsync(SceneName._2_Management);
    }


}
