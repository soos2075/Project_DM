using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierOfSealing : Technical
{
    #region Defalut Technical
    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }

    public override void Init()
    {
        Cycle = 1;

        if (EventManager.Instance.CurrentClearEventData.Check_AlreadyClear_Quest(1141)) //? 1141����Ʈ�� ���ӵǴ� ��� ����Ʈ / 771141�� ��忡�� ��ȭ�ؾ� �߰���
        {
            Set_Seal();
        }
    }
    public override void RemoveTechnical()
    {
        RemoveTurnEvent(MainAction, DayType.Day);
    }
    protected override void MainEvent(int turn)
    {
        Debug.Log("���μ��� �̺�Ʈ");
        TechnicalActionEvent();
    }
    void TechnicalActionEvent()
    {
        int value = Random.Range(15, 31);
        int mana = Random.Range(100, 201);

        if (Main.Instance.DangerOfDungeon > Main.Instance.PopularityOfDungeon)
        {
            Main.Instance.CurrentDay.AddDanger(value);
            Main.Instance.ShowDM(value, Main.TextType.danger, transform, 2);
        }
        else
        {
            Main.Instance.CurrentDay.AddPop(value);
            Main.Instance.ShowDM(value, Main.TextType.pop, transform, 2);
        }


        Main.Instance.CurrentDay.AddMana(mana, Main.DayResult.EventType.Technical);
        Main.Instance.ShowDM(mana, Main.TextType.mana, transform, 2);
    }
    #endregion




    public Sprite sprite_Active;
    public Sprite sprite_Inactive;
    //bool isSealing;

    public void Set_Seal()
    {
        Data = GameManager.Technical.GetData("BarrierOfSealing_Active");

        //isSealing = true;
        GetComponent<SpriteRenderer>().sprite = sprite_Active;

        MainAction = (turn) => { MainEvent(turn); };
        AddTurnEvent(MainAction, DayType.Day);
    }





}
