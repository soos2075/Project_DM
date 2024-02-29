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
            // ���� ������ ����
            Debug.Log("�ɹ� �̺�Ʈ");
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

                var tile = Main.Instance.Floor[i].GetRandomTile();
                var info = new PlacementInfo(Main.Instance.Floor[i], tile);

                if (ranValue > 0.9f)
                {
                    GameManager.Facility.CreateFacility("Herb", info, (int)Herb.HerbType.High);
                }
                else if (ranValue > 0.6f)
                {
                    GameManager.Facility.CreateFacility("Herb", info, (int)Herb.HerbType.Pumpkin);
                }
                else
                {
                    GameManager.Facility.CreateFacility("Herb", info, (int)Herb.HerbType.Low);
                }
            }
        }
    }


}
