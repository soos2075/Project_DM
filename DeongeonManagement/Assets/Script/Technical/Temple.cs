using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temple : Technical
{
    public override void Init()
    {
        Cycle = 1;
    }


    public override int InstanceDate { get; set; }
    public override int Cycle { get; set; }



    protected override void MainEvent(int turn)
    {

    }
    public override void RemoveTechnical()
    {

    }


    public void Pray_Mana()
    {
        //? 마나 1000 기부완료, 변수값 하나 저장해야함
        Main.Instance.CurrentDay.AddPop_Directly(50);
        var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
        msg.Message = $"{50} {UserData.Instance.LocaleText("Message_Get_Pop")}";

        InteractionCounter++;
        if (InteractionCounter == 5)
        {
            Hero_Root();
        }
    }



    public void Hero_Root()
    {
        //? 만약 악마상이 하나라도 있으면 리턴
        if (GameManager.Facility.GetFacilityCount<Devil_Statue>() != 0)
        {
            return;
        }

        Debug.Log("용사엔딩 진입");
        Main.Instance.ChangeEggState();
    }

}
