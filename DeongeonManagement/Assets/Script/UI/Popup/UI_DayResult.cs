using System;
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


    enum ResultText
    {
        //? 마나
        Mana_Get_Facility,
        Mana_Get_Artifacts,
        Mana_Get_Monster,
        Mana_Get_Etc,
        Mana_Get_Bonus,

        Mana_Use_Facility,
        Mana_Use_Monster,
        Mana_Use_Etc,

        Mana_Result_Get,
        Mana_Result_Use,
        Mana_Result,

        //? 골드
        Gold_Get_Facility,
        Gold_Get_Monster,
        Gold_Get_Etc,
        Gold_Get_Bonus,

        Gold_Use_Facility,
        Gold_Use_Monster,
        Gold_Use_Etc,

        Gold_Result_Get,
        Gold_Result_Use,
        Gold_Result,

        //? 모험가
        NPC_Visit,
        NPC_Satisfaction,
        NPC_NonSatisfaction,
        NPC_Empty,
        NPC_Runaway,
        NPC_Die,

        //? 몬스터
        Monster_Battle,
        Monster_Victory,
        Monster_Defeat,
        Monster_LvUp,
        Monster_Trait,
        Monster_Evolution,

        //? 던전
        Dungeon_Pop,
        Dungeon_Danger,
        Dungeon_Rank,
    }

    enum SubTitle
    {
        //? 항목이름 (GridLayoutGroup)
        SubTitle_Mana_Get,
        SubTitle_Mana_Use,
        SubTitle_Gold_Get,
        SubTitle_Gold_Use,
    }

    enum Etcs
    {
        Close,
        ClosePanel,
        MainPanel,
        Stamp,
    }

    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(ResultText));
        Bind<GameObject>(typeof(Etcs));
        Bind<GridLayoutGroup>(typeof(SubTitle));
        GetObject((int)Etcs.Stamp).SetActive(false);

        Wait_Delay = StartCoroutine(CloseActionDelay());

        Init_Result();
        Init_SubTitle();

        ShowResult();
    }


    void Init_Result()
    {
        for (int i = 0; i < Enum.GetNames(typeof(ResultText)).Length; i++)
        {
            GetTMP(i).text = "";
        }

        GetTMP((int)ResultText.Mana_Get_Facility).gameObject.SetActive(false);
        GetTMP((int)ResultText.Mana_Get_Artifacts).gameObject.SetActive(false);
        GetTMP((int)ResultText.Mana_Get_Monster).gameObject.SetActive(false);
        GetTMP((int)ResultText.Mana_Get_Etc).gameObject.SetActive(false);
        GetTMP((int)ResultText.Mana_Get_Bonus).gameObject.SetActive(false);

        GetTMP((int)ResultText.Mana_Use_Facility).gameObject.SetActive(false);
        GetTMP((int)ResultText.Mana_Use_Monster).gameObject.SetActive(false);
        GetTMP((int)ResultText.Mana_Use_Etc).gameObject.SetActive(false);



        GetTMP((int)ResultText.Gold_Get_Facility).gameObject.SetActive(false);
        GetTMP((int)ResultText.Gold_Get_Monster).gameObject.SetActive(false);
        GetTMP((int)ResultText.Gold_Get_Etc).gameObject.SetActive(false);
        GetTMP((int)ResultText.Gold_Get_Bonus).gameObject.SetActive(false);

        GetTMP((int)ResultText.Gold_Use_Facility).gameObject.SetActive(false);
        GetTMP((int)ResultText.Gold_Use_Monster).gameObject.SetActive(false);
        GetTMP((int)ResultText.Gold_Use_Etc).gameObject.SetActive(false);
    }

    void ShowResult()
    {
        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ마나
        if (Result.Mana_Get_Facility > 0)
        {
            Show_Contents($"{Result.Mana_Get_Facility}", ResultText.Mana_Get_Facility);
            Show_SubTitle(UserData.Instance.LocaleText("시설"), SubTitle.SubTitle_Mana_Get);
        }
        if (Result.Mana_Get_Artifacts > 0)
        {
            Show_Contents($"{Result.Mana_Get_Artifacts}", ResultText.Mana_Get_Artifacts);
            Show_SubTitle(UserData.Instance.LocaleText("아티팩트"), SubTitle.SubTitle_Mana_Get);
        }
        if (Result.Mana_Get_Monster > 0)
        {
            Show_Contents($"{Result.Mana_Get_Monster}", ResultText.Mana_Get_Monster);
            Show_SubTitle(UserData.Instance.LocaleText("전투"), SubTitle.SubTitle_Mana_Get);
        }
        if (Result.Mana_Get_Etc > 0)
        {
            Show_Contents($"{Result.Mana_Get_Etc}", ResultText.Mana_Get_Etc);
            Show_SubTitle(UserData.Instance.LocaleText("기타등등"), SubTitle.SubTitle_Mana_Get);
        }

        //? 마나 보너스 처리
        int temp_mana = Result.Mana_Get_Etc + Result.Mana_Get_Facility + Result.Mana_Get_Monster + Result.Mana_Get_Artifacts;
        float ratio = GameManager.Buff.CurrentBuff.Orb_blue > 0 ? 0.2f : 0;
        ratio += (GameManager.Buff.ManaBonus * 0.01f);
        if (ratio > 0)
        {
            int bonus = Mathf.RoundToInt(temp_mana * ratio);
            Debug.Log($"비율은{ratio} @@ 보너스는 {bonus}");

            Result.AddMana(bonus, Main.DayResult.EventType.ResultBonus);
            Show_Contents($"{Result.Mana_Get_Bonus}", ResultText.Mana_Get_Bonus);
            Show_SubTitle(UserData.Instance.LocaleText("보너스"), SubTitle.SubTitle_Mana_Get);
        }



        if (Result.Mana_Use_Facility > 0)
        {
            Show_Contents($"{Result.Mana_Use_Facility}", ResultText.Mana_Use_Facility);
            Show_SubTitle(UserData.Instance.LocaleText("시설 배치"), SubTitle.SubTitle_Mana_Use);
        }
        if (Result.Mana_Use_Monster > 0)
        {
            Show_Contents($"{Result.Mana_Use_Monster}", ResultText.Mana_Use_Monster);
            Show_SubTitle(UserData.Instance.LocaleText("유닛"), SubTitle.SubTitle_Mana_Use);
        }
        if (Result.Mana_Use_Etc > 0)
        {
            Show_Contents($"{Result.Mana_Use_Etc}", ResultText.Mana_Use_Etc);
            Show_SubTitle(UserData.Instance.LocaleText("기타등등"), SubTitle.SubTitle_Mana_Use);
        }

        int get_Mana = Result.Mana_Get_Etc + Result.Mana_Get_Facility + Result.Mana_Get_Monster + Result.Mana_Get_Artifacts + Result.Mana_Get_Bonus;
        int use_Mana = Result.Mana_Use_Etc + Result.Mana_Use_Facility + Result.Mana_Use_Monster;

        GetTMP((int)ResultText.Mana_Result_Get).text = $"{get_Mana}";
        GetTMP((int)ResultText.Mana_Result_Use).text = $"-{use_Mana}";
        GetTMP((int)ResultText.Mana_Result).text = $"{get_Mana - use_Mana}";




        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ골드
        if (Result.Gold_Get_Facility > 0)
        {
            Show_Contents($"{Result.Gold_Get_Facility}", ResultText.Gold_Get_Facility);
            Show_SubTitle(UserData.Instance.LocaleText("시설"), SubTitle.SubTitle_Gold_Get);
        }
        if (Result.Gold_Get_Monster > 0)
        {
            Show_Contents($"{Result.Gold_Get_Monster}", ResultText.Gold_Get_Monster);
            Show_SubTitle(UserData.Instance.LocaleText("전투"), SubTitle.SubTitle_Gold_Get);
        }
        if (Result.Gold_Get_Etc > 0)
        {
            Show_Contents($"{Result.Gold_Get_Etc}", ResultText.Gold_Get_Etc);
            Show_SubTitle(UserData.Instance.LocaleText("기타등등"), SubTitle.SubTitle_Gold_Get);
        }

        //? 골드 보너스 처리
        int temp_gold = Result.Gold_Get_Etc + Result.Gold_Get_Facility + Result.Gold_Get_Monster;
        //float ratio_gold = GameManager.Buff.CurrentBuff.Orb_blue > 0 ? 0.2f : 0;
        float ratio_gold = (GameManager.Buff.GoldBonus * 0.01f);
        if (ratio_gold > 0)
        {
            int bonus = Mathf.RoundToInt(temp_gold * ratio_gold);
            Debug.Log($"골드비율은{ratio} @@ 골드보너스는 {bonus}");

            Result.AddGold(bonus, Main.DayResult.EventType.ResultBonus);
            Show_Contents($"{Result.Gold_Get_Bonus}", ResultText.Gold_Get_Bonus);
            Show_SubTitle(UserData.Instance.LocaleText("보너스"), SubTitle.SubTitle_Gold_Get);
        }



        if (Result.Gold_Use_Facility > 0)
        {
            Show_Contents($"{Result.Gold_Use_Facility}", ResultText.Gold_Use_Facility);
            Show_SubTitle(UserData.Instance.LocaleText("시설 배치"), SubTitle.SubTitle_Gold_Use);
        }
        if (Result.Gold_Use_Monster > 0)
        {
            Show_Contents($"{Result.Gold_Use_Monster}", ResultText.Gold_Use_Monster);
            Show_SubTitle(UserData.Instance.LocaleText("유닛"), SubTitle.SubTitle_Gold_Use);
        }
        if (Result.Gold_Use_Etc > 0)
        {
            Show_Contents($"{Result.Gold_Use_Etc}", ResultText.Gold_Use_Etc);
            Show_SubTitle(UserData.Instance.LocaleText("기타등등"), SubTitle.SubTitle_Gold_Use);
        }

        int get_Gold = Result.Gold_Get_Etc + Result.Gold_Get_Facility + Result.Gold_Get_Monster + Result.Gold_Get_Bonus;
        int use_Gold = Result.Gold_Use_Etc + Result.Gold_Use_Facility + Result.Gold_Use_Monster;

        GetTMP((int)ResultText.Gold_Result_Get).text = $"{get_Gold}";
        GetTMP((int)ResultText.Gold_Result_Use).text = $"-{use_Gold}";
        GetTMP((int)ResultText.Gold_Result).text = $"{get_Gold - use_Gold}";


        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ모험가
        GetTMP((int)ResultText.NPC_Visit).text = $"{Result.NPC_Visit}";
        //GetTMP((int)ResultText.NPC_Prisoner).text = $"{Result.NPC_Visit}";
        GetTMP((int)ResultText.NPC_Die).text = $"{Result.NPC_Kill}";
        GetTMP((int)ResultText.NPC_Satisfaction).text = $"{Result.NPC_Satisfaction}";
        GetTMP((int)ResultText.NPC_NonSatisfaction).text = $"{Result.NPC_NonSatisfaction}";
        GetTMP((int)ResultText.NPC_Empty).text = $"{Result.NPC_Empty}";
        GetTMP((int)ResultText.NPC_Runaway).text = $"{Result.NPC_Runaway}";


        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ몬스터
        GetTMP((int)ResultText.Monster_Battle).text = $"{Result.Monster_Battle}";
        GetTMP((int)ResultText.Monster_Victory).text = $"{Result.Monster_Victory}";
        GetTMP((int)ResultText.Monster_Defeat).text = $"{Result.Monster_Defeat}";
        GetTMP((int)ResultText.Monster_LvUp).text = $"{Result.Monster_LvUp}";
        GetTMP((int)ResultText.Monster_Trait).text = $"{Result.Monster_Trait}";
        GetTMP((int)ResultText.Monster_Evolution).text = $"{Result.Monster_Evolution}";


        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ던전
        GetTMP((int)ResultText.Dungeon_Pop).text = $"{Result.Origin_Pop} → {Current.Origin_Pop}";
        GetTMP((int)ResultText.Dungeon_Danger).text = $"{Result.Origin_Danger} → {Current.Origin_Danger}";
        GetTMP((int)ResultText.Dungeon_Rank).text = $"{(Define.DungeonRank)Result.Origin_Rank}";

        if (Result.Origin_Rank < Current.Origin_Rank)
        {
            GetTMP((int)ResultText.Dungeon_Rank).text = $"{(Define.DungeonRank)Result.Origin_Rank} → {(Define.DungeonRank)Current.Origin_Rank}";
            GetObject((int)Etcs.Stamp).SetActive(true);
        }
    }

    void Show_Contents(string contents, ResultText title)
    {
        if (GetTMP((int)title).gameObject.activeInHierarchy == false)
        {
            GetTMP((int)title).gameObject.SetActive(true);
            GetTMP((int)title).text = contents;
        }
    }



    void Init_SubTitle()
    {
        for (int i = 0; i < Enum.GetNames(typeof(SubTitle)).Length; i++)
        {
            var sub = Get<GridLayoutGroup>(i);
            for (int j = 0; j < sub.transform.childCount; j++)
            {
                sub.transform.GetChild(j).GetComponent<TextMeshProUGUI>().text = "";
                sub.transform.GetChild(j).gameObject.SetActive(false);
            }
        }
    }
    void Show_SubTitle(string titleName, SubTitle title)
    {
        var sub = Get<GridLayoutGroup>((int)title);
        for (int i = 0; i < sub.transform.childCount; i++)
        {
            if (sub.transform.GetChild(i).gameObject.activeInHierarchy == false)
            {
                sub.transform.GetChild(i).gameObject.SetActive(true);
                sub.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = titleName;
                break;
            }
        }
    }




    Main.DayResult Result;

    Main.DayResult Current;
    public void TextContents(Main.DayResult data, Main.DayResult current)
    {
        Result = data;
        Current = current;
    }


    Coroutine Wait_Delay;
    IEnumerator CloseActionDelay() //? 1초정도 딜레이 있다가 꺼지도록(바로꺼지는거 방지)
    {
        yield return null;
        yield return UserData.Instance.Wait_GamePlay;
        yield return new WaitForSecondsRealtime(1);
        GetObject(((int)Etcs.Close)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        //GetObject(((int)Etcs.ClosePanel)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);

        Wait_Delay = null;
    }




    public override bool EscapeKeyAction()
    {
        if (Wait_Delay == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }


    //enum Texts
    //{
    //    Mana,
    //    Gold,
    //    Prisoner,
    //    Kill,

    //    Use_Mana,
    //    Use_Gold,

    //    Fame,
    //    Danger,
    //    DungeonLv,

    //    Monster_LvUp,
    //    Monster_Injury,

    //    Final,
    //}

    //enum Panel
    //{
    //    Panel,
    //    ClosePanel,
    //}

    //public void Init_Legacy()
    //{
    //    base.Init();
    //    Bind<Image>(typeof(Panel));


    //    Wait_Delay = StartCoroutine(CloseActionDelay());
    //    GetImage(((int)Panel.Panel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
    //    GetImage(((int)Panel.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);


    //    Bind<TextMeshProUGUI>(typeof(Texts));


    //    GetTMP(((int)Texts.Mana)).text = $"+ {UserData.Instance.LocaleText("Mana")} : " +
    //        $"{Result.Get_Mana.ToString().SetTextColorTag(Define.TextColor.Bold)}";

    //    if (GameManager.Buff.CurrentBuff.Orb_blue > 0)
    //    {
    //        int add = (int)(Result.Get_Mana * 0.2f);

    //        GetTMP(((int)Texts.Mana)).text += $" (+{add})".SetTextColorTag(Define.TextColor.SkyBlue);
    //        Result.AddMana(add);
    //    }

    //    GetTMP(((int)Texts.Gold)).text = $"+ {UserData.Instance.LocaleText("Gold")} : " +
    //        $"{Result.Get_Gold.ToString().SetTextColorTag(Define.TextColor.Bold)}";


    //    GetTMP(((int)Texts.Kill)).text = $"+ {UserData.Instance.LocaleText("모험가")} : {Util.SetTextColorTag(Result.Get_Kill.ToString(), Define.TextColor.Bold)}";
    //    GetTMP(((int)Texts.Prisoner)).text = $"+ {UserData.Instance.LocaleText("포로")} : {Util.SetTextColorTag(Result.Get_Prisoner.ToString(), Define.TextColor.Bold)}";
    //    GetTMP(((int)Texts.Kill)).text = "";
    //    GetTMP(((int)Texts.Prisoner)).text = "";

    //    GetTMP(((int)Texts.Use_Mana)).text = $"- {UserData.Instance.LocaleText("Mana")} : {Result.Use_Mana.ToString().SetTextColorTag(Define.TextColor.npc_red).SetTextColorTag(Define.TextColor.Bold)}";
    //    GetTMP(((int)Texts.Use_Gold)).text = $"- {UserData.Instance.LocaleText("Gold")} : {Result.Use_Gold.ToString().SetTextColorTag(Define.TextColor.npc_red).SetTextColorTag(Define.TextColor.Bold)}";

    //    GetTMP(((int)Texts.Monster_LvUp)).text = $"{UserData.Instance.LocaleText("Up")} {UserData.Instance.LocaleText("몬스터")} : {Util.SetTextColorTag(GameManager.Monster.LevelUpList.Count.ToString(), Define.TextColor.Bold)}";
    //    GetTMP(((int)Texts.Monster_Injury)).text = $"{UserData.Instance.LocaleText("Weak")} {UserData.Instance.LocaleText("몬스터")} : {Util.SetTextColorTag(GameManager.Monster.InjuryMonster.ToString(), Define.TextColor.Bold)}";

    //    GetTMP(((int)Texts.Fame)).text = $"{UserData.Instance.LocaleText("Popularity")} : {Util.SetTextColorTag(Result.Origin_Pop.ToString(), Define.TextColor.Bold)} → " +
    //        $"{Util.SetTextColorTag(Main.Instance.PopularityOfDungeon.ToString(), Define.TextColor.Bold)}";

    //    GetTMP(((int)Texts.Danger)).text = $"{UserData.Instance.LocaleText("Danger")} : {Util.SetTextColorTag(Result.Origin_Danger.ToString(), Define.TextColor.Bold)} → " +
    //        $"{Util.SetTextColorTag(Main.Instance.DangerOfDungeon.ToString(), Define.TextColor.Bold)}";


    //    if (Result.Origin_Rank != Main.Instance.DungeonRank)
    //    {
    //        GetTMP(((int)Texts.DungeonLv)).text = $"{UserData.Instance.LocaleText("Rank")} : {(Define.DungeonRank)Result.Origin_Rank} → {(Define.DungeonRank)Main.Instance.DungeonRank}";
    //        GetTMP(((int)Texts.DungeonLv)).text += "   UP!".SetTextColorTag(Define.TextColor.red).SetTextColorTag(Define.TextColor.Bold);
    //    }
    //    else
    //    {
    //        GetTMP(((int)Texts.DungeonLv)).text = $"{UserData.Instance.LocaleText("Rank")} : <b>{(Define.DungeonRank)Main.Instance.DungeonRank}</b>";
    //    }


    //    int manaResult = Result.Get_Mana - Result.Use_Mana;
    //    string manaResult_Text;
    //    if (manaResult >= 0)
    //    {
    //        manaResult_Text = $"+ {manaResult}".ToString().SetTextColorTag(Define.TextColor.LightGreen);
    //    }
    //    else
    //    {
    //        manaResult_Text = $"- {Mathf.Abs(manaResult)}".ToString().SetTextColorTag(Define.TextColor.npc_red);
    //    }

    //    int goldResult = Result.Get_Gold - Result.Use_Gold;
    //    string goldResult_Text;
    //    if (goldResult >= 0)
    //    {
    //        goldResult_Text = $"+ {goldResult}".ToString().SetTextColorTag(Define.TextColor.LightGreen);
    //    }
    //    else
    //    {
    //        goldResult_Text = $"- {Mathf.Abs(goldResult)}".ToString().SetTextColorTag(Define.TextColor.npc_red);
    //    }

    //    GetTMP(((int)Texts.Final)).text =
    //        $"{UserData.Instance.LocaleText("Mana")} : <b>{Result.Origin_Mana} {manaResult_Text} = {Main.Instance.Player_Mana.ToString().SetTextColorTag(Define.TextColor.LightYellow)}</b>\n" +
    //        $"{UserData.Instance.LocaleText("Gold")} : <b>{Result.Origin_Gold} {goldResult_Text} = {Main.Instance.Player_Gold.ToString().SetTextColorTag(Define.TextColor.LightYellow)}</b>";
    //}




}
