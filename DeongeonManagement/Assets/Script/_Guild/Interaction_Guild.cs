using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_Guild : MonoBehaviour
{
    // 고유 이름
    //public string Name;

    // 고유 ID (진행중인 대화가 없을 때 돌아갈 번호)
    public int Original_Index;


    // 퀘스트가 있다면 더해줄 번호
    public List<int> InstanceQuestList = new List<int>();

    public List<int> OptionList = new List<int>();

    void Start()
    {
        
    }

    SpriteOutline outline;
    public void Contact()
    {
        Debug.Log("특정 키 누르라고 UI 띄움 ");


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
