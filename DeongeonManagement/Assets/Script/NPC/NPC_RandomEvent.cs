using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class NPC_RandomEvent : NPC
{
    NPC_Type_RandomEvent NPCType { get { return (NPC_Type_RandomEvent)EventID; } }

    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }

    protected override void Start_Setting()
    {
        //? 공격타입과 투사체 세팅
        switch (NPCType)
        {
            case NPC_Type_RandomEvent.Karen:
            case NPC_Type_RandomEvent.Romys:
                RunawayHpRatio = 999;
                break;

            case NPC_Type_RandomEvent.Stan:
            case NPC_Type_RandomEvent.Euh:
                RunawayHpRatio = 4;
                break;

            case NPC_Type_RandomEvent.Siri:
            case NPC_Type_RandomEvent.Mastia:
                RunawayHpRatio = 2;
                break;
        }
    }


    protected override Define.TileType[] AvoidTileType { get { return AvoidTile(); } }

    Define.TileType[] AvoidTile() //? 어차피 제일 처음 길찾기 조건이고, 길을 못찾으면 다시 찾을 땐 이거 없이 찾으니까 괜찮
    {
        switch (State)
        {
            case NPCState.Runaway:
            case NPCState.Return_Satisfaction:
                return new Define.TileType[] { Define.TileType.Facility, Define.TileType.Monster };

            default:
                return new Define.TileType[] { Define.TileType.NPC };
        }
    }

    protected override void SetRandomClothes() //? 복장
    {
        SpriteLibrary sla = GetComponentInChildren<SpriteLibrary>();

        switch (NPCType)
        {
            case NPC_Type_RandomEvent.Mastia:
                sla.spriteLibraryAsset = Managers.Sprite.Get_NPC_Anim("RE_Mastia");
                break;

            case NPC_Type_RandomEvent.Karen:
                sla.spriteLibraryAsset = Managers.Sprite.Get_NPC_Anim("RE_Karen");
                break;

            case NPC_Type_RandomEvent.Stan:
                sla.spriteLibraryAsset = Managers.Sprite.Get_NPC_Anim("RE_Stan");
                break;

            case NPC_Type_RandomEvent.Euh:
                sla.spriteLibraryAsset = Managers.Sprite.Get_NPC_Anim("RE_Euh");
                break;

            case NPC_Type_RandomEvent.Romys:
                sla.spriteLibraryAsset = Managers.Sprite.Get_NPC_Anim("RE_Romys");
                break;

            case NPC_Type_RandomEvent.Siri:
                sla.spriteLibraryAsset = Managers.Sprite.Get_NPC_Anim("RE_Siri");
                break;
        }
    }

    public override void Departure(Vector3 startPoint, Vector3 endPoint) //? 던전에 도착했을 때
    {
        base.Departure(startPoint, endPoint);

        switch (NPCType)
        {
            case NPC_Type_RandomEvent.Mastia:
                StartCoroutine(EventCor(DialogueName.Mastia_Enter));
                break;

            case NPC_Type_RandomEvent.Karen:
                StartCoroutine(EventCor(DialogueName.Karen_Enter));
                break;

            case NPC_Type_RandomEvent.Stan:
                StartCoroutine(EventCor(DialogueName.Stan_Enter));
                break;

            case NPC_Type_RandomEvent.Euh:
                StartCoroutine(EventCor(DialogueName.Euh_Enter));
                break;

            case NPC_Type_RandomEvent.Romys:
                StartCoroutine(EventCor(DialogueName.Romys_Enter));
                break;

            case NPC_Type_RandomEvent.Siri:
                StartCoroutine(EventCor(DialogueName.Siri_Enter));
                break;
        }
    }



    protected override void SetPriorityList(PrioritySortOption option)
    {
        if (PriorityList != null) PriorityList.Clear();

        switch (NPCType)
        {
            case NPC_Type_RandomEvent.Karen:
            case NPC_Type_RandomEvent.Romys:
            case NPC_Type_RandomEvent.Stan:
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                AddPriorityList(GetPriorityPick(typeof(Treasure)), AddPos.Back, option);
                break;

            case NPC_Type_RandomEvent.Euh:
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                AddPriorityList(GetPriorityPick(typeof(Herb)), AddPos.Back, option);
                AddPriorityList(GetPriorityPick(typeof(Mineral)), AddPos.Back, option);
                break;


            case NPC_Type_RandomEvent.Siri:
            case NPC_Type_RandomEvent.Mastia:
                AddPriorityList(GetPriorityPick(typeof(Treasure)), AddPos.Back, option);
                AddPriorityList(GetPriorityPick(typeof(Herb)), AddPos.Back, option);
                AddPriorityList(GetPriorityPick(typeof(Mineral)), AddPos.Back, option);
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Back, option);
                break;
        }
    }



    protected override void NPC_Return_Empty()
    {
        ReturnDefault();
        //Return_AP_Zero();
    }
    protected override void NPC_Return_Satisfaction()
    {
        ReturnDefault();
        //Return_AP_Zero();
    }
    protected override void NPC_Return_NonSatisfaction()
    {
        ReturnDefault();
        //Return_AP_Zero();
    }
    protected override void NPC_Runaway()
    {
        ReturnDefault();
        //Return_AP_Zero();
    }


    void Return_AP_Zero() //? 카린이랑 류미스가 너무 세서 못잡을 것 같을 때 사용하면 되는데 음... 너무 쉬우려나?
    {
        if (ActionPoint <= 0)
        {
            var emotion = transform.Find("Emotions");
            emotion.gameObject.SetActive(false);
            switch (NPCType)
            {
                case NPC_Type_RandomEvent.Karen:
                    if (!GameManager.Monster.Check_Evolution("FlameGolem"))
                    {
                        Managers.Dialogue.ShowDialogue_PlayerRoom(1712002, this, (npc) => InactiveCallback());
                        Join_Event("Karen");
                    }
                    break;

                case NPC_Type_RandomEvent.Romys:
                    if (!GameManager.Monster.Check_Evolution("Salinu"))
                    {
                        Managers.Dialogue.ShowDialogue_PlayerRoom(1715002, this, (npc) => InactiveCallback());
                        Join_Event("Romys");
                    }
                    break;
            }
        }
    }


    void ReturnDefault()
    {
        //? 조건 맞으면 바로 대사 띄우고 대사 끝난다음에 유닛으로 합류하기
        switch (NPCType)
        {
            case NPC_Type_RandomEvent.Mastia:
                if (!GameManager.Monster.Check_Evolution("Pixie"))
                {
                    var emotion = transform.Find("Emotions");
                    emotion.gameObject.SetActive(false);
                    Managers.Dialogue.ShowDialogue_PlayerRoom(1711002, this, (npc) => InactiveCallback());
                    Join_Event("Mastia");
                }
                break;

            case NPC_Type_RandomEvent.Siri:
                {
                    var emotion = transform.Find("Emotions");
                    emotion.gameObject.SetActive(false);
                    Managers.Dialogue.ShowDialogue_PlayerRoom(1716002, this, (npc) => InactiveCallback());
                    Join_Event("Siri");
                }
                break;


            case NPC_Type_RandomEvent.Stan:
                if (!GameManager.Monster.Check_Evolution("Lilith"))
                {
                    var emotion = transform.Find("Emotions");
                    emotion.gameObject.SetActive(false);
                    Managers.Dialogue.ShowDialogue_PlayerRoom(1713002, this, (npc) => InactiveCallback());
                    Join_Event("Stan");
                }
                break;

            case NPC_Type_RandomEvent.Euh:
                if (!GameManager.Monster.Check_Evolution("Griffin"))
                {
                    var emotion = transform.Find("Emotions");
                    emotion.gameObject.SetActive(false);
                    Managers.Dialogue.ShowDialogue_PlayerRoom(1714002, this, (npc) => InactiveCallback());
                    Join_Event("Euh");
                }
                break;

        }
    }

    protected override void NPC_Die()
    {
        //? 조건 맞으면 바로 대사 띄우고 대사 끝난다음에 유닛으로 합류하기
        switch (NPCType)
        {
            case NPC_Type_RandomEvent.Karen:
                if (!GameManager.Monster.Check_Evolution("FlameGolem"))
                {
                    Managers.Dialogue.ShowDialogue_PlayerRoom(1712002, this, (npc) => InactiveCallback());
                    Join_Event("Karen");
                }
                else
                {
                    GameManager.NPC.InactiveNPC(this);
                }
                break;

            case NPC_Type_RandomEvent.Romys:
                if (!GameManager.Monster.Check_Evolution("Salinu"))
                {
                    Managers.Dialogue.ShowDialogue_PlayerRoom(1715002, this, (npc) => InactiveCallback());
                    Join_Event("Romys");
                }
                else
                {
                    GameManager.NPC.InactiveNPC(this);
                }
                break;

            case NPC_Type_RandomEvent.Stan:
                if (!GameManager.Monster.Check_Evolution("Lilith"))
                {
                    Managers.Dialogue.ShowDialogue_PlayerRoom(1713002, this, (npc) => InactiveCallback());
                    Join_Event("Stan");
                }
                else
                {
                    GameManager.NPC.InactiveNPC(this);
                }
                break;

            case NPC_Type_RandomEvent.Euh:
                if (!GameManager.Monster.Check_Evolution("Griffin"))
                {
                    Managers.Dialogue.ShowDialogue_PlayerRoom(1714002, this, (npc) => InactiveCallback());
                    Join_Event("Karen");
                }
                else
                {
                    GameManager.NPC.InactiveNPC(this);
                }
                break;
            default:
                GameManager.NPC.InactiveNPC(this);
                break;
        }

        UI_EventBox.AddEventText($"◈{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");
    }

    void InactiveCallback()
    {
        GameManager.NPC.InactiveNPC(this);
    }

    void Join_Event(string keyName)
    {
        EventManager.Instance.AddTurnOverEventReserve(
            () =>
            {
                Debug.Log($"{keyName} 합류");
                GameManager.Monster.Resize_AddOne();
                var mon = GameManager.Monster.CreateMonster(keyName, false, true);

                //var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
                //ui.TargetMonster(mon);
                //ui.SetStateText($"<b>{UserData.Instance.LocaleText("New")}".SetTextColorTag(Define.TextColor.Plus_Green) +
                //    $"{UserData.Instance.LocaleText("유닛")}".SetTextColorTag(Define.TextColor.Plus_Green));
            });

    }
}
