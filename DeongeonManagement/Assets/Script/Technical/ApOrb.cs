using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApOrb : Technical
{
    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }



    public override void Init()
    {
        MainAction = (turn) => { MainEvent(turn); };

        AddTurnEvent(MainAction, DayType.Night);
    }


    public override void RemoveTechnical()
    {
        RemoveTurnEvent(MainAction, DayType.Night);
    }

    protected override void MainEvent(int day)
    {
        Main.Instance.Player_AP++;
    }



}
