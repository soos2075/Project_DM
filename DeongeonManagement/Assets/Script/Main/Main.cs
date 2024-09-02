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
            _instance = FindAnyObjectByType<Main>();
            if (_instance == null)
            {
                Debug.Log("!!!!Main이 없음");

                var go = new GameObject(name: "@Main");
                _instance = go.AddComponent<Main>();
                //DontDestroyOnLoad(go);
            }
        }
    }
    #endregion



    #region Transform & GameObject

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

    private GameObject _player;
    public Transform Player
    {
        get
        {
            if (_player == null || _player.activeInHierarchy == false)
            {
                Init_Player();
            }
            return _player.transform;
        }
    }

    #endregion



    #region TextMesh
    public DamageNumber dm_large; // 1
    public DamageNumber dm_small; // 0

    public enum TextType
    {
        pop,
        danger,
        mana,
        gold,
        exp,
        hp,
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

            case TextType.hp:
                _msg += "hp";
                var dm6 = origin.Spawn(_pos.position, _msg);
                dm6.SetColor(Color.red);
                break;
        }
    }

    public void ShowDM_MSG(string _value, Vector3 _pos, Color _color, int _sizeOption = 0)
    {
        DamageNumber origin = _sizeOption == 0 ? dm_small : dm_large;

        var dm5 = origin.Spawn(_pos, _value);
        dm5.SetColor(_color);
        dm5.SetScale(0.7f);
        dm5.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder = 1000;
    }

    #endregion

    void Start()
    {
        //Debug.Log("디버그용 Start");
        //NewGame_Init();
        //Default_Init();
        //Test_Init();
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
            return _ui_main; 
        }
    }

    FloorInitializer _floorInitializer;
    FloorInitializer Floor_Initializer
    {
        get
        {
            if (_floorInitializer == null)
            {
                _floorInitializer = FindAnyObjectByType<FloorInitializer>();
            }
            return _floorInitializer;
        }
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

        UserData.Instance.SetData(PrefsKey.NewGameTimes, UserData.Instance.GetDataInt(PrefsKey.NewGameTimes) + 1);
        UserData.Instance.NewGameConfig();
        EventManager.Instance.NewGameReset();


        ActiveFloor_Basement = 4;
        ActiveFloor_Technical = 0;
        DungeonRank = 1;

        Player_Mana = 100;
        Player_Gold = 200;
        AP_MAX = 2;
        Player_AP = 0;


        Init_BasementFloor();
        Init_Animation();
        UI_Main.Start_Main();
        Init_DayResult();
        ExpansionConfirm();
        GameManager.Technical.Expantion_Technical();

        StartCoroutine(NewGameInitAndMessage());

        _DefaultSetting = true;

        UserData.Instance.isClear = false;

        NewGame_GetClearBonus();
    }

    void NewGame_GetClearBonus()
    {
        if (CollectionManager.Instance.RoundClearData != null)
        {
            if (CollectionManager.Instance.RoundClearData.dataApply)
            {
                Debug.Log("클리어 데이터 적용");
                GameManager.Monster.Load_MonsterData(CollectionManager.Instance.RoundClearData.MonsterList);
            }
        }
    }



    IEnumerator NewGameInitAndMessage()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        Instantiate_DayOne();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
        message.DelayTime = 2;
        message.Message = UserData.Instance.LocaleText("Message_First");

        //yield return new WaitUntil(() => Managers.UI._popupStack.Count == 0);
        //var tutorial = Managers.UI.ShowPopUp<UI_Tutorial>();

        yield return new WaitUntil(() => Managers.UI._popupStack.Count == 0);
        var tutorial = Managers.UI.ShowPopUpNonPush<UI_GuidanceArrow>();
    }
    void Instantiate_DayOne()
    {
        Floor_Initializer.NewGame_Init();

        Init_Player();

        Floor_Initializer.Init_FirstPlay_Bonus();

        Managers.Dialogue.ShowDialogueUI(DialogueName.Prologue, Player);
    }

    public void Init_Player()
    {
        BasementTile tile2 = null;
        Floor[0].TileMap.TryGetValue(new Vector2Int(3, 2), out tile2);
        PlacementInfo info2 = new PlacementInfo(Floor[0], tile2);


        if (GameManager.Placement.Find_Placement("Player") != null)
        {
            Debug.Log("플레이어 찾음(Placement)");
            var ppp = GameManager.Placement.Find_Placement("Player");
            if (GameManager.Placement.DisableCheck(ppp.GetComponent<Player>()) == false)
            {
                if (tile2.Original != null)
                {
                    GameManager.Placement.PlacementClear_Completely(tile2.Original);
                }
                GameManager.Placement.PlacementConfirm(ppp.GetComponent<Player>(), info2);
            }

            _player = ppp.gameObject;
            _player.GetComponent<Player>().HP = _player.GetComponent<Player>().HP_Max;
            return;
        }


        //? 처음생성(아예 Player가 없는경우 - 새게임 or 로드게임)
        var player = GameManager.Placement.CreatePlacementObject("Player", info2, PlacementType.Monster);
        var component = player as Player;
        component.MonsterInit();
        component.Level_Stat(DungeonRank);
        component.State = Monster.MonsterState.Placement;
        GameManager.Placement.PlacementConfirm(player, info2);

        _player = player.GetObject();
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
        //Final_Score = data.Final_Score;

        DungeonRank = data.DungeonLV;
        PopularityOfDungeon = data.FameOfDungeon;
        DangerOfDungeon = data.DangerOfDungeon;

        Player_Mana = data.Player_Mana;
        Player_Gold = data.Player_Gold;
        Player_AP = data.Player_AP;
        AP_MAX = data.AP_MAX;

        //Prisoner = data.Prisoner;

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
        Init_Player();

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

        StartCoroutine(Wait_FacilityLoad());
    }

    IEnumerator Wait_FacilityLoad()
    {
        yield return null;
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


    //public int Final_Score { get; private set; }

    //void AddScore(DayResult day)
    //{
    //    //? 점수시스템은 리뉴얼이 필요. 킬점수와 비슷하게 만족시켜서 돌아간 점수도 줘야함. 빈손으로 돌아가면 -는 아니고 0점이여도 만족은 점수를 높게

    //    int score = day.Get_Mana;
    //    score += day.Get_Gold;
    //    score += day.Get_Prisoner * 50;
    //    score += day.Get_Kill * 100;
    //    score += day.Get_Satisfaction * 120;

    //    Final_Score += score;
    //}

    int pop;
    public int PopularityOfDungeon { get { return pop; } private set { pop = Mathf.Clamp(value, 0, value); } }
    int danger;
    public int DangerOfDungeon { get { return danger; } private set { danger = Mathf.Clamp(value, 0, value); } }


    //private int _dungeonRank;
    public int DungeonRank { get; private set; } = 1;
    //public string DungeonRank_Alphabet { get { return ((Define.DungeonRank)_dungeonRank).ToString(); } }
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


    public List<DayResult> DayList { get; set; } = new List<DayResult>();

    public DayResult CurrentDay { get; set; }


    public class DayResult
    {
        //? 기존 상태 정보
        public int Origin_Mana;
        public int Origin_Gold;
        public int Origin_Pop;
        public int Origin_Danger;
        public int Origin_Rank;

        public void SetOrigin(int mana, int gold, int pop, int danger, int rank)
        {
            Origin_Mana = mana;
            Origin_Gold = gold;
            Origin_Pop = pop;
            Origin_Danger = danger;
            Origin_Rank = rank;
        }


        public enum EventType
        {
            Facility,
            Artifacts,
            //Entrance,
            Monster,
            //Battle,
            Etc,
            ResultBonus,
            Technical,
        }

        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ마나
        public int Mana_Get_Facility;
        public int Mana_Get_Artifacts;
        public int Mana_Get_Monster;
        public int Mana_Get_Etc;
        public int Mana_Get_Bonus;
        public void AddMana(int value, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Facility:
                    Mana_Get_Facility += value;
                    break;

                case EventType.Artifacts:
                    Mana_Get_Artifacts += value;
                    break;

                case EventType.Monster:
                    Mana_Get_Monster += value;
                    break;

                case EventType.Etc:
                    Mana_Get_Etc += value;
                    break;

                case EventType.ResultBonus:
                    Mana_Get_Bonus += value;
                    break;

                default:
                    Mana_Get_Etc += value;
                    break;
            }
            Instance.Player_Mana += value;
        }
        public int Mana_Use_Facility;
        public int Mana_Use_Monster;
        public int Mana_Use_Etc;
        public int Mana_Use_Technical;
        public void SubtractMana(int value, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Facility:
                    Mana_Use_Facility += value;
                    break;

                case EventType.Monster:
                    Mana_Use_Monster += value;
                    break;

                case EventType.Etc:
                    Mana_Use_Etc += value;
                    break;

                case EventType.Technical:
                    Mana_Use_Technical += value;
                    break;

                default:
                    Mana_Use_Etc += value;
                    break;
            }
            Instance.Player_Mana -= value;
        }



        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ골드
        public int Gold_Get_Facility;
        public int Gold_Get_Monster;
        public int Gold_Get_Etc;
        public int Gold_Get_Bonus;
        public void AddGold(int value, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Facility:
                    Gold_Get_Facility += value;
                    break;

                case EventType.Monster:
                    Gold_Get_Monster += value;
                    break;

                case EventType.Etc:
                    Gold_Get_Etc += value;
                    break;

                case EventType.ResultBonus:
                    Gold_Get_Bonus += value;
                    break;

                default:
                    Gold_Get_Etc += value;
                    break;
            }
            Instance.Player_Gold += value;
        }

        public int Gold_Use_Facility;
        public int Gold_Use_Monster;
        public int Gold_Use_Etc;
        public int Gold_Use_Technical;
        public void SubtractGold(int value, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Facility:
                    Gold_Use_Facility += value;
                    break;

                case EventType.Monster:
                    Gold_Use_Monster += value;
                    break;

                case EventType.Etc:
                    Gold_Use_Etc += value;
                    break;

                case EventType.Technical:
                    Gold_Use_Technical += value;
                    break;

                default:
                    Gold_Use_Etc += value;
                    break;
            }
            Instance.Player_Gold -= value;
        }

        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ모험가
        public int NPC_Visit;
        public int NPC_Prisoner;
        public int NPC_Kill;
        public int NPC_Satisfaction;
        public int NPC_NonSatisfaction;
        public int NPC_Empty;
        public int NPC_Runaway;


        public void AddVisit(int value) { NPC_Visit += value; }
        public void AddPrisoner(int value) { NPC_Prisoner += value; }
        public void AddKill(int value) { NPC_Kill += value; }
        public void AddSatisfaction(int value) { NPC_Satisfaction += value; }
        public void AddNonSatisfaction(int value) { NPC_NonSatisfaction += value; }
        public void AddEmpty(int value) { NPC_Empty += value; }
        public void AddRunaway(int value) { NPC_Runaway += value; }


        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ몬스터
        public int Monster_Battle;
        public int Monster_Victory;
        public int Monster_Defeat;
        public int Monster_LvUp;
        public int Monster_Trait;
        public int Monster_Evolution;

        public void AddBattle(int value) { Monster_Battle += value; }
        public void AddVictory(int value) { Monster_Victory += value; }
        public void AddDefeat(int value) { Monster_Defeat += value; }
        public void AddLvUp(int value) { Monster_LvUp += value; }
        public void AddTrait(int value) { Monster_Trait += value; }
        public void AddEvolution(int value) { Monster_Evolution += value; }



        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ던전
        public int GetPopularity;
        public int GetDanger;
        public void AddPop(int _value) { GetPopularity += _value; }
        public void AddDanger(int _value) { GetDanger += _value; }



        public DayResult()
        {

        }
        public DayResult(Save_DayResult result)
        {
            Origin_Mana = result.Origin_Mana;
            Origin_Gold = result.Origin_Gold;
            Origin_Pop = result.Origin_Pop;
            Origin_Danger = result.Origin_Danger;
            Origin_Rank = result.Origin_Rank;

            Mana_Get_Facility = result.Mana_Get_Facility;
            Mana_Get_Artifacts = result.Mana_Get_Artifacts;
            Mana_Get_Monster = result.Mana_Get_Monster;
            Mana_Get_Etc = result.Mana_Get_Etc;
            Mana_Get_Bonus = result.Mana_Get_Bonus;

            Mana_Use_Facility = result.Mana_Use_Facility;
            Mana_Use_Monster = result.Mana_Use_Monster;
            Mana_Use_Etc = result.Mana_Use_Etc;
            Mana_Use_Technical = result.Mana_Use_Technical;



            Gold_Get_Facility = result.Gold_Get_Facility;
            Gold_Get_Monster = result.Gold_Get_Monster;
            Gold_Get_Etc = result.Gold_Get_Etc;
            Gold_Get_Bonus = result.Gold_Get_Bonus;

            Gold_Use_Facility = result.Gold_Use_Facility;
            Gold_Use_Monster = result.Gold_Use_Monster;
            Gold_Use_Etc = result.Gold_Use_Etc;
            Gold_Use_Technical = result.Gold_Use_Technical;




            NPC_Visit = result.NPC_Visit;
            NPC_Prisoner = result.NPC_Prisoner;
            NPC_Kill = result.NPC_Kill;
            NPC_Satisfaction = result.NPC_Satisfaction;
            NPC_NonSatisfaction = result.NPC_NonSatisfaction;
            NPC_Empty = result.NPC_Empty;
            NPC_Runaway = result.NPC_Runaway;

            Monster_Battle = result.Monster_Battle;
            Monster_Victory = result.Monster_Victory;
            Monster_Defeat = result.Monster_Defeat;
            Monster_LvUp = result.Monster_LvUp;
            Monster_Trait = result.Monster_Trait;
            Monster_Evolution = result.Monster_Evolution;

            GetPopularity = result.GetPopularity;
            GetDanger = result.GetDanger;
        }
    }


    void Init_DayResult()
    {
        CurrentDay = new DayResult();
        CurrentDay.SetOrigin(Player_Mana, Player_Gold, PopularityOfDungeon, DangerOfDungeon, DungeonRank);
    }




    void DayOver_Dayresult()
    {
        DayList.Add(CurrentDay);
        //AddScore(CurrentDay);

        PopularityOfDungeon += CurrentDay.GetPopularity;
        DangerOfDungeon += CurrentDay.GetDanger;

        if (EventManager.Instance.TryRankUp(PopularityOfDungeon, DangerOfDungeon))
        {
            Dungeon_RankUP();
        }

        Player_AP = AP_MAX;

        //? 위가 적용 아래가 새로교체
        Init_DayResult();

        StartCoroutine(Show_DayResult());
    }

    IEnumerator Show_DayResult()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        Managers.UI.Popup_Reservation(() =>
        {
            var ui = Managers.UI.ShowPopUp<UI_DayResult>();
            ui.TextContents(DayList[Turn - 1], CurrentDay);
        });


        StartCoroutine(AutoSave());
    }




    public int GetTotalMana()
    {
        int mana = Player_Mana;

        foreach (var item in DayList)
        {
            mana += item.Mana_Use_Etc;
            mana += item.Mana_Use_Facility;
            mana += item.Mana_Use_Monster;
        }
        if (CurrentDay != null)
        {
            mana += CurrentDay.Mana_Use_Etc;
            mana += CurrentDay.Mana_Use_Facility;
            mana += CurrentDay.Mana_Use_Monster;
        }

        return mana;
    }

    public int GetTotalGold()
    {
        int gold = Player_Gold;

        foreach (var item in DayList)
        {
            gold += item.Gold_Use_Etc;
            gold += item.Gold_Use_Facility;
            gold += item.Gold_Use_Monster;
        }
        if (CurrentDay != null)
        {
            gold += CurrentDay.Gold_Use_Etc;
            gold += CurrentDay.Gold_Use_Facility;
            gold += CurrentDay.Gold_Use_Monster;
        }

        return gold;
    }

    public int GetTotalVisit()
    {
        int visit = 0;
        foreach (var item in DayList)
        {
            visit += item.NPC_Visit;
        }
        if (CurrentDay != null)
        {
            visit += CurrentDay.NPC_Visit;
        }
        return visit;
    }

    public int GetTotalKill()
    {
        int kill = 0;
        foreach (var item in DayList)
        {
            kill += item.NPC_Kill;
        }
        if (CurrentDay != null)
        {
            kill += CurrentDay.NPC_Kill;
        }
        return kill;
    }

    public int GetTotalSatisfaction()
    {
        int npc = 0;
        foreach (var item in DayList)
        {
            npc += item.NPC_Satisfaction;
        }
        if (CurrentDay != null)
        {
            npc += CurrentDay.NPC_Satisfaction;
        }
        return npc;
    }

    public int GetTotalReturn()
    {
        int npc = 0;
        foreach (var item in DayList)
        {
            npc += item.NPC_Empty;
        }
        if (CurrentDay != null)
        {
            npc += CurrentDay.NPC_Empty;
        }
        return npc;
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
                DayEvent();

                //? 대사 이벤트 등 턴 이벤트
                Main_TurnStartEvent();

                BattleManager.Instance.TurnStart();
                EventManager.Instance.TurnStart();
                GameManager.NPC.TurnStart();
                GameManager.Monster.MonsterTurnStartEvent();
                GameManager.Facility.TurnStartEvent();
            }
            else
            {
                Init_Player(); //? 플레이어 없으면 재소환
                NightEvent();
                DayMonsterEvent();
                GameManager.Monster.MonsterTurnOverEvent();
                GameManager.Facility.TurnOverEvent();
                EventManager.Instance.TurnOver();
                UI_Main.TurnOverEvent();


                //? 대사 이벤트 등 턴 이벤트
                Main_TurnOverEvent();
                //? 결과창과 저장
                DayOver_Dayresult(); 
            }
        }
    }

    //public void DayChange()
    //{
    //    Managers.UI.CloseAll();

    //    Management = !Management;

    //    DayChangeAnimation();
    //}


    public void DayChange_Start()
    {
        Managers.UI.CloseAll();
        Management = false;
        DayChangeAnimation();
    }

    public void DayChange_Over()
    {
        Managers.UI.CloseAll();
        Management = true;
        DayChangeAnimation();
    }




    void Main_TurnStartEvent()
    {
        UI_EventBox.AddEventText($"※{Turn}{UserData.Instance.LocaleText("Event_DayStart")}※");


        switch (Turn)
        {

            case 1:
                //? 원래 1일차 종료부터 가능했던 정보확인을 1일차 시작때부터 할 수 있도록 변경
                UI_Main.Active_Floor();
                break;

            case 3:
                Debug.Log("3일차 시작 이벤트 - 모험가 한명 무조건 소환");
                GameManager.NPC.AddEventNPC(EventNPCType.Event_Day3, 7);
                break;

            case 7: //? 모험가 이벤트
                GameManager.NPC.AddEventNPC(EventNPCType.Event_Day8, 10);
                break;


            case 9: //? 고블린 임시 이벤트
                //GameManager.NPC.AddEventNPC(EventNPCType.Event_Goblin_Leader, 10);
                break;

            case 15:
                //Debug.Log("15일차 시작 이벤트 - 패배 트리거 이벤트 모험가 소환");
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Event_Day15, 9);
                //GameManager.NPC.AddEventNPC(NPCManager.NPCType.Adventurer_0, 9);
                break;

            case 20:
                Debug.Log("20일차 시작 이벤트 - 붉은 모험단 소환");
                GameManager.NPC.AddEventNPC(EventNPCType.A_Tanker, 10f);
                GameManager.NPC.AddEventNPC(EventNPCType.A_Warrior, 10.5f);
                GameManager.NPC.AddEventNPC(EventNPCType.A_Wizard, 11f);
                GameManager.NPC.AddEventNPC(EventNPCType.A_Elf, 11.5f);
                break;

            case 25:
                Debug.Log("25일차 시작 이벤트 - 길드 토벌단 1");
                Day25Event_Direction();
                break;

            case 30:
                Debug.Log("30일차 시작 이벤트 - 길드 토벌단2 + 붉은 모험단");
                Day30Event_Direction();
                break;
        }
    }


    void Day25Event_Direction()
    {
        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);


        List<NPC> sol1List = new List<NPC>();
        List<NPC> sol2List = new List<NPC>();

        var cap_A = GameManager.NPC.InstantiateNPC_Event(EventNPCType.Captine_A);
        cap_A.transform.position = Dungeon.transform.position + (Vector3.right * 1.5f);
        GameManager.Placement.Visible(cap_A);

        for (int i = 0; i < 7; i++)
        {
            var sol_1 = GameManager.NPC.InstantiateNPC_Event(EventNPCType.Event_Soldier1);
            sol_1.transform.position = Dungeon.transform.position + (Vector3.right * 0.5f * i) + Vector3.right * 2.5f;
            sol_1.Anim_State = NPC.animState.left;
            sol_1.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol_1);
            sol1List.Add(sol_1);
        }

        var cap_B = GameManager.NPC.InstantiateNPC_Event(EventNPCType.Captine_B);
        cap_B.transform.position = Dungeon.transform.position + (Vector3.right * -1.5f);
        cap_B.Anim_State = NPC.animState.left;
        cap_B.Anim_State = NPC.animState.Idle;
        GameManager.Placement.Visible(cap_B);

        for (int i = 0; i < 7; i++)
        {
            var sol_1 = GameManager.NPC.InstantiateNPC_Event(EventNPCType.Event_Soldier2);
            sol_1.transform.position = Dungeon.transform.position + (Vector3.right * -0.5f * i) + Vector3.right * -2.5f;
            sol_1.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol_1);
            sol2List.Add(sol_1);
        }

        Managers.Dialogue.ShowDialogueUI($"Day25_Event", cap_A.transform);
        StartCoroutine(Wait_Day25_Dialogue(cap_A, cap_B, sol1List, sol2List));
    }


    IEnumerator Wait_Day25_Dialogue(NPC cap_A, NPC cap_B, List<NPC> sol1, List<NPC> sol2)
    {
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        cap_A.Departure(cap_A.transform.position, Dungeon.position);
        foreach (var item in sol1)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

        yield return new WaitForSeconds(6);

        cap_B.Departure(cap_B.transform.position, Dungeon.position);
        foreach (var item in sol2)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }
    }



    void Day30Event_Direction()
    {
        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);


        List<NPC> bloodSong = new List<NPC>();
        //? 피의노래 파티원 생성
        {
            var party = GameManager.NPC.InstantiateNPC_Event(EventNPCType.B_Tanker);
            party.transform.position = Dungeon.transform.position + (Vector3.left * 6.5f);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }
        {
            var party = GameManager.NPC.InstantiateNPC_Event(EventNPCType.B_Warrior);
            party.transform.position = Dungeon.transform.position + (Vector3.left * 7);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }
        {
            var party = GameManager.NPC.InstantiateNPC_Event(EventNPCType.B_Wizard);
            party.transform.position = Dungeon.transform.position + (Vector3.left * 7.5f);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }
        {
            var party = GameManager.NPC.InstantiateNPC_Event(EventNPCType.B_Elf);
            party.transform.position = Dungeon.transform.position + (Vector3.left * 8);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }

        //? 대장급 생성
        var Cap_A = GameManager.NPC.InstantiateNPC_Event(EventNPCType.Captine_A);
        Cap_A.transform.position = Dungeon.transform.position + (Vector3.right * 1);
        Cap_A.Anim_State = NPC.animState.right;
        Cap_A.Anim_State = NPC.animState.Ready;
        GameManager.Placement.Visible(Cap_A);

        var Cap_B = GameManager.NPC.InstantiateNPC_Event(EventNPCType.Captine_B);
        Cap_B.transform.position = Dungeon.transform.position + (Vector3.right * 5);
        Cap_B.Anim_State = NPC.animState.right;
        Cap_B.Anim_State = NPC.animState.Ready;
        GameManager.Placement.Visible(Cap_B);

        var Captine_C = GameManager.NPC.InstantiateNPC_Event(EventNPCType.Captine_C);
        Captine_C.transform.position = Dungeon.transform.position + (Vector3.left * 1.5f);
        GameManager.Placement.Visible(Captine_C);


        List<NPC> sol1List = new List<NPC>();
        List<NPC> sol2List = new List<NPC>();
        List<NPC> sol3List = new List<NPC>();

        for (int i = 0; i < 5; i++)
        {
            var sol = GameManager.NPC.InstantiateNPC_Event(EventNPCType.Event_Soldier1);
            sol.transform.position = Dungeon.transform.position + (Vector3.right * 0.5f * i) + Vector3.right * 2.0f;
            sol.Anim_State = NPC.animState.left;
            sol.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol);
            sol1List.Add(sol);
        }
        for (int i = 0; i < 5; i++)
        {
            var sol = GameManager.NPC.InstantiateNPC_Event(EventNPCType.Event_Soldier2);
            sol.transform.position = Dungeon.transform.position + (Vector3.right * 0.5f * i) + Vector3.right * 6.0f;
            sol.Anim_State = NPC.animState.left;
            sol.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol);
            sol2List.Add(sol);
        }
        for (int i = 0; i < 5; i++)
        {
            var sol = GameManager.NPC.InstantiateNPC_Event(EventNPCType.Event_Soldier3);
            sol.transform.position = Dungeon.transform.position + (Vector3.left * 0.5f * i) + Vector3.left * 2.5f;
            sol.Anim_State = NPC.animState.right;
            sol.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol);
            sol3List.Add(sol);
        }



        Managers.Dialogue.ShowDialogueUI($"Day30_Event", Captine_C.transform);
        StartCoroutine(Wait_Day30_Dialogue(Cap_A, Cap_B, Captine_C, sol1List, sol2List, sol3List, bloodSong));
    }

    IEnumerator Wait_Day30_Dialogue(NPC cap_A, NPC cap_B, NPC cap_C, List<NPC> sol1, List<NPC> sol2, List<NPC> sol3, List<NPC> bloodSong)
    {
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        cap_A.Departure(cap_A.transform.position, Dungeon.position);
        foreach (var item in sol1)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

        yield return new WaitForSeconds(6);

        cap_B.Departure(cap_B.transform.position, Dungeon.position);
        foreach (var item in sol2)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

        yield return new WaitForSeconds(10);

        cap_C.Departure(cap_C.transform.position, Dungeon.position);
        foreach (var item in sol3)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

        yield return new WaitForSeconds(8);

        foreach (var item in bloodSong)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

    }



    [Obsolete]
    void TestEnding()
    {
        Debug.Log("엔딩 테스트");

        //? 이건 30일 됐을 때 뜨는거
        Managers.Dialogue.ShowDialogueUI(DialogueName.Day30_Over, Player);

        //? 이건 바로 엔딩씬 스킵하고 회차설정으로 넘어가는 부분
        //var ending = Managers.UI.ShowPopUp<UI_Ending>();
    }

    void Main_TurnOverEvent()
    {
        UI_EventBox.AddEventText($"※{Turn}{UserData.Instance.LocaleText("Event_DayOver")}※");
        ChangeEggState();

        switch (Turn)
        {
            case 1:
                Debug.Log("1일차 종료 이벤트 - 시설배치");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Facility, Player);
                UI_Main.Active_Button(UI_Management.ButtonEvent._1_Facility);
                UI_Main.SetNotice(UI_Management.OverlayImages.OverlayImage_Facility, true);
                //UI_Main.Active_Floor();

                StartCoroutine(Wait_AP_Tutorial());
                break;

            case 2:
                Debug.Log("2일차 종료 이벤트 - 몬스터");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Monster, Player);
                UI_Main.Active_Button(UI_Management.ButtonEvent._2_Summon);
                UI_Main.Active_Button(UI_Management.ButtonEvent._3_Management);
                UI_Main.SetNotice(UI_Management.OverlayImages.OverlayImage_Monster, true);
                UI_Main.SetNotice(UI_Management.OverlayImages.OverlayImage_Summon, true);
                break;

            case 3:
                Debug.Log("3일차 종료 이벤트 - 테크니컬");
                Technical_Expansion();
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Technical, Player);
                break;

            case 4:
                Debug.Log("4일차 종료 이벤트 - 비밀방");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Egg, Player);
                break;

            case 5:
                Debug.Log("5일차 종료 이벤트 - 길드");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Guild, Player);
                UI_Main.Active_Button(UI_Management.ButtonEvent._4_Guild);
                break;

            case 6:
                Debug.Log("6일차 종료 이벤트 - 수정");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Orb, Player);
                break;


            case 15: //? 데모 종료구간
                //DEMO_15DAY();
                //return;
                break;

            case 20:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Day20_Over, Player);
                break;

            case 30:
