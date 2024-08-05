using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    #region Singleton
    private static CollectionManager _instance;
    public static CollectionManager Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<CollectionManager>();
            if (_instance == null)
            {
                GameObject go = new GameObject { name = "@CollectionManager" };
                _instance = go.AddComponent<CollectionManager>();
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
            else
            {
                DontDestroyOnLoad(_instance);
            }
        }

        Init_Data();
        Init_EndingData();
    }
    #endregion



    void Start()
    {
        //Init_Data();
        Init_Register();


        Managers.Data.LoadCollectionData();

        //Register_Monster[2].isRegist = true;
        //Register_Monster[3].isRegist = true;
        //Register_Facility[1].isRegist = true;
        //Register_NPC[1].isRegist = true;
        //Register_Technical[1].isRegist = true;

        Managers.Data.LoadClearData();
    }



    SO_Facility[] FacilityData;
    SO_Monster[] MonsterData;
    SO_NPC[] NpcData;
    SO_Technical[] TechnicalData;
    SO_Ending[] EndingData;


    void Init_Data()
    {
        FacilityData = Resources.LoadAll<SO_Facility>("Data/Facility");
        MonsterData = Resources.LoadAll<SO_Monster>("Data/Monster");
        NpcData = Resources.LoadAll<SO_NPC>("Data/NPC");
        TechnicalData = Resources.LoadAll<SO_Technical>("Data/Technical");
        EndingData = Resources.LoadAll<SO_Ending>("Data/Ending");


        Array.Sort(FacilityData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(MonsterData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(NpcData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(TechnicalData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(EndingData, (a, b) => a.id.CompareTo(b.id));

        foreach (var mon in MonsterData)
        {
            mon.TraitableList.Sort((a, b) => a.CompareTo(b));
        }
    }



    public List<CollectionUnitRegist<SO_Monster>> Register_Monster { get; private set; }
    public List<CollectionUnitRegist<SO_Facility>> Register_Facility { get; private set; }
    public List<CollectionUnitRegist<SO_NPC>> Register_NPC { get; private set; }
    public List<CollectionUnitRegist<SO_Technical>> Register_Technical { get; private set; }
    public List<CollectionUnitRegist<SO_Ending>> Register_Ending { get; private set; }

    void Init_Register()
    {
        Register_Monster = new List<CollectionUnitRegist<SO_Monster>>();
        for (int i = 0; i < MonsterData.Length; i++)
        {
            Register_Monster.Add(new CollectionUnitRegist<SO_Monster>(MonsterData[i], new Regist_Info(), i+1));
        }

        Register_Facility = new List<CollectionUnitRegist<SO_Facility>>();
        for (int i = 0; i < FacilityData.Length; i++)
        {
            if (FacilityData[i].id < 0) continue;
            Register_Facility.Add(new CollectionUnitRegist<SO_Facility>(FacilityData[i], new Regist_Info(), i+1));
        }

        Register_NPC = new List<CollectionUnitRegist<SO_NPC>>();
        for (int i = 0; i < NpcData.Length; i++)
        {
            Register_NPC.Add(new CollectionUnitRegist<SO_NPC>(NpcData[i], new Regist_Info(), i+1));
        }

        Register_Technical = new List<CollectionUnitRegist<SO_Technical>>();
        for (int i = 0; i < TechnicalData.Length; i++)
        {
            Register_Technical.Add(new CollectionUnitRegist<SO_Technical>(TechnicalData[i], new Regist_Info(), i+1));
        }

        Register_Ending = new List<CollectionUnitRegist<SO_Ending>>();
        for (int i = 0; i < EndingData.Length; i++)
        {
            Register_Ending.Add(new CollectionUnitRegist<SO_Ending>(EndingData[i], new Regist_Info(), i+1));
        }
    }



    public CollectionUnitRegist<T> Get_Collection<T>(T SO_Data) where T : ScriptableObject, I_SO_Collection
    {
        foreach (var item in Register_Monster)
        {
            if (item.unit == SO_Data) return item as CollectionUnitRegist<T>;
        }
        foreach (var item in Register_Facility)
        {
            if (item.unit == SO_Data) return item as CollectionUnitRegist<T>;
        }
        foreach (var item in Register_NPC)
        {
            if (item.unit == SO_Data) return item as CollectionUnitRegist<T>;
        }
        foreach (var item in Register_Technical)
        {
            if (item.unit == SO_Data) return item as CollectionUnitRegist<T>;
        }
        foreach (var item in Register_Ending)
        {
            if (item.unit == SO_Data) return item as CollectionUnitRegist<T>;
        }

        return null;
    }

    //public void Change_Collection<T>(CollectionUnitRegist<T> collection) where T : ScriptableObject
    //{

    //}


    //public void Add_Collection<T>(T SO_Data) where T : ScriptableObject
    //{
    //    foreach (var item in Register_Monster)
    //    {
    //        if (item.unit == SO_Data)
    //        {
    //            item.isRegist = true;
    //            return;
    //        }
    //    }
    //    foreach (var item in Register_Facility)
    //    {
    //        if (item.unit == SO_Data)
    //        {
    //            item.isRegist = true;
    //            return;
    //        }
    //    }
    //    foreach (var item in Register_NPC)
    //    {
    //        if (item.unit == SO_Data)
    //        {
    //            item.isRegist = true;
    //            return;
    //        }
    //    }
    //    foreach (var item in Register_Technical)
    //    {
    //        if (item.unit == SO_Data)
    //        {
    //            item.isRegist = true;
    //            return;
    //        }
    //    }
    //    foreach (var item in Register_Ending)
    //    {
    //        if (item.unit == SO_Data)
    //        {
    //            item.isRegist = true;
    //            return;
    //        }
    //    }
    //}

    //public void Add_Collection(string label_Name)
    //{
    //    foreach (var item in Register_Monster)
    //    {
    //        if (item.unit.labelName == label_Name)
    //        {
    //            item.isRegist = true;
    //            return;
    //        }
    //    }

    //}
    //public void Add_Collection(int dataIndex)
    //{
    //    foreach (var item in Register_Monster)
    //    {
    //        if (item.unit.id == dataIndex)
    //        {
    //            item.isRegist = true;
    //            return;
    //        }
    //    }
    //}






    public void LoadCollectionData(Dictionary<int, Regist_Info> data)
    {
        foreach (var item in Register_Monster)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.Apply_Info(isRegist);
            }
        }
        foreach (var item in Register_Facility)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.Apply_Info(isRegist);
            }
        }
        foreach (var item in Register_NPC)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.Apply_Info(isRegist);
            }
        }
        foreach (var item in Register_Technical)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.Apply_Info(isRegist);
            }
        }
        foreach (var item in Register_Ending)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.Apply_Info(isRegist);
            }
        }
    }

    public Dictionary<int, Regist_Info> SaveCollectionData()
    {
        var Register = new Dictionary<int, Regist_Info>();
        for (int i = 0; i < Register_Facility.Count; i++)
        {
            Register.Add(Register_Facility[i].unit.id, new Regist_Info(
                Register_Facility[i].info.isRegist,
                Register_Facility[i].info.UnlockPoint,
                Register_Facility[i].info.level_1_Unlock,
                Register_Facility[i].info.level_2_Unlock,
                Register_Facility[i].info.level_3_Unlock,
                Register_Facility[i].info.level_4_Unlock,
                Register_Facility[i].info.level_5_Unlock));
        }
        for (int i = 0; i < Register_Monster.Count; i++)
        {
            Register.Add(Register_Monster[i].unit.id, new Regist_Info(
                Register_Monster[i].info.isRegist,
                Register_Monster[i].info.UnlockPoint,
                Register_Monster[i].info.level_1_Unlock,
                Register_Monster[i].info.level_2_Unlock,
                Register_Monster[i].info.level_3_Unlock,
                Register_Monster[i].info.level_4_Unlock,
                Register_Monster[i].info.level_5_Unlock));
        }
        for (int i = 0; i < Register_NPC.Count; i++)
        {
            Register.Add(Register_NPC[i].unit.id, new Regist_Info(
                Register_NPC[i].info.isRegist,
                Register_NPC[i].info.UnlockPoint,
                Register_NPC[i].info.level_1_Unlock,
                Register_NPC[i].info.level_2_Unlock,
                Register_NPC[i].info.level_3_Unlock,
                Register_NPC[i].info.level_4_Unlock,
                Register_NPC[i].info.level_5_Unlock));
        }
        for (int i = 0; i < Register_Technical.Count; i++)
        {
            Register.Add(Register_Technical[i].unit.id, new Regist_Info(
                Register_Technical[i].info.isRegist,
                Register_Technical[i].info.UnlockPoint,
                Register_Technical[i].info.level_1_Unlock,
                Register_Technical[i].info.level_2_Unlock,
                Register_Technical[i].info.level_3_Unlock,
                Register_Technical[i].info.level_4_Unlock,
                Register_Technical[i].info.level_5_Unlock));
        }
        for (int i = 0; i < Register_Ending.Count; i++)
        {
            Register.Add(Register_Ending[i].unit.id, new Regist_Info(
                Register_Ending[i].info.isRegist,
                Register_Ending[i].info.UnlockPoint,
                Register_Ending[i].info.level_1_Unlock,
                Register_Ending[i].info.level_2_Unlock,
                Register_Ending[i].info.level_3_Unlock,
                Register_Ending[i].info.level_4_Unlock,
                Register_Ending[i].info.level_5_Unlock));
        }

        return Register;
    }


    public class CollectionUnitRegist<T> where T : I_SO_Collection
    {
        public T unit;
        public int CollectionNumber;

        public Regist_Info info;


        public CollectionUnitRegist(T _so_Data, Regist_Info _regist, int num)
        {
            unit = _so_Data;
            info = _regist;
            CollectionNumber = num;
        }

        public void Apply_Info(Regist_Info infoData)
        {
            info = infoData;
        }

        public void AddPoint(int value = 1)
        {
            info.UnlockPoint += value;
            info.Unlocking();
        }
    }



    #region SO_Ending

    Dictionary<string, SO_Ending> Ending_Dictionary;
    public void Init_EndingData()
    {
        Ending_Dictionary = new Dictionary<string, SO_Ending>();

        foreach (var item in EndingData)
        {
            Ending_Dictionary.Add(item.keyName, item);
        }
    }

    public SO_Ending GetData(string _keyName)
    {
        SO_Ending content = null;
        if (Ending_Dictionary.TryGetValue(_keyName, out content))
        {
            return content;
        }

        Debug.Log($"{_keyName}: Data Not Exist");
        return null;
    }

    #endregion


    #region Multi Play
    public RoundData RoundClearData { get; set; }


    public class RoundData
    {
        public bool dataApply;

        public int multiplayCount;

        public Save_MonsterData[] MonsterList;

        //public Save_MonsterData multiSavedMonster1;
        //public Save_MonsterData multiSavedMonster2;
        //public Save_MonsterData multiSavedMonster3;

        public string[] BonusList;

        //public string Bonus_Monster1;
        //public string Bonus_Monster2;
        //public string Bonus_Monster3;


        int monsterCount;
        int bonusCount;

        public RoundData()
        {
            MonsterList = new Save_MonsterData[3];
            BonusList = new string[3];

            monsterCount = 0;
            bonusCount = 0;
        }


        public ClearDataLog dataLog;
        public void Init_LogData(DataManager.SaveData saveData)
        {
            //? 데모말고 정식버전에서는 저장할 때 받아온 saveData의 DayResult에서 이것저것 수치를 뽑아와서 dataLog에 넣어주면 됨
        }

        public void Init_Count(int multiCount)
        {
            multiplayCount = multiCount;
        }
        public void Init_Monster(Save_MonsterData monster)
        {
            MonsterList[monsterCount] = monster;

            // 딱히 필요없는 과정일수도 있음 그냥 불러올 때 초기화 해주면 되는 부분이라
            monster.State = Monster.MonsterState.Standby;
            monster.FloorIndex = -1;
            monster.PosIndex = Vector2Int.zero;

            monsterCount++;
        }
        public void Init_Bonus(string monsterDataName)
        {
            BonusList[bonusCount] = monsterDataName;

            bonusCount++;
        }
    }

    public class ClearDataLog
    {
        public int mana;
        public int gold;


        public int visit;
        public int satisfaction;
        public int return_Empty;
        public int kill;
        //public int prisoner;


        public int rank;
        public int pop;
        public int danger;

        public float clearTime;

        public int monsterCount;
        public string highestMonster;
        public int highestMonsterLv;
    }



    public RoundData SaveMultiData()
    {
        if (RoundClearData != null)
        {
            return RoundClearData;
        }
        else
        {
            return null;
        }
    }

    public void LoadMultiData(RoundData Data)
    {
        RoundClearData = Data;
    }


    #endregion


}

