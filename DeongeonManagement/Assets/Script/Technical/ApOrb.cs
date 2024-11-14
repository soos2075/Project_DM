using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApOrb : Technical
{
    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }


    //? 그냥 Main에서 ArOrb 체크만 해서 +1하는걸로 변경함

    public override void Init()
    {
        //MainAction = (turn) => { MainEvent(turn); };

        //AddTurnEvent(MainAction, DayType.Night);
    }


    public override void RemoveTechnical()
    {
        //RemoveTurnEvent(MainAction, DayType.Night);
    }

    protected override void MainEvent(int day)
    {
        //Main.Instance.Player_AP++;
    }



}
