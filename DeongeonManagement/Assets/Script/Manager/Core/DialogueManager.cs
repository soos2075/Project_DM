using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager
{
    //? 너무 데이터가 많아지면 딕셔너리로 한번 더 등록해줘도 됨
    public SO_DialogueData[] so_DataAll;
    float textSpeed;

    public void Init()
    {
        so_DataAll = Resources.LoadAll<SO_DialogueData>("Data/Dialogue");

        CurrentTextSpeed = UserData.Instance.GetDataInt(PrefsKey.TextSpeed, 5);
    }

    int textSpeedOption;
    public int CurrentTextSpeed { get { return textSpeedOption; } set { SetTextSpeed(value); } }

    void SetTextSpeed(int _value)
    {
        textSpeedOption = _value;
        textSpeed = (11 - _value) * 0.01f;
        UserData.Instance.SetData(PrefsKey.TextSpeed, textSpeedOption);
    }



    public SO_DialogueData GetDialogue(string dialogueName)
    {
        return SearchData(dialogueName);
    }


    SO_DialogueData SearchData(string dialogueName)
    {
        foreach (var item in so_DataAll)
        {
            if (item.name == dialogueName || item.dialogueName == dialogueName)
            {
                return item;
            }
        }
        Debug.Log($"{dialogueName} 데이터 없음");
        return null;
    }


    public IDialogue ShowDialogueUI(SO_DialogueData data, Transform pos = null)
    {
        Managers.Instance.StartCoroutine(HideCanvas());

        switch (data.Type)
        {
            case SO_DialogueData.DialogueType.Quest:
                currentDialogue = Managers.UI.ShowPopUpAlone<UI_QuestBoard>();
                currentDialogue.Data = data;
                currentDialogue.TextDelay = textSpeed;
                break;

            case SO_DialogueData.DialogueType.Bubble:
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
        SO_DialogueData data = GetDialogue(dialogueName);
        if (data == null)
        {
            Debug.Log($"dialogueData 없음 : {dialogueName}");
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
            SO_DialogueData data = GetDialogue($"Guild_{optionList[i] + id}");
            if (data == null)
            {
                Debug.Log($"선택지 없음 : Guild_{optionList[i] + id}");
                continue;
            }

            var btn = Managers.Resource.Instantiate("UI/PopUp/Element/OptionButton");
            btn.GetComponent<UI_OptionButton>().SetAction((PointerEventData pointer) => ButtonAction(data, id, pointer), data.dialogueName);
            currentDialogue.AddOption(btn);
        }
    }
    void ButtonAction(SO_DialogueData data, int id, PointerEventData pointer)
    {
        Managers.UI.CloseAll();
        ShowDialogueUI(data);
        var npc = GameObject.FindAnyObjectByType<GuildManager>().GetInteraction(id);
        npc.OptionList.RemoveAt(pointer.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex());
    }


    public void ShowOption(string diaID)
    {
        SO_DialogueData data = GetDialogue(diaID);
        if (data == null)
        {
            Debug.Log($"선택지 없음 : {diaID}");
            return;
        }

        var btn = Managers.Resource.Instantiate("UI/PopUp/Element/OptionButton");
        btn.GetComponent<UI_OptionButton>().SetAction((data) => WahtTheAction(data), data.dialogueName);
        currentDialogue.AddOption(btn);
    }
    void WahtTheAction(PointerEventData pointer)
    {

    }

    //public void ShowOption(int diaID)
    //{

    //}


}



