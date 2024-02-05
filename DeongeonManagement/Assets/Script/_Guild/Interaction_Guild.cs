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
    public List<int> Current_QuestList = new List<int>();

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

        if (Current_QuestList.Count > 0)
        {
            questIndex = Current_QuestList[0];
            Current_QuestList.RemoveAt(0);
        }

        return $"Guild_{Original_Index + questIndex}";
    }
}
