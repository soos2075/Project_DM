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

        Init_UnitEventRoom();
        Init_UnitEventAction();
    }

    #region SO_Data
    SO_Monster[] so_data;

    public void Init_LocalData()
    {
        so_data = Resources.LoadAll<SO_Monster>("Data/Monster");
        foreach (var item in so_data)
        {
            string[] datas = Managers.Data.GetTextData_Monster(item.id);

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

    //public void Init_LocalData()
    //{
    //    so_data = Resources.LoadAll<SO_Monster>("Data/Monster");
    //    foreach (var item in so_data)
    //    {
    //        string[] datas = Managers.Data.GetTextData_Object(item.id);

    //        if (datas == null)
    //        {
    //            Debug.Log($"{item.id} : CSV Data Not Exist");
    //            continue;
    //        }

    //        item.labelName = datas[0];
    //        item.detail = datas[1];


    //        if (datas[2].Contains("@Op1::"))
    //        {
    //            string op1 = datas[2].Substring(datas[2].IndexOf("@Op1::") + 6, datas[2].IndexOf("::Op1") - (datas[2].IndexOf("@Op1::") + 6));
    //            item.evolutionHint = op1;
    //        }

    //        if (datas[2].Contains("@Op2::"))
    //        {
    //            string op2 = datas[2].Substring(datas[2].IndexOf("@Op2::") + 6, datas[2].IndexOf("::Op2") - (datas[2].IndexOf("@Op2::") + 6));
    //            item.evolutionDetail = op2;
    //        }
    //    }
    //}

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
                    if (item.times > 3)
                    {
                        item.times = 3;
                    }
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

        //? 플레이어는 유닛으로 등록이 안되있으니 따로 호출해줘야함
        Main.Instance.Player.GetComponent<Player>().TurnStart();
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

        //? 플레이어는 유닛으로 등록이 안되있으니 따로 호출해줘야함
        Main.Instance.Player.GetComponent<Player>().TurnOver();

        Main.Instance.CurrentStatistics.Highest_Unit_Lv = GetCurrentHighestLv();
        Main.Instance.CurrentStatistics.Hightest_Unit_Size = GetCurrentMonsterSize();
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
            mon.Regist_Evloution_Callback();

            //if (mon is GreyHound)
            //{
            //    mon.UnitDialogueEvent.ClearEvent(UnitDialogueEventLabel.GreyHound_Evolution);
            //}
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
    const int slotSize = 12;

    Monster[] Monsters;
    //public int TrainingCount { get; set; } = 2;

    public int GetSlotSize()
    {
        return Monsters.Length;
    }
    public int GetCurrentMonsterSize()
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
    public int GetCurrentHighestLv()
    {
        var unitAll = GetMonsterAll();
        int currentLv = 0;
        foreach (var item in unitAll)
        {
            if (item.LV >= currentLv)
            {
                currentLv = item.LV;
            }
        }
        return currentLv;
    }

    public bool Check_ExistUnit<T>() where T : Monster
    {
        foreach (var monster in Monsters)
        {
            if (monster != null && monster is T)
            {
                return true;
            }
        }
        return false;
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

    public Monster GetMonster(int slotNumber)
    {
        if (Monsters.Length > slotNumber && Monsters[slotNumber] != null)
        {
            return Monsters[slotNumber];
        }

        return null;
    }
    public Monster GetMonster<T>()
    {
        foreach (var monster in Monsters)
        {
            if (monster != null && monster is T)
            {
                return monster;
            }
        }
        return null;
    }

    public int GetMonsterSlotIndex(Monster mon)
    {
        for (int i = 0; i < Monsters.Length; i++)
        {
            if (Monsters[i] == mon)
            {
                return i;
            }
        }
        return 0;
    }



    public List<Monster> GetMonsterAll()
    {
        List<Monster> monsterList = new List<Monster>();

        foreach (var monster in Monsters)
        {
            if (monster != null)
            {
                monsterList.Add(monster);
            }
        }

        return monsterList;
    }

    public List<Monster> GetMonsterAll(Monster.MonsterState monsterState)
    {
        List<Monster> monsterList = new List<Monster>();

        foreach (var monster in GetMonsterAll())
        {
            if (monster.State == monsterState)
            {
                monsterList.Add(monster);
            }
        }

        return monsterList;
    }


    public Monster CreateMonster(string keyName, bool isEvolution = false, bool popUpUI = false)
    {
        var data = GetData(keyName);
        if (data != null)
        {
            var mon = GameManager.Placement.CreatePlacementObject(data.prefabPath, null, PlacementType.Monster) as Monster;

            if (isEvolution) //? 새로 생성하려는 몹이 진화상태인 몹이라면
            {
                mon.Create_EvolutionMonster_Init();
            }
            else
            {
                mon.MonsterInit();
                mon.Initialize_Status();
            }

            mon.AddCollectionPoint();
            AddMonster(mon);


            if (popUpUI)
            {
                var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
                ui.TargetMonster(mon);
                ui.SetStateText($"<b>{UserData.Instance.LocaleText("New")}".SetTextColorTag(Define.TextColor.Plus_Green) +
                    $"{UserData.Instance.LocaleText("유닛")}".SetTextColorTag(Define.TextColor.Plus_Green));
            }
            return mon;
        }
        return null;
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
        int size = slotSize + Main.Instance.AddUnitSlotCount + (Main.Instance.DungeonRank - 1) * 3;
        //if (EventManager.Instance.CurrentClearEventData.Check_AlreadyClear(DialogueName.Heroine_40))
        //{
        //    size++;
        //}
        Array.Resize(ref Monsters, size);
    }

    public void Resize_AddOne()
    {
        Main.Instance.AddUnitSlotCount++;
        Resize_MonsterSlot();
    }



    void Init_MonsterSlot()
    {
        int size = slotSize + Main.Instance.AddUnitSlotCount + (Main.Instance.DungeonRank - 1) * 3;
        Monsters = new Monster[size];
    }


    public void ChangeMonsterSlot(int first, int second)
    {
        var mon = Monsters[first];
        Monsters[first] = Monsters[second];
        Monsters[second] = mon;

        if (GetMonster(first) != null)
        {
            GetMonster(first).MonsterID = first;
        }
        if (GetMonster(second) != null)
        {
            GetMonster(second).MonsterID = second;
        }
    }


    #endregion

    #region 세이브 데이터
    public Save_MonsterData[] GetSaveData_Monster()
    {
        Save_MonsterData[] savedata = new Save_MonsterData[Monsters.Length + 1];

        {        //? 플레이어 데이터를 첫번째로 저장
            var playerData = new Save_MonsterData();
            playerData.SetData(Main.Instance.Player.GetComponent<Monster>());
            savedata[0] = playerData;
        }


        for (int i = 0; i < savedata.Length - 1; i++)
        {
            if (Monsters[i] != null)
            {
                var newData = new Save_MonsterData();
                newData.SetData(Monsters[i]);
                savedata[i + 1] = newData;
            }
        }

        return savedata;
    }

    public void Load_MonsterData(Save_MonsterData[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (i == 0)
            {
                var player = Main.Instance.Player.GetComponent<Player>();
                player.Player_DataLoad(data[0]);
                continue;
            }



            if (data[i] != null)
            {
                var mon = GameManager.Placement.CreatePlacementObject($"{data[i].PrefabPath}", null, PlacementType.Monster) as Monster;
                if (data[i].Evolution == Monster.Evolution.Complete)
                {
                    mon.Load_EvolutionMonster();
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

                Monsters[data[i].SlotIndex] = mon;
                mon.MonsterID = data[i].SlotIndex;
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
        SpriteLibraryAsset newSLA = null;

        foreach (var item in Monster_SLA)
        {
            if (item.name == _SLA)
            {
                newSLA = item;
                //_monster.GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset = item;
            }
        }

        //? 구버전 신버전이 있음 (예를 들면 Fairy). 근데 무조건 신버전으로 업데이트 한다음 교체하기로
        foreach (var item in Monster_SLA_New)
        {
            if (item.name == _SLA)
            {
                newSLA = item;
                //_monster.GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset = item;
            }
        }

        if (newSLA != null)
        {
            _monster.GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset = newSLA;
            return;
        }
        Debug.Log($"SpriteAsset is Null : {_SLA}");
    }

    //public void ChangeSLA_New(Monster _monster, string _SLA)
    //{
    //    foreach (var item in Monster_SLA_New)
    //    {
    //        if (item.name == _SLA)
    //        {
    //            _monster.GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset = item;
    //        }
    //    }
    //}


    #endregion


    #region Unit Dialogue Event

    Dictionary<string, Action<Monster>> UnitEventAction = new Dictionary<string, Action<Monster>>();

    void Init_UnitEventAction()
    {
        UnitEventAction.Add("Buy_Artifact", (unit) => {
            Debug.Log("@@@@" + State);
            switch (State)
            {
                case SelectState.Yes:
                    Debug.Log("아티팩트 구매");
                    Main.Instance.CurrentDay.SubtractGold(400, Main.DayResult.EventType.Artifacts);
                    GameManager.Artifact.Add_RandomArtifact();
                    unit.StatUP(StatEnum.ATK, 2, true);
                    break;

                case SelectState.No:
                    break;

                case SelectState.Yes2:
                    Debug.Log("아티팩트 구매 * 10");
                    Main.Instance.CurrentDay.SubtractGold(4000, Main.DayResult.EventType.Artifacts);
                    for (int i = 0; i < 10; i++)
                    {
                        GameManager.Artifact.Add_RandomArtifact();
                    }
                    unit.StatUP(StatEnum.ATK, 20, true);
                    break;

                case SelectState.Yes_Fail:
                    Debug.Log("골드부족");
                    //Managers.Dialogue.ShowDialogueUI((int)UnitDialogueEventLabel.Utori_NoGold);
                    break;
            }

            unit.UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Utori_ArtifactBuy, true);
            Main.Instance.Player_AP++;
        });




        UnitEventAction.Add("Evolution_Rena", (unit) => {
            unit.GetComponent<Heroine>().Evolution_Rena();
        });

        UnitEventAction.Add("Evolution_Hound", (unit) => {
            unit.GetComponent<GreyHound>().Evolution_Hound();
        });


        UnitEventAction.Add("Succubus_Refeat", (unit) => {
            unit.GetComponent<Succubus>().Refeat_Evolution();
        });

        UnitEventAction.Add("Succubus_WaitAnswer", (unit) => {

            switch (State)
            {
                case SelectState.Yes:
                    unit.GetComponent<Succubus>().Evolution_Lilith();
                    Main.Instance.Player.GetComponent<Monster>().StatUP(StatEnum.HP, -33, true);
                    Main.Instance.Player.GetComponent<Monster>().StatUP(StatEnum.ATK, 0, false);
                    Main.Instance.Player.GetComponent<Monster>().StatUP(StatEnum.DEF, 5, false);
                    Main.Instance.Player.GetComponent<Monster>().StatUP(StatEnum.AGI, 0, false);
                    Main.Instance.Player.GetComponent<Monster>().StatUP(StatEnum.LUK, 15, false);
                    break;
                case SelectState.No:
                    unit.GetComponent<Succubus>().Refeat_Evolution();
                    break;
            }
        });




        UnitEventAction.Add("Random_UP", (unit) => {
            int ran = UnityEngine.Random.Range(0, 5);
            switch (ran)
            {
                case 1:
                    unit.StatUP(StatEnum.ATK, 2, true);
                    break;

                case 2:
                    unit.StatUP(StatEnum.DEF, 2, true);
                    break;

                case 3:
                    unit.StatUP(StatEnum.AGI, 2, true);
                    break;

                case 4:
                    unit.StatUP(StatEnum.LUK, 2, true);
                    break;

                default:
                    unit.StatUP(StatEnum.HP, 10, true);
                    break;
            }
        });

        UnitEventAction.Add("Random_UP2", (unit) => {
            int ran = UnityEngine.Random.Range(0, 5);
            switch (ran)
            {
                case 1:
                    unit.StatUP(StatEnum.ATK, 3, true);
                    break;

                case 2:
                    unit.StatUP(StatEnum.DEF, 3, true);
                    break;

                case 3:
                    unit.StatUP(StatEnum.AGI, 3, true);
                    break;

                case 4:
                    unit.StatUP(StatEnum.LUK, 3, true);
                    break;

                default:
                    unit.StatUP(StatEnum.HP, 15, true);
                    break;
            }
        });


        UnitEventAction.Add("HP_UP", (unit) => { unit.StatUP(StatEnum.HP, 10, true); });
        UnitEventAction.Add("ATK_UP", (unit) => { unit.StatUP(StatEnum.ATK, 2, true); });
        UnitEventAction.Add("DEF_UP", (unit) => { unit.StatUP(StatEnum.DEF, 2, true); });
        UnitEventAction.Add("AGI_UP", (unit) => { unit.StatUP(StatEnum.AGI, 2, true); });
        UnitEventAction.Add("LUK_UP", (unit) => { unit.StatUP(StatEnum.LUK, 2, true); });

        UnitEventAction.Add("HP_UP2", (unit) => { unit.StatUP(StatEnum.HP, 15, true); });
        UnitEventAction.Add("ATK_UP2", (unit) => { unit.StatUP(StatEnum.ATK, 3, true); });
        UnitEventAction.Add("DEF_UP2", (unit) => { unit.StatUP(StatEnum.DEF, 3, true); });
        UnitEventAction.Add("AGI_UP2", (unit) => { unit.StatUP(StatEnum.AGI, 3, true); });
        UnitEventAction.Add("LUK_UP2", (unit) => { unit.StatUP(StatEnum.LUK, 3, true); });

        UnitEventAction.Add("HP_UP3", (unit) => { unit.StatUP(StatEnum.HP, 20, true); });
        UnitEventAction.Add("ATK_UP3", (unit) => { unit.StatUP(StatEnum.ATK, 4, true); });
        UnitEventAction.Add("DEF_UP3", (unit) => { unit.StatUP(StatEnum.DEF, 4, true); });
        UnitEventAction.Add("AGI_UP3", (unit) => { unit.StatUP(StatEnum.AGI, 4, true); });
        UnitEventAction.Add("LUK_UP3", (unit) => { unit.StatUP(StatEnum.LUK, 4, true); });

        UnitEventAction.Add("Player_UP", (unit) => { 
            Main.Instance.Player.GetComponent<Monster>().StatUP(StatEnum.HP, 10, true);
            Main.Instance.Player.GetComponent<Monster>().StatUP(StatEnum.ATK, 2, false);
            Main.Instance.Player.GetComponent<Monster>().StatUP(StatEnum.DEF, 2, false);
            Main.Instance.Player.GetComponent<Monster>().StatUP(StatEnum.AGI, 2, false);
            Main.Instance.Player.GetComponent<Monster>().StatUP(StatEnum.LUK, 2, false);
        });

        UnitEventAction.Add("All_UP", (unit) => {
            unit.StatUP(StatEnum.HP, 10, true);
            unit.StatUP(StatEnum.ATK, 2, false);
            unit.StatUP(StatEnum.DEF, 2, false);
            unit.StatUP(StatEnum.AGI, 2, false);
            unit.StatUP(StatEnum.LUK, 2, false);
        });
    }
    public UnitEventRoom Room;
    public Transform PlayerPos;
    public Transform UnitPos;

    void Init_UnitEventRoom()
    {
        Room = GameManager.FindAnyObjectByType<UnitEventRoom>();
        PlayerPos = Room.transform.GetChild(0);
        UnitPos = Room.transform.GetChild(1);
        Room.gameObject.SetActive(false);
    }



    public enum SelectState
    {
        None,
        Yes,
        No,
        Yes_Fail,
        No_Fail,

        Yes2,
        No2,
    }
    public SelectState State = SelectState.None;



    public void StartUnitEventAction(int DialogueID, Monster target)
    {
        Action<Monster> statUp = null;
        switch ((UnitDialogueEventLabel)DialogueID)
        {
            //? 능력치 오르면 안되는 반복퀘스트
            case UnitDialogueEventLabel.Utori_ArtifactBuy:
                UnitEventAction.TryGetValue("Buy_Artifact", out statUp);
                break;


            case UnitDialogueEventLabel.BloodySlime_First:
            case UnitDialogueEventLabel.Heroin_First:
                UnitEventAction.TryGetValue("HP_UP2", out statUp);
                break;


                //? 아래 두개는 EventManager에서 호출중
            //case UnitDialogueEventLabel.Succubus_Yes:
            //case UnitDialogueEventLabel.Succubus_No:
            case UnitDialogueEventLabel.Succubus_Evolution1:
                UnitEventAction.TryGetValue("Succubus_Refeat", out statUp);
                break;

            case UnitDialogueEventLabel.Succubus_Evolution2:
                UnitEventAction.TryGetValue("Succubus_WaitAnswer", out statUp);
                break;


            case UnitDialogueEventLabel.Heroin_Defeat:
                UnitEventAction.TryGetValue("All_UP", out statUp);
                break;

            case UnitDialogueEventLabel.Heroin_Training:
            case UnitDialogueEventLabel.Heroin_Lv18:
            case UnitDialogueEventLabel.Heroin_Day15:
                UnitEventAction.TryGetValue("Player_UP", out statUp);
                if (UnitEventAction.TryGetValue("All_UP", out Action<Monster> tempA)) statUp += tempA;
                break;

            //case UnitDialogueEventLabel.Heroin_Defeat:
            //    break;

            case UnitDialogueEventLabel.Heroin_Day3:
            case UnitDialogueEventLabel.Heroin_Day6:
            case UnitDialogueEventLabel.Heroin_Day9:
            case UnitDialogueEventLabel.Heroin_Lv5:
            case UnitDialogueEventLabel.Heroin_Lv10:
            case UnitDialogueEventLabel.Heroin_Lv15:
                UnitEventAction.TryGetValue("Random_UP2", out statUp);
                break;

            case UnitDialogueEventLabel.Heroin_CallName:
                UnitEventAction.TryGetValue("Evolution_Rena", out statUp);
                break;

            case UnitDialogueEventLabel.GreyHound_Evolution:
                UnitEventAction.TryGetValue("Evolution_Hound", out statUp);
                break;


            default:
                UnitEventAction.TryGetValue("Random_UP", out statUp);
                break;
        }


        Room.gameObject.SetActive(true);
        GameManager.Instance.StartCoroutine(UnitEvent(DialogueID, target, statUp));
        //? 클리어는 호출과 동시에 바로 진행
        //target.UnitDialogueEvent.ClearEvent(DialogueID);
    }


    IEnumerator UnitEvent(int DialogueID, Monster target, Action<Monster> OverCallback)
    {
        //? 페이드인아웃
        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);


        //? 카메라 대화방으로 이동
        var Cam = Camera.main.GetComponent<CameraControl>();
        Cam.SaveCurrentState();
        Cam.transform.position = new Vector3(Room.transform.position.x, Room.transform.position.y, Cam.transform.position.z);
        Cam.pixelCam.assetsPPU = 30;

        //? 플레이어와 유닛 대화의방으로 이동
        var player = Main.Instance.Player;

        var UnitOriginPos = target.transform.position;
        var PlayerOriginPos = player.position;

        player.position = PlayerPos.position;
        target.transform.position = UnitPos.position;
        target.transform.localScale = new Vector3(-1, 1, 1);
        if (target.State == Monster.MonsterState.Standby)
        {
            target.GetComponentInChildren<SpriteRenderer>(true).enabled = true;
        }

        //? 실제 대화 호출 및 진행 (ui를 없애야되서 먼저 시작)
        Managers.Dialogue.ShowDialogueUI(DialogueID, player);


        //? 세팅시간 1초 기다리기
        yield return new WaitForSeconds(1);


        //? 대화끝날때까지 기다리기
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        //? 페이드인아웃
        var fade2 = Managers.UI.ShowPopUp<UI_Fade>();
        fade2.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);

        //? 대화 끝나고 이것저것 세팅 (플레이어와 유닛 위치 되돌리기, 카메라 되돌리기)
        player.position = PlayerOriginPos;
        target.transform.position = UnitOriginPos;
        target.transform.localScale = Vector3.one;
        if (target.State == Monster.MonsterState.Standby)
        {
            target.GetComponentInChildren<SpriteRenderer>(true).enabled = false;
        }
        Cam.SetOriginState();
        Room.gameObject.SetActive(false);

        OverCallback?.Invoke(target);
    }

    #endregion


}
//? Dialogue ID 규칙 : 1 + Monster Key ID(3자리) + 이벤트번호 (낮은 이벤트가 항상 먼저 발생하므로 00~09 까지는 특수이벤트용, 일반 이벤트는 10부터 시작
public enum UnitDialogueEventLabel
{
    Slime_First = 100100,
    BloodySlime_First = 150100,


    Fairy_First = 100500,
    Pixie_First = 150500,


    GreyHound_First = 100700,
    GreyHound_Lv20 = 100701,
    GreyHound_Evolution = 100702,


    Salamandra_First = 100600,
    Salinu_First = 150600,

    Succubus_First = 102000,
    Succubus_Evolution1 = 102001,
    Succubus_Evolution2 = 102002,

    Succubus_Yes = 102003,
    Succubus_No = 102004,

    Lilith_First = 152000,



    Heroin_First = 190100,
    Heroin_Defeat = 190101,
    Heroin_Training = 190102,

    //? 엔딩조건 00,01,02, 11,12,13, 21,22,23 이벤트를 모두 볼 것
    Heroin_EndingRoot = 190103,
    Heroin_Root_Ture = 190104,
    Heroin_Root_False = 190105,

    Heroin_CallName = 190109,



    Heroin_Day3 = 190111,
    Heroin_Day6 = 190112,
    Heroin_Day9 = 190113,
    Heroin_Day15 = 190114,

    Heroin_Lv5 = 190121,
    Heroin_Lv10 = 190122,
    Heroin_Lv15 = 190123,
    Heroin_Lv18 = 190124,


    //? 우투리
    Utori_First = 190500,
    Utori_Yes = 190511,
    Utori_No = 190512,
    Utori_Yes2 = 190513,

    Utori_NoGold = 190520,
    Utori_ArtifactBuy = 190599,
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
        hpMax = monster.HP_MAX;
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

    public int SlotIndex;

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
    public HashSet<TraitGroup> currentDisableTraitList;

    //? 이벤트 리스트
    public Monster.UnitEvent unitEvent;

    public void SetData(Monster monster)
    {
        CustomName = monster.CustomName;
        PrefabPath = monster.Data.prefabPath;
        LV = monster.LV;
        HP = monster.HP;
        HP_MAX = monster.HP_MAX;
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

        SlotIndex = monster.MonsterID;

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

        traitCounter = monster.traitCounter.DeepCopy();
        currentTraitList = monster.SaveTraitList();
        currentDisableTraitList = monster.SaveDisableTraitList();
        unitEvent = monster.UnitDialogueEvent.DeepCopy();
    }
}


