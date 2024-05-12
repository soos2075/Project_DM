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

        if (day % Cycle == 0)
        {
            // 실제 실행할 로직
            Debug.Log("꽃밭 이벤트");
            CreateHerb();
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
                    GameManager.Facility.CreateFacility("Herb_High", info);
                }
                else if (ranValue > 0.6f)
                {
                    GameManager.Facility.CreateFacility("Herb_Radish", info);
                }
                else
                {
                    GameManager.Facility.CreateFacility("Herb_Low", info);
                }
            }
        }
    }


}
