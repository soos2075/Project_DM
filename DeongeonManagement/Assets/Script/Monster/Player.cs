using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Monster
{
    public override SO_Monster Data { get; set; }
    public override void MonsterInit()
    {
        PlacementType = PlacementType.Monster;

        Data = GameManager.Monster.GetData("Player_lv1");
        Initialize_Status();
        LV = 1;
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
                Data = GameManager.Monster.GetData("Player_lv1");
                break;

            case 2:
                Data = GameManager.Monster.GetData("Player_lv2");
                break;

            case 3:
                Data = GameManager.Monster.GetData("Player_lv3");
                break;
        }
        Initialize_Status();
        LV = dungeonLV;
    }

}
