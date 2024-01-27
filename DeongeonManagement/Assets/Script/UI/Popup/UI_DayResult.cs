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


        GetTMP(((int)Texts.Mana)).text = $"오늘 얻은 마나 : {Main.Instance.CurrentDay.Mana}";
        GetTMP(((int)Texts.Gold)).text = $"오늘 얻은 골드 : {Main.Instance.CurrentDay.Gold}";
        GetTMP(((int)Texts.Prisoner)).text = $"오늘 잡은 모험가 : {Main.Instance.CurrentDay.Prisoner}";
        GetTMP(((int)Texts.Kill)).text = $"오늘 물리친 모험가 : {Main.Instance.CurrentDay.Kill}";
    }



}
