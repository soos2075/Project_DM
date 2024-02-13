using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public float h;
    public float v;

    Rigidbody2D rig;
    Animator anim;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

   
    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(h, v).normalized;
        rig.velocity = move * speed;




        if (anim.GetInteger("h") != h)
        {
            anim.SetInteger("h", (int)h);
            anim.SetBool("isMove", true);
        }
        else if (anim.GetInteger("v") != v)
        {
            if (h == 1)
            {
                return;
            }
            anim.SetInteger("v", (int)v);
            anim.SetBool("isMove", true);

        }
        else
        {
            anim.SetBool("isMove", false);
        }




        if (current_NPC != null)
        {
            StartTalk();
        }
    }



    void StartTalk()
    {
        if (Managers.Dialogue.GetState() == DialogueManager.DialogueState.Talking)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            //Managers.Dialogue.ShowDialogueUI(current_NPC.StartDialogue());
            current_NPC.StartDialogue();
            //isTalking = true;
            //StartCoroutine(WaitOverTalking());
        }
    }

    //IEnumerator WaitOverTalking()
    //{
    //    yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);
    //    isTalking = false;
    //}
    //bool isTalking;

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




    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Debug.Log("각종 데이터 처리");
            Managers.Scene.AddLoadAction_OneTime(() => Main.Instance.Default_Init());
            Managers.Scene.AddLoadAction_OneTime(() => Managers.Data.LoadGame("AutoSave"));
            Managers.Scene.AddLoadAction_OneTime(() => Main.Instance.Player_AP = 0);

            //for (int i = 0; i < GuildManager.Instance.DungeonBackAction.Count; i++)
            //{
            //    Managers.Scene.AddLoadAction_Once(GuildManager.Instance.DungeonBackAction[i]);
            //}
            for (int i = 0; i < GuildManager.Instance.DungeonBackAction.Count; i++)
            {
                Managers.Scene.AddLoadAction_OneTime(GuildManager.Instance.DungeonBackAction[i]);
            }
            GuildManager.Instance.DungeonBackAction.Clear();
            Managers.Scene.AddLoadAction_OneTime(() => FindObjectOfType<UI_Management>().Texts_Refresh());

            Managers.Scene.LoadSceneAsync("2_Management");
        }
    }
}
