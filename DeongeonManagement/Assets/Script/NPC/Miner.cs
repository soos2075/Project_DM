using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : NPC
{


    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get; set; }

    void Init_AvoidType()
    {
        AvoidTileType = new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };
    }

    protected override void SetPriorityList()
    {
        Init_AvoidType();

        if (PriorityList != null) PriorityList.Clear();


        var list1 = GetPriorityPick(typeof(Mineral));

        AddList(list1);


        {
            var list5 = GetFacilityPick(Facility.FacilityType.NPCEvent);
            AddList(list5);
        }
       

        PickToProbability(GetPriorityPick(typeof(Entrance_Egg)), (PlacementInfo.Place_Floor.FloorIndex + Rank) * 0.04f);
    }


    protected override void NPC_Return_Empty()
    {
        Main.Instance.CurrentDay.Fame -= 1;
        Main.Instance.CurrentDay.Danger -= 1;
    }
    protected override void NPC_Return_Satisfaction()
    {
        Main.Instance.CurrentDay.Fame += (2 + Data.Rank);
    }
    protected override void NPC_Runaway()
    {
        Main.Instance.CurrentDay.Danger += (2 + Data.Rank);
    }
    protected override void NPC_Die()
    {
        var prison = GameManager.Technical.Prison;
        if (prison != null)
        {
            var ran = Random.Range(0, 10);
            if (ran > 3)
            {
                NPC_Captive();
                return;
            }
        }

        UI_EventBox.AddEventText($"��{Name_KR} (��)�� ������");
        Main.Instance.CurrentDay.AddKill(1);

        Main.Instance.CurrentDay.Danger += (3 + Data.Rank);
        Main.Instance.CurrentDay.AddGold(Data.Rank * Random.Range(20, 30));
    }
    protected override void NPC_Captive()
    {
        UI_EventBox.AddEventText($"��{Name_KR} (��)�� ���η� ����");
        Main.Instance.CurrentDay.AddPrisoner(1);

        Main.Instance.CurrentDay.Danger += (3 + Data.Rank);
        Main.Instance.CurrentDay.AddGold(Data.Rank * Random.Range(40, 60));
    }
}
