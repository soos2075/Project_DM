using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Monster
{
    public override MonsterData Data { get; set; }

    public override string Detail_KR { get { return detail; } }
    private string detail;
    public override void MonsterInit()
    {
        PlacementType = PlacementType.Monster;

        Name = "�ʺ��� ���� ������";
        detail = "���跹���� ��� ���� ������ ����� ���̿���.";

        Data = new MonsterData();
        Data.Battle_AP = 2;
        Data.Battle_Interval = 0;
}

    public override void TurnStart()
    {
        //MoveSelf();
    }

    public override void MoveSelf()
    {
        //Cor_Moving = StartCoroutine(MoveCor());
    }



    public void Level_Stat(int dungeonLV)
    {
        switch (dungeonLV)
        {
            case 1:
                LV = 1;
                HP = 50;
                HP_Max = 50;

                ATK = 12;
                DEF = 4;
                AGI = 4;
                LUK = 4;
                break;

            case 2:
                LV = 2;
                HP = 100;
                HP_Max = 100;

                ATK = 24;
                DEF = 8;
                AGI = 8;
                LUK = 8;
                break;


            case 3:
                LV = 3;
                HP = 150;
                HP_Max = 150;

                ATK = 36;
                DEF = 12;
                AGI = 12;
                LUK = 12;
                break;
        }
    }

}
