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
    }

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Panel));

        GetImage(((int)Panel.Panel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);


        Bind<TextMeshProUGUI>(typeof(Texts));


        GetTMP(((int)Texts.Mana)).text = $"얻은 마나 : {Util.SetTextColorTag(Result.Get_Mana.ToString(), Define.TextColor.LightGreen).SetTextColorTag(Define.TextColor.Bold) }";
        GetTMP(((int)Texts.Gold)).text = $"얻은 골드 : {Result.Get_Gold.ToString().SetTextColorTag(Define.TextColor.LightGreen).SetTextColorTag(Define.TextColor.Bold)}";
        GetTMP(((int)Texts.Prisoner)).text = $"잡은 포로 : {Util.SetTextColorTag(Result.Get_Prisoner.ToString(), Define.TextColor.Bold)}";
        GetTMP(((int)Texts.Kill)).text = $"물리친 모험가 : {Util.SetTextColorTag(Result.Get_Kill.ToString(), Define.TextColor.Bold)}";

        GetTMP(((int)Texts.Use_Mana)).text = $"사용한 마나 : {Result.Use_Mana.ToString().SetTextColorTag(Define.TextColor.npc_red).SetTextColorTag(Define.TextColor.Bold)}";
        GetTMP(((int)Texts.Use_Gold)).text = $"사용한 골드 : {Result.Use_Gold.ToString().SetTextColorTag(Define.TextColor.npc_red).SetTextColorTag(Define.TextColor.Bold)}";

        GetTMP(((int)Texts.Monster_LvUp)).text = $"레벨업한 몬스터 : {Util.SetTextColorTag(GameManager.Monster.LevelUpList.Count.ToString(), Define.TextColor.Bold)}";
        GetTMP(((int)Texts.Monster_Injury)).text = $"부상당한 몬스터 : {Util.SetTextColorTag(GameManager.Monster.InjuryMonster.ToString(), Define.TextColor.Bold)}";

        GetTMP(((int)Texts.Fame)).text = $"유명도 : {Util.SetTextColorTag(Result.fame_perv.ToString(), Define.TextColor.Bold)} → " +
            $"{Util.SetTextColorTag(Main.Instance.PopularityOfDungeon.ToString(), Define.TextColor.Bold)}";

        GetTMP(((int)Texts.Danger)).text = $"위험도 : {Util.SetTextColorTag(Result.danger_perv.ToString(), Define.TextColor.Bold)} → " +
            $"{Util.SetTextColorTag(Main.Instance.DangerOfDungeon.ToString(), Define.TextColor.Bold)}";


        if (Result.dungeonRank != Main.Instance.DungeonRank)
        {
            GetTMP(((int)Texts.DungeonLv)).text = $"던전 랭크 : {Result.dungeonRank} → {Main.Instance.DungeonRank}";
            GetTMP(((int)Texts.DungeonLv)).text += "   UP!".SetTextColorTag(Define.TextColor.red).SetTextColorTag(Define.TextColor.Bold);
        }
        else
        {
            GetTMP(((int)Texts.DungeonLv)).text = $"던전 랭크 : {Main.Instance.DungeonRank}";
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
            $"마나 : {Result.Origin_Mana} {manaResult_Text} = {Main.Instance.Player_Mana.ToString().SetTextColorTag(Define.TextColor.LightYellow)}\n" +
            $"골드 : {Result.Origin_Gold} {goldResult_Text} = {Main.Instance.Player_Gold.ToString().SetTextColorTag(Define.TextColor.LightYellow)}";
    }



    Main.DayResult Result;

    public void TextContents(Main.DayResult data)
    {
        Result = data;
    }



}
