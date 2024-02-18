using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApOrb : Technical
{
    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }



    public override void Init()
    {
        Main.Instance.AddAP();
    }


    public override void RemoveTechnical()
    {
        Main.Instance.SubtractAP();
    }

    protected override void MainEvent(int day)
    {
        
    }



}
