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

        //Managers.Data.LoadClearData();
    }



    SO_Facility[] FacilityData;
    SO_Monster[] MonsterData;
    SO_NPC[] NpcData;
    SO_Technical[] TechnicalData;
    SO_Artifact[] ArtifactData;
    SO_Trait[] TraitData;
    SO_BattleStatus[] BattleStatusData;
    SO_Title[] TitleData;
    SO_Ending[] EndingData;


    void Init_Data()
    {
        FacilityData = Resources.LoadAll<SO_Facility>("Data/Facility");
        MonsterData = Resources.LoadAll<SO_Monster>("Data/Monster");
        NpcData = Resources.LoadAll<SO_NPC>("Data/NPC");
        TechnicalData = Resources.LoadAll<SO_Technical>("Data/Technical");
        ArtifactData = Resources.LoadAll<SO_Artifact>("Data/Artifact");
        TraitData = Resources.LoadAll<SO_Trait>("Data/Trait");
        BattleStatusData = Resources.LoadAll<SO_BattleStatus>("Data/BattleStatus");
        TitleData = Resources.LoadAll<SO_Title>("Data/Title");
        EndingData = Resources.LoadAll<SO_Ending>("Data/Ending");


        Array.Sort(FacilityData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(MonsterData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(NpcData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(TechnicalData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(ArtifactData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(TraitData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(BattleStatusData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(TitleData, (a, b) => a.id.CompareTo(b.id));
        Array.Sort(EndingData, (a, b) => a.id.CompareTo(b.id));

        foreach (var mon in MonsterData)
        {
            mon.traitList_Exp.Sort((a, b) => a.CompareTo(b));
        }
    }



    public List<CollectionUnitRegist<SO_Monster>> Register_Monster { get; private set; }
    public List<CollectionUnitRegist<SO_Facility>> Register_Facility { get; private set; }
    public List<CollectionUnitRegist<SO_NPC>> Register_NPC { get; private set; }

    public List<CollectionUnitRegist<SO_Technical>> Register_Technical { get; private set; }
    public List<CollectionUnitRegist<SO_Artifact>> Register_Artifact { get; private set; }
    public List<CollectionUnitRegist<SO_Trait>> Register_Trait { get; private set; }
    public List<CollectionUnitRegist<SO_BattleStatus>> Register_BattleStatus { get; private set; }
    public List<CollectionUnitRegist<SO_Title>> Register_Title { get; private set; }


    public void DataResetAndNewGame()
    {
        Init_Register();
    }


    void Init_Register()
    {
        Register_Monster = new List<CollectionUnitRegist<SO_Monster>>();
        for (int i = 0; i < MonsterData.Length; i++)
        {
            if (MonsterData[i].View_Collection == false) continue;
            Register_Monster.Add(new CollectionUnitRegist<SO_Monster>(MonsterData[i], new Regist_Info(), i+1));
        }

        Register_Facility = new List<CollectionUnitRegist<SO_Facility>>();
        for (int i = 0; i < FacilityData.Length; i++)
        {
            if (FacilityData[i].View_Collection == false) continue;
            if (FacilityData[i].id < 0) continue;
            Register_Facility.Add(new CollectionUnitRegist<SO_Facility>(FacilityData[i], new Regist_Info(), i+1));
        }

        Register_NPC = new List<CollectionUnitRegist<SO_NPC>>();
        for (int i = 0; i < NpcData.Length; i++)
        {
            if (NpcData[i].View_Collection == false) continue;
            var regist = new Regist_Info();
            regist.Set_LevelCap(NpcData[i].levelCaps);
            Register_NPC.Add(new CollectionUnitRegist<SO_NPC>(NpcData[i], regist, i + 1));
        }

        Register_Technical = new List<CollectionUnitRegist<SO_Technical>>();
        for (int i = 0; i < TechnicalData.Length; i++)
        {
            if (TechnicalData[i].View_Collection == false) continue;
            Register_Technical.Add(new CollectionUnitRegist<SO_Technical>(TechnicalData[i], new Regist_Info(), i+1));
        }

        Register_Artifact = new List<CollectionUnitRegist<SO_Artifact>>();
        for (int i = 0; i < ArtifactData.Length; i++)
        {
            if (ArtifactData[i].View_Collection == false) continue;
            Register_Artifact.Add(new CollectionUnitRegist<SO_Artifact>(ArtifactData[i], new Regist_Info(), i + 1));
        }

        Register_Trait = new List<CollectionUnitRegist<SO_Trait>>();
        for (int i = 0; i < TraitData.Length; i++)
        {
            Register_Trait.Add(new CollectionUnitRegist<SO_Trait>(TraitData[i], new Regist_Info(), i + 1));
        }

        Register_BattleStatus = new List<CollectionUnitRegist<SO_BattleStatus>>();
        for (int i = 0; i < BattleStatusData.Length; i++)
        {
            Register_BattleStatus.Add(new CollectionUnitRegist<SO_BattleStatus>(BattleStatusData[i], new Regist_Info(), i + 1));
        }

        Register_Title = new List<CollectionUnitRegist<SO_Title>>();
        for (int i = 0; i < TitleData.Length; i++)
        {
            //if (TraitData[i].isCollected == false) continue;
            Register_Title.Add(new CollectionUnitRegist<SO_Title>(TitleData[i], new Regist_Info(), i + 1));
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
        foreach (var item in Register_Artifact)
        {
            if (item.unit == SO_Data) return item as CollectionUnitRegist<T>;
        }
        foreach (var item in Register_Trait)
        {
            if (item.unit == SO_Data) return item as CollectionUnitRegist<T>;
        }
        foreach (var item in Register_BattleStatus)
        {
            if (item.unit == SO_Data) return item as CollectionUnitRegist<T>;
        }
        foreach (var item in Register_Title)
        {
            if (item.unit == SO_Data) return item as CollectionUnitRegist<T>;
        }

        return null;
    }

    //? 컬렉션 데이터 확인용인데 필요없는 항목들도 있음 (상태이상같은거)
    public CollectionUnitRegist<SO_Monster> Get_Collection_Monster(string _keyName)
    {
        foreach (var item in Register_Monster) { if (item.unit.keyName == _keyName) return item; }
        return null;
    }
    public CollectionUnitRegist<SO_Facility> Get_Collection_Facility(string _keyName)
    {
        foreach (var item in Register_Facility) { if (item.unit.keyName == _keyName) return item; }
        return null;
    }
    public CollectionUnitRegist<SO_NPC> Get_Collection_NPC(string _keyName)
    {
        foreach (var item in Register_NPC) { if (item.unit.keyName == _keyName) return item; }
        return null;
    }
    public CollectionUnitRegist<SO_Technical> Get_Collection_Technical(string _keyName)
    {
        foreach (var item in Register_Technical) { if (item.unit.keyName == _keyName) return item; }
        return null;
    }
    public CollectionUnitRegist<SO_Artifact> Get_Collection_Artifact(string _keyName)
    {
        foreach (var item in Register_Artifact) { if (item.unit.keyName == _keyName) return item; }
        return null;
    }
    public CollectionUnitRegist<SO_Trait> Get_Collection_Trait(string _keyName)
    {
        foreach (var item in Register_Trait) { if (item.unit.traitName == _keyName) return item; }
        return null;
    }
    public CollectionUnitRegist<SO_Title> Get_Collection_Title(string _keyName)
    {
        foreach (var item in Register_Title) { if (item.unit.keyName == _keyName) return item; }
        return null;
    }


    //public CollectionUnitRegist<T> Get_Collection_KeyName<T>(string _keyName) where T : ScriptableObject, I_SO_Collection
    //{

    //    foreach (var item in Register_Monster)
    //    {
    //        if (item.unit.keyName == _keyName) return item as CollectionUnitRegist<T>;
    //    }
    //    foreach (var item in Register_Facility)
    //    {
    //        if (item.unit.keyName == _keyName) return item as CollectionUnitRegist<T>;
    //    }
    //    foreach (var item in Register_NPC)
    //    {
    //        if (item.unit.keyName == _keyName) return item as CollectionUnitRegist<T>;
    //    }
    //    foreach (var item in Register_Technical)
    //    {
    //        if (item.unit.keyName == _keyName) return item as CollectionUnitRegist<T>;
    //    }
    //    foreach (var item in Register_Artifact)
    //    {
    //        if (item.unit.keyName == _keyName) return item as CollectionUnitRegist<T>;
    //    }
    //    foreach (var item in Register_Trait)
    //    {
    //        if (item.unit.traitName == _keyName) return item as CollectionUnitRegist<T>;
    //    }
    //    //foreach (var item in Register_BattleStatus)
    //    //{
    //    //    if (item.unit.id == _keyName) return item as CollectionUnitRegist<T>;
    //    //}
    //    foreach (var item in Register_Title)
    //    {
    //        if (item.unit.keyName == _keyName) return item as CollectionUnitRegist<T>;
    //    }

    //    return null;
    //}




    #region 세이브 / 로드  // Trait이랑 BattleStatus같은건 딱히 점수가 없어서 저장/세이브도 필요없음
    public void LoadCollectionData(Dictionary<int, Regist_Info> data)
    {
        foreach (var item in Register_Monster)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.Apply_Info(isRegist.DeepCopy());
            }
        }
        foreach (var item in Register_Facility)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.Apply_Info(isRegist.DeepCopy());
            }
        }
        foreach (var item in Register_NPC)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.Apply_Info(isRegist.DeepCopy());
            }
        }
        foreach (var item in Register_Technical)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id, out isRegist))
            {
                item.Apply_Info(isRegist.DeepCopy());
            }
        }
        foreach (var item in Register_Artifact)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id + 10000, out isRegist))
            {
                item.Apply_Info(isRegist.DeepCopy());
            }
        }
        //foreach (var item in Register_Trait)
        //{
        //    Regist_Info isRegist;
        //    if (data.TryGetValue(item.unit.id + 20000, out isRegist))
        //    {
        //        item.Apply_Info(isRegist.DeepCopy());
        //    }
        //}
        foreach (var item in Register_Title)
        {
            Regist_Info isRegist;
            if (data.TryGetValue(item.unit.id + 30000, out isRegist))
            {
                item.Apply_Info(isRegist.DeepCopy());
            }
        }
    }

    public Dictionary<int, Regist_Info> SaveCollectionData()
    {
        var Register = new Dictionary<int, Regist_Info>();
        for (int i = 0; i < Register_Facility.Count; i++)
        {
            Register.Add(Register_Facility[i].unit.id, Register_Facility[i].info.DeepCopy());
        }

        for (int i = 0; i < Register_Monster.Count; i++)
        {
            Register.Add(Register_Monster[i].unit.id, Register_Monster[i].info.DeepCopy());
        }

        for (int i = 0; i < Register_NPC.Count; i++)
        {
            Register.Add(Register_NPC[i].unit.id, Register_NPC[i].info.DeepCopy());
        }

        for (int i = 0; i < Register_Technical.Count; i++)
        {
            Register.Add(Register_Technical[i].unit.id, Register_Technical[i].info.DeepCopy());
        }

        //? 아래부터는 ID 값이 겹치는게 있어서 ID 앞에 10000~90000을 붙여주고 로드할 때 빼자
        //? Arti - 1 / Trait - 2 / Ending - 9
        for (int i = 0; i < Register_Artifact.Count; i++)
        {
            Register.Add(Register_Artifact[i].unit.id + 10000, Register_Artifact[i].info.DeepCopy());
        }

        //for (int i = 0; i < Register_Trait.Count; i++)
        //{
        //    Register.Add(Register_Trait[i].unit.id + 20000, Register_Trait[i].info.DeepCopy());
        //}

        for (int i = 0; i < Register_Title.Count; i++)
        {
            Register.Add(Register_Title[i].unit.id + 30000, Register_Title[i].info.DeepCopy());
        }

        return Register;
    }
    #endregion





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

    public SO_Ending GetData_Ending(Endings _keyName)
    {
        SO_Ending content = null;
        if (Ending_Dictionary.TryGetValue(_keyName.ToString(), out content))
        {
            return content;
        }

        Debug.Log($"{_keyName}: Data Not Exist");
        return null;
    }

    #endregion


    #region Multi Play

    //public void GameClear(DataManager.SaveData data)
    //{
    //    if (RoundClearData == null)
    //    {
    //        RoundClearData = new RoundData();
    //    }

    //    RoundClearData.Init_Data(data);
    //    Managers.Data.SaveClearData();

    //    //? 컬렉션 데이터 업데이트
    //    Managers.Data.SaveCollectionData();
    //}




    //public RoundData RoundClearData { get; set; }


    //? 나중에 이거랑 아래 클리어로그 항목 전체 다 삭제하면 댐 (업데이트 미리 공지는 때려야할듯)
    public class RoundData
    {
        //? 클리어 횟수
        public int clearCounter;
        //? 최고 클리어 난이도 (0~4)
        public int highestDifficultyLevel;

        //? 엔딩별 클리어 카운트
        public Dictionary<Endings, int> endingClearCount;

        ////? 시작 몬스터 해금조건 - 진화 / 컬렉션포인트 / 엔딩 등 특정 조건 달성하기
        ////public Dictionary<StartUnit, bool> startUnitList;

        ////? 시작 아티팩트 해금조건 - 이하동문
        ////public Dictionary<StartArtifact, bool> startArtifactList;


        //? 클리어 회차의 데이터
        public Dictionary<int, ClearDataLog> dataLog;


        public RoundData()
        {
            endingClearCount = new Dictionary<Endings, int>();
            dataLog = new Dictionary<int, ClearDataLog>();

            //endingClearCount.Add(Endings.Dog, 0);
            //endingClearCount.Add(Endings.Dragon, 0);
            //endingClearCount.Add(Endings.Ravi, 0);
            //endingClearCount.Add(Endings.Cat, 0);
            //endingClearCount.Add(Endings.Demon, 0);
            //endingClearCount.Add(Endings.Hero, 0);

            foreach (Endings value in Enum.GetValues(typeof(Endings)))
            {
                endingClearCount.Add(value, 0);
            }
        }

        public ClearDataLog Get_Datalog(SO_Ending endings)
        {
            foreach (var item in dataLog)
            {
                if (endings.keyName == item.Value.endings.ToString())
                {
                    return item.Value;
                }
            }
            return null;
        }




        public int GetClearPoint()
        {
            int point = 0;

            point += clearCounter * 5;
            point += highestDifficultyLevel * 10;

            int anotherEnding = 0;
            foreach (var item in endingClearCount)
            {
                if (item.Value > 0)
                {
                    anotherEnding++;
                }
            }

            point += anotherEnding * 10;

            return point;
        }

        public bool EndingClearCheck(Endings ending)
        {
            if (endingClearCount[ending] > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public void Init_Data(DataManager.SaveData saveData)
        {
            //? 만약 이미 같은 ID로 클리어 데이터가 존재한다면 클리어 기록만 업데이트하고 나머지는 스킵 / 반대로 말하면 클리어 보상은 최초 한번만 인정
            //? 2024 1212 위와 같은 경우로 설정을 해놨는데, 만약 다른 엔딩을 하나의 파일로 보는 경우가 꽤 있음 (세이브 로드로 엔딩작만 하는 사람)
            //? 이런 경우 다른 엔딩이면 인정을 해주는것도 나쁘지 않을듯.
            //? 대신 클리어 카운터는 올리지 않고, 딱 엔딩만 추가하는걸로
            ClearDataLog result = null;
            if (dataLog.TryGetValue(saveData.savefileConfig.fileID, out result))
            {
                if (saveData.endgins == result.endings)
                {
                    Update_LogData(saveData);
                    return;
                }
                else
                {
                    Update_LogData(saveData);
                    Add_Ending(saveData);
                    return;
                }
            }

            clearCounter++;
            Add_LogData(saveData);
            OverwriteDifficulty(saveData);
            Add_Ending(saveData);
        }


        void Add_LogData(DataManager.SaveData saveData)
        {
            var clear = new ClearDataLog();
            clear.Set_Data(saveData);
            dataLog.Add(saveData.savefileConfig.fileID, clear);
        }
        void Update_LogData(DataManager.SaveData saveData)
        {
            var clear = new ClearDataLog();
            clear.Set_Data(saveData);
            dataLog[saveData.savefileConfig.fileID] = clear;
        }

        void OverwriteDifficulty(DataManager.SaveData saveData)
        {
            var dif = saveData.savefileConfig.Difficulty;
            if (highestDifficultyLevel < (int)dif)
            {
                highestDifficultyLevel = (int)dif;
            }
        }
        void Add_Ending(DataManager.SaveData saveData)
        {
            endingClearCount[saveData.endgins] += 1;
        }

    }

    

    public class ClearDataLog
    {
        public int mana;
        public int gold;

        public int rank;
        public int pop;
        public int danger;

        public int visit;
        //public int satisfaction;
        //public int return_Empty;
        //public int kill;
        //public int prisoner;


        public float clearTime;
        public Endings endings;

        public int difficultyLevel;

        //public int monsterCount;
        //public string highestMonster;
        //public int highestMonsterLv;

        public void Set_Data(DataManager.SaveData data)
        {
            endings = data.endgins;
            difficultyLevel = data.difficultyLevel;
            clearTime = data.playTimes;
        }
    }



    //public RoundData SaveMultiData()
    //{
    //    if (RoundClearData != null)
    //    {
    //        return RoundClearData;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    //public void LoadMultiData(RoundData Data)
    //{
    //    RoundClearData = Data;
    //}


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


    public int[] levelCap;

    public Regist_Info()
    {
        levelCap = new int[5] { 5, 15, 30, 50, 100 };
    }
    //public Regist_Info(bool regist, int count, bool level1, bool level2, bool level3, bool level4, bool level5)
    //{
    //    isRegist = regist;

    //    UnlockPoint = count;

    //    level_1_Unlock = level1;
    //    level_2_Unlock = level2;
    //    level_3_Unlock = level3;
    //    level_4_Unlock = level4;
    //    level_5_Unlock = level5;
    //}

    public Regist_Info DeepCopy()
    {
        Regist_Info newInfo = (Regist_Info)this.MemberwiseClone();
        return newInfo;
    }


    public void Set_LevelCap(int[] caps)
    {
        if (caps == null || caps.Length == 0)
        {
            return;
        }
        levelCap = caps;
    }

    public void Unlocking()
    {
        if (UnlockPoint > 0)
        {
            isRegist = true;
        }
        if (UnlockPoint >= levelCap[0])
        {
            level_1_Unlock = true;
        }
        if (UnlockPoint >= levelCap[1])
        {
            level_2_Unlock = true;
        }
        if (UnlockPoint >= levelCap[2])
        {
            level_3_Unlock = true;
        }
        if (UnlockPoint >= levelCap[3])
        {
            level_4_Unlock = true;
        }
        if (UnlockPoint >= levelCap[4])
        {
            level_5_Unlock = true;
        }
    }
}
