using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prison : Technical
{
    public override void Init()
    {
        //Date = Main.Instance.Turn;
        Cycle = 1;

        //AddTurnEvent((turn) => MainEvent(turn), DayType.Night);

        //FindObjectOfType<UI_Technical>().CurrentTechnical = this;
    }


    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }



    protected override void MainEvent(int turn)
    {
        
    }
    public override void RemoveTechnical()
    {
        GameManager.Technical.Prison = null;
    }

    //? �� ���� �̺�Ʈ�� ���� �׳� Ŭ������ ���縸 �ϸ� ��.


}
