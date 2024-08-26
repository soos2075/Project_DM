using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HerbFarm : Technical
{

    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }


    public override void Init()
    {
        SetLevel();

        Cycle = 2;
        MainAction = (turn) => { MainEvent(turn); };

        AddTurnEvent(MainAction, DayType.Day);
    }

    public override void RemoveTechnical()
    {
        RemoveTurnEvent(MainAction, DayType.Day);
    }


    protected override void MainEvent(int turn)
    {
        int day = turn - InstanceDate;

        if (day % Cycle == 1) //? 1인 이유는 만들고나서 다음턴에 발동하고 그 다음부터 사이클로 돌아가는게 맞는 것 같음.
        {
            // 실제 실행할 로직
            Debug.Log("꽃밭 이벤트");
            CreateHerb();
        }
    }


    string high_10;
    string middle_20;
    string normal_60;


    void SetLevel()
    {
        var sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.sprite = Managers.Sprite.Get_Technical(Data.spritePath);

        switch (Data.techLv)
        {
            case 1:
                high_10 = "Herb_High";
                middle_20 = "Herb_Radish";
                normal_60 = "Herb_Low";
                break;

            case 2:
                high_10 = "Herb_Spinach";
                middle_20 = "Herb_High";
                normal_60 = "Herb_Bracken";
                break;

            case 3:
                high_10 = "Herb_Aloe";
                middle_20 = "Herb_Fatsia";
                normal_60 = "Herb_Radish";
                break;

            default:
                high_10 = "Herb_High";
                middle_20 = "Herb_Radish";
                normal_60 = "Herb_Low";
                break;
        }
    }



    void CreateHerb()
    {
        float ranValue;


        for (int i = 0; i < Main.Instance.ActiveFloor_Basement; i++)
        {
            if (i == 3)
            {
                continue;
            }

            for (int k = 0; k < 3; k++)
            {
                ranValue = UnityEngine.Random.value;

                bool isFind;
                var tile = Main.Instance.Floor[i].GetRandomTile(out isFind);
                //? 100번 돌동안 빈공간 못찾았으면 그냥 스킵
                if (isFind == false) continue;


                var info = new PlacementInfo(Main.Instance.Floor[i], tile);

                if (ranValue > 0.9f)
                {
                    GameManager.Facility.CreateFacility(high_10, info);
                }
                else if (ranValue > 0.6f)
                {
                    GameManager.Facility.CreateFacility(middle_20, info);
                }
                else
                {
                    GameManager.Facility.CreateFacility(normal_60, info);
                }
            }
        }
    }


}
