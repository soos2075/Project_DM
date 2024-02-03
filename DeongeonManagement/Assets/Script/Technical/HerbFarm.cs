using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HerbFarm : Technical
{
    public override void Init()
    {
        Date = Main.Instance.Turn;
        Cycle = 2;

        AddTurnEvent((turn) => MainEvent(turn), DayType.Day);

        //FindObjectOfType<UI_Technical>().CurrentTechnical = this;
    }


    int Date { get; set; }
    int Cycle { get; set; }



    protected override void MainEvent(int turn)
    {
        int day = turn - Date;

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


        for (int i = 0; i < 3; i++)
        {
            ranValue = UnityEngine.Random.value;

            var tile = Main.Instance.Floor[i].GetRandomTile();
            var info = new PlacementInfo(Main.Instance.Floor[i], tile);

            if (ranValue > 0.7f)
            {
                GameManager.Facility.CreateFacility("Herb_High", info);
            }
            else
            {
                GameManager.Facility.CreateFacility("Herb_Low", info);
            }
        }
    }

}
