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
            item.evolutionHint = datas[2];
            item.evolutionDetail = datas[3];
        }
    }

    public List<SO_Monster> GetSummonList(int _DungeonRank = 1)
    {
        List<SO_Monster> list = new List<SO_Monster>();

        foreach (var item in so_data)
        {
            if (item.unlockRank <= _DungeonRank)
            {
                list.Add(item);
            }
        }

        list.Sort((a, b) => a.id.CompareTo(b.id));
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
    public int InjuryMonster { get; set; }
    public void AddLevelUpEvent(Monster _monster)
    {
        //if (LevelUpList == null)
        //{
        //    LevelUpList = new List<MonsterStatusTemporary>();
        //}
        //else 
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
            if (monster != null && monster.State == Monster.MonsterState.Placement)
            {
                monster.TurnStart();
            }
        }
    }



    #region 실제 인스턴트
    public Monster[] Monsters;
    public int TrainingCount { get; set; } = 2;


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
        Array.Resize(ref Monsters, 6 + Main.Instance.DungeonRank);
    }

    void Init_MonsterSlot()
    {
        Monsters = new Monster[6 + Main.Instance.DungeonRank];
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
                mon.MonsterInit();
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

        InjuryMonster = 0;
    }

    #endregion

    #region Sprite Library Asset

    SpriteLibraryAsset[] Monster_SLA;
    void Init_SLA()
    {
        Monster_SLA = Resources.LoadAll<SpriteLibraryAsset>("Monster_Animation/SLA_Monster");
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
    public string spritePath;


    public void SetData(Monster monster)
    {
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
        spritePath = monster.Data.spritePath;
    }
}


