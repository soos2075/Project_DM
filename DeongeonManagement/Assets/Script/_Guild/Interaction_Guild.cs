using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_Guild : MonoBehaviour
{

    public GuildNPC_LabelName label;
    //? 아래 세개를 이거로 통합(이건 실제 인스턴트가 아님)
    public GuildNPC_Data data;


    //// 고유 ID (진행중인 대화가 없을 때 돌아갈 번호)
    //public int Original_Index;

    //// 퀘스트가 있다면 더해줄 번호
    //public List<int> InstanceQuestList = new List<int>();

    //public List<int> OptionList = new List<int>();

    void Start()
    {
        foreach (var item in EventManager.Instance.CurrentGuildData)
        {
            if (item.Original_Index == (int)label)
            {
                data = item;
            }
        }

        int turn = EventManager.Instance.CurrentTurn;

        switch (GuildManager.Instance.GetData(data.Original_Index).DayOption)
        {
            case Guild_DayOption.Special:
                gameObject.SetActive(false);
                break;

            case Guild_DayOption.Odd:
                if (turn % 2 != 1) gameObject.SetActive(false);
                break;

            case Guild_DayOption.Even:
                if (turn % 2 != 0) gameObject.SetActive(false);
                break;

            case Guild_DayOption.Multiple_3:
                if (turn % 3 != 0) gameObject.SetActive(false);
                break;

            case Guild_DayOption.Multiple_4:
                if (turn % 4 != 0) gameObject.SetActive(false);
                break;

            case Guild_DayOption.Multiple_5:
                if (turn % 5 != 0) gameObject.SetActive(false);
                break;

            case Guild_DayOption.Multiple_7:
                if (turn % 7 != 0) gameObject.SetActive(false);
                break;
        }

        foreach (var instance in GuildManager.Instance.Instance_GuildNPC)
        {
            if (label == instance)
            {
                gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (data.InstanceQuestList.Count > 0 || data.OptionList.Count > 0)
        {
            if (eventKey == null)
            {
                eventKey = Managers.Resource.Instantiate("Guild/Event", transform);
            }
        }
        else if(data.InstanceQuestList.Count == 0 && data.OptionList.Count == 0)
        {
            if (eventKey != null)
            {
                Managers.Resource.Destroy(eventKey);
                eventKey = null;
            }
        }
    }


    public void AddQuest(int _index)
    {
        if (_index / 100 == 0)
        {
            data.InstanceQuestList.Add(_index);
        }
        else
        {
            data.OptionList.Add(_index);
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

        if (data.InstanceQuestList.Count > 0)
        {
            questIndex = data.InstanceQuestList[0];
            data.InstanceQuestList.RemoveAt(0);

            Managers.Dialogue.ShowDialogueUI(data.Original_Index + questIndex, transform);
            return;
        }

        if (data.OptionList.Count > 0)
        {
            Managers.Dialogue.ShowDialogueUI(data.Original_Index + 1, transform);
            return;
        }

        Managers.Dialogue.ShowDialogueUI(data.Original_Index, transform);
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
        Managers.Dialogue.OneTimeOption(data.OptionList, data.Original_Index);
    }
}

