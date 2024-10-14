using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : Technical
{
    public override void Init()
    {
        Cycle = 1;
    }


    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }



    protected override void MainEvent(int turn)
    {

    }
    public override void RemoveTechnical()
    {
        
    }


}
