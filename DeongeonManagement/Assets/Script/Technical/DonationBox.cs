using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonationBox : Technical
{
    public override void Init()
    {
        Cycle = 1;

        //action = (turn) => MainEvent(turn);
        //AddTurnEvent(action, DayType.Night);

        var pos = Managers.Resource.Instantiate($"Technical/DonationBox_Entrance");
        GameManager.Technical.Donation_Pos = pos.transform;
    }


    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }

    Action<int> action;


    public override void RemoveTechnical()
    {
        //RemoveTurnEvent(action, DayType.Night);
        GameManager.Technical.Donation = null;
        Destroy(GameManager.Technical.Donation_Pos.gameObject);
        GameManager.Technical.Donation_Pos = null;
    }
    protected override void MainEvent(int turn)
    {

    }




}
