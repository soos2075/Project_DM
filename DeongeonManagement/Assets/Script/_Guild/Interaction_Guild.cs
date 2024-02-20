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

    private void Update()
    {
        if (InstanceQuestList.Count > 0 && eventKey == null)
        {
            eventKey = Managers.Resource.Instantiate("Guild/Event", transform);
        }
        else if(InstanceQuestList.Count == 0 && eventKey != null)
        {
            Managers.Resource.Destroy(eventKey);
            eventKey = null;
        }
    }


    public void AddQuest(int _index)
    {
        if (_index / 100 == 0)
        {
            InstanceQuestList.Add(_index);
        }
        else
        {
            OptionList.Add(_index);
        }
    }




    SpriteOutline outline;
    GameObject key;
    GameObject eventKey;
    public void Contact()
    {
        if (eventKey == null)
        {
            if (key == null)
            {
                key = Managers.Resource.Instantiate("Guild/Key_E", transform);
            }
            else
            {
                key.SetActive(true);
            }
        }

        


        if (TryGetComponent<SpriteOutline>(out outline))
        {
            outline.Outline = true;
        }

    }
    public void ContactOff()
    {
        if (key != null)
        {
            key.SetActive(false);
        }

        if (TryGetComponent<SpriteOutline>(out outline))
        {
            outline.Outline = false;
        }
    }

    public void StartDialogue()
    {
        if (key != null)
        {
            key.SetActive(false);
        }

        int questIndex = 0;

        if (InstanceQuestList.Count > 0)
        {
            questIndex = InstanceQuestList[0];
            InstanceQuestList.RemoveAt(0);
            //return $"Guild_{Original_Index + questIndex}";

            Managers.Dialogue.ShowDialogueUI($"Guild_{Original_Index + questIndex}", transform);
            return;
        }

        if (OptionList.Count > 0)
        {
            //return $"Guild_{Original_Index + 1}";
            Managers.Dialogue.ShowDialogueUI($"Guild_{Original_Index + 1}", transform);
            return;
        }


        //return $"Guild_{Original_Index}";
        Managers.Dialogue.ShowDialogueUI($"Guild_{Original_Index}", transform);
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

