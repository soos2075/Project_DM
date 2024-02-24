using DamageNumbersPro;
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
    #region TextMesh
    Transform guild;
    Transform dungeon;

    public DamageNumber dmMesh;
    public DamageNumber dmMesh_dungeon;

    public void ShowDM_Guild(string _msg, Color _color)
    {
        var dm = dmMesh.Spawn(guild.position, _msg);
        dm.SetColor(_color);
    }
    public void ShowDM_Dungeon(string _msg, Color _color)
    {
        var dm = dmMesh.Spawn(dungeon.position, _msg);
        dm.SetColor(_color);
    }


    #endregion

    void Start()
    {
        guild = transform.GetChild(0);
        dungeon = transform.GetChild(1);

        //Debug.Log("����׿� Start");
        //NewGame_Init();
        //Default_Init();
        Test_Init();
    }
    [Obsolete]
    public void Test_Init()
    {
        ActiveFloor_Basement = 5;
        ActiveFloor_Technical = 2;
        DungeonRank = 2;

        DangerOfDungeon = 100;
        PopularityOfDungeon = 500;

        Player_Mana = 3000;
        Player_Gold = 3000;
        Player_AP = 30;
        AP_MAX = 30;

        Init_BasementFloor();
        Init_Animation();
        UI_Main.Start_Main();
        UI_Main.ButtonAllActive();

        Init_DayResult();
        ExpansionConfirm();
        GameManager.Technical.Expantion_Technical();

        Init_Secret();
        Init_Basic();
        Init_Statue();

        EventManager.Instance.Load_EventData();
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
        Instantiate_DayOne();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Time.timeScale == 1);
        var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
        message.Message = "������ �پ��� �ü��� ���͵��� ��ġ�Ͽ� ���谡�鿡�� ������ ��������!\n\n" +
            "������ �⺻������ ���谡���� �������� ���ϴ� ��� �ൿ���� ���� �� �־��.";

    }
    void Instantiate_DayOne()
    {
        Init_Secret();
        Init_Basic();
        Init_Statue();
        Init_EggEntrance();

        Managers.Dialogue.ShowDialogueUI("Prologue", GameObject.Find("Player").transform);
    }


    void Init_Secret()
    {
        BasementTile tile = null;
        Floor[3].TileMap.TryGetValue(new Vector2Int(0, 3), out tile);
        PlacementInfo info = new PlacementInfo(Floor[3], tile);
        var egg = GameManager.Placement.CreateOnlyOne($"Facility/Special_MagicEgg", info, PlacementType.Facility);
        GameManager.Placement.PlacementConfirm(egg, info, true);
        //GameManager.Facility.CreateFacility_OnlyOne("Special_MagicEgg", info, true);
        Init_Player();
    }

    public void Init_Player()
    {
        if (GameObject.Find("Player") != null)
        {
            return;
        }

        BasementTile tile2 = null;
        Floor[3].TileMap.TryGetValue(new Vector2Int(3, 3), out tile2);
        PlacementInfo info2 = new PlacementInfo(Floor[3], tile2);
        var player = GameManager.Placement.CreatePlacementObject("Player", info2, PlacementType.Monster);
        var component = player as Player;
        component.MonsterInit();
        component.Level_Stat(DungeonRank);
        component.State = Monster.MonsterState.Placement;
        GameManager.Placement.PlacementConfirm(player, info2);
    }

    void Init_Basic()
    {
        for (int k = 0; k < 8; k++)
        {
            BasementTile tile = Floor[0].GetRandomTile();
            var info = new PlacementInfo(Floor[0], tile);
            var obj = GameManager.Facility.CreateFacility("Herb", info);
            var herb = obj as Herb;
            herb.OptionIndex = (int)Herb.HerbType.Low;
        }
        for (int k = 0; k < 2; k++)
        {
            BasementTile tile = Floor[1].GetRandomTile();
            var info = new PlacementInfo(Floor[1], tile);
            var obj = GameManager.Facility.CreateFacility("Herb", info);
            var herb = obj as Herb;
            herb.OptionIndex = (int)Herb.HerbType.High;
        }
        for (int k = 0; k < 5; k++)
        {
            BasementTile tile = Floor[2].GetRandomTile();
            var info = new PlacementInfo(Floor[2], tile);
            var facil = GameManager.Facility.CreateFacility("Mineral", info);
            var mineral = facil as Mineral;
            mineral.OptionIndex = (int)Mineral.MineralType.Rock;
        }
    }

    void Init_EggEntrance()
    {
        {
            var tile = Main.Instance.Floor[3].GetRandomTile();
            Main.Instance.Floor[3].TileMap.TryGetValue(new Vector2Int(12, 3), out tile);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[3], tile);

            var obj = GameManager.Facility.CreateFacility_OnlyOne("Obstacle", info, true);
        }

        {
            var tile = Main.Instance.Floor[2].GetRandomTile();
            Main.Instance.Floor[2].TileMap.TryGetValue(new Vector2Int(0, 0), out tile);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[2], tile);

            var obj = GameManager.Facility.CreateFacility_OnlyOne("Obstacle", info, true);
        }
    }

    void Init_Statue()
    {
        //? ���� ������
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(7, 0), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Avatar", info, true);
            var statue = obj as Statue;
            statue.statueType = Statue.StatueType.Mana;
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(7, 1), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Avatar", info, true);
            var statue = obj as Statue;
            statue.statueType = Statue.StatueType.Mana;
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(8, 0), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Avatar", info, true);
            var statue = obj as Statue;
            statue.statueType = Statue.StatueType.Mana;
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(8, 1), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Avatar", info, true);
            var statue = obj as Statue;
            statue.statueType = Statue.StatueType.Mana;
        }

        //? ��� ������
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(7, 4), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Avatar", info, true);
            var statue = obj as Statue;
            statue.statueType = Statue.StatueType.Gold;
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(7, 5), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Avatar", info, true);
            var statue = obj as Statue;
            statue.statueType = Statue.StatueType.Gold;
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(8, 4), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Avatar", info, true);
            var statue = obj as Statue;
            statue.statueType = Statue.StatueType.Gold;
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(8, 5), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Avatar", info, true);
            var statue = obj as Statue;
            statue.statueType = Statue.StatueType.Gold;
        }

    }


    #region Load Data
    public void SetLoadData(DataManager.SaveData data)
    {
        Turn = data.turn;
        Final_Score = data.Final_Score;

        DungeonRank = data.DungeonLV;
        PopularityOfDungeon = data.FameOfDungeon;
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


        //? �÷��̾�� �˼�ȯ
        Init_Secret();

        //? ���� ����
        EventManager.Instance.Load_EventData();

        UI_Main.DungeonExpansion();
        UI_Main.Texts_Refresh();
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

    int _pop;
    public int PopularityOfDungeon { get { return _pop; } private set { _pop = Mathf.Clamp(value, 0, value); } }
    int _danger;
    public int DangerOfDungeon { get { return _danger; } private set { _danger = Mathf.Clamp(value, 0, value); } }


    public int DungeonRank { get; set; }
    public int Player_Mana { get; private set; }
    public int Player_Gold { get; private set; }


    public int AP_MAX { get; private set; }

    public void Set_AP_Max(int _ap)
    {
        AP_MAX = _ap;
    }

    int player_ap;
    public int Player_AP { get { return player_ap; } set { player_ap = value; UI_Main.AP_Refresh(); } }
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
            Instance.Player_Mana += value;
        }
        public void AddGold(int value)
        {
            Get_Gold += value;
            Instance.Player_Gold += value;
        }
        public void AddPrisoner(int value)
        {
            Get_Prisoner += value;
        }
        public void AddKill(int value)
        {
            Get_Kill += value;
        }


        //? ���
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

        public int Popularity { get; private set; }
        public int Danger { get; private set; }

        public void AddPop(int _value)
        {
            Popularity += _value;
            if (_value > 0)
            {
                Instance.ShowDM_Dungeon($"+{_value} pop", Color.green);
            }
            else
            {
                Instance.ShowDM_Dungeon($"{_value} pop", Color.green);
            }
        }
        public void AddDanger(int _value)
        {
            Danger += _value;
            if (_value > 0)
            {
                Instance.ShowDM_Dungeon($"+{_value} danger", Color.red);
            }
            else
            {
                Instance.ShowDM_Dungeon($"{_value} danger", Color.red);
            }
        }

        public int fame_perv;
        public int danger_perv;
        public int dungeonRank;
    }


    void Init_DayResult()
    {
        CurrentDay = new DayResult();
        CurrentDay.SetOrigin(Player_Mana, Player_Gold, Prisoner);
        CurrentDay.fame_perv = PopularityOfDungeon;
        CurrentDay.danger_perv = DangerOfDungeon;
        CurrentDay.dungeonRank = DungeonRank;
    }




    void DayOver_Dayresult()
    {
        DayList.Add(CurrentDay);
        AddScore(CurrentDay);

        Player_AP = AP_MAX;

        //Player_Mana += CurrentDay.Get_Mana;
        //Player_Gold += CurrentDay.Get_Gold;
        PopularityOfDungeon += CurrentDay.Popularity;
        DangerOfDungeon += CurrentDay.Danger;

        EventManager.Instance.TryRankUp(PopularityOfDungeon, DangerOfDungeon);

        var ui = Managers.UI.ShowPopUp<UI_DayResult>();
        ui.TextContents(DayList[Turn - 1]);
        //ui.RankUpResult(EventManager.Instance.TryRankUp(FameOfDungeon, DangerOfDungeon));
        //? ���� ���� �Ʒ��� ���α�ü

        Init_DayResult();
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
                Start_Entrance();
                TurnStartEvent();
                DayEvent();
                BattleManager.Instance.TurnStart();
                EventManager.Instance.TurnStart();
                GameManager.NPC.TurnStart();
                GameManager.Monster.MonsterTurnStartEvent();
            }
            else
            {
                Init_Player(); //? �÷��̾� ������ ���ȯ
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
        UI_EventBox.AddEventText($"��{Turn}���� ���ۡ�");


        switch (Turn)
        {
            case 0:
                break;

            case 1:
                //Debug.Log("1���� ���� �̺�Ʈ �߻�");
                
                break;

            case 3:
                Debug.Log("3���� ���� �̺�Ʈ - ���谡 �Ѹ� ������ ��ȯ");
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;


            case 8:
                Debug.Log("8���� ���� �̺�Ʈ - �й� Ʈ���� �̺�Ʈ ���谡 ��ȯ");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;

            case 15:
                Debug.Log("8���� ���� �̺�Ʈ - �й� Ʈ���� �̺�Ʈ ���谡 ��ȯ");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;

            case 23:
                Debug.Log("8���� ���� �̺�Ʈ - �й� Ʈ���� �̺�Ʈ ���谡 ��ȯ");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;

            case 30:
                Debug.Log("8���� ���� �̺�Ʈ - �й� Ʈ���� �̺�Ʈ ���谡 ��ȯ");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;


            default:
                break;
        }
    }
    public void TurnOverEvent()
    {
        UI_EventBox.AddEventText($"��{Turn}���� �����");


        switch (Turn)
        {

            case 1:
                Debug.Log("1���� ���� �̺�Ʈ - �ü���ġ");
                Managers.Dialogue.ShowDialogueUI("Facility", GameObject.Find("Player").transform);
                UI_Main.Active_Button(UI_Management.ButtonEvent._1_Facility);
                UI_Main.Active_Floor();
                break;

            case 2:
                Debug.Log("2���� ���� �̺�Ʈ - ����");
                Managers.Dialogue.ShowDialogueUI("Monster", GameObject.Find("Player").transform);
                UI_Main.Active_Button(UI_Management.ButtonEvent._2_Summon);
                UI_Main.Active_Button(UI_Management.ButtonEvent._3_Management);
                break;

            case 3:
                Debug.Log("4���� ���� �̺�Ʈ - ��ũ����");
                Technical_Expansion();
                Managers.Dialogue.ShowDialogueUI("Technical", GameObject.Find("Player").transform);
                break;

            case 4:
                Debug.Log("4���� ���� �̺�Ʈ - ��й�");
                Managers.Dialogue.ShowDialogueUI("EggAppear", GameObject.Find("Player").transform);
                break;

            case 5:
                Debug.Log("5���� ���� �̺�Ʈ - ���");
                Managers.Dialogue.ShowDialogueUI("Guild", GameObject.Find("Player").transform);
                UI_Main.Active_Button(UI_Management.ButtonEvent._4_Guild);
                break;

            case 6:
                break;

            case 15:
                Debug.Log("�ӽ÷� 15�� ����Ŭ����");
                Managers.Dialogue.ShowDialogueUI("Ending", GameObject.Find("Player").transform);
                break;

            case 30:
                Debug.Log("����Ŭ����");
                break;


            default:
                break;
        }

        StartCoroutine(AutoSave());
    }


    IEnumerator AutoSave()
    {
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => Time.timeScale == 1 && Managers.UI._reservationQueue.Count == 0 && Managers.UI._popupStack.Count == 0);
        Debug.Log($"�ڵ����� : {Turn}����");
        Managers.Data.SaveToJson("AutoSave", 0);
    }



    void DayMonsterEvent()
    {
        StartCoroutine(WaitForResultUI());
    }

    IEnumerator WaitForResultUI()
    {
        yield return new WaitForEndOfFrame();

        GameManager.Monster.InjuryMonster = 0;

        if (GameManager.Monster.LevelUpList.Count != 0)
        {
            foreach (var item in GameManager.Monster.LevelUpList)
            {
                Managers.UI.Popup_Reservation(() =>
                {
                    for (int i = 0; i < item.times; i++)
                    {
                        item.monster.LevelUp(false);
                    }
                    var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
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
    public Action PurchaseAction { get; set; }

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


    void Start_Entrance()
    {
        for (int i = 0; i < ActiveFloor_Basement; i++)
        {
            Floor[i].Init_Entrance();
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
            //Floor[i].Init_Entrance();
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
