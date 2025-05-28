using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class NPC_SubEvent : NPC
{
    NPC_Type_SubEvent NPCType { get { return (NPC_Type_SubEvent)EventID; } }

    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }

    protected override void Start_Setting()
    {
        //? 공격타입과 투사체 세팅
        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine:
                //AttackOption.SetProjectile(AttackType.Bow, "LegucyElf", "ElfA");
                RunawayHpRatio = 3;
                if (GameManager.Technical.Prison != null)
                {
                    RunawayHpRatio = 1000;
                }
                KillGold = 50;
                break;

            case NPC_Type_SubEvent.Lightning:
                //AttackOption.SetProjectile(AttackType.Normal, "Fireball", "4");
                ActionPoint = 1;
                AlwaysOverlap = true;
                RunawayHpRatio = 2;
                break;

            case NPC_Type_SubEvent.Judgement:
                RunawayHpRatio = 999;
                break;

            case NPC_Type_SubEvent.Venom:
                RunawayHpRatio = 999;
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
                switch (NPCType)
                {
                    case NPC_Type_SubEvent.Heroine:
                        return new Define.TileType[] { Define.TileType.NPC };

                    case NPC_Type_SubEvent.Lightning:
                        return new Define.TileType[] { };

                    default:
                        return new Define.TileType[] { Define.TileType.Facility };
                }
        }
    }

    protected override void SetRandomClothes() //? 복장
    {
        SpriteLibrary sla = GetComponentInChildren<SpriteLibrary>();

        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine: //? 디폴트가 궁수라 일단 그대로 둠
                break;

            case NPC_Type_SubEvent.Lightning:
                sla.spriteLibraryAsset = Managers.Sprite.Get_NPC_Anim("SE_Lightning");
                break;


                //? 얘넨 프리팹이 별도
            //case NPC_Type_SubEvent.Judgement:
            //    sla.spriteLibraryAsset = Managers.Sprite.Get_NPC_Anim("Main_Judgement");
            //    break;

            //case NPC_Type_SubEvent.Venom:
            //    sla.spriteLibraryAsset = Managers.Sprite.Get_NPC_Anim("Main_Venom");
            //    break;
        }
    }

    public override void Departure(Vector3 startPoint, Vector3 endPoint) //? 던전에 도착했을 때
    {
        base.Departure(startPoint, endPoint);

        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine:
                if (UserData.Instance.FileConfig.firstAppear_Heroine == false)
                {
                    UserData.Instance.FileConfig.firstAppear_Heroine = true;
                    StartCoroutine(EventCor(DialogueName.Heroine_Appear));
                }
                break;

            case NPC_Type_SubEvent.Lightning:
                if (EventManager.Instance.CurrentClearEventData.Check_AlreadyClear(DialogueName.Lightning_Enter_First))
                {
                    StartCoroutine(EventCor(DialogueName.Lightning_Enter));
                }
                else
                {
                    StartCoroutine(EventCor(DialogueName.Lightning_Enter_First));
                }
                break;
        }
    }



    protected override void SetPriorityList(PrioritySortOption option)
    {
        if (PriorityList != null) PriorityList.Clear();

        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine:
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                break;

            case NPC_Type_SubEvent.Lightning:
                PriorityList = new List<BasementTile>();
                break;

            case NPC_Type_SubEvent.Judgement:
            case NPC_Type_SubEvent.Venom:
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                var add_egg = GetPriorityPick(typeof(SpecialEgg));
                AddList(add_egg);
                var portal = GetPriorityPick(typeof(Entrance_Egg));
                AddList(portal);
                break;
        }
    }



    protected override void NPC_Return_Empty()
    {
        switch (NPCType)
        {
            case NPC_Type_SubEvent.Lightning:
                var emotion = transform.Find("Emotions");
                emotion.GetComponent<SpriteRenderer>().sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Element_State", "Perfect");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Lightning_Success, transform);
                EventManager.Instance.Add_GuildQuest_Special(12020);
                EventManager.Instance.ClearQuestAction(12000);
                Main.Instance.CurrentDay.AddPop(100);
                Main.Instance.ShowDM(100, Main.TextType.pop, transform, 1);
                //AddCollectionPoint();
                break;
        }

    }


    protected override void NPC_Return_Satisfaction()
    {
        switch (NPCType)
        {
            case NPC_Type_SubEvent.Lightning:
                var emotion = transform.Find("Emotions");
                emotion.GetComponent<SpriteRenderer>().sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Element_State", "Bad");
                if (EventManager.Instance.CurrentClearEventData.Check_AlreadyClear(DialogueName.Lightning_Fail_First))
                {
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Lightning_Fail, transform);
                }
                else
                {
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Lightning_Fail_First, transform);
                }
                break;
        }
    }
    protected override void NPC_Return_NonSatisfaction()
    {
        switch (NPCType)
        {
            case NPC_Type_SubEvent.Lightning:
                var emotion = transform.Find("Emotions");
                emotion.GetComponent<SpriteRenderer>().sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Element_State", "Bad");
                if (EventManager.Instance.CurrentClearEventData.Check_AlreadyClear(DialogueName.Lightning_Fail_First))
                {
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Lightning_Fail, transform);
                }
                else
                {
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Lightning_Fail_First, transform);
                }
                break;
        }
    }
    protected override void NPC_Runaway()
    {
        switch (NPCType)
        {
            case NPC_Type_SubEvent.Lightning:
                var emotion = transform.Find("Emotions");
                emotion.GetComponent<SpriteRenderer>().sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Element_State", "Bad");
                if (EventManager.Instance.CurrentClearEventData.Check_AlreadyClear(DialogueName.Lightning_Fail_First))
                {
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Lightning_Fail, transform);
                }
                else
                {
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Lightning_Fail_First, transform);
                }
                break;
        }
    }
    protected override void NPC_Die()
    {
        if (GameManager.Technical.Prison != null)
        {
            Captive();
        }
        else
        {
            switch (NPCType)
            {
                case NPC_Type_SubEvent.Heroine:
                    EventManager.Instance.Add_GuildQuest_Special(4030, true);
                    break;

                case NPC_Type_SubEvent.Lightning:
                    Main.Instance.CurrentDay.AddDanger(50);
                    Main.Instance.ShowDM(50, Main.TextType.danger, transform, 1);
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Lightning_Defeat, transform);
                    EventManager.Instance.ClearQuestAction(12000);
                    GuildManager.Instance.RemoveInstanceGuildNPC(GuildNPC_LabelName.Lightning);
                    GuildManager.Instance.AddDeleteGuildNPC(GuildNPC_LabelName.Lightning);
                    break;
            }
        }

        Dead();
        //GameManager.NPC.InactiveNPC(this);
        UI_EventBox.AddEventText($"◈{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");

    }


    void Captive()
    {
        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine:
                EventManager.Instance.AddTurnOverEventReserve(() => Managers.Dialogue.ShowDialogueUI(DialogueName.Heroine_Prison, Main.Instance.Player));
                EventManager.Instance.ClearQuestAction(4020);
                EventManager.Instance.ClearQuestAction(4030);
                EventManager.Instance.Clear_GuildQuest(4030);
                break;

            case NPC_Type_SubEvent.Lightning:
                Main.Instance.CurrentDay.AddDanger(50);
                Main.Instance.ShowDM(50, Main.TextType.danger, transform, 1);
                Managers.Dialogue.ShowDialogueUI(DialogueName.Lightning_Defeat, transform);
                EventManager.Instance.ClearQuestAction(12000);
                GuildManager.Instance.RemoveInstanceGuildNPC(GuildNPC_LabelName.Lightning);
                GuildManager.Instance.AddDeleteGuildNPC(GuildNPC_LabelName.Lightning);
                break;

            case NPC_Type_SubEvent.Venom:
            case NPC_Type_SubEvent.Judgement:
                break;
        }
    }


    void Dead()
    {
        Managers.Dialogue.ActionReserve(() => InactiveCallback());
    }
    void InactiveCallback()
    {
        GameManager.NPC.InactiveNPC(this);
    }

}
