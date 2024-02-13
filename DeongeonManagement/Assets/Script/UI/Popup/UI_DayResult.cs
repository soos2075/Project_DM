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


        GetTMP(((int)Texts.Mana)).text = $"얻은 마나 : {Util.SetTextColorTag(Result.Get_Mana.ToString(), Define.TextColor.LightGreen) }";
        GetTMP(((int)Texts.Gold)).text = $"얻은 골드 : {Result.Get_Gold.ToString().SetTextColorTag(Define.TextColor.LightGreen)}";
        GetTMP(((int)Texts.Prisoner)).text = $"잡은 포로 : {Result.Get_Prisoner}";
        GetTMP(((int)Texts.Kill)).text = $"물리친 모험가 : {Result.Get_Kill}";

        GetTMP(((int)Texts.Use_Mana)).text = $"사용한 마나 : {Result.Use_Mana.ToString().SetTextColorTag(Define.TextColor.npc_red)}";
        GetTMP(((int)Texts.Use_Gold)).text = $"사용한 골드 : {Result.Use_Gold.ToString().SetTextColorTag(Define.TextColor.npc_red)}";

        GetTMP(((int)Texts.Monster_LvUp)).text = $"레벨업한 몬스터 : {Result.Monster_LvUP}";
        GetTMP(((int)Texts.Monster_Injury)).text = $"부상당한 몬스터 : {Result.Monster_Injury}";

        GetTMP(((int)Texts.Fame)).text = $"유명도 : {Result.fame_perv} → {Main.Instance.FameOfDungeon}";
        GetTMP(((int)Texts.Danger)).text = $"위험도 : {Result.danger_perv} → {Main.Instance.DangerOfDungeon}";

        if (rankup)
        {
            GetTMP(((int)Texts.DungeonLv)).text = $"던전 등급 : {Main.Instance.DungeonRank - 1} → {Main.Instance.DungeonRank}";
        }
        else
        {
            GetTMP(((int)Texts.DungeonLv)).text = $"던전 등급 : {Main.Instance.DungeonRank}";
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
    bool rankup;

    public void TextContents(Main.DayResult data)
    {
        Result = data;
    }

    public void RankUpResult(bool _result)
    {
        rankup = _result;
    }





}