#if DEMO_BUILD
                Debug.Log("데모클리어");
                var clear = new CollectionManager.ClearDataLog();
                clear.mana = GetTotalMana();
                clear.gold = GetTotalGold();

                clear.visit = GetTotalVisit();
                clear.kill = GetTotalKill();
                clear.satisfaction = GetTotalSatisfaction();
                clear.return_Empty = GetTotalReturn();

                clear.pop = PopularityOfDungeon;
                clear.danger = DangerOfDungeon;
                clear.rank = DungeonRank;
                UserData.Instance.FileConfig.PlayTimeApply();
                clear.clearTime = UserData.Instance.FileConfig.PlayTimes;
                clear.monsterCount = GameManager.Monster.GetCurrentMonster();
                int highestLv = 0;
                string highestMonster = "";
                foreach (var mon in GameManager.Monster.Monsters)
                {
                    if (mon != null && mon.LV > highestLv)
                    {
                        highestMonster = mon.Name;
                        highestLv = mon.LV;
                    }
                }
                clear.highestMonster = highestMonster;
                clear.highestMonsterLv = highestLv;

                DemoManager.Instance.DemoClearData(clear);
                AutoSave_Instant();
#endif

                Managers.Dialogue.ShowDialogueUI(DialogueName.Day30_Over, Player);
                break;

            default:
                break;
        }
    }





    IEnumerator AutoSave()
    {
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => Time.timeScale > 0 && Managers.UI._reservationQueue.Count == 0 && Managers.UI._popupStack.Count == 0);
        Debug.Log($"자동저장 : {Turn}일차");
        Managers.Data.SaveAndAddFile("AutoSave", 0);
    }

    void AutoSave_Instant()
    {
        Managers.Data.SaveAndAddFile("AutoSave", 0);
    }



    IEnumerator Wait_AP_Tutorial()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Managers.UI._popupStack.Count == 0);

        var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
        message.DelayTime = 2;
        message.Message = UserData.Instance.LocaleText("Message_Tutorial_AP");
    }



    [Obsolete]
    void DEMO_15DAY()
    {
        Managers.Resource.Instantiate($"UI/_UI_BIC_DEMO_CLEAR");
    }






    void DayMonsterEvent()
    {
        StartCoroutine(WaitForResultUI());
    }

    IEnumerator WaitForResultUI()
    {
        CurrentDay.Monster_LvUp = GameManager.Monster.LevelUpList.Count;

        yield return new WaitForEndOfFrame();

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


    public Coroutine QuickPlacement { get; set; }


    public BasementFloor[] Floor { get; set; }

    public BasementFloor CurrentFloor { get; set; }
    public BasementTile CurrentTile { get; set; }
    public Action CurrentAction { get; set; }

    //public Action PurchaseAction { get; set; }

    public bool isContinueOption { get; set; }
    public PurchaseInfo CurrentPurchase { get; set; }
    public class PurchaseInfo
    {
        public int mana;
        public int gold;
        public int ap;
        public int rank;

        public bool isContinuous;

        public PurchaseInfo(int _mana, int _gold, int _ap, bool _isContinue,  int _rank = 0)
        {
            mana = _mana;
            gold = _gold;
            ap = _ap;
            rank = _rank;
            isContinuous = _isContinue;
            Main.Instance.isContinueOption = _isContinue;
        }


        public bool PurchaseCheck()
        {
            if (Main.Instance.Player_Mana < mana)
            {
                //var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
                //msg.Message = UserData.Instance.LocaleText("Message_No_Mana");
                return false;
            }
            if (Main.Instance.Player_Gold < gold)
            {
                //var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
                //msg.Message = UserData.Instance.LocaleText("Message_No_Gold");
                return false;
            }
            if (Main.Instance.Player_AP < ap)
            {
                //var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
                //msg.Message = UserData.Instance.LocaleText("Message_No_AP");
                return false;
            }
            if (Main.Instance.DungeonRank < rank)
            {
                //var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
                //msg.Message = UserData.Instance.LocaleText("Message_No_Rank");
                return false;
            }

            //Main.Instance.CurrentDay.SubtractMana(mana, DayResult.EventType.Facility);
            //Main.Instance.CurrentDay.SubtractGold(gold, DayResult.EventType.Facility);
            //Main.Instance.Player_AP -= ap;
            return true;
        }

        public void PurchaseConfirm()
        {
            Main.Instance.CurrentDay.SubtractMana(mana, DayResult.EventType.Facility);
            Main.Instance.CurrentDay.SubtractGold(gold, DayResult.EventType.Facility);
            Main.Instance.Player_AP -= ap;
        }
    }



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


            if (i == 0)
            {
                Floor[i].LabelName = $"{UserData.Instance.LocaleText("숨겨진곳")}";
            }
            else
            {
                Floor[i].LabelName = $"{UserData.Instance.LocaleText("지하")} {i} {UserData.Instance.LocaleText("층")}";
            }
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
            Floor[0].Hidden = true;
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
            ui.transform.position = Floor[ActiveFloor_Basement].transform.position + new Vector3(0, 5, 0);

            ui.GetComponent<UI_Expansion_Floor>().SetContents(ActiveFloor_Basement, 200, 200, 2);
        }
    }



    public void ResetCurrentAction()
    {
        Main.Instance.CurrentBoundary = null;
        Main.Instance.CurrentAction = null;
        Main.Instance.CurrentTile = null;
        //Main.Instance.PurchaseAction = null;
        Main.Instance.CurrentPurchase = null;
        Managers.UI.ClosePopupPick(FindAnyObjectByType<UI_DungeonPlacement>());
        Managers.UI.PauseOpen();
        Time.timeScale = 0;

        FindAnyObjectByType<UI_Management>().Show_MainCanvas();
    }


    #endregion





    #region Ending

    public Endings CurrentEndingState { get; set; }

    GameObject _egg;
    public GameObject EggObj { 
        get
        {
            if (_egg == null)
            {
                //eggObj = GameObject.Find("Special_MagicEgg");
                _egg = GameManager.Placement.Find_Placement("Special_MagicEgg").gameObject;
            }
            return _egg;
        }
        set 
        {
            _egg = value;
        } 
    }

