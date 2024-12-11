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
        //Mana_Result_Get,
        //Mana_Result_Use,
        Mana_Result,

        //Gold_Result_Get,
        //Gold_Result_Use,
        Gold_Result,


        //? ���谡
        NPC_Visit,
        NPC_Satisfaction,
        NPC_NonSatisfaction,
        NPC_Empty,
        NPC_Runaway,
        NPC_Die,


        //? ����
        Monster_Battle,
        Monster_LvUp,
        Monster_Trait,
        Monster_Evolution,

        Monster_Victory,
        Monster_Draw,
        Monster_Defeat,


        //? ����
        Dungeon_Pop,
        Dungeon_Danger,
        Dungeon_Rank,
    }

    enum SubTitle  //? (GridLayoutGroup) / �� �׸��� �ִ� 6������.
    {

        //? ������ ��� �ʵ�
        ManaGroup,
        GoldGroup,
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
        //? ���� ȹ��ѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤ�
        Add_ValueBox(SubTitle.ManaGroup, "Facility", "�ü�", Result.Mana_Get_Facility, Result.Mana_Use_Facility);
        Add_ValueBox(SubTitle.ManaGroup, "Battle", "����", Result.Mana_Get_Monster, Result.Mana_Use_Monster);
        Add_ValueBox(SubTitle.ManaGroup, "Etc", "��Ÿ���", Result.Mana_Get_Etc, Result.Mana_Use_Etc);
        Add_ValueBox(SubTitle.ManaGroup, "Artifact", "��Ƽ��Ʈ", Result.Mana_Get_Artifacts);


        //? ���� ���ʽ� ó��
        int mana_Temp = Result.Mana_Get_Etc + Result.Mana_Get_Facility + Result.Mana_Get_Monster + Result.Mana_Get_Artifacts;
        float ratio = GameManager.Buff.CurrentBuff.Orb_blue > 0 ? 0.2f : 0;
        ratio += (GameManager.Buff.ManaBonus * 0.01f);

        int mana_bonus = Mathf.RoundToInt(mana_Temp * ratio);
        //Debug.Log($"������{ratio} @@ ���ʽ��� {mana_bonus}");

        Result.AddMana(mana_bonus, Main.DayResult.EventType.ResultBonus);
        Add_ValueBox(SubTitle.ManaGroup, "Bonus", "���ʽ�", Result.Mana_Get_Bonus);
        //ResultCheckAndShow(Result.Mana_Get_Bonus, SubTitle.Value_Mana_Get, SubTitle.SubTitle_Mana_Get, "���ʽ�");


        //? ���� ���� ���ѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤ�
        int get_Mana = mana_Temp + mana_bonus;
        int use_Mana = Result.Mana_Use_Etc + Result.Mana_Use_Facility + Result.Mana_Use_Monster;

        //GetTMP((int)ResultText.Mana_Result_Get).text = $"{get_Mana}";
        //GetTMP((int)ResultText.Mana_Result_Use).text = $"{use_Mana * -1}";
        int manaResult = get_Mana - use_Mana;
        if (manaResult > 0)
        {
            GetTMP((int)ResultText.Mana_Result).text = $"+{manaResult}";
        }
        else
        {
            GetTMP((int)ResultText.Mana_Result).text = $"{manaResult}";
        }






        //? ��� ȹ��ѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤ�
        Add_ValueBox(SubTitle.GoldGroup, "Facility", "�ü�", Result.Gold_Get_Facility, Result.Gold_Use_Facility);
        Add_ValueBox(SubTitle.GoldGroup, "Battle", "����", Result.Gold_Get_Monster, Result.Gold_Use_Monster);
        Add_ValueBox(SubTitle.GoldGroup, "Etc", "��Ÿ���", Result.Gold_Get_Etc, Result.Gold_Use_Etc);
        Add_ValueBox(SubTitle.GoldGroup, "Technical", "Ư�� �ü�", Result.Gold_Get_Technical, Result.Gold_Use_Technical);


        //? ��� ���ʽ� ó��
        int gold_Temp = Result.Gold_Get_Etc + Result.Gold_Get_Facility + Result.Gold_Get_Monster + Result.Gold_Get_Technical;

        float ratio_gold = (GameManager.Buff.GoldBonus * 0.01f);
        int gold_bonus = Mathf.RoundToInt(gold_Temp * ratio_gold);
        //Debug.Log($"��������{ratio_gold} @@ ��庸�ʽ��� {gold_bonus}");

        Result.AddGold(gold_bonus, Main.DayResult.EventType.ResultBonus);
        Add_ValueBox(SubTitle.ManaGroup, "Bonus", "���ʽ�", Result.Gold_Get_Bonus);
        //ResultCheckAndShow(Result.Gold_Get_Bonus, SubTitle.Value_Gold_Get, SubTitle.SubTitle_Gold_Get, "���ʽ�");


        //? ��� ���� ��� �ѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤ�
        int get_Gold = gold_Temp + gold_bonus;
        int use_Gold = Result.Gold_Use_Etc + Result.Gold_Use_Facility + Result.Gold_Use_Monster;

        //GetTMP((int)ResultText.Gold_Result_Get).text = $"{get_Gold}";
        //GetTMP((int)ResultText.Gold_Result_Use).text = $"{use_Gold * -1}";

        int goldResult = get_Gold - use_Gold;
        if (goldResult > 0)
        {
            GetTMP((int)ResultText.Gold_Result).text = $"+{goldResult}";
        }
        else
        {
            GetTMP((int)ResultText.Gold_Result).text = $"{goldResult}";
        }



        //? �ѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѸ��谡
        GetTMP((int)ResultText.NPC_Visit).text = $"{Result.NPC_Visit}";
        //GetTMP((int)ResultText.NPC_Prisoner).text = $"{Result.NPC_Visit}";
        GetTMP((int)ResultText.NPC_Die).text = $"{Result.NPC_Defeat}";
        GetTMP((int)ResultText.NPC_Satisfaction).text = $"{Result.NPC_Satisfaction}";
        GetTMP((int)ResultText.NPC_NonSatisfaction).text = $"{Result.NPC_NonSatisfaction}";
        GetTMP((int)ResultText.NPC_Empty).text = $"{Result.NPC_Empty}";
        GetTMP((int)ResultText.NPC_Runaway).text = $"{Result.NPC_Runaway}";


        //? �ѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѸ���
        GetTMP((int)ResultText.Monster_Battle).text = $"{Result.Monster_Battle}";
        GetTMP((int)ResultText.Monster_LvUp).text = $"{Result.Monster_LvUp}";
        GetTMP((int)ResultText.Monster_Trait).text = $"{Result.Monster_Trait}";
        GetTMP((int)ResultText.Monster_Evolution).text = $"{Result.Monster_Evolution}";

        GetTMP((int)ResultText.Monster_Victory).text = $"{Result.Monster_Victory}";
        GetTMP((int)ResultText.Monster_Defeat).text = $"{Result.Monster_Defeat}";
        int draw = Result.Monster_Battle - (Result.Monster_Victory + Result.Monster_Defeat);
        GetTMP((int)ResultText.Monster_Draw).text = $"{draw}";

        //? �ѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѤѴ���
        GetTMP((int)ResultText.Dungeon_Pop).text = $"{Result.Origin_Pop} �� {Current.Origin_Pop}";
        GetTMP((int)ResultText.Dungeon_Danger).text = $"{Result.Origin_Danger} �� {Current.Origin_Danger}";
        GetTMP((int)ResultText.Dungeon_Rank).text = $"{(Define.DungeonRank)Result.Origin_Rank}";

        if (Result.Origin_Rank < Current.Origin_Rank)
        {
            GetTMP((int)ResultText.Dungeon_Rank).text = $"{(Define.DungeonRank)Result.Origin_Rank} �� {(Define.DungeonRank)Current.Origin_Rank}";
            GetObject((int)Etcs.Stamp).SetActive(true);
        }
    }



    void Init_SubTitle()
    {
        for (int i = 0; i < Enum.GetNames(typeof(SubTitle)).Length; i++)
        {
            var sub = Get<GridLayoutGroup>(i);
            for (int j = 0; j < sub.transform.childCount; j++)
            {
                sub.transform.GetChild(i).Find("Image").GetComponent<Image>().sprite = Managers.Sprite.GetClear();
                sub.transform.GetChild(i).Find("Title").GetComponent<TextMeshProUGUI>().text = "";
                sub.transform.GetChild(i).Find("Value").GetComponent<TextMeshProUGUI>().text = "";
                sub.transform.GetChild(j).gameObject.SetActive(false);
            }
        }
    }


    void Add_ValueBox(SubTitle boxName, string imageName, string titleName, int value_Add, int value_Sub = 0)
    {
        if (value_Sub == 0 && value_Add == 0)
        {
            return;
        }

        var sub = Get<GridLayoutGroup>((int)boxName);

        for (int i = 0; i < sub.transform.childCount; i++)
        {
            if (sub.transform.GetChild(i).gameObject.activeInHierarchy == false)
            {
                sub.transform.GetChild(i).gameObject.SetActive(true);
                sub.transform.GetChild(i).Find("Image").GetComponent<Image>().sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "DayResult", imageName);
                sub.transform.GetChild(i).Find("Title").GetComponent<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText(titleName)}";

                if (value_Sub != 0)
                {
                    sub.transform.GetChild(i).Find("Value").GetComponent<TextMeshProUGUI>().text = $"{value_Add} - {value_Sub} = {value_Add - value_Sub}";
                }
                else
                {
                    sub.transform.GetChild(i).Find("Value").GetComponent<TextMeshProUGUI>().text = $"{value_Add}";
                }

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
    IEnumerator CloseActionDelay() //? 1������ ������ �ִٰ� ��������(�ٷβ����°� ����)
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
