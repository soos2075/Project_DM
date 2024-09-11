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
        Mana_Result_Get,
        Mana_Result_Use,
        Mana_Result,

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

    enum SubTitle  //? (GridLayoutGroup) / 각 항목은 최대 6개까지.
    {
        //? 항목 이름 
        SubTitle_Mana_Get,
        SubTitle_Mana_Use,
        SubTitle_Gold_Get,
        SubTitle_Gold_Use,

        //? 실제 입력값
        Value_Mana_Get,
        Value_Mana_Use,
        Value_Gold_Get,
        Value_Gold_Use,
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

        for (int i = 0; i < Enum.GetNames(typeof(SubTitle)).Length; i++)
        {
            var sub = Get<GridLayoutGroup>(i);
            for (int j = 0; j < sub.transform.childCount; j++)
            {
                sub.transform.GetChild(j).gameObject.SetActive(false);
            }
        }

    }

    void ShowResult()
    {
        //? 마나 획득ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        ResultCheckAndShow(Result.Mana_Get_Facility, SubTitle.Value_Mana_Get, SubTitle.SubTitle_Mana_Get, "시설");
        ResultCheckAndShow(Result.Mana_Get_Artifacts, SubTitle.Value_Mana_Get, SubTitle.SubTitle_Mana_Get, "아티팩트");
        ResultCheckAndShow(Result.Mana_Get_Monster, SubTitle.Value_Mana_Get, SubTitle.SubTitle_Mana_Get, "전투");
        ResultCheckAndShow(Result.Mana_Get_Etc, SubTitle.Value_Mana_Get, SubTitle.SubTitle_Mana_Get, "기타등등");

        //? 마나 보너스 처리
        int mana_Temp = Result.Mana_Get_Etc + Result.Mana_Get_Facility + Result.Mana_Get_Monster + Result.Mana_Get_Artifacts;
        float ratio = GameManager.Buff.CurrentBuff.Orb_blue > 0 ? 0.2f : 0;
        ratio += (GameManager.Buff.ManaBonus * 0.01f);

        int mana_bonus = Mathf.RoundToInt(mana_Temp * ratio);
        Debug.Log($"비율은{ratio} @@ 보너스는 {mana_bonus}");

        Result.AddMana(mana_bonus, Main.DayResult.EventType.ResultBonus);
        ResultCheckAndShow(Result.Mana_Get_Bonus, SubTitle.Value_Mana_Get, SubTitle.SubTitle_Mana_Get, "보너스");


        //? 마나 사용ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        ResultCheckAndShow(Result.Mana_Use_Facility, SubTitle.Value_Mana_Use, SubTitle.SubTitle_Mana_Use, "시설 배치");
        ResultCheckAndShow(Result.Mana_Use_Monster, SubTitle.Value_Mana_Use, SubTitle.SubTitle_Mana_Use, "유닛");
        ResultCheckAndShow(Result.Mana_Use_Etc, SubTitle.Value_Mana_Use, SubTitle.SubTitle_Mana_Use, "기타등등");


        //? 마나 최종 계산ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        int get_Mana = mana_Temp + mana_bonus;
        int use_Mana = Result.Mana_Use_Etc + Result.Mana_Use_Facility + Result.Mana_Use_Monster;

        GetTMP((int)ResultText.Mana_Result_Get).text = $"{get_Mana}";
        GetTMP((int)ResultText.Mana_Result_Use).text = $"{use_Mana * -1}";
        GetTMP((int)ResultText.Mana_Result).text = $"{get_Mana - use_Mana}";




        //? 골드 획득ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        ResultCheckAndShow(Result.Gold_Get_Facility, SubTitle.Value_Gold_Get, SubTitle.SubTitle_Gold_Get, "시설");
        ResultCheckAndShow(Result.Gold_Get_Monster, SubTitle.Value_Gold_Get, SubTitle.SubTitle_Gold_Get, "전투");
        ResultCheckAndShow(Result.Gold_Get_Technical, SubTitle.Value_Gold_Get, SubTitle.SubTitle_Gold_Get, "특수 시설");
        ResultCheckAndShow(Result.Gold_Get_Etc, SubTitle.Value_Gold_Get, SubTitle.SubTitle_Gold_Get, "기타등등");

        //? 골드 보너스 처리
        int gold_Temp = Result.Gold_Get_Etc + Result.Gold_Get_Facility + Result.Gold_Get_Monster + Result.Gold_Get_Technical;

        float ratio_gold = (GameManager.Buff.GoldBonus * 0.01f);
        int gold_bonus = Mathf.RoundToInt(gold_Temp * ratio_gold);
        Debug.Log($"골드비율은{ratio_gold} @@ 골드보너스는 {gold_bonus}");

        Result.AddGold(gold_bonus, Main.DayResult.EventType.ResultBonus);
        ResultCheckAndShow(Result.Gold_Get_Bonus, SubTitle.Value_Gold_Get, SubTitle.SubTitle_Gold_Get, "보너스");


        //? 골드 사용ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        ResultCheckAndShow(Result.Gold_Use_Facility, SubTitle.Value_Gold_Use, SubTitle.SubTitle_Gold_Use, "시설 배치");
        ResultCheckAndShow(Result.Gold_Use_Monster, SubTitle.Value_Gold_Use, SubTitle.SubTitle_Gold_Use, "유닛");
        ResultCheckAndShow(Result.Gold_Use_Etc, SubTitle.Value_Gold_Use, SubTitle.SubTitle_Gold_Use, "기타등등");



        //? 골드 최종 계산 ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        int get_Gold = gold_Temp + gold_bonus;
        int use_Gold = Result.Gold_Use_Etc + Result.Gold_Use_Facility + Result.Gold_Use_Monster;

        GetTMP((int)ResultText.Gold_Result_Get).text = $"{get_Gold}";
        GetTMP((int)ResultText.Gold_Result_Use).text = $"{use_Gold * -1}";
        GetTMP((int)ResultText.Gold_Result).text = $"{get_Gold - use_Gold}";





        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ모험가
        GetTMP((int)ResultText.NPC_Visit).text = $"{Result.NPC_Visit}";
        //GetTMP((int)ResultText.NPC_Prisoner).text = $"{Result.NPC_Visit}";
        GetTMP((int)ResultText.NPC_Die).text = $"{Result.NPC_Defeat}";
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


    void ResultCheckAndShow(int value, SubTitle valueBox, SubTitle titleBox, string localText)
    {
        if (value > 0)
        {
            Show_SubTitle($"{value}", valueBox);
            Show_SubTitle(UserData.Instance.LocaleText(localText), titleBox);
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
    void Show_SubTitle(string titleName, SubTitle boxName)
    {
        var sub = Get<GridLayoutGroup>((int)boxName);
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



}
