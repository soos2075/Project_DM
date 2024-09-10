using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class MonsterManager
{
    public void Init()
    {
        Init_LocalData();
        Init_SLA();
        Init_MonsterSlot();

        Managers.Scene.BeforeSceneChangeAction = () => StopAllMoving();
    }

    #region SO_Data
    SO_Monster[] so_data;

    public void Init_LocalData()
    {
        so_data = Resources.LoadAll<SO_Monster>("Data/Monster");
        foreach (var item in so_data)
        {
            string[] datas = null;
            switch (UserData.Instance.Language)
            {
                case Define.Language.EN:
                    Managers.Data.ObjectsLabel_EN.TryGetValue(item.id, out datas);
                    break;

                case Define.Language.KR:
                    Managers.Data.ObjectsLabel_KR.TryGetValue(item.id, out datas);
                    break;

                case Define.Language.JP:
                    Managers.Data.ObjectsLabel_JP.TryGetValue(item.id, out datas);
                    break;
            }

            if (datas == null)
            {
                Debug.Log($"{item.id} : CSV Data Not Exist");
                continue;
            }

            item.labelName = datas[0];
            item.detail = datas[1];


            if (datas[2].Contains("@Op1::"))
            {
                string op1 = datas[2].Substring(datas[2].IndexOf("@Op1::") + 6, datas[2].IndexOf("::Op1") - (datas[2].IndexOf("@Op1::") + 6));
                item.evolutionHint = op1;
            }

            if (datas[2].Contains("@Op2::"))
            {
                string op2 = datas[2].Substring(datas[2].IndexOf("@Op2::") + 6, datas[2].IndexOf("::Op2") - (datas[2].IndexOf("@Op2::") + 6));
                item.evolutionDetail = op2;
            }
        }
    }

    public List<SO_Monster> GetSummonList(int _DungeonRank = 1)
    {
        List<SO_Monster> list = new List<SO_Monster>();

        foreach (var item in so_data)
        {
            if (item.unlockRank <= _DungeonRank && item.View_Store)
            {
                list.Add(item);
            }
        }

        list.Sort((a, b) => a.id.CompareTo(b.id));
        list.Sort((a, b) => a.unlockRank.CompareTo(b.unlockRank));
        return list;
    }


    public SO_Monster GetData(string _keyName)
    {
        foreach (var item in so_data)
        {
            if (item.keyName == _keyName)
            {
                return item;
            }
        }
        Debug.Log($"{_keyName}: Data Not Exist");
        return null;
    }
    #endregion



    void StopAllMoving()
    {
        foreach (var item in Monsters)
        {
            if (item != null)
            {
                item.StopAllCoroutines();
            }
        }
    }



    public List<MonsterStatusTemporary> LevelUpList { get; set; } = new List<MonsterStatusTemporary>();
    //public int InjuryMonster { get; set; }
    public void AddLevelUpEvent(Monster _monster)
    {
        if (LevelUpList.Count > 0)
        {
            foreach (var item in LevelUpList)
            {
                if (item.monster == _monster)
                {
                    item.times++;
                    return;
                }
            }
        }

        var TemporaryData = new MonsterStatusTemporary(_monster);
        LevelUpList.Add(TemporaryData);
    }
    public void RemoveLevelUpEvent(Monster _monster)
    {
        if (LevelUpList.Count == 0)
        {
            return;
        }
        LevelUpList.RemoveAll((item) => item.monster == _monster);
    }


    public void MonsterTurnStartEvent()
    {
        foreach (var monster in Monsters)
        {
            if (monster != null)
            {
                monster.TurnStart();
            }
        }
    }

    public void MonsterTurnOverEvent()
    {
        foreach (var monster in Monsters)
        {
            if (monster != null)
            {
                monster.TurnOver();
            }
        }
    }

    #region 진화 등록 (1마리만 가능하게끔)


    //? 아래 함수가 해야할 역할 - 1. 자신을 제외한 모든 같은 종류의 몬스터의 진화 상태를 Exclude로 변경하기
    public void Regist_Evolution(string monster)
    {
        Change_EvolutionState(GetData(monster));
    }

    //? 몬스터가 새로 생성될 때 이미 같은 타입의 진화형태가 등록된게 있으면 false를 반환
    public bool Check_Evolution(string evolution)
    {
        var data = GetData(evolution);
        foreach (var mon in Monsters)
        {
            if (mon != null && mon.Data == data)
            {
                return false;
            }
        }

        return true;
    }

    void Change_EvolutionState(SO_Monster monster)
    {
        var sameList = Get_SameMonsterList(monster);

        foreach (var mon in sameList)
        {
            mon.EvolutionState = Monster.Evolution.Exclude;
        }
    }


    List<Monster> Get_SameMonsterList(SO_Monster monster)
    {
        List<Monster> sameList = new List<Monster>();
        foreach (var mon in Monsters)
        {
            if (mon != null && mon.Data == monster)
            {
                sameList.Add(mon);
            }
        }

        return sameList;
    }


    #endregion

    #region 실제 인스턴트
    public Monster[] Monsters;
    //public int TrainingCount { get; set; } = 2;


    public int GetCurrentMonster()
    {
        int count = 0;
        foreach (var monster in Monsters)
        {
            if (monster != null)
            {
                count++;
            }
        }
        return count;
    }

    public bool MaximumCheck()
    {
        foreach (var monster in Monsters)
        {
            if (monster == null)
            {
                return true;
            }
        }

        Debug.Log("Monster Maximum");
        return false;
    }
    public void AddMonster(Monster mon)
    {
        for (int i = 0; i < Monsters.Length; i++)
        {
            if (Monsters[i] == null)
            {
                Monsters[i] = mon;
                mon.MonsterID = i;
                break;
            }
        }
    }

    public void ReleaseMonster(int monsterID)
    {
        if (Monsters[monsterID] != null)
        {
            var mon = Monsters[monsterID];
            GameManager.Placement.PlacementClear(mon);
            Managers.Resource.Destroy(mon.gameObject);
            Monsters[monsterID] = null;
        }
    }


    public void Resize_MonsterSlot()
    {
        int size = 10 + (Main.Instance.DungeonRank - 1) * 2;
        if (EventManager.Instance.CurrentClearEventData.Check_AlreadyClear(DialogueName.Heroine_40))
        {
            size++;
        }

        Array.Resize(ref Monsters, size);
    }

    void Init_MonsterSlot()
    {
        int size = 10 + (Main.Instance.DungeonRank - 1) * 2;
        if (EventManager.Instance.CurrentClearEventData.Check_AlreadyClear(DialogueName.Heroine_40))
        {
            size++;
        }

        Monsters = new Monster[size];
    }

    #endregion

    #region 세이브 데이터
    public Save_MonsterData[] GetSaveData_Monster()
    {
        Save_MonsterData[] savedata = new Save_MonsterData[Monsters.Length];


        for (int i = 0; i < savedata.Length; i++)
        {
            if (Monsters[i] != null)
            {
                var newData = new Save_MonsterData();
                newData.SetData(Monsters[i]);
                savedata[i] = newData;
            }
        }

        return savedata;
    }

    public void Load_MonsterData(Save_MonsterData[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] != null)
            {
                var mon = GameManager.Placement.CreatePlacementObject($"{data[i].PrefabPath}", null, PlacementType.Monster) as Monster;
                if (data[i].Evolution == Monster.Evolution.Complete)
                {
                    mon.MonsterInit_Evolution();
                }
                else
                {
                    mon.MonsterInit();
                }
                mon.Initialize_Status();
                mon.Initialize_Load(data[i]);

                if (data[i].FloorIndex != -1)
                {
                    BasementFloor floor = Main.Instance.Floor[data[i].FloorIndex];
                    BasementTile tile = null;
                    if (floor.TileMap.TryGetValue(data[i].PosIndex, out tile))
                    {
                        GameManager.Placement.PlacementConfirm(mon, new PlacementInfo(floor, tile));
                        Main.Instance.Floor[data[i].FloorIndex].MaxMonsterSize--;
                    }
                }

                Monsters[i] = mon;
                Monsters[i].MonsterID = i;
            }
        }
    }

    #endregion

    #region Sprite Library Asset

    SpriteLibraryAsset[] Monster_SLA;
    SpriteLibraryAsset[] Monster_SLA_New;
    void Init_SLA()
    {
        Monster_SLA = Resources.LoadAll<SpriteLibraryAsset>("Animation/Monster_Animation/SLA_Monster");
        Monster_SLA_New = Resources.LoadAll<SpriteLibraryAsset>("Animation/_Monstser_Anim/SLA_Monster");
    }
    public void ChangeSLA(Monster _monster, string _SLA)
    {
        foreach (var item in Monster_SLA)
        {
            if (item.name == _SLA)
            {
                _monster.GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset = item;
            }
        }
    }

    public void ChangeSLA_New(Monster _monster, string _SLA)
    {
        foreach (var item in Monster_SLA_New)
        {
            if (item.name == _SLA)
            {
                _monster.GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset = item;
            }
        }
    }


    #endregion
}

