using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Monster
{
    public override MonsterType Type { get; set; }
    public override MonsterData Data { get; set; }

    public override string Detail_KR { get { return detail; } }
    private string detail;
    public override void MonsterInit()
    {
        Type = MonsterType.Normal_Fixed;
        PlacementType = Define.PlacementType.Monster;
        //Data = GameManager.Monster.GetMonsterData("Skeleton");
        detail = "모험이 끝난 모험가입니다. 모험가로서의 지식은 상당한 편이에요.";
    }

    public void Level_Stat(int dungeonLV)
    {
        switch (dungeonLV)
        {
            case 1:
                LV = 1;
                HP = 25;
                HP_Max = 25;

                ATK = 6;
                DEF = 4;
                AGI = 4;
                LUK = 4;

                break;

            case 2:
                LV = 2;
                HP = 50;
                HP_Max = 50;

                ATK = 12;
                DEF = 8;
                AGI = 8;
                LUK = 8;
                break;


            case 3:
                LV = 3;
                HP = 75;
                HP_Max = 75;

                ATK = 18;
                DEF = 12;
                AGI = 12;
                LUK = 12;
                break;
        }
    }

}
