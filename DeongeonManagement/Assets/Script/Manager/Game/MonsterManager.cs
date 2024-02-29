using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class MonsterManager
{
    public void Init()
    {
        Monsters = new Monster[7];

        Init_SLA();
        Init_Data();
        Init_Data_Evolution();

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
            if (item.Name_Dict == monsterName)
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
            monster.Name_Dict = "Slime";
            monster.prefabPath = "Monster/Slime";
            monster.detail = "자세히 보면 의외로 귀여운 슬라임입니다. 약하긴 하지만 성장가능성이 무궁무진한 몬스터입니다.";
            monster.sprite = Managers.Sprite.GetSprite("Monster/Slime");

            monster.Evolution_Hint = "진화 힌트 : 최대 레벨을 찍고 길드게시판을 확인";
            monster.Evolution_Detail = "슬라임 헌터 처치";

            monster.ManaCost = 120;

            monster.LV = 1;
            monster.MAXLV = 25;
            monster.HP = 35;
            monster.ATK = 5;
            monster.DEF = 4;
            monster.AGI = 2;
            monster.LUK = 3;

            monster.HP_chance= 1.6f;
            monster.ATK_chance = 0.45f;
            monster.DEF_chance = 0.25f;
            monster.AGI_chance = 0.15f;
            monster.LUK_chance = 0.1f;

            monster.Battle_AP = 1;
            monster.Battle_Interval = 5;

            MonsterDatas.Add(monster);
        }

        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "어스골렘";
            monster.Name_Dict = "EarthGolem";
            monster.prefabPath = "Monster/EarthGolem";
            monster.detail = "던전에 마나를 융합해 창조해낸 골렘입니다. 튼튼하고 강해서 모험가들을 상대로 제격이에요. 하지만 조금 한계가 있을지도?";
            monster.sprite = Managers.Sprite.GetSprite("Monster/EarthGolem");

            monster.Evolution_Hint = "진화 힌트 : 최대 레벨을 찍고 길드게시판을 확인";
            monster.Evolution_Detail = "원소술사 처치";

            monster.ManaCost = 250;

            monster.LV = 3;
            monster.MAXLV = 18;
            monster.HP = 60;
            monster.ATK = 9;
            monster.DEF = 4;
            monster.AGI = 4;
            monster.LUK = 4;

            monster.HP_chance = 1.3f;
            monster.ATK_chance = 0.75f;
            monster.DEF_chance = 0.15f;
            monster.AGI_chance = 0.1f;
            monster.LUK_chance = 0.08f;

            monster.Battle_AP = 1;
            monster.Battle_Interval = 5;

            MonsterDatas.Add(monster);
        }


    }

    public void AddLevel_2()
    {
        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "머쉬보이";
            monster.Name_Dict = "MushBoy";
            monster.prefabPath = "Monster/MushBoy";
            monster.detail = "어둡고 습한곳을 좋아하는 버섯몬스터입니다. 높은 공격력을 지녔어요.";
            monster.sprite = Managers.Sprite.GetSprite("Monster/MushBoy");

            monster.ManaCost = 350;

            monster.LV = 1;
            monster.MAXLV = 30;
            monster.HP = 50;
            monster.ATK = 15;
            monster.DEF = 2;
            monster.AGI = 2;
            monster.LUK = 7;

            monster.HP_chance = 1.6f;
            monster.ATK_chance = 1.15f;
            monster.DEF_chance = 0.1f;
            monster.AGI_chance = 0.15f;
            monster.LUK_chance = 0.2f;

            monster.Battle_AP = 2;
            monster.Battle_Interval = 3f;

            MonsterDatas.Add(monster);
        }

        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "이블아이";
            monster.Name_Dict = "EvilEye";
            monster.prefabPath = "Monster/EvilEye";
            monster.detail = "매력적인 눈을 지닌 몬스터에요. 생김새때문에 피부가 약할 것 같지만, 실은 엄청 단단하기 때문에 수비용으로 적절합니다.";
            monster.sprite = Managers.Sprite.GetSprite("Monster/EvilEye");

            monster.ManaCost = 425;

            monster.LV = 5;
            monster.MAXLV = 25;
            monster.HP = 90;
            monster.ATK = 9;
            monster.DEF = 10;
            monster.AGI = 6;
            monster.LUK = 2;

            monster.HP_chance = 1.8f;
            monster.ATK_chance = 0.35f;
            monster.DEF_chance = 0.55f;
            monster.AGI_chance = 0.2f;
            monster.LUK_chance = 0.05f;

            monster.Battle_AP = 2;
            monster.Battle_Interval = 3f;

            MonsterDatas.Add(monster);
        }

        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "페어리";
            monster.Name_Dict = "Fairy";
            monster.prefabPath = "Monster/Fairy";
            monster.detail = "순수한 마나의 결정체인 요정 몬스터입니다. 여러방면에서 좋은 밸런스를 가지고 있고, 잠재력도 높은 편입니다.";
            monster.sprite = Managers.Sprite.GetSprite("Monster/Fairy");

            monster.ManaCost = 600;

            monster.LV = 1;
            monster.MAXLV = 35;
            monster.HP = 45;
            monster.ATK = 8;
            monster.DEF = 8;
            monster.AGI = 6;
            monster.LUK = 4;

            monster.HP_chance = 1.6f;
            monster.ATK_chance = 0.55f;
            monster.DEF_chance = 0.35f;
            monster.AGI_chance = 0.2f;
            monster.LUK_chance = 0.25f;

            monster.Battle_AP = 1;
            monster.Battle_Interval = 3f;

            MonsterDatas.Add(monster);
        }
    }

    public void AddLevel_3()
    {
        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "살라만드라";
            monster.Name_Dict = "Salamandra";
            monster.prefabPath = "Monster/Salamandra";
            monster.detail = "원시적인 형태를 지닌 몬스터입니다. 불을 내뿜어 적을 공격합니다. 종족 특성으로 적을 위압하여 빨리 지치게 만들어요.";
            monster.sprite = Managers.Sprite.GetSprite("Monster/Salamandra");

            monster.ManaCost = 980;

            monster.LV = 1;
            monster.MAXLV = 40;
            monster.HP = 65;
            monster.ATK = 12;
            monster.DEF = 9;
            monster.AGI = 4;
            monster.LUK = 4;

            monster.HP_chance = 1.8f;
            monster.ATK_chance = 0.9f;
            monster.DEF_chance = 0.4f;
            monster.AGI_chance = 0.15f;
            monster.LUK_chance = 0.15f;

            monster.Battle_AP = 3;
            monster.Battle_Interval = 3f;

            MonsterDatas.Add(monster);
        }

        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "그레이하운드";
            monster.Name_Dict = "GreyHound";
            monster.prefabPath = "Monster/GreyHound";
            monster.detail = "친근감이 느껴지는 늑대형 몬스터입니다. 매우 용맹하고 재빠른 몸놀림을 지녔어요.";
            monster.sprite = Managers.Sprite.GetSprite("Monster/GreyHound");

            monster.ManaCost = 750;

            monster.LV = 10;
            monster.MAXLV = 35;
            monster.HP = 80;
            monster.ATK = 17;
            monster.DEF = 5;
            monster.AGI = 9;
            monster.LUK = 8;

            monster.HP_chance = 1.4f;
            monster.ATK_chance = 0.55f;
            monster.DEF_chance = 0.35f;
            monster.AGI_chance = 0.35f;
            monster.LUK_chance = 0.15f;

            monster.Battle_AP = 2;
            monster.Battle_Interval = 3f;

            MonsterDatas.Add(monster);
        }
    }



    #endregion

    #region 진화 데이터
    public List<MonsterData> MonsterDatas_Evolution { get; } = new List<MonsterData>();

    public MonsterData GetMonsterData_Evolution(string monsterName)
    {
        foreach (var item in MonsterDatas_Evolution)
        {
            if (item.Name_Dict == monsterName)
            {
                return item;
            }
        }
        Debug.Log($"{monsterName} 데이터를 찾지 못함");
        return null;
    }

    void Init_Data_Evolution()
    {
        {
            MonsterData monster = new MonsterData();
            monster.Name_Dict = "BloodySlime";
            monster.prefabPath = "Monster/Slime";
            monster.Name_KR = "블러디 슬라임";
            monster.detail = "슬라임이 진화해서 블러디 슬라임이 되었습니다. 겉으로 보기엔 별로 달라진게 없어보여도 상당히 강력해졌습니다.";
            monster.sprite = Managers.Sprite.GetSprite($"Monster/{monster.Name_Dict}");

            monster.Evolution_Hint = "진화 완료";

            monster.LV = 15;
            monster.MAXLV = 59;
            monster.HP = 85;
            monster.ATK = 20;
            monster.DEF = 12;
            monster.AGI = 8;
            monster.LUK = 10;

            monster.HP_chance = 2.2f;
            monster.ATK_chance = 0.7f;
            monster.DEF_chance = 0.45f;
            monster.AGI_chance = 0.25f;
            monster.LUK_chance = 0.18f;

            monster.Battle_AP = 2;
            monster.Battle_Interval = 5;

            MonsterDatas_Evolution.Add(monster);
        }

        {
            MonsterData monster = new MonsterData();
            monster.Name_Dict = "FlameGolem";
            monster.prefabPath = "Monster/EarthGolem";
            monster.Name_KR = "플레임골렘";
            monster.detail = "어스골렘이 성장해 플레임골렘으로 진화했습니다. 더 튼튼하고 더 강해졌습니다.";
            monster.sprite = Managers.Sprite.GetSprite($"Monster/{monster.Name_Dict}");

            monster.Evolution_Hint = "진화 완료";

            monster.LV = 10;
            monster.MAXLV = 39;
            monster.HP = 100;
            monster.ATK = 18;
            monster.DEF = 8;
            monster.AGI = 8;
            monster.LUK = 8;

            monster.HP_chance = 1.3f;
            monster.ATK_chance = 1.15f;
            monster.DEF_chance = 0.3f;
            monster.AGI_chance = 0.2f;
            monster.LUK_chance = 0.16f;

            monster.Battle_AP = 2;
            monster.Battle_Interval = 5;

            MonsterDatas_Evolution.Add(monster);
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
                var mon = GameManager.Placement.CreatePlacementObject($"{data[i].PrefabPath}", null, PlacementType.Monster) as Monster;
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

public class MonsterData
{
    public int ManaCost { get; set; }
    public string Name_KR { get; set; }
    public string Name_Dict { get; set; }


    public string Evolution_Hint { get; set; }
    public string Evolution_Detail { get; set; }



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


    public int Battle_AP { get; set; }
    public float Battle_Interval { get; set; }



    public string prefabPath;

    public string detail;
    public Sprite sprite;

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


    public int FloorIndex { get; set; }
    public Vector2Int PosIndex { get; set; }

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


