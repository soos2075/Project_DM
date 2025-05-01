using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildManager : MonoBehaviour
{
    #region Singleton
    private static GuildManager _instance;
    public static GuildManager Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<GuildManager>();
            if (_instance == null)
            {
                GameObject go = new GameObject { name = "@GuildManager" };
                _instance = go.AddComponent<GuildManager>();
            }
            DontDestroyOnLoad(_instance);
        }
    }


    private void Awake()
    {
        Initialize();
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        Init_LocalData();
    }

    #endregion




    void Start()
    {
        //Init_LocalData();
    }


    #region SO_Data

    SO_Guild_NPC[] so_data;
    Dictionary<int, SO_Guild_NPC> Guild_Dictionary { get; set; }
    void Init_LocalData()
    {
        Guild_Dictionary = new Dictionary<int, SO_Guild_NPC>();
        so_data = Resources.LoadAll<SO_Guild_NPC>("Data/Guild");

        foreach (var item in so_data)
        {
            Guild_Dictionary.Add(item.Original_Index, item);
        }

        Init_CurrentGuildData();
    }


    public void Init_CurrentGuildData()
    {
        EventManager.Instance.CurrentGuildData = new List<GuildNPC_Data>();
        foreach (var item in so_data)
        {
            var data = new GuildNPC_Data();
            data.SetData(item.Original_Index, item.InstanceQuestList, item.OptionList, new List<int>());
            EventManager.Instance.CurrentGuildData.Add(data);
        }
    }


    public SO_Guild_NPC GetData(int _id)
    {
        SO_Guild_NPC npc = null;
        if (Guild_Dictionary.TryGetValue(_id, out npc))
        {
            return npc;
        }

        Debug.Log($"{_id}: Data Not Exist");
        return null;
    }


    public SO_Guild_NPC[] GetDataAll()
    {
        return so_data;
    }

    #endregion




    #region Save
    public HashSet<int> Data_SaveInstanceNPC()
    {
        HashSet<int> saveData = new HashSet<int>();

        foreach (var item in Instance_GuildNPC)
        {
            saveData.Add((int)item);
        }
        return saveData;
    }
    public HashSet<int> Data_SaveDeleteNPC()
    {
        HashSet<int> saveData = new HashSet<int>();

        foreach (var item in Delete_GuildNPC)
        {
            saveData.Add((int)item);
        }
        return saveData;
    }


    public void Data_LoadInstanceNPC(HashSet<int> npcs)
    {
        Instance_GuildNPC = new HashSet<GuildNPC_LabelName>();

        if (npcs == null) return;

        foreach (var item in npcs)
        {
            Instance_GuildNPC.Add((GuildNPC_LabelName)item);
        }
    }
    public void Data_LoadDeleteNPC(HashSet<int> npcs)
    {
        Delete_GuildNPC = new HashSet<GuildNPC_LabelName>();

        if (npcs == null) return;

        foreach (var item in npcs)
        {
            Delete_GuildNPC.Add((GuildNPC_LabelName)item);
        }
    }


    #endregion



    #region Guild Scene

    public void NewGameReset()
    {
        Instance_GuildNPC = new HashSet<GuildNPC_LabelName>();
        Delete_GuildNPC = new HashSet<GuildNPC_LabelName>();
    }





    public void GuildEnter()
    {
        UserData.Instance.GamePlay_Normal();
        //Guild_In_GetGuildData();
        Guild_In_DailyQuest();

        //AddBackAction(() => Guild_Out_SaveGuildData());
    }

    //Dictionary<int, Interaction_Guild> NPC_Active_Dict;

    //? 길드에 추가될 NPC 리스트
    public HashSet<GuildNPC_LabelName> Instance_GuildNPC = new HashSet<GuildNPC_LabelName>();



    public void AddInstanceGuildNPC(GuildNPC_LabelName npc)
    {
        Instance_GuildNPC.Add(npc);
    }
    public void RemoveInstanceGuildNPC(GuildNPC_LabelName npc)
    {
        Instance_GuildNPC.Remove(npc);
    }

    //? 길드에 있으면 안되는 NPC 리스트 - 위에것보다 이걸 우선시함
    public HashSet<GuildNPC_LabelName> Delete_GuildNPC = new HashSet<GuildNPC_LabelName>();
    public void AddDeleteGuildNPC(GuildNPC_LabelName npc)
    {
        Delete_GuildNPC.Add(npc);
    }
    public void RemoveDeleteGuildNPC(GuildNPC_LabelName npc)
    {
        Delete_GuildNPC.Remove(npc);
    }



    public GuildNPC_Data GetInteraction(int id)
    {
        GuildNPC_Data target = null;

        foreach (var item in EventManager.Instance.CurrentGuildData)
        {
            if (item.Original_Index == id)
            {
                target = item;
            }
        }

        return target;
    }


    public Action DungeonBackAction { get; set; }

    public void AddBackAction(Action action)
    {
        DungeonBackAction += action;
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




    //void Guild_Out_SaveGuildData()
    //{
    //    var newList = new List<GuildNPC_Data>();

    //    var removeList = new List<GuildNPC_Data>();

    //    //? 기존의 저장 데이터와 새로 저장할 데이터의 index가 같으면 갱신, 없으면 추가
    //    foreach (var oldData in SaveGuildData)
    //    {
    //        if (NPC_Active_Dict.ContainsKey(oldData.Original_Index))
    //        {
    //            var newData = new GuildNPC_Data();
    //            newData.SetData(oldData.Original_Index, NPC_Active_Dict[oldData.Original_Index].InstanceQuestList, NPC_Active_Dict[oldData.Original_Index].OptionList);
    //            newList.Add(newData);

    //            //? 삭제는 foreach문이 끝난다음 해야댐
    //            removeList.Add(oldData);
    //        }
    //        else
    //        {
    //            newList.Add(oldData);
    //        }
    //    }

    //    foreach (var item in removeList)
    //    {
    //        SaveGuildData.Remove(item);
    //        NPC_Active_Dict.Remove(item.Original_Index);
    //    }



    //    //? 기존의 저장데이터에 없는 새로 추가된 npc의 데이터
    //    foreach (var npc in NPC_Active_Dict)
    //    {
    //        var data = new GuildNPC_Data();
    //        data.SetData(npc.Value.Original_Index, npc.Value.InstanceQuestList, npc.Value.OptionList);

    //        newList.Add(data);
    //    }

    //    //var data1 = Managers.Data.TestLoadFile($"DM_Save_{24}").eventData;
    //    EventManager.Instance.CurrentGuildData = newList;
    //    //var data2 = Managers.Data.TestLoadFile($"DM_Save_{24}").eventData;
    //}

    #endregion


}
[Serializable]
public class GuildNPC_Data
{
    // id
    public int Original_Index;

