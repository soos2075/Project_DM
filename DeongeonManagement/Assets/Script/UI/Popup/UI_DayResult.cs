using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DayResult : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Texts
    {
        Mana,
        Gold,
        Prisoner,
        Kill,
        Final,
    }



    public override void Init()
    {
        base.Init();

        Bind<TextMeshProUGUI>(typeof(Texts));


        GetTMP(((int)Texts.Mana)).text = $"���� ���� ���� : {Main.Instance.CurrentDay.Mana}";
        GetTMP(((int)Texts.Gold)).text = $"���� ���� ��� : {Main.Instance.CurrentDay.Gold}";
        GetTMP(((int)Texts.Prisoner)).text = $"���� ���� ���谡 : {Main.Instance.CurrentDay.Prisoner}";
        GetTMP(((int)Texts.Kill)).text = $"���� ����ģ ���谡 : {Main.Instance.CurrentDay.Kill}";
    }



}
