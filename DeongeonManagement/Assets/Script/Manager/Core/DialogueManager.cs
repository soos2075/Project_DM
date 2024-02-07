using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager
{
    //? �ʹ� �����Ͱ� �������� ��ųʸ��� �ѹ� �� ������൵ ��
    public SO_DialogueData[] so_DataAll;

    public void Init()
    {
        so_DataAll = Resources.LoadAll<SO_DialogueData>("Data");
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
        Debug.Log($"{dialogueName} ������ ����");
        return null;
    }


    public void ShowDialogueUI(SO_DialogueData data)
    {
        currentDialogue = Managers.UI.ShowPopUpAlone<UI_Dialogue>();
        currentDialogue.Data = data;
    }
    public void ShowDialogueUI(string dialogueName)
    {
        SO_DialogueData data = GetDialogue(dialogueName);
        if (data == null)
        {
            return;
        }

        currentDialogue = Managers.UI.ShowPopUpAlone<UI_Dialogue>();
        currentDialogue.Data = data;
    }

    public UI_Dialogue currentDialogue;
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
                Debug.Log($"������ ���� : Guild_{optionList[i] + id}");
                continue;
            }

            var btn = Managers.Resource.Instantiate("UI/PopUp/Element/OptionButton");
            btn.GetComponent<UI_OptionButton>().SetAction((PointerEventData pointer) => ButtonAction(data, id, pointer), data.dialogueName);
            currentDialogue.AddOption(btn);
        }
    }
    void ButtonAction(SO_DialogueData data, int id, PointerEventData pointer)
    {
        ShowDialogueUI(data);
        var npc = GuildManager.Instance.GetInteraction(id);
        npc.OptionList.RemoveAt(pointer.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex());
    }


    public void ShowOption(string diaID)
    {
        SO_DialogueData data = GetDialogue(diaID);
        if (data == null)
        {
            Debug.Log($"������ ���� : {diaID}");
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



