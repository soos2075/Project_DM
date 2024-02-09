using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        Use_Mana,
        Use_Gold,
    }

    enum Panel
    {
        Panel,
    }

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Panel));

        GetImage(((int)Panel.Panel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);


        Bind<TextMeshProUGUI>(typeof(Texts));


        GetTMP(((int)Texts.Mana)).text = $"���� ���� ���� : {Util.SetTextColorTag(Result.Get_Mana.ToString(), Define.TextColor.green) }";
        GetTMP(((int)Texts.Gold)).text = $"���� ���� ��� : {Result.Get_Gold.ToString().SetTextColorTag(Define.TextColor.green)}";
        GetTMP(((int)Texts.Prisoner)).text = $"���� ���� ���� : {Result.Get_Prisoner}";
        GetTMP(((int)Texts.Kill)).text = $"���� ����ģ ���谡 : {Result.Get_Kill}";

        GetTMP(((int)Texts.Use_Mana)).text = $"����� ���� : {Result.Use_Mana.ToString().SetTextColorTag(Define.TextColor.red)}";
        GetTMP(((int)Texts.Use_Gold)).text = $"����� ��� : {Result.Use_Gold.ToString().SetTextColorTag(Define.TextColor.red)}";


        int manaResult = Result.Get_Mana - Result.Use_Mana;
        string manaResult_Text;
        if (manaResult >= 0)
        {
            manaResult_Text = $"+ {manaResult}".ToString().SetTextColorTag(Define.TextColor.green);
        }
        else
        {
            manaResult_Text = $"- {Mathf.Abs(manaResult)}".ToString().SetTextColorTag(Define.TextColor.red);
        }

        int goldResult = Result.Get_Gold - Result.Use_Gold;
        string goldResult_Text;
        if (goldResult >= 0)
        {
            goldResult_Text = $"+ {goldResult}".ToString().SetTextColorTag(Define.TextColor.green);
        }
        else
        {
            goldResult_Text = $"- {Mathf.Abs(goldResult)}".ToString().SetTextColorTag(Define.TextColor.red);
        }

        string goodjob = $"���߾��!".SetTextColorTag(Define.TextColor.blue);

        GetTMP(((int)Texts.Final)).text = 
            $"���� : {Result.Origin_Mana} {manaResult_Text} = {Main.Instance.Player_Mana.ToString().SetTextColorTag(Define.TextColor.yellow)}\n" +
            $"��� : {Result.Origin_Gold} {goldResult_Text} = {Main.Instance.Player_Gold.ToString().SetTextColorTag(Define.TextColor.yellow)}\n" +
            $"�� : {goodjob}";
    }



    Main.DayResult Result;

    public void TextContents(Main.DayResult data)
    {
        Result = data;
    }






}
