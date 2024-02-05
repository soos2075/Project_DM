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
    public List<int> Current_QuestList = new List<int>();

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

        if (Current_QuestList.Count > 0)
        {
            questIndex = Current_QuestList[0];
            Current_QuestList.RemoveAt(0);
        }

        return $"Guild_{Original_Index + questIndex}";
    }
}
