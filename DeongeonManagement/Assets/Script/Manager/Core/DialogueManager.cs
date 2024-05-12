using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager
{
    public void Init()
    {
        CurrentTextSpeed = UserData.Instance.GetDataInt(PrefsKey.TextSpeed, 5);

        Init_GetLocalizationData();
    }


    #region Data
    Dictionary<DialogueName, DialogueData> currentData;
    Dictionary<DialogueName, DialogueData> CurrentLanguageDialogue
    {
        get
        {
            if (currentData == null)
            {
                Init_GetLocalizationData();
            }
            return currentData;
        }
    }


    public void Init_GetLocalizationData()
    {
        switch (UserData.Instance.Language)
        {
            case Define.Language.EN:
                currentData = Managers.Data.Dialogue_EN;
                break;


            case Define.Language.KR:
                currentData = Managers.Data.Dialogue_KR;
                break;

            case Define.Language.JP:
                currentData = Managers.Data.Dialogue_JP;
                break;
        }
    }


    public DialogueData GetDialogue(DialogueName dialogueName)
    {
        DialogueData data = null;

        if (CurrentLanguageDialogue.TryGetValue(dialogueName, out data))
        {
            return data;
        }
        Debug.Log($"{dialogueName} : Dialogue Data Not Exist");
        return null;
    }
    public DialogueData GetDialogue(string dialogueName)
    {
        object parse = null;
        if (Enum.TryParse(typeof(DialogueName), dialogueName, out parse))
        {
            return GetDialogue((DialogueName)Enum.Parse(typeof(DialogueName), dialogueName));
        }
        else
        {
            return null;
        }
    }
    public DialogueData GetDialogue(int dialogueID)
    {
        return GetDialogue((DialogueName)dialogueID);
    }



    #endregion





    //? 너무 데이터가 많아지면 딕셔너리로 한번 더 등록해줘도 됨
    float textSpeed;

    int textSpeedOption;
    public int CurrentTextSpeed { get { return textSpeedOption; } set { SetTextSpeed(value); } }

    void SetTextSpeed(int _value)
    {
        textSpeedOption = _value;
        textSpeed = (11 - _value) * 0.01f;
        UserData.Instance.SetData(PrefsKey.TextSpeed, textSpeedOption);
    }



    public IDialogue ShowDialogueUI(DialogueData data, Transform pos = null)
    {
        Managers.Instance.StartCoroutine(HideCanvas());

        switch (data.Type)
        {
            case DialogueData.DialogueType.Quest:
                currentDialogue = Managers.UI.ShowPopUpAlone<UI_QuestBoard>();
                currentDialogue.Data = data;
                currentDialogue.TextDelay = textSpeed;
                break;

            case DialogueData.DialogueType.Bubble:
                currentDialogue = Managers.UI.ShowPopUpAlone<UI_DialogueBubble>();
                currentDialogue.Data = data;
                currentDialogue.TextDelay = textSpeed;
                var bubble = currentDialogue as UI_DialogueBubble;
                bubble.bubble_Position = pos;
                break;
        }
        return currentDialogue;
    }
    public void ShowDialogueUI(string dialogueName, Transform pos = null)
    {
        DialogueData data = GetDialogue(dialogueName);
        if (data == null)
        {
            Debug.Log($"dialogueData 없음 : {dialogueName}");
            return;
        }
        ShowDialogueUI(data, pos);
    }
    public void ShowDialogueUI(DialogueName dialogueName, Transform pos = null)
    {
        DialogueData data = GetDialogue(dialogueName);
        if (data == null)
        {
            Debug.Log($"dialogueData 없음 : {dialogueName}");
            return;
        }
        ShowDialogueUI(data, pos);
    }
    public void ShowDialogueUI(int dialogueID, Transform pos = null)
    {
        DialogueData data = GetDialogue(dialogueID);
        if (data == null)
        {
            Debug.Log($"dialogueData 없음 : {dialogueID}");
            return;
        }
        ShowDialogueUI(data, pos);
    }


    IEnumerator HideCanvas()
    {
        var canvas = GameObject.FindObjectsOfType<Canvas>();

        foreach (var item in canvas)
        {
            item.enabled = false;
        }

        yield return null;
        yield return new WaitUntil(() => GetState() == DialogueState.None);

        foreach (var item in canvas)
        {
            if (item != null)
            {
                item.enabled = true;
            }
        }
    }




    public IDialogue currentDialogue;
    public enum DialogueState
    {
        None,
        Talking,
    }

    public DialogueState GetState()
    {
        if (currentDialogue != null)
        {
            return DialogueState.Talking;
        }
        else
        {
            return DialogueState.None;
        }
    }


    public void OneTimeOption(List<int> optionList, int id)
    {
        for (int i = 0; i < optionList.Count; i++)
        {
            DialogueData data = GetDialogue(optionList[i] + id);
            if (data == null)
            {
                Debug.Log($"선택지 없음 : ID {optionList[i] + id}");
                continue;
            }

            var btn = Managers.Resource.Instantiate("UI/PopUp/Element/OptionButton");
            btn.GetComponent<UI_OptionButton>().SetAction((PointerEventData pointer) => ButtonAction(data, id, pointer), data.dialogueName);
            currentDialogue.AddOption(btn);
        }
    }
    void ButtonAction(DialogueData data, int id, PointerEventData pointer)
    {
        Managers.UI.CloseAll();
        ShowDialogueUI(data);
        var npc = GameObject.FindAnyObjectByType<GuildManager>().GetInteraction(id);
        npc.OptionList.RemoveAt(pointer.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex());
    }


    //public void ShowOption(string diaID)
    //{
    //    DialogueData data = GetDialogue(diaID);
    //    if (data == null)
    //    {
    //        Debug.Log($"선택지 없음 : {diaID}");
    //        return;
    //    }

    //    var btn = Managers.Resource.Instantiate("UI/PopUp/Element/OptionButton");
    //    btn.GetComponent<UI_OptionButton>().SetAction((data) => WahtTheAction(data), data.dialogueName);
    //    currentDialogue.AddOption(btn);
    //}
    //void WahtTheAction(PointerEventData pointer)
    //{

    //}

    //public void ShowOption(int diaID)
    //{

    //}


}