void ChangeEggState()
    {
        Debug.Log($"{Turn}일차 종료\nTotal Mana : {GetTotalMana()}\nTotal Gold : {GetTotalGold()}");

        if (Turn < 5)
        {
            CurrentEndingState = Endings.Dog;
            EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Lv1"));
        }
        else if (Turn < 10 && Turn >= 5)
        {
            CurrentEndingState = Endings.Dog;
            EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Lv2"));
        }
        else if(Turn < 15 && Turn >= 10)
        {
            CurrentEndingState = Endings.Dog;
            EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Lv3"));
        }
        else if(Turn >= 15)
        {
            SelectEnding();
        }

        UserData.Instance.EndingState = CurrentEndingState;
    }

    // 각 조건을 독립되게 할지, 아님 state하나로만 할지는 고민중. 독립되게 한다면 여러 조건을 달성했을 때, 선택지를 줄 수 있음.
    // 아니면 조건에 선행 엔딩을 보게 만들면 또 억제가 되기도 하고.. 뭐 암튼 데모는 dog엔딩으로 픽스하자.
    void SelectEnding()
    {
        if (DangerOfDungeon > PopularityOfDungeon)
        {
            CurrentEndingState = Endings.Dragon;
            EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Dragon"));
        }
        else
        {
            CurrentEndingState = Endings.Dog;
            EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Dog"));
        }


#if DEMO_BUILD
        // 데모버전이면 무조건 Dog엔딩
        return;
#endif

        //if (DangerOfDungeon > 500)
        //{
        //    CurrentEndingState = Endings.Dragon;
        //    EggSprite.SetCategoryAndLabel("Egg", "Dragon");
        //    eggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Dragon"));
        //}

        //if (GetTotalMana() >= 10000)
        //{
        //    CurrentEndingState = Endings.Slime;
        //    EggSprite.SetCategoryAndLabel("Egg", "Slime");
        //    eggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Slime"));
        //}
    }