public class Regist_Info
{
    public bool isRegist;

    public bool level_1_Unlock;
    public bool level_2_Unlock;
    public bool level_3_Unlock;
    public bool level_4_Unlock;
    public bool level_5_Unlock;

    //? 이 횟수에 따라 위에 레벨들을 하나씩 true로 하면 될듯. 이 unlock카운트 또한 따로 저장되어야함
    public int UnlockPoint;


    public Regist_Info()
    {

    }
    public Regist_Info(bool regist, int count, bool level1, bool level2, bool level3, bool level4, bool level5)
    {
        isRegist = regist;

        UnlockPoint = count;

        level_1_Unlock = level1;
        level_2_Unlock = level2;
        level_3_Unlock = level3;
        level_4_Unlock = level4;
        level_5_Unlock = level5;
    }

    public void Unlocking()
    {
        if (UnlockPoint > 0)
        {
            isRegist = true;
        }
        if (UnlockPoint > 5)
        {
            level_1_Unlock = true;
        }
        if (UnlockPoint > 15)
        {
            level_2_Unlock = true;
        }
        if (UnlockPoint > 30)
        {
            level_3_Unlock = true;
        }
        if (UnlockPoint > 50)
        {
            level_4_Unlock = true;
        }
        if (UnlockPoint > 100)
        {
            level_5_Unlock = true;
        }
    }
}