    // 우선순위 퀘스트 리스트
    public List<int> InstanceQuestList;
    // 선택지 옵션 리스트
    public List<int> OptionList;

    //? 이미 클리어한 퀘스트 목록
    public List<int> AlreadyClearList;

    //public GuildNPC_Data(int id, List<int> startQuest, List<int> startOption)
    //{
    //    Original_Index = id;
    //    InstanceQuestList = startQuest;
    //    OptionList = startOption;
    //}

    public void Remove_Option(int index)
    {
        if (OptionList.Count - 1 < index) //? 만약 0들어왔는데 카운트 0이면 리턴
        {
            return;
        }

        if (OptionList[index] % 100 == 99)
        {
            return;
        }

        OptionList.RemoveAt(index);
    }


    public void OneTimeOptionButton()
    {
        Managers.Dialogue.OneTimeOption(OptionList, Original_Index);
    }

    public void AddQuest(int _questIndex, bool special = true)
    {
        if (special && !InstanceQuestList.Contains(_questIndex) && !AlreadyClearList.Contains(_questIndex))
        {
            InstanceQuestList.Add(_questIndex);
        }

        if (!special && !OptionList.Contains(_questIndex))
        {
            OptionList.Add(_questIndex);
        }
    }
    public void RemoveQuest(int _questIndex)
    {
        foreach (var item in InstanceQuestList)
        {
            if (item == _questIndex)
            {
                InstanceQuestList.Remove(item);
                return;
            }
        }
        foreach (var item in OptionList)
        {
            if (item == _questIndex)
            {
                OptionList.Remove(item);
                return;
            }
        }
    }
    public void ClearQuest(int _questIndex)
    {
        RemoveQuest(_questIndex);

        if (!AlreadyClearList.Contains(_questIndex))
        {
            AlreadyClearList.Add(_questIndex);
        }
    }

    public void SetData(int _id, List<int> _questList, List<int> _optionList, List<int> _clearList)
    {
        Original_Index = _id;
        InstanceQuestList = new List<int>(_questList);
        OptionList = new List<int>(_optionList);
        AlreadyClearList = new List<int>(_clearList);
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

        List<int> tempC = new List<int>();
        foreach (var item in AlreadyClearList)
        {
            tempC.Add(item);
        }

        newData.InstanceQuestList = tempA;
        newData.OptionList = tempB;
        newData.AlreadyClearList = tempC;

        return newData;
    }
}

public enum Guild_DayOption
{
    Special = -1, //? 원래는 없는데 따로 조건으로 등장하는 NPC
    Always = 0,
    Odd = 1,
    Even = 2,
    Multiple_3 = 30,
    Multiple_4 = 40,
    Multiple_5 = 50,
    //Multiple_6 = 60,
    Multiple_7 = 70,
}

public enum GuildNPC_LabelName
{
    QuestZone = 1000,

    StaffA = 2000,
    StaffB = 3000,
    Heroine = 4000,

    DogGirl = 5000,

    DummyB = 6000,
    DummyC = 7000,
    DummyD = 8000,

    DeathMagician = 10000,

    Peddler = 11000,

    Lightning = 12000,

    RetiredHero = 15000,

    Soothsayer = 16000,
}