using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHunter : NPC
{
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get; set; }

    void Init_AvoidType()
    {
        AvoidTileType = new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility };
    }

    protected override void SetPriorityList()
    {
        Init_AvoidType();

        if (PriorityList != null) PriorityList.Clear();

        SetHunterType();
    }


    public enum HunterType
    {
        Slime = 1100,
        Skeleton = 1200,
    }
    public HunterType Hunter { get; set; }


    void SetHunterType()
    {
        switch (Hunter)
        {
            case HunterType.Slime:
                AddList(GetPriorityPick(typeof(Slime)));
                break;

            case HunterType.Skeleton:
                AddList(GetPriorityPick(typeof(Skeleton)));
                break;
        }
    }


    protected override void NPC_Die()
    {
        Main.Instance.CurrentDay.AddKill(1);
        UI_EventBox.AddEventText($"◈{Name_KR} (이)가 {PlacementInfo.Place_Floor.Name_KR}에서 쓰러짐");
        GameManager.NPC.InactiveNPC(this);

        //Main.Instance.CurrentDay.Fame += 1;
        //Main.Instance.CurrentDay.Danger += 5;
        //Main.Instance.CurrentDay.AddGold(Data.Rank * UnityEngine.Random.Range(20, 30));

        HunterOverAction();
    }

    void HunterOverAction()
    {
        //? 타입번호 = 해당 몬스터의 퀘스트 번호
        EventManager.Instance.RemoveQuestAction((int)Hunter);

        switch (Hunter)
        {
            case HunterType.Slime:
                //? 몬스터 진화
                Debug.Log("슬라임 진~~화 실제 액션 진행하면댐");
                break;

            case HunterType.Skeleton:
                //? 몬스터 진화
                Debug.Log("스켈레톤 진~~화");
                break;
        }
    }
}
