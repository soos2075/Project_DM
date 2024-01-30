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
        Name_KR = "������";
        Detail = "2�ϸ��� �ֺ��� ���� ���ʸ� �������ݴϴ�. ���� ���� ���ʰ� ���ö��� �־��!";

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
            // ���� ������ ����
            Debug.Log("�ɹ� �̺�Ʈ");
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
