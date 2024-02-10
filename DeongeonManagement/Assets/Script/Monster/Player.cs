using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Monster
{
    public override MonsterType Type { get; set; }
    public override MonsterData Data { get; set; }

    public override void MonsterInit()
    {
        Type = MonsterType.Normal_Fixed;
        PlacementType = Define.PlacementType.Monster;
        //Data = GameManager.Monster.GetMonsterData("Skeleton");


    }

    public void Level_Stat(int dungeonLV)
    {
        switch (dungeonLV)
        {
            case 1:
                LV = 1;
                HP = 30;
                HP_Max = 30;

                ATK = 10;
                DEF = 5;
                AGI = 5;
                LUK = 5;

                break;

            case 2:
                LV = 2;
                HP = 50;
                HP_Max = 50;

                ATK = 20;
                DEF = 10;
                AGI = 10;
                LUK = 10;
                break;
        }
    }

}