public class DialogueData
{
    public enum DialogueType
    {
        Quest,
        Bubble,
    }

    public int id;
    public string dialogueName;
    public DialogueType Type;

    public List<TextData> TextDataList = new List<TextData>();

    public class TextData
    {
        [TextArea(1, 100)]
        public string optionString;
        [TextArea(3, 100)]
        public string mainText;

        public TextData(string _option, string _mainText)
        {
            optionString = _option;
            mainText = _mainText;
        }
    }
}

public enum DialogueName
{
    // Day Event = 0~499
    Prologue = 0,
    Tutorial_Facility = 1,
    Tutorial_Monster = 2,
    Tutorial_Technical = 3,
    Tutorial_Egg = 4,
    Tutorial_Guild = 5,


    Day3_Event = 33,

    Day8_Event = 83,
    Day8_Event_Die = 84,

    Day15_Event = 153,
    Day15_Event_Die = 154,

    Day20_Event = 200,
    Day20_Over = 201,

    Day25_Event = 250,
    Day30_Event = 300,


    Day30_Over = 302,

    Herbalist0_Appear = 400,
    Miner0_Appear = 401,

    Elf_Appear = 410,
    Wizard_Appear = 420,


    // Main Event = 500~999
    Opening_1 = 501,
    Opening_2 = 502,
    Opening_3 = 503,
    Opening_4 = 504,

    Ending_Demo = 600,


    // Ending = 700~
    Ending_Common = 700,

    Dog_1 = 711,
    Dog_2 = 712,
    Dog_3 = 713,
    Dog_4 = 714,

    // Dungeon Expension
    Expansion_4 = 990,



    // QuestBoard ID = 1000
    Quest_0 = 1000,
    Quest_1 = 1001,
    Quest_10 = 1010,
    Quest_100 = 1100,
    Quest_101 = 1101,

    // Staff A ID : 2000
    Staff_A_0 = 2000,
    Staff_A_1 = 2001,
    Staff_A_100 = 2100,
    Staff_A_101 = 2101,

    // Staff B ID : 3000
    Staff_B_0 = 3000,
    Staff_B_10 = 3010,


    // Heroine ID : 4000
    Heroine_0 = 4000,
    Heroine_10 = 4010,

    // 미정
    Odd_0 = 5000,

    // 미정
    Even_0 = 6000,

    // QuestNPC : 9000
    Hunter_Slime = 9000,
    Hunter_EarthGolem = 9001,


}

