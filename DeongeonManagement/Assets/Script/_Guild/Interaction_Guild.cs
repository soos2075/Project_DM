using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_Guild : MonoBehaviour
{

    public GuildNPC_LabelName label;

    public GuildNPC_Data data;

    void Start()
    {
        outline = GetComponentInChildren<SpriteOutline>(true);

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

        //? 다회차 전용 NPC
        if (GuildManager.Instance.GetData(data.Original_Index).MultipleNPC && UserData.Instance.FileConfig.PlayRounds == 1)
        {
            gameObject.SetActive(false);
        }


        if (GuildManager.Instance.GetData(data.Original_Index).FirstDay > turn)
        {
            gameObject.SetActive(false);
        }

        foreach (var instance in GuildManager.Instance.Instance_GuildNPC)
        {
            if (label == instance)
            {
                gameObject.SetActive(true);
            }
        }
        foreach (var instance in GuildManager.Instance.Delete_GuildNPC)
        {
            if (label == instance)
            {
                gameObject.SetActive(false);
            }
        }

        DefaultAnimState();
    }

    private void Update()
    {
        if (Managers.Dialogue.GetState() == DialogueManager.DialogueState.Talking)
        {
            if (eventKey != null)
            {
                Managers.Resource.Destroy(eventKey);
                eventKey = null;
            }
            if (key != null)
            {
                Managers.Resource.Destroy(key);
                key = null;
            }
            return;
        }

        GuildHelper.Icon icon = GuildHelper.Icon.Question_Yellow;

        if (data.InstanceQuestList.Count > 0)
        {
            icon = GuildHelper.Icon.Question_Yellow;
            //if (eventKey == null)
            //{
            //    //eventKey = Managers.Resource.Instantiate("Guild/Event", transform);
            //    eventKey = GuildHelper.Instance.GetIcon(GuildHelper.Icon.Question_Yellow);
            //    eventKey.transform.position = transform.position + Vector3.up;
            //}
        }
        else if (data.OptionList.Count > 0)
        {
            icon = GuildHelper.Icon.Question_Blue;
            //if (eventKey == null)
            //{
            //    //eventKey = Managers.Resource.Instantiate("Guild/Event", transform);
            //    eventKey = GuildHelper.Instance.GetIcon(GuildHelper.Icon.Question_Blue);
            //    eventKey.transform.position = transform.position + Vector3.up;
            //}
        }
        else if(data.InstanceQuestList.Count == 0 && data.OptionList.Count == 0)
        {
            if (eventKey != null)
            {
                Managers.Resource.Destroy(eventKey);
                eventKey = null;
            }
            return;
        }


        if (eventKey == null)
        {
            eventKey = GuildHelper.Instance.GetIcon(icon);
            eventKey.transform.parent = transform;
            eventKey.transform.position = transform.position + Vector3.up;
        }
        else
        {
            eventKey.GetComponentInChildren<SpriteRenderer>().sprite = GuildHelper.Instance.GetIconSprite(icon);
        }
    }


    void DefaultAnimState()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }


        Animator anim;
        if (TryGetComponent<Animator>(out anim))
        {
            anim.speed = Random.Range(0.5f, 0.8f);
        }

        switch (label)
        {
            case GuildNPC_LabelName.QuestZone:
                int questCount = data.InstanceQuestList.Count + data.OptionList.Count;
                if (questCount == 0)
                {
                    GetComponent<SpriteRenderer>().sprite = GuildHelper.Instance.board_little;
                }
                else if (questCount == 1)
                {
                    GetComponent<SpriteRenderer>().sprite = GuildHelper.Instance.board_little;
                }
                else if (questCount > 1)
                {
                    GetComponent<SpriteRenderer>().sprite = GuildHelper.Instance.board_many;
                }
                break;
            case GuildNPC_LabelName.StaffA:
                break;

            case GuildNPC_LabelName.StaffB:
                break;

            case GuildNPC_LabelName.Heroine:
                anim.Play(Define.ANIM_Idle_Sit);
                break;

            case GuildNPC_LabelName.DogGirl:
                break;

            case GuildNPC_LabelName.DummyB:
                break;

            case GuildNPC_LabelName.DummyC:
                break;

            case GuildNPC_LabelName.DummyD:
                break;

            case GuildNPC_LabelName.Lightning:
                anim.Play(Define.ANIM_Idle_Sit);
                break;


            case GuildNPC_LabelName.RetiredHero:
                break;
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
                //key = Managers.Resource.Instantiate("Guild/Key_E", transform);
                key = GuildHelper.Instance.GetIcon(GuildHelper.Icon.Dialogue);
                key.transform.parent = transform;
                key.transform.position = transform.position + Vector3.up;
            }
            else
            {
                key.SetActive(true);
            }
        }

        if (outline != null)
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

        if (outline != null)
        {
            outline.Outline = false;
        }
    }

    public void StartDialogue()
    {
        //if (key != null)
        //{
        //    key.SetActive(false);
        //}


        int questIndex = 0;

        if (data.InstanceQuestList.Count > 0)
        {
            questIndex = data.InstanceQuestList[0];
            data.ClearQuest(questIndex);
            //data.InstanceQuestList.Remove(questIndex);
            //data.AlreadyClearList.Add(questIndex);

            Managers.Dialogue.ShowDialogueUI(data.Original_Index + questIndex, transform);
            return;
        }

        if (data.OptionList.Count > 0) //? 선택지를 띄워줌. id+1이 옵션대화임
        {
            //questIndex = data.OptionList[0];
            //data.OptionList.Remove(questIndex);
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

