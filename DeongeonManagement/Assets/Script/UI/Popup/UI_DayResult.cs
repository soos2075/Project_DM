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

        Use_Mana,
        Use_Gold,

        Fame,
        Danger,
        DungeonLv,

        Monster_LvUp,
        Monster_Injury,

        Final,
    }

    enum Panel
    {
        Panel,
        ClosePanel,
    }

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Panel));


        StartCoroutine(CloseActionDelay());
        //GetImage(((int)Panel.Panel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        //GetImage(((int)Panel.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);


        Bind<TextMeshProUGUI>(typeof(Texts));


        GetTMP(((int)Texts.Mana)).text = $"+ {UserData.Instance.GetLocaleText("Mana")} : {Util.SetTextColorTag(Result.Get_Mana.ToString(), Define.TextColor.LightGreen).SetTextColorTag(Define.TextColor.Bold) }";
        GetTMP(((int)Texts.Gold)).text = $"+ {UserData.Instance.GetLocaleText("Gold")} : {Result.Get_Gold.ToString().SetTextColorTag(Define.TextColor.LightGreen).SetTextColorTag(Define.TextColor.Bold)}";
        GetTMP(((int)Texts.Prisoner)).text = $"+ {UserData.Instance.GetLocaleText("포로")} : {Util.SetTextColorTag(Result.Get_Prisoner.ToString(), Define.TextColor.Bold)}";
        GetTMP(((int)Texts.Kill)).text = $"+ {UserData.Instance.GetLocaleText("모험가")} : {Util.SetTextColorTag(Result.Get_Kill.ToString(), Define.TextColor.Bold)}";

        GetTMP(((int)Texts.Use_Mana)).text = $"- {UserData.Instance.GetLocaleText("Mana")} : {Result.Use_Mana.ToString().SetTextColorTag(Define.TextColor.npc_red).SetTextColorTag(Define.TextColor.Bold)}";
        GetTMP(((int)Texts.Use_Gold)).text = $"- {UserData.Instance.GetLocaleText("Gold")} : {Result.Use_Gold.ToString().SetTextColorTag(Define.TextColor.npc_red).SetTextColorTag(Define.TextColor.Bold)}";

        GetTMP(((int)Texts.Monster_LvUp)).text = $"{UserData.Instance.GetLocaleText("Up")} {UserData.Instance.GetLocaleText("몬스터")} : {Util.SetTextColorTag(GameManager.Monster.LevelUpList.Count.ToString(), Define.TextColor.Bold)}";
        GetTMP(((int)Texts.Monster_Injury)).text = $"{UserData.Instance.GetLocaleText("Weak")} {UserData.Instance.GetLocaleText("몬스터")} : {Util.SetTextColorTag(GameManager.Monster.InjuryMonster.ToString(), Define.TextColor.Bold)}";

        GetTMP(((int)Texts.Fame)).text = $"{UserData.Instance.GetLocaleText("Popularity")} : {Util.SetTextColorTag(Result.fame_perv.ToString(), Define.TextColor.Bold)} → " +
            $"{Util.SetTextColorTag(Main.Instance.PopularityOfDungeon.ToString(), Define.TextColor.Bold)}";

        GetTMP(((int)Texts.Danger)).text = $"{UserData.Instance.GetLocaleText("Danger")} : {Util.SetTextColorTag(Result.danger_perv.ToString(), Define.TextColor.Bold)} → " +
            $"{Util.SetTextColorTag(Main.Instance.DangerOfDungeon.ToString(), Define.TextColor.Bold)}";


        if (Result.dungeonRank != Main.Instance.DungeonRank)
        {
            GetTMP(((int)Texts.DungeonLv)).text = $"{UserData.Instance.GetLocaleText("Rank")} : {Result.dungeonRank} → {Main.Instance.DungeonRank}";
            GetTMP(((int)Texts.DungeonLv)).text += "   UP!".SetTextColorTag(Define.TextColor.red).SetTextColorTag(Define.TextColor.Bold);
        }
        else
        {
            GetTMP(((int)Texts.DungeonLv)).text = $"{UserData.Instance.GetLocaleText("Rank")} : <b>{Main.Instance.DungeonRank}</b>";
        }


        int manaResult = Result.Get_Mana - Result.Use_Mana;
        string manaResult_Text;
        if (manaResult >= 0)
        {
            manaResult_Text = $"+ {manaResult}".ToString().SetTextColorTag(Define.TextColor.LightGreen);
        }
        else
        {
            manaResult_Text = $"- {Mathf.Abs(manaResult)}".ToString().SetTextColorTag(Define.TextColor.npc_red);
        }

        int goldResult = Result.Get_Gold - Result.Use_Gold;
        string goldResult_Text;
        if (goldResult >= 0)
        {
            goldResult_Text = $"+ {goldResult}".ToString().SetTextColorTag(Define.TextColor.LightGreen);
        }
        else
        {
            goldResult_Text = $"- {Mathf.Abs(goldResult)}".ToString().SetTextColorTag(Define.TextColor.npc_red);
        }

        GetTMP(((int)Texts.Final)).text =
            $"{UserData.Instance.GetLocaleText("Mana")} : <b>{Result.Origin_Mana} {manaResult_Text} = {Main.Instance.Player_Mana.ToString().SetTextColorTag(Define.TextColor.LightYellow)}</b>\n" +
            $"{UserData.Instance.GetLocaleText("Gold")} : <b>{Result.Origin_Gold} {goldResult_Text} = {Main.Instance.Player_Gold.ToString().SetTextColorTag(Define.TextColor.LightYellow)}</b>";
    }



    Main.DayResult Result;

    public void TextContents(Main.DayResult data)
    {
        Result = data;
    }



    IEnumerator CloseActionDelay() //? 1초정도 딜레이 있다가 꺼지도록(바로꺼지는거 방지)
    {
        yield return new WaitForSecondsRealtime(1);
        GetImage(((int)Panel.Panel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        GetImage(((int)Panel.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
    }


}