#endregion
}
public class Save_DayResult
{
    public int Origin_Mana;
    public int Origin_Gold;
    public int Origin_Pop;
    public int Origin_Danger;
    public int Origin_Rank;

    public int Mana_Get_Facility;
    public int Mana_Get_Artifacts;
    public int Mana_Get_Monster;
    public int Mana_Get_Etc;
    public int Mana_Get_Bonus;

    public int Mana_Use_Facility;
    public int Mana_Use_Monster;
    public int Mana_Use_Etc;
    public int Mana_Use_Technical;


    public int Gold_Get_Facility;
    public int Gold_Get_Monster;
    public int Gold_Get_Etc;
    public int Gold_Get_Bonus;

    public int Gold_Use_Facility;
    public int Gold_Use_Monster;
    public int Gold_Use_Etc;
    public int Gold_Use_Technical;


    public int NPC_Visit;
    public int NPC_Prisoner;
    public int NPC_Kill;
    public int NPC_Satisfaction;
    public int NPC_NonSatisfaction;
    public int NPC_Empty;
    public int NPC_Runaway;

    public int Monster_Battle;
    public int Monster_Victory;
    public int Monster_Defeat;
    public int Monster_LvUp;
    public int Monster_Trait;
    public int Monster_Evolution;

    public int GetPopularity;
    public int GetDanger;