public class MonsterStatusTemporary
{
    public Monster monster;
    public int lv;
    public int hpMax;
    public int atk;
    public int def;
    public int agi;
    public int luk;

    public int times;

    public MonsterStatusTemporary(Monster _monster)
    {
        monster = _monster;
        lv = monster.LV;
        hpMax = monster.HP_Max;
        atk = monster.ATK;
        def = monster.DEF;
        agi = monster.AGI;
        luk = monster.LUK;

        times = 1;
    }
}

[System.Serializable]
public class Save_MonsterData
{
    public string CustomName;
    public string PrefabPath { get; set; }
    public int LV { get; set; }
    public int HP { get; set; }
    public int HP_MAX { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int AGI { get; set; }
    public int LUK { get; set; }

    public float HP_chance { get; set; }
    public float ATK_chance { get; set; }
    public float DEF_chance { get; set; }
    public float AGI_chance { get; set; }
    public float LUK_chance { get; set; }

    public Monster.MonsterState State { get; set; }
    public Monster.MoveType MoveMode { get; set; }

    public Monster.Evolution Evolution { get; set; }
    public int BattleCount;
    public int BattlePoint;


    public int FloorIndex { get; set; }
    public Vector2Int PosIndex { get; set; }


    //? GameManager가 아닌 세이브파일만으로 데이터를 받아와야 할 경우에 사용할거
    public string savedName;
    public string categoryName;
    public string labelName;


    //? 몬스터 기록 데이터(특성용으로 쓰는데 도감용으로 써도 무방할듯)
    public Monster.TraitCounter traitCounter;

    //? 특성리스트
    public List<int> currentTraitList;

    public void SetData(Monster monster)
    {
        CustomName = monster.CustomName;
        PrefabPath = monster.Data.prefabPath;
        LV = monster.LV;
        HP = monster.HP;
        HP_MAX = monster.HP_Max;
        ATK = monster.ATK;
        DEF = monster.DEF;
        AGI = monster.AGI;
        LUK = monster.LUK;

        HP_chance = monster.hp_chance;
        ATK_chance = monster.atk_chance;
        DEF_chance = monster.def_chance;
        AGI_chance = monster.agi_chance;
        LUK_chance = monster.luk_chance;

        Evolution = monster.EvolutionState;
        BattleCount = monster.BattlePoint_Count;
        BattlePoint = monster.BattlePoint_Rank;

        State = monster.State;
        MoveMode = monster.Mode;

        if (monster.PlacementInfo != null)
        {
            FloorIndex = monster.PlacementInfo.Place_Floor.FloorIndex;
            PosIndex = monster.PlacementInfo.Place_Tile.index;
        }
        else
        {
            FloorIndex = -1;
        }


        savedName = monster.Data.labelName;
        categoryName = monster.Data.SLA_category;
        labelName = monster.Data.SLA_label;

        traitCounter = monster.traitCounter;
        currentTraitList = monster.SaveTraitList();
    }
}


