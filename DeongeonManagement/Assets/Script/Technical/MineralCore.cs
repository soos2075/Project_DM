using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralCore : Technical
{
    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }


    public override void Init()
    {
        SetLevel();

        Cycle = 3;
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

        if (day % Cycle == 1) //? 1�� ������ ������� �����Ͽ� �ߵ��ϰ� �� �������� ����Ŭ�� ���ư��°� �´� �� ����.
        {
            Debug.Log("���� �̺�Ʈ");
            CreateAction();
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
                high_10 = "Mineral_Diamond";
                middle_20 = "Mineral_Stone";
                normal_60 = "Mineral_Rock";
                break;

            case 2:
                high_10 = "Mineral_Rune";
                middle_20 = "Mineral_Ruby";
                normal_60 = "Mineral_Coal";
                break;

            case 3:
                high_10 = "Mineral_Iron";
                middle_20 = "Mineral_Coal";
                normal_60 = "Mineral_Crystal";
                break;



            default:
                high_10 = "Mineral_Diamond";
                middle_20 = "Mineral_Stone";
                normal_60 = "Mineral_Rock";
                break;
        }
    }



    void CreateAction()
    {
        float ranValue;

        for (int i = 1; i < Main.Instance.ActiveFloor_Basement; i++)
        {
            for (int k = 0; k < 3; k++)
            {
                ranValue = UnityEngine.Random.value;

                bool isFind;
                var tile = Main.Instance.Floor[i].GetRandomTile(out isFind);
                //? 100�� ������ ����� ��ã������ �׳� ��ŵ
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
