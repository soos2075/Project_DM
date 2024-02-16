using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    #region singleton
    private static Main _instance;
    public static Main Instance { get { Init(); return _instance; } }

    static void Init()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<Main>();
            if (_instance == null)
            {
                var go = new GameObject(name: "@Main");
                _instance = go.AddComponent<Main>();
                DontDestroyOnLoad(go);
            }
        }
    }
    #endregion

    //private void Awake()
    //{
       
    //}

    void Start()
    {
        Debug.Log("디버그용 Start");
        //NewGame_Init();
        //Default_Init();
        Test_Init();
    }
    [Obsolete]
    void Test_Init()
    {
        ActiveFloor_Basement = 5;
        ActiveFloor_Technical = 2;
        DungeonRank = 2;

        DangerOfDungeon = 100;
        FameOfDungeon = 80;
        //Turn = 7;

        Player_Mana = 3000;
        Player_Gold = 3000;
        Player_AP = 100;
        AP_MAX = 100;

        Init_BasementFloor();
        Init_Animation();
        UI_Main.Start_Main();
        UI_Main.ButtonAllActive();

        Init_DayResult();
        ExpansionConfirm();
        GameManager.Technical.Expantion_Technical();

        Init_Secret();
        Init_Basic();
    }


    UI_Management _ui_main;
    UI_Management UI_Main 
    {
        get
        {
            if (_ui_main == null)
            {
                //_ui_main = FindObjectOfType<UI_Management>();
                _ui_main = FindAnyObjectByType<UI_Management>();
            }
            return _ui_main; } 
    }

    bool _DefaultSetting = false;

    public void Default_Init()
    {
        if (_DefaultSetting)
        {
            return;
        }


        Init_BasementFloor();
        Init_Animation();
        UI_Main.Start_Main();

        _DefaultSetting = true;
    }

    public void NewGame_Init()
    {
        if (_DefaultSetting)
        {
            return;
        }

        ActiveFloor_Basement = 4;
        ActiveFloor_Technical = 0;
        DungeonRank = 1;

        Player_Mana = 300;
        Player_Gold = 300;
        Player_AP = 2;
        AP_MAX = 2;


        Init_BasementFloor();
        Init_Animation();
        UI_Main.Start_Main();
        Init_DayResult();
        ExpansionConfirm();
        GameManager.Technical.Expantion_Technical();

        StartCoroutine(NextStart());

        _DefaultSetting = true;
    }




    IEnumerator NextStart()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        Instantiate_Egg();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Time.timeScale == 1);
        var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
        message.Message = "던전에 다양한 시설과 몬스터들을 배치하여 모험가들에게 마나를 얻으세요!\n\n" +
            "마나는 기본적으로 모험가들이 던전에서 행하는 모든 행동에서 얻을 수 있어요.";

    }
    void Instantiate_Egg()
    {
        Init_Secret();
        Init_Basic();

        Managers.Dialogue.ShowDialogueUI("Prologue", GameObject.Find("Player").transform);
    }


    void Init_Secret()
    {
        BasementTile tile = null;
        Floor[3].TileMap.TryGetValue(new Vector2Int(0, 3), out tile);
        PlacementInfo info = new PlacementInfo(Floor[3], tile);
        var egg = GameManager.Placement.CreateOnlyOne($"Facility/Special_MagicEgg", info, Define.PlacementType.Facility);
        GameManager.Placement.PlacementConfirm(egg, info, true);
        //GameManager.Facility.CreateFacility_OnlyOne("Special_MagicEgg", info, true);



        BasementTile tile2 = null;
        Floor[3].TileMap.TryGetValue(new Vector2Int(3, 3), out tile2);
        PlacementInfo info2 = new PlacementInfo(Floor[3], tile2);
        var player = GameManager.Placement.CreatePlacementObject("Player", info2, Define.PlacementType.Monster);
        var component = player as Player;
        component.MonsterInit();
        component.Level_Stat(1);
        component.State = Monster.MonsterState.Placement;
        GameManager.Placement.PlacementConfirm(player, info2);
    }
    void Init_Basic()
    {
        for (int k = 0; k < 5; k++)
        {
            BasementTile tile = Floor[0].GetRandomTile();
            var info = new PlacementInfo(Floor[0], tile);
            GameManager.Facility.CreateFacility("Herb_Low", info);
        }
        for (int k = 0; k < 2; k++)
        {
            BasementTile tile = Floor[1].GetRandomTile();
            var info = new PlacementInfo(Floor[1], tile);
            GameManager.Facility.CreateFacility("Herb_Low", info);
        }
        for (int k = 0; k < 4; k++)
        {
            BasementTile tile = Floor[2].GetRandomTile();
            var info = new PlacementInfo(Floor[2], tile);
            GameManager.Facility.CreateFacility("Mineral_Rock", info);
        }
    }



    #region Load Data
    public void SetLoadData(DataManager.SaveData data)
    {
        Turn = data.turn;
        Final_Score = data.Final_Score;

        DungeonRank = data.DungeonLV;
        FameOfDungeon = data.FameOfDungeon;
        DangerOfDungeon = data.DangerOfDungeon;

        Player_Mana = data.Player_Mana;
        Player_Gold = data.Player_Gold;
        Player_AP = data.Player_AP;
        AP_MAX = data.AP_MAX;

        Prisoner = data.Prisoner;

        CurrentDay = data.CurrentDay;

        DayList = data.DayResultList;

        ActiveFloor_Basement = (data.ActiveFloor_Basement);
        ActiveFloor_Technical = (data.ActiveFloor_Technical);
        ExpansionConfirm();
        GameManager.Technical.Expantion_Technical();
        UI_Main.DungeonExpansion();

        UI_Main.Texts_Refresh();



        //? 플레이어랑 알소환
        Init_Secret();
    }
    #endregion


    #region Management
    public int ActiveFloor_Basement { get; private set; }
    public int ActiveFloor_Technical { get; private set; }

    public void Basement_Expansion()
    {
        if (ActiveFloor_Basement < Floor.Length)
        {
            ActiveFloor_Basement++;
            ExpansionConfirm();

            if (ActiveFloor_Basement == 5)
            {
                Technical_Expansion();
            }
        }
    }
    public void Technical_Expansion()
    {
        if (ActiveFloor_Technical < GameManager.Technical.Floor_Technical.Length)
        {
            ActiveFloor_Technical++;
            GameManager.Technical.Expantion_Technical();
        }
    }


    public int Final_Score { get; private set; }

    void AddScore(DayResult day)
    {
        int score = day.Get_Mana;
        score += day.Get_Gold;
        score += day.Get_Prisoner * 50;
        score += day.Get_Kill * 100;

        Final_Score += score;
    }

    int _fame;
    public int FameOfDungeon { get { return _fame; } private set { _fame = Mathf.Clamp(value, 0, value); } }
    int _danger;
    public int DangerOfDungeon { get { return _danger; } private set { _danger = Mathf.Clamp(value, 0, value); } }


    public int DungeonRank { get; set; }
    public int Player_Mana { get; private set; }
    public int Player_Gold { get; private set; }

    
    public int AP_MAX { get; private set; }

    public void AddAP()
    {
        AP_MAX++;
    }
    public int Player_AP { get; set; }
    public int Prisoner { get; set; }


    public List<DayResult> DayList { get; private set; } = new List<DayResult>();

    public DayResult CurrentDay { get; set; }


    public class DayResult
    {
        public int Origin_Mana;
        public int Origin_Gold;
        public int Origin_Prisoner;

        public void SetOrigin(int mana, int gold, int prisoner)
        {
            Origin_Mana = mana;
            Origin_Gold = gold;
            Origin_Prisoner = prisoner;
        }


        public int Get_Mana;
        public int Get_Gold;
        public int Get_Prisoner;
        public int Get_Kill;

        public int Use_Mana;
        public int Use_Gold;
        public int Use_Prisoner;
        public int Use_Kill;


        public void AddMana(int value)
        {
            Get_Mana += value;
        }
        public void AddGold(int value)
        {
            Get_Gold += value;
        }
        public void AddPrisoner(int value)
        {
            Get_Prisoner += value;
        }
        public void AddKill(int value)
        {
            Get_Kill += value;
        }


        //? 사용
        public void SubtractMana(int value)
        {
            Use_Mana += value;
            Instance.Player_Mana -= value;
        }
        public void SubtractGold(int value)
        {
            Use_Gold += value;
            Instance.Player_Gold -= value;
        }
        public void SubtractPrisoner(int value)
        {
            Use_Prisoner += value;
        }
        public void SubtractKill(int value)
        {
            Use_Kill += value;
        }


        public int Fame { get; set; }
        public int Danger { get; set; }

        public int fame_perv;
        public int danger_perv;

        public int dungeonRank;
    }


    void Init_DayResult()
    {
        CurrentDay = new DayResult();
        CurrentDay.SetOrigin(Player_Mana, Player_Gold, Prisoner);
        CurrentDay.fame_perv = FameOfDungeon;
        CurrentDay.danger_perv = DangerOfDungeon;
        CurrentDay.dungeonRank = DungeonRank;
    }




    void DayOver_Dayresult()
    {
        DayList.Add(CurrentDay);
        AddScore(CurrentDay);


        Player_Mana += CurrentDay.Get_Mana;
        Player_Gold += CurrentDay.Get_Gold;
        Player_AP = AP_MAX;
        FameOfDungeon += CurrentDay.Fame;
        DangerOfDungeon += CurrentDay.Danger;

        DangerOfDungeon += 10;

        EventManager.Instance.TryRankUp(FameOfDungeon, DangerOfDungeon);

        var ui = Managers.UI.ShowPopUp<UI_DayResult>();
        ui.TextContents(DayList[Turn - 1]);
        //ui.RankUpResult(EventManager.Instance.TryRankUp(FameOfDungeon, DangerOfDungeon));
        //? 위가 적용 아래가 새로교체

        CurrentDay = new DayResult();
        CurrentDay.SetOrigin(Player_Mana, Player_Gold, Prisoner);
        CurrentDay.fame_perv = FameOfDungeon;
        CurrentDay.danger_perv = DangerOfDungeon;
    }

    #endregion








    #region TechnicalEvent
    public List<Action<int>> DayActions { get; set; } = new List<Action<int>>();
    public List<Action<int>> NightActions { get; set; } = new List<Action<int>>();

    public TechnicalFloor CurrentTechnical { get; set; }

    void DayEvent ()
    {
        for (int i = 0; i < DayActions.Count; i++)
        {
            DayActions[i].Invoke(Turn);
        }
    }
    void NightEvent()
    {
        for (int i = 0; i < NightActions.Count; i++)
        {
            NightActions[i].Invoke(Turn);
        }
    }


    #endregion


    #region Day
    public int Turn { get; set; } = 0;


    private bool _Management = true;
    public bool Management
    {
        get { return _Management; }
        set
        {
            _Management = value;
            if (_Management == false)
            {
                Turn++;
                TurnStartEvent();
                DayEvent();
                GameManager.NPC.TurnStart();
            }
            else
            {
                DayOver_Dayresult();
                DayMonsterEvent();
                NightEvent();
                TurnOverEvent();
                UI_Main.Texts_Refresh();
            }
        }
    }


    public void DayChange()
    {

        Managers.UI.CloseAll();

        Management = !Management;

        DayChangeAnimation();
    }





    public void TurnStartEvent()
    {
        UI_EventBox.AddEventText($"※{Turn}일차 시작※");


        switch (Turn)
        {
            case 0:
                break;

            case 1:
                //Debug.Log("1일차 시작 이벤트 발생");
                
                break;

            case 3:
                Debug.Log("3일차 시작 이벤트 - 모험가 한명 무조건 소환");
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;


            case 8:
                Debug.Log("8일차 시작 이벤트 - 패배 트리거 이벤트 모험가 소환");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;

            case 15:
                Debug.Log("8일차 시작 이벤트 - 패배 트리거 이벤트 모험가 소환");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;

            case 23:
                Debug.Log("8일차 시작 이벤트 - 패배 트리거 이벤트 모험가 소환");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;

            case 30:
                Debug.Log("8일차 시작 이벤트 - 패배 트리거 이벤트 모험가 소환");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;


            default:
                break;
        }
    }
    public void TurnOverEvent()
    {
        UI_EventBox.AddEventText($"※{Turn}일차 종료※");


        switch (Turn)
        {

            case 1:
                Debug.Log("1일차 종료 이벤트 - 시설배치");
                Managers.Dialogue.ShowDialogueUI("Facility", GameObject.Find("Player").transform);
                UI_Main.Active_Button(UI_Management.ButtonEvent._1_Facility);
                UI_Main.Active_Floor();
                break;

            case 2:
                Debug.Log("2일차 종료 이벤트 - 몬스터");
                Managers.Dialogue.ShowDialogueUI("Monster", GameObject.Find("Player").transform);
                UI_Main.Active_Button(UI_Management.ButtonEvent._2_Summon);
                UI_Main.Active_Button(UI_Management.ButtonEvent._3_Management);
                break;

            case 3:
                Debug.Log("4일차 종료 이벤트 - 테크니컬");
                Technical_Expansion();
                Managers.Dialogue.ShowDialogueUI("Technical", GameObject.Find("Player").transform);
                break;

            case 4:
                Debug.Log("4일차 종료 이벤트 - 비밀방");
                Managers.Dialogue.ShowDialogueUI("EggAppear", GameObject.Find("Player").transform);
                break;

            case 5:
                Debug.Log("5일차 종료 이벤트 - 길드");
                Managers.Dialogue.ShowDialogueUI("Guild", GameObject.Find("Player").transform);
                UI_Main.Active_Button(UI_Management.ButtonEvent._4_Guild);
                break;

            case 6:
                break;

            default:
                break;
        }

        StartCoroutine(AutoSave());
    }


    IEnumerator AutoSave()
    {
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => Time.timeScale == 1);
        Debug.Log($"자동저장 : {Turn}일차");
        Managers.Data.SaveToJson("AutoSave", 0);
    }



    void DayMonsterEvent()
    {
        if (GameManager.Monster.LevelUpList != null)
        {
            foreach (var item in GameManager.Monster.LevelUpList)
            {
                Managers.UI.Popup_Reservation(() => 
                {
                    for (int i = 0; i < item.times; i++)
                    {
                        item.monster.LevelUp(false);
                    }
                    var ui = Managers.UI.ShowPopUp<UI_StatusUp>();
                    ui.TargetMonster(item.monster, item.lv, item.hpMax, item.atk, item.def, item.agi, item.luk);
                });
            }
            GameManager.Monster.LevelUpList.Clear();
        }
    }



    #endregion




   





   







    #region Animation
    Animator ani_MainUI;
    Animator ani_Sky;
    VerticalLayoutGroup layout;

    void Init_Animation()
    {
        ani_MainUI = UI_Main.GetComponent<Animator>();
        ani_Sky = GameObject.Find("SkyBackground").GetComponent<Animator>();
        layout = UI_Main.GetComponentInChildren<VerticalLayoutGroup>();
    }


    public void DayChangeAnimation() 
    {
        layout.enabled = false;

        ani_MainUI.SetBool("Management", Management);
        ani_Sky.SetBool("Management", Management);
    }


    #endregion








    #region Floor

    public BasementFloor[] Floor { get; set; }

    public BasementFloor CurrentFloor { get; set; }
    public BasementTile CurrentTile { get; set; }
    public Action CurrentAction { get; set; }

    public Vector2Int[] CurrentBoundary { get; set; } = Define.Boundary_Cross_1;

    void Init_BasementFloor()
    {
        Floor = FindObjectsOfType<BasementFloor>();
        System.Array.Sort(Floor, (a, b) =>
        {
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        });

        for (int i = 0; i < Floor.Length; i++)
        {
            Floor[i].FloorIndex = i;
            Floor[i].Init_Floor();

            //Floor[i].gameObject.SetActive(false);
        }
    }



    void ExpansionConfirm()
    {
        for (int i = 0; i < Floor.Length; i++)
        {
            Floor[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < ActiveFloor_Basement; i++)
        {
            Floor[i].gameObject.SetActive(true);
            Floor[i].Init_Entrance();
        }


        if (ActiveFloor_Basement >= 4)
        {
            Floor[3].Hidden = true;
        }
        //DungeonExpansionUI();
        //UI_Main.DungeonExpansion();

        Camera.main.GetComponent<CameraControl>().LimitRefresh();
    }

    public void DungeonExpansionUI()
    {
        if (Floor.Length > ActiveFloor_Basement)
        {
            var ui = Managers.Resource.Instantiate("UI/PopUp/Element/UI_Expansion_Floor");
            ui.transform.position = Floor[ActiveFloor_Basement].transform.position;

            ui.GetComponent<UI_Expansion_Floor>().SetContents(ActiveFloor_Basement, 200, 200, 2);
        }
    }

    #endregion

}
