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
        //? ����Ÿ�԰� ����ü ����
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

    Define.TileType[] AvoidTile() //? ������ ���� ó�� ��ã�� �����̰�, ���� ��ã���� �ٽ� ã�� �� �̰� ���� ã���ϱ� ����
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

    protected override void SetRandomClothes() //? ����
    {
        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine: //? ��� Builder�� ��� ���� �ؾ���...�ٵ� �̷��� �ƿ� �������� �и��ϴ°� ���� �ʳ��ͱ⵵�ϰ�?

                break;

            case NPC_Type_SubEvent.DungeonRacer:
                var collection = characterBuilder.SpriteCollection;

                characterBuilder.Rebuild();
                break;


        }
    }

    public override void Departure(Vector3 startPoint, Vector3 endPoint) //? ������ �������� ��
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

        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine:
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                break;

            case NPC_Type_SubEvent.DungeonRacer:
                PriorityList = new List<BasementTile>();
                break;
        }
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
            switch (NPCType)
            {
                case NPC_Type_SubEvent.Heroine:
                    EventManager.Instance.Add_GuildQuest_Special(4030, true);
                    break;

                case NPC_Type_SubEvent.DungeonRacer:
                    break;
            }
        }

        GameManager.NPC.InactiveNPC(this);
        UI_EventBox.AddEventText($"��{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");

        //AddCollectionPoint();
    }


    void Captive()
    {
        switch (NPCType)
        {
            case NPC_Type_SubEvent.Heroine:
                EventManager.Instance.AddTurnOverEventReserve(() => Managers.Dialogue.ShowDialogueUI(DialogueName.Heroine_Prison, Main.Instance.Player));
                EventManager.Instance.ClearQuestAction(774020);
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
