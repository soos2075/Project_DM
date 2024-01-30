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
        Name_KR = "허브농장";
        Detail = "2일마다 주변의 층에 약초를 공급해줍니다. 가끔 좋은 약초가 나올때도 있어요!";

        Date = Main.Instance.Turn;
        Cycle = 2;

        AddTurnEvent((turn) => MainEvent(turn), DayType.Day);

        //FindObjectOfType<UI_Technical>().CurrentTechnical = this;
    }


    int Date { get; set; }
    int Cycle { get; set; }
    public override string Name_KR { get; set; }
    public override string Detail { get; set; }

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
            IPlacementable obj;

            if (ranValue > 0.7f)
            {
                obj = Managers.Placement.CreatePlacementObject("Facility/Herb_High", null, Define.PlacementType.Facility);
            }
            else
            {
                obj = Managers.Placement.CreatePlacementObject("Facility/Herb_Low", null, Define.PlacementType.Facility);
            }

            var pos = Main.Instance.Floor[i].GetRandomTile(obj);
            var info = new PlacementInfo(Main.Instance.Floor[i], pos);

            Managers.Placement.PlacementConfirm(obj, info);
        }
        
    }


}
