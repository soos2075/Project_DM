using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                AttackOption.SetProjectile(AttackType.Normal, "LegucyElf", "ElfA");
                RunawayHpRatio = 1000;
                KillGold = 50;
                break;
            //case NPC_Type_SubEvent.DungeonRacer:
            //    AttackOption.SetProjectile(AttackType.Magic, "Fireball", "4");
            //    break;
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

                    case NPC_Type_SubEvent.DungeonRacer:
                        return new Define.TileType[] { };

                    default:
                        return new Define.TileType[] { Define.TileType.Facility };
                }
        }
    }

    protected override void SetRandomClothes() //? 복장
    {
        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine: //? 얘는 Builder가 없어서 따로 해야함...근데 이러면 아예 프리팹을 분리하는게 낫지 않나싶기도하고?

                break;

            case NPC_Type_SubEvent.DungeonRacer:
                var collection = characterBuilder.SpriteCollection;

                characterBuilder.Rebuild();
                break;


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
        }
    }



    protected override void SetPriorityList(PrioritySortOption option)
    {
        if (PriorityList != null) PriorityList.Clear();

        List<BasementTile> main = null;
        //List<BasementTile> sub1 = null;
        //List<BasementTile> sub2 = null;

        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine:
                main = GetPriorityPick(typeof(Monster));
                break;

            case NPC_Type_SubEvent.DungeonRacer:
                PriorityList = new List<BasementTile>();
                break;
                //case NPC_Type_Hunter.Hunter_Slime:
                //    main = GetPriorityPick(typeof(Slime));
                //    break;
        }

        //? 메인이랑 서브는 위에서 결정
        switch (option)
        {
            case PrioritySortOption.Random:
                break;
            case PrioritySortOption.SortByDistance:
                SortByDistance(main);
                //SortByDistance(sub1);
                //SortByDistance(sub2);
                break;
        }

        AddList(main);
        //AddList(sub1);
        //AddList(sub2);

        //{//? 우물 등 모험가 유용 이벤트
        //    Add_Wells();
        //}
        //{//? 에그서치
        //    var add_egg = GetPriorityPick(typeof(SpecialEgg));
        //    AddList(add_egg);
        //}
        //{//? 전이진 서치
        //    PickToProbability(GetPriorityPick(typeof(Entrance_Egg)), (PlacementInfo.Place_Floor.FloorIndex + Rank) * 0.04f);
        //}
    }



    protected override void NPC_Return_Empty()
    {

    }
    protected override void NPC_Return_Satisfaction()
    {

    }
    protected override void NPC_Return_NonSatisfaction()
    {

    }
    protected override void NPC_Runaway()
    {

    }
    protected override void NPC_Die()
    {
        if (GameManager.Technical.Prison != null)
        {
            Captive();
        }
        else
        {
            EventManager.Instance.Add_GuildQuest_Special(4030, true);
        }


        UI_EventBox.AddEventText($"◈{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");
        GameManager.NPC.InactiveNPC(this);
        //AddCollectionPoint();
    }


    void Captive()
    {
        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine:
                EventManager.Instance.TurnOverEventReserve = () => Managers.Dialogue.ShowDialogueUI(DialogueName.Heroine_Prison, Main.Instance.Player);
                EventManager.Instance.RemoveQuestAction(4020);
                EventManager.Instance.Clear_GuildQuest(4030);
                break;

            case NPC_Type_SubEvent.DungeonRacer:
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