    public Save_DayResult()
    {

    }
    public Save_DayResult(Main.DayResult result)
    {
        Origin_Mana = result.Origin_Mana;
        Origin_Gold = result.Origin_Gold;
        Origin_Pop = result.Origin_Pop;
        Origin_Danger = result.Origin_Danger;
        Origin_Rank = result.Origin_Rank;

        Mana_Get_Facility = result.Mana_Get_Facility;
        Mana_Get_Artifacts = result.Mana_Get_Artifacts;
        Mana_Get_Monster = result.Mana_Get_Monster;
        Mana_Get_Etc = result.Mana_Get_Etc;
        Mana_Get_Bonus = result.Mana_Get_Bonus;

        Mana_Use_Facility = result.Mana_Use_Facility;
        Mana_Use_Monster = result.Mana_Use_Monster;
        Mana_Use_Etc = result.Mana_Use_Monster;
        Mana_Use_Technical = result.Mana_Use_Technical;


        Gold_Get_Facility = result.Gold_Get_Facility;
        Gold_Get_Monster = result.Gold_Get_Monster;
        Gold_Get_Etc = result.Gold_Get_Etc;
        Gold_Get_Bonus = result.Gold_Get_Bonus;

        Gold_Use_Facility = result.Gold_Use_Facility;
        Gold_Use_Monster = result.Gold_Use_Monster;
        Gold_Use_Etc = result.Gold_Use_Etc;
        Gold_Use_Technical = result.Gold_Use_Technical;


        NPC_Visit = result.NPC_Visit;
        NPC_Prisoner = result.NPC_Prisoner;
        NPC_Kill = result.NPC_Kill;
        NPC_Satisfaction = result.NPC_Satisfaction;
        NPC_NonSatisfaction = result.NPC_NonSatisfaction;
        NPC_Empty = result.NPC_Empty;
        NPC_Runaway = result.NPC_Runaway;

        Monster_Battle = result.Monster_Battle;
        Monster_Victory = result.Monster_Victory;
        Monster_Defeat = result.Monster_Defeat;
        Monster_LvUp = result.Monster_LvUp;
        Monster_Trait = result.Monster_Trait;
        Monster_Evolution = result.Monster_Evolution;

        GetPopularity = result.GetPopularity;
        GetDanger = result.GetDanger;
    }

}

