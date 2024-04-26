using DamageNumbersPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
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
    public Transform Guild
    {
        get
        {
            if (guild == null)
            {
                guild = transform.GetChild(0);
            }
            return guild;
        }
        set { guild = value; }
    }
    Transform dungeon;
    public Transform Dungeon
    {
        get
        {
            if (dungeon == null)
            {
                dungeon = transform.GetChild(1);
            }
            return dungeon;
        }
        set { dungeon = value; }
    }

    public DamageNumber dm_large; // 1
    public DamageNumber dm_small; // 0

    public enum TextType
    {
        pop,
        danger,
        mana,
        gold,
        exp,
    }
    public void ShowDM(int _value, TextType _textType, Transform _pos, int _sizeOption = 0)
    {
        DamageNumber origin = _sizeOption == 0 ? dm_small : dm_large;
        SoundManager.Instance.PlaySound($"SFX/Add_{_textType.ToString()}");

        string _msg = _value > 0 ? $"+{_value} " : $"{_value} ";
        switch (_textType)
        {
            case TextType.pop:
                _msg += "pop";
                var dm = origin.Spawn(_pos.position, _msg);
                dm.SetColor(Color.green);
                break;

            case TextType.danger:
                _msg += "danger";
                var dm2 = origin.Spawn(_pos.position, _msg);
                dm2.SetColor(Color.red);
                break;

            case TextType.mana:
                _msg += "mana";
                var dm3 = origin.Spawn(_pos.position, _msg);
                dm3.SetColor(Color.blue);
                break;

            case TextType.gold:
                _msg += "gold";
                var dm4 = origin.Spawn(_pos.position, _msg);
                dm4.SetColor(Color.yellow);
                break;

            case TextType.exp:
                _msg += "exp";
                var dm5 = origin.Spawn(_pos.position, _msg);
                dm5.SetColor(Color.white);
                break;
        }
    }

    #endregion

    void Start()
    {
        //Debug.Log("디버그용 Start");
        //NewGame_Init();
        //Default_Init();
        //Test_Init();
    }
    [Obsolete]
    public void Test_Init()
    {
        //ActiveFloor_Basement = 5;
        //ActiveFloor_Technical = 2;
        //DungeonRank = 3;
        //EventManager.Instance.RankUpEvent();

        DangerOfDungeon = 190;
        PopularityOfDungeon = 200;

        Player_Mana = 30000;
        Player_Gold = 30000;
        Player_AP = 20;
        AP_MAX = 20;

        //Init_BasementFloor();
        //Init_Animation();
        //UI_Main.Start_Main();
        //UI_Main.ButtonAllActive();

        //Init_DayResult();
        //ExpansionConfirm();
        //GameManager.Technical.Expantion_Technical();

        //Init_Secret();
        //Init_Basic();
        //Init_Statue();

        //EventManager.Instance.RankUpEvent();

    }
    IEnumerator waitA()
    {
        yield return null;
        EventManager.Instance.AddQuestAction(1100);
        EventManager.Instance.AddQuestAction(1200);
    }


    UI_Management _ui_main;
    UI_Management UI_Main
    {
        get
        {
            if (_ui_main == null)
            {
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
        Player_Gold = 200;
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

        UserData.Instance.isClear = false;

        NewGame_GetClearBonus();
    }

    void NewGame_GetClearBonus()
    {
        if (CollectionManager.Instance.PlayData != null)
        {
            if (CollectionManager.Instance.PlayData.dataApply)
            {
                Debug.Log("클리어 데이터 적용");
                GameManager.Monster.Load_MonsterData(CollectionManager.Instance.PlayData.MonsterList);
            }
        }
    }



    IEnumerator NextStart()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        Instantiate_DayOne();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Time.timeScale > 0);
        var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
        message.Message = UserData.Instance.GetLocaleText("Message_First");

    }
    void Instantiate_DayOne()
    {
        Init_Secret();
        Init_Basic();
        Init_Statue();
        Init_EggEntrance();

        Managers.Dialogue.ShowDialogueUI(DialogueName.Prologue, GameObject.Find("Player").transform);
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
            var obj = GameManager.Facility.CreateFacility("Herb_Low", info);
        }
        for (int k = 0; k < 2; k++)
        {
            BasementTile tile = Floor[1].GetRandomTile();
            var info = new PlacementInfo(Floor[1], tile);
            var obj = GameManager.Facility.CreateFacility("Herb_High", info);
        }
        for (int k = 0; k < 5; k++)
        {
            BasementTile tile = Floor[2].GetRandomTile();
            var info = new PlacementInfo(Floor[2], tile);
            var facil = GameManager.Facility.CreateFacility("Mineral_Rock", info);
        }
    }

    void Init_EggEntrance()
    {
        {
            var tile = Main.Instance.Floor[3].GetRandomTile();
            Main.Instance.Floor[3].TileMap.TryGetValue(new Vector2Int(12, 3), out tile);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[3], tile);

            var obj = GameManager.Facility.CreateFacility("Obstacle", info);
        }

        {
            var tile = Main.Instance.Floor[2].GetRandomTile();
            Main.Instance.Floor[2].TileMap.TryGetValue(new Vector2Int(0, 0), out tile);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[2], tile);

            var obj = GameManager.Facility.CreateFacility("Obstacle", info);
        }
    }

    void Init_Statue()
    {
        //? 골드 스태츄
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(10, 4), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Gold", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(10, 5), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Gold", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(11, 4), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Gold", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(11, 5), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Gold", info);
        }

        //? 마나 스태츄
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(10, 0), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Mana", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(10, 1), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Mana", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(11, 0), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Mana", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(11, 1), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Statue_Mana", info);
        }


        //? 개 스태츄
        if (UserData.Instance.FileConfig.Statue_Dog)
        {
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(7, 4), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Statue_Dog", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(7, 5), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Statue_Dog", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(8, 4), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Statue_Dog", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(8, 5), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Statue_Dog", info);
            }
        }
        else
        {
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(7, 4), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(7, 5), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(8, 4), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(8, 5), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
            }
        }

        //? 드래곤 스태츄
        if (UserData.Instance.FileConfig.Statue_Dragon)
        {
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(7, 0), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Statue_Dragon", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(7, 1), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Statue_Dragon", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(8, 0), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Statue_Dragon", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(8, 1), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Statue_Dragon", info);
            }
        }
        else
        {
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(7, 0), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(7, 1), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(8, 0), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
            }
            {
                BasementTile tile = null;
                Floor[3].TileMap.TryGetValue(new Vector2Int(8, 1), out tile);
                PlacementInfo info = new PlacementInfo(Floor[3], tile);
                var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
            }
        }

        //? 3번째 조각상 자리
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(4, 0), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(4, 1), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(5, 0), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(5, 1), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
        }

        //? 4번째 조각상 자리
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(4, 4), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(4, 5), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(5, 4), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
        }
        {
            BasementTile tile = null;
            Floor[3].TileMap.TryGetValue(new Vector2Int(5, 5), out tile);
            PlacementInfo info = new PlacementInfo(Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility($"Obstacle", info);
        }


        Init_Statue_Sprite();
    }


    void Init_Statue_Sprite()
    {
        SLA_ObjectManager.Instance.CreateObject("Statue_Mana", new Vector3(-3.5f, -15f, 0));
        SLA_ObjectManager.Instance.SetLabel("Statue_Mana", "Mana", "Entry");

        SLA_ObjectManager.Instance.CreateObject("Statue_Gold", new Vector3(-3.5f, -13f, 0));
        SLA_ObjectManager.Instance.SetLabel("Statue_Gold", "Gold", "Entry");


        if (UserData.Instance.FileConfig.Statue_Dog)
        {
            SLA_ObjectManager.Instance.CreateObject("Statue_Dog", new Vector3(-5f, -13f, 0));
            SLA_ObjectManager.Instance.SetLabel("Statue_Dog", "Dog", "Entry");
        }

        if (UserData.Instance.FileConfig.Statue_Dragon)
        {
            SLA_ObjectManager.Instance.CreateObject("Statue_Dragon", new Vector3(-5f, -15f, 0));
            SLA_ObjectManager.Instance.SetLabel("Statue_Dragon", "Dragon", "Entry");
        }
    }


    #region Save / Load

    public void GetPropertyValue(out int _pop, out int _danger, out int _currentAP)
    {
        _pop = this.pop;
        _danger = this.danger;
        _currentAP = currentAP;
    }


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

        CurrentDay = new DayResult(data.CurrentDay);
        DayList = new List<DayResult>();
        foreach (var item in data.DayResultList)
        {
            DayList.Add(new DayResult(item));
        }

        ActiveFloor_Basement = (data.ActiveFloor_Basement);
        ActiveFloor_Technical = (data.ActiveFloor_Technical);
        ExpansionConfirm();
        GameManager.Technical.Expantion_Technical();


        //? 플레이어랑 알소환
        Init_Secret();

        //? 레벨 적용
        EventManager.Instance.RankUpEvent();

        UI_Main.DungeonExpansion();
        UI_Main.Texts_Refresh();



        //Type type = CurrentDay.GetType();
        //// 클래스의 모든 필드 정보 가져오기
        //FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

        //// 각 필드의 이름과 값 출력
        //foreach (FieldInfo field in fields)
        //{
        //    object value = field.GetValue(CurrentDay);
        //    Debug.Log($"{field.Name}: {value}");
        //}
        Init_Statue_Sprite();
        ChangeEggState();
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

    int pop;
    public int PopularityOfDungeon { get { return pop; } private set { pop = Mathf.Clamp(value, 0, value); } }
    int danger;
    public int DangerOfDungeon { get { return danger; } private set { danger = Mathf.Clamp(value, 0, value); } }

    public int DungeonRank { get; private set; }
    public void Dungeon_RankUP()
    {
        DungeonRank++;
        AP_MAX = DungeonRank + 1;
        EventManager.Instance.RankUpEvent();
    }

    public int Player_Mana { get; private set; }
    public int Player_Gold { get; private set; }

    public int AP_MAX { get; private set; }

    int currentAP;
    public int Player_AP { get { return currentAP; } set { currentAP = value; UI_Main.AP_Refresh(); } }
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

        public int GetPopularity;
        public int GetDanger;

        public void AddPop(int _value)
        {
            GetPopularity += _value;
        }
        public void AddDanger(int _value)
        {
            GetDanger += _value;
        }

        public int fame_perv;
        public int danger_perv;
        public int dungeonRank;


        public DayResult()
        {

        }
        public DayResult(Save_DayResult result)
        {
            Origin_Mana = result.Origin_Mana;
            Origin_Gold = result.Origin_Gold;
            Origin_Prisoner = result.Origin_Prisoner;

            Get_Mana = result.Get_Mana;
            Get_Gold = result.Get_Gold;
            Get_Prisoner = result.Get_Prisoner;
            Get_Kill = result.Get_Kill;

            Use_Mana = result.Use_Mana;
            Use_Gold = result.Use_Gold;
            Use_Prisoner = result.Use_Prisoner;
            Use_Kill = result.Use_Kill;

            GetPopularity = result.GetPopularity;
            GetDanger = result.GetDanger;

            fame_perv = result.fame_perv;
            danger_perv = result.danger_perv;
            dungeonRank = result.dungeonRank;
        }
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

        PopularityOfDungeon += CurrentDay.GetPopularity;
        DangerOfDungeon += CurrentDay.GetDanger;

        if (EventManager.Instance.TryRankUp(PopularityOfDungeon, DangerOfDungeon))
        {
            Dungeon_RankUP();
        }

        Player_AP = AP_MAX;

        var ui = Managers.UI.ShowPopUp<UI_DayResult>();
        ui.TextContents(DayList[Turn - 1]);
        //ui.RankUpResult(EventManager.Instance.TryRankUp(FameOfDungeon, DangerOfDungeon));
        //? 위가 적용 아래가 새로교체

        Init_DayResult();
    }

    #endregion








    #region TechnicalEvent
    public List<Action<int>> DayActions { get; set; } = new List<Action<int>>();
    public List<Action<int>> NightActions { get; set; } = new List<Action<int>>();

    public TechnicalFloor CurrentTechnical { get; set; }

    void DayEvent()
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
                Init_Player(); //? 플레이어 없으면 재소환
                Start_Entrance();
                TurnStartEvent();
                DayEvent();
                BattleManager.Instance.TurnStart();
                EventManager.Instance.TurnStart();
                GameManager.NPC.TurnStart();
                GameManager.Monster.MonsterTurnStartEvent();
                GameManager.Facility.TurnStartEvent();
            }
            else
            {
                Init_Player(); //? 플레이어 없으면 재소환
                DayOver_Dayresult();
                TurnOverEvent();
                NightEvent();
                DayMonsterEvent();
                GameManager.Facility.TurnOverEvent();
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
        UI_EventBox.AddEventText($"※{Turn}{UserData.Instance.GetLocaleText("Event_DayStart")}※");


        switch (Turn)
        {

            case 1:
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Captine_A, 10f);
                //for (int i = 0; i < 7; i++)
                //{
                //    GameManager.NPC.AddEventNPC(NPCManager.NPCType.Event_Soldier1, 11 + (0.2f * i));
                //}

                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Captine_B, 13f);
                //for (int i = 0; i < 7; i++)
                //{
                //    GameManager.NPC.AddEventNPC(NPCManager.NPCType.Event_Soldier2, 14 + (0.2f * i));
                //}


                GameManager.NPC.AddEventNPC(NPCManager.NPCType.Captine_C, 10f);
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.B_Tanker, 11f);
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.B_Warrior, 11.5f);
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.B_Wizard, 12f);
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.B_Elf, 12.5f);


                break;

            case 3:
                Debug.Log("3일차 시작 이벤트 - 모험가 한명 무조건 소환");
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.Event_Day3, 7);
                break;


            case 8:
                Debug.Log("8일차 시작 이벤트 - 패배 트리거 이벤트 모험가 소환");
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.Event_Day8, 9);
                break;

            case 15:
                Debug.Log("15일차 시작 이벤트 - 패배 트리거 이벤트 모험가 소환");
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.Event_Day15, 9);
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;

            case 20:
                Debug.Log("20일차 시작 이벤트 - 붉은 모험단 소환");
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.A_Tanker, 10f);
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.A_Warrior, 10.5f);
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.A_Wizard, 11f);
                GameManager.NPC.AddEventNPC(NPCManager.NPCType.A_Elf, 11.5f);
                break;

            case 25:
                Debug.Log("25일차 시작 이벤트 - 길드 토벌단 1");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;

            case 30:
                Debug.Log("8일차 시작 이벤트 - 길드 토벌단 2 + 붉은 모험단");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;


            default:
                break;
        }
    }


    [Obsolete]
    void TestEnding()
    {
        Debug.Log("엔딩 테스트");

        //? 이건 30일 됐을 때 뜨는거
        Managers.Dialogue.ShowDialogueUI(DialogueName.Day30_Over, GameObject.Find("Player").transform);

        //? 이건 바로 엔딩씬 스킵하고 회차설정으로 넘어가는 부분
        //var ending = Managers.UI.ShowPopUp<UI_Ending>();
    }

    public void TurnOverEvent()
    {
        UI_EventBox.AddEventText($"※{Turn}{UserData.Instance.GetLocaleText("Event_DayOver")}※");


        switch (Turn)
        {
            //case 0:
            //    Debug.Log("0일차, 각종 테스트");
            //    var ending = Managers.UI.ShowPopUp<UI_Ending>();
            //    break;

            case 1:
                Debug.Log("1일차 종료 이벤트 - 시설배치");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Facility, GameObject.Find("Player").transform);
                UI_Main.Active_Button(UI_Management.ButtonEvent._1_Facility);
                UI_Main.Active_Floor();
                break;

            case 2:
                Debug.Log("2일차 종료 이벤트 - 몬스터");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Monster, GameObject.Find("Player").transform);
                UI_Main.Active_Button(UI_Management.ButtonEvent._2_Summon);
                UI_Main.Active_Button(UI_Management.ButtonEvent._3_Management);
                break;

            case 3:
                //TestEnding();

                Debug.Log("3일차 종료 이벤트 - 테크니컬");
                Technical_Expansion();
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Technical, GameObject.Find("Player").transform);
                break;

            case 4:
                Debug.Log("4일차 종료 이벤트 - 비밀방");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Egg, GameObject.Find("Player").transform);
                break;

            case 5:
                Debug.Log("5일차 종료 이벤트 - 길드");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Guild, GameObject.Find("Player").transform);
                UI_Main.Active_Button(UI_Management.ButtonEvent._4_Guild);
                break;

            case 6:
                //TestEnding();
                break;

            case 15:
                //Debug.Log("임시로 15일 게임클리어");
                //Managers.Dialogue.ShowDialogueUI(DialogueName.Ending_Demo, GameObject.Find("Player").transform);
                // 임시
                //Managers.Dialogue.ShowDialogueUI(DialogueName.Day30_Over, GameObject.Find("Player").transform);
                break;

            case 20:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Day20_Over, GameObject.Find("Player").transform);
                break;

            case 30:
                Debug.Log("게임클리어");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Day30_Over, GameObject.Find("Player").transform);
                break;


            default:
                break;
        }


        ChangeEggState();
        StartCoroutine(AutoSave());
    }


    IEnumerator AutoSave()
    {
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => Time.timeScale > 0 && Managers.UI._reservationQueue.Count == 0 && Managers.UI._popupStack.Count == 0);
        Debug.Log($"자동저장 : {Turn}일차");
        Managers.Data.SaveAndAddFile("AutoSave", 0);
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


            if (i == 3)
            {
                Floor[i].LabelName = $"{UserData.Instance.GetLocaleText("숨겨진곳")}";
            }
            else
            {
                Floor[i].LabelName = $"{UserData.Instance.GetLocaleText("지하")} {i + 1} {UserData.Instance.GetLocaleText("층")}";
            }


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
            var legacy = FindAnyObjectByType<UI_Expansion_Floor>();
            if (legacy != null)
            {
                Destroy(legacy.gameObject);
            }

            var ui = Managers.Resource.Instantiate("UI/PopUp/Element/UI_Expansion_Floor");
            ui.transform.position = Floor[ActiveFloor_Basement].transform.position;

            ui.GetComponent<UI_Expansion_Floor>().SetContents(ActiveFloor_Basement, 200, 200, 2);
        }
    }

    #endregion





    #region
    public enum EggState
    {
        Default_Level_1,
        Default_Level_2,
        Default_Level_3,

        Normal,
        Happy,
        Bad,
    }

    public EggState Egg_State { get; set; }

    GameObject eggObj;
    SpriteResolver EggSprite
    {
        get
        {
            if (eggObj == null)
            {
                eggObj = GameObject.Find("Special_MagicEgg");
            }
            return eggObj.GetComponentInChildren<SpriteResolver>(); }
        set { } 
    }

    void ChangeEggState()
    {
        if (Turn < 10)
        {
            Egg_State = EggState.Default_Level_1;
            EggSprite.SetCategoryAndLabel("Egg", "Level_1");
        }
        else if (Turn < 15 && Turn >= 10)
        {
            Egg_State = EggState.Default_Level_2;
            EggSprite.SetCategoryAndLabel("Egg", "Level_2");
        }
        else if(Turn < 20 && Turn >= 15)
        {
            Egg_State = EggState.Default_Level_3;
            EggSprite.SetCategoryAndLabel("Egg", "Level_3");
        }
        else if(Turn >= 20)
        {
            Egg_State = EggState.Default_Level_3;
            EggSprite.SetCategoryAndLabel("Egg", "Level_3");
            //여기서 상세조건에 따라 알 생긴거 변화하면 됨 위험도나 인기도, 혹은 이벤트에 따라
            // 아님 마나를 보유하고 있게할까? 근데 그러려면 힌트를 줘야함. 글고 정상적으로 클리어시에 마나를 얼마정도 모았는지에 대한 로그도 필요하고.
        }
    }



    #endregion
}
public class Save_DayResult
{
    public int Origin_Mana;
    public int Origin_Gold;
    public int Origin_Prisoner;

    public int Get_Mana;
    public int Get_Gold;
    public int Get_Prisoner;
    public int Get_Kill;

    public int Use_Mana;
    public int Use_Gold;
    public int Use_Prisoner;
    public int Use_Kill;

    public int GetPopularity;
    public int GetDanger;

    public int fame_perv;
    public int danger_perv;
    public int dungeonRank;


    public Save_DayResult()
    {

    }
    public Save_DayResult(Main.DayResult result)
    {
        Origin_Mana = result.Origin_Mana;
        Origin_Gold = result.Origin_Gold;
        Origin_Prisoner = result.Origin_Prisoner;

        Get_Mana = result.Get_Mana;
        Get_Gold = result.Get_Gold;
        Get_Prisoner = result.Get_Prisoner;
        Get_Kill = result.Get_Kill;

        Use_Mana = result.Use_Mana;
        Use_Gold = result.Use_Gold;
        Use_Prisoner = result.Use_Prisoner;
        Use_Kill = result.Use_Kill;

        GetPopularity = result.GetPopularity;
        GetDanger = result.GetDanger;

        fame_perv = result.fame_perv;
        danger_perv = result.danger_perv;
        dungeonRank = result.dungeonRank;
    }

}

