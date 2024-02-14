using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonationBox : Technical
{
    public override void Init()
    {
        //Date = Main.Instance.Turn;
        Cycle = 1;

        action = (turn) => MainEvent(turn);
        AddTurnEvent(action, DayType.Night);

        //FindObjectOfType<UI_Technical>().CurrentTechnical = this;
    }


    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }

    Action<int> action;


    public override void RemoveTechnical()
    {
        RemoveTurnEvent(action, DayType.Night);
        //Debug.Log("입구에 코인박스도 삭제해야함");
        Destroy(GameObject.Find("DonationBox_Entrance"));
    }
    protected override void MainEvent(int turn)
    {
        GetGold();
    }

    void GetGold()
    {
        int people = GameManager.NPC.Current_NPCList.Count;

        Main.Instance.CurrentDay.AddGold(people * 5);
        Debug.Log($"입장료 획득 : {people * 5}");
    }



}
