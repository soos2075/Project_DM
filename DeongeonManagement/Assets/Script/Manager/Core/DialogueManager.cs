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

            case Define.Language.SCC:
                currentData = Managers.Data.Dialogue_SCC;
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
        EventManager.Instance.CurrentClearEventData.AddClear((DialogueName)data.id);
        return currentDialogue;
    }
    //public void ShowDialogueUI(string dialogueName, Transform pos = null)
    //{
    //    DialogueData data = GetDialogue(dialogueName);
    //    if (data == null)
    //    {
    //        Debug.Log($"dialogueData 없음 : {dialogueName}");
    //        return;
    //    }
    //    ShowDialogueUI(data, pos);
    //}
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
                if (item.GetComponent<UI_DayResult>() != null)
                {
                    Time.timeScale = 0;
                }
            }
        }
    }



    public bool AllowPerfectSkip { get; set; } = true;



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


    public Action ReserveAction { get; set; }
    Coroutine WaitCor;

    public void ActionReserve(Action action) //? 대화 끝나고 바로 시작할 액션을 예약해놓는곳
    {
        if (ReserveAction == null)
        {
            ReserveAction = action;
        }
        else
        {
            ReserveAction += action;
        }


        if (WaitCor == null)
        {
            WaitCor = Managers.Instance.StartCoroutine(WaitDialogueAndAction());
        }
    }

    IEnumerator WaitDialogueAndAction()
    {
        yield return null;

        if (GetState() == DialogueState.Talking)
        {
            yield return new WaitUntil(() => currentDialogue == null);
            yield return null;
        }

        ReserveAction.Invoke();
        ReserveAction = null;
        WaitCor = null;
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
        var npc = GuildManager.Instance.GetInteraction(id);
        npc.OptionList.RemoveAt(pointer.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex());
    }



    public void Show_SelectOption(int[] dialogueList)
    {
        currentDialogue.CloseOptionBox();

        for (int i = 0; i < dialogueList.Length; i++)
        {
            DialogueData data = GetDialogue(dialogueList[i]);
            if (data == null)
            {
                Debug.Log($"선택지 없음 : ID {dialogueList[i]}");
                continue;
            }

            var btn = Managers.Resource.Instantiate("UI/PopUp/Element/OptionButton");
            btn.GetComponent<UI_OptionButton>().SetAction((pointer) => Select_NewDialogue(data), data.dialogueName);
            currentDialogue.AddOption(btn);
        }
    }

    void Select_NewDialogue(DialogueData data)
    {
        currentDialogue.CloseOptionBox();
        ShowDialogueUI(data);
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

    public DialogueData()
    {

    }
    public DialogueData(int _id, DialogueType _type, string _diaName)
    {
        id = _id;
        Type = _type;
        dialogueName = _diaName;
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
    Tutorial_Orb = 6,


    FirstAdvAppear = 33,

    RedHair_Appear = 83,
    RedHair_Defeat = 84,
    RedHair_Return = 85,


    Goblin_Appear = 90,
    Goblin_Satisfiction = 91,
    Goblin_Empty = 92,
    Goblin_Die = 93,

    Goblin_Party = 100,


    Catastrophe_Appear = 140,
    Catastrophe_Return = 141,
    Catastrophe_Return_First = 142,
    Catastrophe_Seal = 143,


    RetiredHero_Appear = 153,
    RetiredHero_Defeat = 154,

    BloodSong_Appear = 200,
    BloodSong_Return = 201,

    Dragon_First = 210,
    Dragon_Second = 211,

    Guild_Raid_1 = 250,
    Guild_Raid_2 = 300,

    Day30_Over = 302,

    Herbalist0_Appear = 400,
    Miner0_Appear = 401,

    Elf_Appear = 410,
    Wizard_Appear = 420,

    Heroine_Appear = 490,
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

    Dragon_1 = 721,
    Dragon_2 = 722,
    Dragon_3 = 723,
    Dragon_4 = 724,

    Ravi_1 = 731,
    Ravi_2 = 732,
    Ravi_3 = 733,

    Cat_1 = 741,
    Cat_2 = 742,
    Cat_3 = 743,
    Cat_4 = 744,
    Cat_5 = 745,
    Cat_6 = 746,
    Cat_7 = 747,
    Cat_8 = 748,

    // Dungeon Expension
    Expansion_4 = 990,


    // QuestBoard ID = 1000
    Quest_0 = 1000,
    Quest_1 = 1001,
    Quest_10 = 1010,
    Quest_100 = 1100,
    Quest_101 = 1101,

    Quest_151 = 1151,


    // Staff A ID : 2000
    Staff_A_0 = 2000,
    Staff_A_1 = 2001,
    Staff_A_100 = 2100,
    Staff_A_101 = 2101,

    // Staff B ID : 3000
    Staff_B_0 = 3000,
    Staff_B_10 = 3010,
    Staff_B_14 = 3014,


    // Heroine ID : 4000
    Heroine_0 = 4000,
    Heroine_10 = 4010,
    Heroine_20 = 4020,
    Heroine_30 = 4030,
    Heroine_Prison = 4031,
    Heroine_40 = 4040,

    // 미정
    Odd_0 = 5000,

    // 미정
    Even_0 = 6000,

    // QuestNPC : 9000
    Hunter_Slime = 9000,
    Hunter_EarthGolem = 9001,

    DeathMagician = 10000,
    DeathMagician_Catastrophe = 10001,
    DeathMagician_DevilStatue = 10002,

    RetiredHero_0 = 15000,
    RetiredHero_10 = 15010,

}

