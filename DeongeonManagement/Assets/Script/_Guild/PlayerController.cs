using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public float h;
    public float v;

    Rigidbody2D rig;
    Animator anim;

    GuildManager guildManager;
    Tilemap tile_borderline;

    float playerSize;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        guildManager = FindAnyObjectByType<GuildManager>();
        tile_borderline = FindAnyObjectByType<TilemapCollider2D>().GetComponent<Tilemap>();

        playerSize = transform.localScale.y;
    }

   
    void Update()
    {
        if (Time.timeScale == 0 || Managers.UI._popupStack.Count > 0)
        {
            return;
        }

        KeyboardEvent();
        MouseEvent();
    }

    private void LateUpdate()
    {
        RunningAnimation();
    }
    void RunningAnimation()
    {
        if (rig.velocity.magnitude > 0)
        {
            anim.Play(Define.ANIM_Running);
            if (rig.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1) * playerSize;
            }
            else
            {
                transform.localScale = Vector3.one * playerSize;
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
            if (moveCor != null)
            {
                StopCoroutine(moveCor);
            }
            current_NPC.StartDialogue();
        }

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(h, v).normalized;
        rig.velocity = move * speed;
    }


    void MouseEvent()
    {
        if (Managers.UI._popupStack.Count > 0)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            MouseClick();
        }
    }


    Coroutine moveCor;
    void MouseClick()
    {
        //스크린 좌표를 월드 좌표로 변환
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (current_NPC != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == current_NPC.gameObject)
            {
                Debug.Log("충돌한 객체: " + hit.collider.gameObject.name);
                current_NPC.StartDialogue();
                return;
            }
        }

        Vector3Int cellPosition = tile_borderline.WorldToCell(worldPosition);
        TileBase tile = tile_borderline.GetTile(cellPosition);
        if (tile != null)
        {
            Debug.Log("충돌한 타일: " + tile.name);
            if (moveCor != null)
            {
                StopCoroutine(moveCor);
            }
            return;
        }

        if (moveCor != null)
        {
            StopCoroutine(moveCor);
        }
        moveCor = StartCoroutine(PlayerMouseMove(worldPosition));
    }

    IEnumerator PlayerMouseMove(Vector3 _movePoint)
    {
        Vector3 direction = _movePoint - transform.position;
        Vector2 move = new Vector2(direction.x, direction.y).normalized;

        float distance = Vector3.Distance(transform.position, _movePoint);
        float timer = 0;
        while (timer < 0.2f)
        {
            yield return null;
            timer += Time.deltaTime;
            rig.velocity = move * speed;
        }

        while (distance > 0.5f && isContact == null)
        {
            yield return null;
            rig.velocity = move * speed;
            distance = Vector2.Distance(transform.position, _movePoint);
        }

        moveCor = null;
    }

    Collision2D isContact;


    Interaction_Guild current_NPC;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Enter : {collision.name}");
        if (collision.CompareTag("Guild_Exit"))
        {
            var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
            ui.SetText("던전으로 돌아갈까요?");
            StartCoroutine(WaitForAnswer(ui));
            return;
        }

        current_NPC = collision.GetComponent<Interaction_Guild>();
        current_NPC.Contact();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"Exit : {collision.name}");
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




    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Debug.Log("각종 데이터 처리");
            Managers.Scene.AddLoadAction_OneTime(() => Main.Instance.Default_Init());
            Managers.Scene.AddLoadAction_OneTime(() => Managers.Data.LoadGame_ToGuild("AutoSave"));
            Managers.Scene.AddLoadAction_OneTime(() => Main.Instance.Player_AP = 0);
            Managers.Scene.AddLoadAction_OneTime(() => Debug.Log("저장한거 먼저 불러오고나서"));

            //for (int i = 0; i < GuildManager.Instance.DungeonBackAction.Count; i++)
            //{
            //    Managers.Scene.AddLoadAction_Once(GuildManager.Instance.DungeonBackAction[i]);
            //}
            //for (int i = 0; i < guildManager.DungeonBackAction.Count; i++)
            //{
            //    Managers.Scene.AddLoadAction_OneTime(guildManager.DungeonBackAction[i]);
            //}
            //guildManager.DungeonBackAction.Clear();

            if (guildManager.DungeonBackAction != null)
            {
                Managers.Scene.AddLoadAction_OneTime(guildManager.DungeonBackAction);
                guildManager.DungeonBackAction = null;
            }

            
            Managers.Scene.AddLoadAction_OneTime(() => FindObjectOfType<UI_Management>().Texts_Refresh());
            Managers.Scene.AddLoadAction_OneTime(() => Debug.Log("유명도 오르는거 실행하는 순서인데 잘 하고 있냐?"));

            Managers.Scene.LoadSceneAsync(SceneName._2_Management);
        }
    }
}
