using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildManager : MonoBehaviour
{
    #region singleton
    //private static GuildManager _instance;
    //public static GuildManager Instance { get { Initialize(); return _instance; } }

    //private static void Initialize()
    //{
    //    if (_instance == null)
    //    {
    //        _instance = FindObjectOfType<GuildManager>();
    //    }
    //}

    //private void Awake()
    //{
    //    Initialize();
    //    if (_instance != null)
    //    {
    //        if (_instance != this)
    //        {
    //            Destroy(gameObject);
    //        }
    //        else
    //        {
    //            DontDestroyOnLoad(gameObject);
    //        }
    //    }
    //}
    #endregion
    // �׻� �ִ¾ֵ�
    public GameObject Always;
    // Ȧ����
    public GameObject DayOdd;
    // ¦����
    public GameObject DayEven;
    // 3�� ���
    public GameObject Triple;
    // 4�� ���
    public GameObject Quadruple;
    // 5�� ���
    public GameObject Quintuple;








    void Start()
    {
        SetActive_NPC();

        GuildEnter();

        //Time.timeScale = 1;
        //UserData.Instance.GameSpeed = 1;
        UserData.Instance.GamePlay_Normal();
    }


    void SetActive_NPC()
    {
        int turn = EventManager.Instance.CurrentTurn;
        Debug.Log(turn + "����");

        Always.SetActive(true);

        if (turn % 2 == 0)
        {
            DayEven.SetActive(true);
        }

        if (turn % 2 == 1)
        {
            DayOdd.SetActive(true);
        }

        if (turn % 3 == 0)
        {
            Triple.SetActive(true);
        }

        if (turn % 4 == 0)
        {
            Quadruple.SetActive(true);
        }

        if (turn % 5 == 0)
        {
            Quintuple.SetActive(true);
        }
    }



    public void GuildEnter() 
    {
        Init_Dictionary();

        Guild_In_GetGuildData();
        Guild_In_GetAddQuest();
        Guild_In_DailyQuest();

        AddBackAction(() => Guild_Out_SaveGuildData());


        //if (NPC_Dict[2000].OptionList.Contains(100) == false)
        //{
        //    NPC_Dict[2000].OptionList.Add(100);
        //}

        //Debug.Log("Ư�� ����Ʈ : " + EventManager.Instance.AddQuest_Special.Count);
        //foreach (var item in EventManager.Instance.AddQuest_Special)
        //{
        //    Debug.Log("Ư�� ����Ʈ : " + item);
        //}
        //Debug.Log("���ϸ� ����Ʈ : " + EventManager.Instance.AddQuest_Daily.Count);
        //foreach (var item in EventManager.Instance.AddQuest_Daily)
        //{
        //    Debug.Log("���ϸ� ����Ʈ : " + item);
        //}
    }


    Dictionary<int, Interaction_Guild> NPC_Dict = new Dictionary<int, Interaction_Guild>();


    void Init_Dictionary()
    {
        var npc = FindObjectsOfType<Interaction_Guild>();

        foreach (var item in npc)
        {
            if (item.isActiveAndEnabled)
            {
                NPC_Dict.Add(item.Original_Index, item);
            }
        }
    }

    public Interaction_Guild GetInteraction(int id)
    {
        Interaction_Guild target = null;

        if (NPC_Dict.TryGetValue(id, out target))
        {
            return target;
        }
        return null;
    }


    public Action DungeonBackAction { get; set; }

    public void AddBackAction(Action action)
    {
        DungeonBackAction += action;
    }


    public List<GuildNPC_Data> SaveGuildData { get; set; }

    void Guild_In_GetGuildData()
    {
        //SaveGuildData = EventManager.Instance.CurrentGuildData;
        SaveGuildData = new List<GuildNPC_Data>();

        if (EventManager.Instance.CurrentGuildData != null)
        {
            foreach (var item in EventManager.Instance.CurrentGuildData)
            {
                SaveGuildData.Add(item.DeepCopy());
            }
        }

        foreach (var _saveNPC in SaveGuildData)
        {
            var currentNPC = GetInteraction(_saveNPC.Original_Index);
            if (currentNPC != null)
            {
                currentNPC.InstanceQuestList = _saveNPC.InstanceQuestList;
                currentNPC.OptionList = _saveNPC.OptionList;
            }
        }
    }
    void Guild_In_GetAddQuest()
    {
        var removeList = new List<int>();

        foreach (var newQuest in EventManager.Instance.AddQuest_Special)
        {
            int id = (newQuest / 1000) * 1000;
            int questIndex = newQuest - id;

            var interaction = GetInteraction(id);
            if (interaction != null)
            {
                interaction.AddQuest(questIndex);
                removeList.Add(newQuest);
            }
        }

        foreach (var item in removeList)
        {
            EventManager.Instance.AddQuest_Special.Remove(item);
        }
    }

    void Guild_In_DailyQuest()
    {
        var removeList = new List<int>();

        foreach (var dailyQuest in EventManager.Instance.AddQuest_Daily)
        {
            int id = (dailyQuest / 1000) * 1000;
            int questIndex = dailyQuest - id;

            var interaction = GetInteraction(id);
            if (interaction != null)
            {
                if (interaction.OptionList.Contains(questIndex) == false)
                {
                    interaction.OptionList.Add(questIndex);
                    removeList.Add(dailyQuest);
                }
            }
        }


        EventManager.Instance.AddQuest_Daily.Clear();
    }




    void Guild_Out_SaveGuildData()
    {
        var newList = new List<GuildNPC_Data>();

        var removeList = new List<GuildNPC_Data>();

        //? ������ ���� �����Ϳ� ���� ������ �������� index�� ������ ����, ������ �߰�
        foreach (var oldData in SaveGuildData)
        {
            if (NPC_Dict.ContainsKey(oldData.Original_Index))
            {
                var newData = new GuildNPC_Data();
                newData.SetData(oldData.Original_Index, NPC_Dict[oldData.Original_Index].InstanceQuestList, NPC_Dict[oldData.Original_Index].OptionList);
                newList.Add(newData);

                //? ������ foreach���� �������� �ؾߴ�
                removeList.Add(oldData);
            }
            else
            {
                newList.Add(oldData);
            }
        }

        foreach (var item in removeList)
        {
            SaveGuildData.Remove(item);
            NPC_Dict.Remove(item.Original_Index);
        }

        

        //? ������ ���嵥���Ϳ� ���� ���� �߰��� npc�� ������
        foreach (var npc in NPC_Dict)
        {
            var data = new GuildNPC_Data();
            data.SetData(npc.Value.Original_Index, npc.Value.InstanceQuestList, npc.Value.OptionList);

            newList.Add(data);
        }

        //var data1 = Managers.Data.TestLoadFile($"DM_Save_{24}").eventData;
        EventManager.Instance.CurrentGuildData = newList;
        //var data2 = Managers.Data.TestLoadFile($"DM_Save_{24}").eventData;
    }
}


public class GuildNPC_Data
{
    // id
    public int Original_Index;

    // �켱���� ����Ʈ ����Ʈ
    public List<int> InstanceQuestList;
    // ������ �ɼ� ����Ʈ
    public List<int> OptionList;


    public void SetData(int _id, List<int> _questList, List<int> _optionList)
    {
        Original_Index = _id;
        InstanceQuestList = new List<int>(_questList);
        OptionList = new List<int>(_optionList);
    }


    public GuildNPC_Data DeepCopy()
    {
        GuildNPC_Data newData = new GuildNPC_Data();

        newData.Original_Index = Original_Index;

        List<int> tempA = new List<int>();
        foreach (var item in InstanceQuestList)
        {
            tempA.Add(item);
        }

        List<int> tempB = new List<int>();
        foreach (var item in OptionList)
        {
            tempB.Add(item);
        }

        newData.InstanceQuestList = tempA;
        newData.OptionList = tempB;

        return newData;
    }
}

public enum Guild_DayOption
{
    Always = 0,
    Odd = 1,
    Even = 2,
    Multiple_3 = 30,
    Multiple_4 = 40,
    Multiple_5 = 50,
    //Multiple_6 = 60,
    Multiple_7 = 70,
}