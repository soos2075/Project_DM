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


    protected override void NPC_Return_Empty()
    {
        
    }
    protected override void NPC_Return_Satisfaction()
    {
        
    }
    protected override void NPC_Runaway()
    {
        
    }

    protected override void NPC_Die()
    {
        Main.Instance.CurrentDay.AddKill(1);
        UI_EventBox.AddEventText($"��{Name_KR} (��)�� {PlacementInfo.Place_Floor.Name_KR}���� ������");
        GameManager.NPC.InactiveNPC(this);

        EventManager.Instance.RemoveQuestAction((int)Hunter);
        switch (Hunter)
        {
            case HunterType.Slime:
                //? ���� ��ȭ
                Debug.Log("������ ��~~ȭ ���� �׼� �����ϸ��");
                break;

            case HunterType.Skeleton:
                //? ���� ��ȭ
                Debug.Log("���̷��� ��~~ȭ");
                break;
        }
    }
    protected override void NPC_Captive()
    {

    }
}
