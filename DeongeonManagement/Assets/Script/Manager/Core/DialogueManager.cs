using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        var ui = Managers.UI.ShowPopUpAlone<UI_Dialogue>();
        ui.Data = data;
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
}
