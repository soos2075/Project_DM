using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_Guild : MonoBehaviour
{
    // ���� �̸�
    //public string Name;

    // ���� ID (�������� ��ȭ�� ���� �� ���ư� ��ȣ)
    public int Original_Index;


    // ����Ʈ�� �ִٸ� ������ ��ȣ
    public List<int> InstanceQuestList = new List<int>();

    public List<int> OptionList = new List<int>();

    void Start()
    {
        
    }

    SpriteOutline outline;
    public void Contact()
    {
        Debug.Log("Ư�� Ű ������� UI ��� ");


        if (TryGetComponent<SpriteOutline>(out outline))
        {
            outline.Outline = true;
        }

    }
    public void ContactOff()
    {

        if (TryGetComponent<SpriteOutline>(out outline))
        {
            outline.Outline = false;
        }
    }

    public string StartDialogue()
    {
        int questIndex = 0;

        if (InstanceQuestList.Count > 0)
        {
            questIndex = InstanceQuestList[0];
            InstanceQuestList.RemoveAt(0);
            return $"Guild_{Original_Index + questIndex}";
        }

        if (OptionList.Count > 0)
        {
            return $"Guild_{Original_Index + 1}";
        }


        return $"Guild_{Original_Index}";
    }


    //public void ShowCurrentOption()
    //{
    //    for (int i = 0; i < OptionList.Count; i++)
    //    {
    //        Managers.Dialogue.ShowOption($"Guild_{Original_Index + OptionList[i]}");
    //    }
    //}

    public void OneTimeOptionButton()
    {
        Managers.Dialogue.OneTimeOption(OptionList, Original_Index);
    }
}
