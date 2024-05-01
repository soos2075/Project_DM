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


        //Register_Monster[3].isRegist = true;
        //Register_Facility[1].isRegist = true;
        //Register_NPC[1].isRegist = true;
        //Register_Technical[1].isRegist = true;

        Managers.Data.LoadClearData();
    }



    public SO_Facility[] FacilityData { get; private set; }
    public SO_Monster[] MonsterData { get; private set; }
    public SO_NPC[] NpcData { get; private set; }
    public SO_Technical[] TechnicalData { get; private set; }
    public SO_Ending[] EndingData { get; private set; }


    void Init_Data()
    {
        FacilityData = Resources.LoadAll<SO_Facility>("Data/Facility");
        MonsterData = Resources.LoadAll<SO_Monster>("Data/Monster");
        NpcData = Resources.LoadAll<SO_NPC>("Data/NPC");
        TechnicalData = Resources.LoadAll<SO_Technical>("Data/Technical");
        EndingData = Resources.LoadAll<SO_Ending>("Data/Ending");
    }


    public List<CollectionUnitRegist<SO_Monster>> Register_Monster;
    public List<CollectionUnitRegist<SO_Facility>> Register_Facility;
    public List<CollectionUnitRegist<SO_NPC>> Register_NPC;
    public List<CollectionUnitRegist<SO_Technical>> Register_Technical;
    public List<CollectionUnitRegist<SO_Ending>> Register_Ending;

    void Init_Register()
    {
        Register_Monster = new List<CollectionUnitRegist<SO_Monster>>();
        for (int i = 0; i < MonsterData.Length; i++)
        {
            Register_Monster.Add(new CollectionUnitRegist<SO_Monster>(MonsterData[i], false));
        }

        Register_Facility = new List<CollectionUnitRegist<SO_Facility>>();
        for (int i = 0; i < FacilityData.Length; i++)
        {
            Register_Facility.Add(new CollectionUnitRegist<SO_Facility>(FacilityData[i], false));
        }

        Register_NPC = new List<CollectionUnitRegist<SO_NPC>>();
        for (int i = 0; i < NpcData.Length; i++)
        {
            Register_NPC.Add(new CollectionUnitRegist<SO_NPC>(NpcData[i], false));
        }

        Register_Technical = new List<CollectionUnitRegist<SO_Technical>>();
        for (int i = 0; i < TechnicalData.Length; i++)
        {
            Register_Technical.Add(new CollectionUnitRegist<SO_Technical>(TechnicalData[i], false));
        }

        Register_Ending = new List<CollectionUnitRegist<SO_Ending>>();
        for (int i = 0; i < EndingData.Length; i++)
        {
            Register_Ending.Add(new CollectionUnitRegist<SO_Ending>(EndingData[i], false));
        }
    }




    public void LoadCollectionData(Dictionary<int, bool> data)
    {
        foreach (var item in Register_Monster)
        {
            bool isRegist = false;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.isRegist = isRegist;
            }
        }
        foreach (var item in Register_Facility)
        {
            bool isRegist = false;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.isRegist = isRegist;
            }
        }
        foreach (var item in Register_NPC)
        {
            bool isRegist = false;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.isRegist = isRegist;
            }
        }
        foreach (var item in Register_Technical)
        {
            bool isRegist = false;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.isRegist = isRegist;
            }
        }
        foreach (var item in Register_Ending)
        {
            bool isRegist = false;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.isRegist = isRegist;
            }
        }
    }

    public Dictionary<int, bool> SaveCollectionData()
    {
        var Register = new Dictionary<int, bool>();
        for (int i = 0; i < Register_Facility.Count; i++)
        {
            Register.Add(Register_Facility[i].unit.id, Register_Facility[i].isRegist);
        }
        for (int i = 0; i < Register_Monster.Count; i++)
        {
            Register.Add(Register_Monster[i].unit.id, Register_Monster[i].isRegist);
        }
        for (int i = 0; i < Register_NPC.Count; i++)
        {
            Register.Add(Register_NPC[i].unit.id, Register_NPC[i].isRegist);
        }
        for (int i = 0; i < Register_Technical.Count; i++)
        {
            Register.Add(Register_Technical[i].unit.id, Register_Technical[i].isRegist);
        }
        for (int i = 0; i < Register_Ending.Count; i++)
        {
            Register.Add(Register_Ending[i].unit.id, Register_Ending[i].isRegist);
        }

        return Register;
    }


    public class CollectionUnitRegist<T> where T : ScriptableObject
    {
        public T unit;
        public bool isRegist;
        //? 세부정보가 필요하면 여기 추가하면됨(진화조건, 최대스탯, 기타등등)

        public CollectionUnitRegist(T _so_Data, bool _regist)
        {
            unit = _so_Data;
            isRegist = _regist;
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
    public MultiplayData PlayData { get; set; }


    public class MultiplayData
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

        public MultiplayData()
        {
            MonsterList = new Save_MonsterData[3];
            BonusList = new string[3];

            monsterCount = 0;
            bonusCount = 0;
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



    public MultiplayData SaveMultiData()
    {
        if (PlayData != null)
        {
            return PlayData;
        }
        else
        {
            return null;
        }
    }

    public void LoadMultiData(MultiplayData Data)
    {
        PlayData = Data;
    }


    #endregion


}


