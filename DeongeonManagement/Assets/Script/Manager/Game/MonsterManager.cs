using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager
{
    public void Init()
    {
        Monsters = new Monster[7];

        Init_Data();

        Managers.Scene.BeforeSceneChangeAction = () => StopAllMoving();
    }


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
    public Monster[] Monsters { get; set; }
    public int TrainingCount { get; set; } = 2;


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


    #endregion

    #region 소환 데이터
    public List<MonsterData> MonsterDatas { get; } = new List<MonsterData>();

    public MonsterData GetMonsterData(string monsterName)
    {
        foreach (var item in MonsterDatas)
        {
            if (item.Name == monsterName)
            {
                return item;
            }
        }
        Debug.Log($"{monsterName} 데이터를 찾지 못함");
        return null;
    }

    void Init_Data()
    {
        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "슬라임";
            monster.Name = "Slime";
            monster.prefabPath = "Monster/Slime";
            monster.detail = "의외로 귀여운 슬라임입니다. 약하긴 하지만 그렇다고 슬라임을 버리실건가요? 분명 열심히 키우면 보답받을거에요!";
            monster.sprite = Managers.Sprite.GetSprite("Monster/Slime");

            monster.ManaCost = 120;

            monster.LV = 1;
            monster.MAXLV = 25;
            monster.HP = 15;
            monster.ATK = 2;
            monster.DEF = 4;
            monster.AGI = 2;
            monster.LUK = 3;

            monster.HP_chance= 1.2f;
            monster.ATK_chance = 0.3f;
            monster.DEF_chance = 0.25f;
            monster.AGI_chance = 0.15f;
            monster.LUK_chance = 0.1f;

            MonsterDatas.Add(monster);
        }

        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "스켈레톤";
            monster.Name = "Skeleton";
            monster.prefabPath = "Monster/Skeleton";
            monster.detail = "조금은 무서운 스켈레톤입니다. 튼튼하고 강해서 모험가들을 상대로 제격이에요. 하지만 조금 한계가 있을지도?";
            monster.sprite = Managers.Sprite.GetSprite("Monster/Skeleton");

            monster.ManaCost = 250;

            monster.LV = 3;
            monster.MAXLV = 20;
            monster.HP = 30;
            monster.ATK = 5;
            monster.DEF = 4;
            monster.AGI = 4;
            monster.LUK = 4;

            monster.HP_chance = 1.5f;
            monster.ATK_chance = 0.4f;
            monster.DEF_chance = 0.15f;
            monster.AGI_chance = 0.1f;
            monster.LUK_chance = 0.08f;

            MonsterDatas.Add(monster);
        }



    }

    public void AddLevel2()
    {
        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "머쉬보이";
            monster.Name = "MushBoy";
            monster.prefabPath = "Monster/MushBoy";
            monster.detail = "어둡고 습한곳을 좋아하는 버섯소년이에요. 강력한 독을 지니고 있어 모험가들에게 위협이 됩니다. 하지만 사실 건드리지 않으면 안전해요.";
            monster.sprite = Managers.Sprite.GetSprite("Monster/MushBoy");

            monster.ManaCost = 300;

            monster.LV = 1;
            monster.MAXLV = 30;
            monster.HP = 25;
            monster.ATK = 6;
            monster.DEF = 2;
            monster.AGI = 2;
            monster.LUK = 5;

            monster.HP_chance = 1.25f;
            monster.ATK_chance = 0.6f;
            monster.DEF_chance = 0.1f;
            monster.AGI_chance = 0.15f;
            monster.LUK_chance = 0.2f;

            MonsterDatas.Add(monster);
        }

        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "이블아이";
            monster.Name = "EvilEye";
            monster.prefabPath = "Monster/EvilEye";
            monster.detail = "매력적인 눈을 지닌 몬스터에요. 생김새때문에 피부가 약할 것 같지만, 실은 엄청 단단하기 때문에 괜찮다네요.";
            monster.sprite = Managers.Sprite.GetSprite("Monster/EvilEye");

            monster.ManaCost = 400;

            monster.LV = 5;
            monster.MAXLV = 30;
            monster.HP = 35;
            monster.ATK = 5;
            monster.DEF = 8;
            monster.AGI = 6;
            monster.LUK = 1;

            monster.HP_chance = 1.6f;
            monster.ATK_chance = 0.4f;
            monster.DEF_chance = 0.4f;
            monster.AGI_chance = 0.2f;
            monster.LUK_chance = 0.05f;

            MonsterDatas.Add(monster);
        }
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
        for (int i = 0; i < Monsters.Length; i++)
        {
            if (data[i] != null)
            {
                var mon = GameManager.Placement.CreatePlacementObject($"Monster/{data[i].Name}", null, Define.PlacementType.Monster) as Monster;
                mon.MonsterInit();
                mon.Initialize_Status();
                mon.Initialize_SaveData(data[i]);

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

public class MonsterData
{
    public int ManaCost { get; set; }
    public string Name_KR { get; set; }

    public string Name { get; set; }



    public int LV { get; set; }
    public int MAXLV { get; set; }

    public int HP { get; set; }

    public int ATK { get; set; }
    public int DEF { get; set; }
    public int AGI { get; set; }
    public int LUK { get; set; }


    public float HP_chance { get; set; }
    public float ATK_chance { get; set; }
    public float DEF_chance { get; set; }
    public float AGI_chance { get; set; }
    public float LUK_chance { get; set; }



    public string prefabPath;

    public string detail;
    public Sprite sprite;

}

[System.Serializable]
public class Save_MonsterData
{
    public string Name { get; set; }
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

    public bool Evolution { get; set; }


    public int FloorIndex { get; set; }
    public Vector2Int PosIndex { get; set; }

    public void SetData(Monster monster)
    {
        Name = monster.Data.Name;
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

        Evolution = monster.Evolution;

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
    }
}


