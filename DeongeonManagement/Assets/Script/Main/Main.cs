using DamageNumbersPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
    public DamageNumber dm_large; // 2
    public DamageNumber dm_middle; // 1
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
        DamageNumber origin = dm_small;
        if (_sizeOption == 1) origin = dm_middle;
        if (_sizeOption == 2) origin = dm_large;

        SoundManager.Instance.PlaySound($"SFX/Add_{_textType.ToString()}");

        Vector3 offset = _pos.position + new Vector3(0, 0.5f, 0);

        string _msg = _value > 0 ? $"+{_value} " : $"{_value} ";
        switch (_textType)
        {
            case TextType.pop:
                _msg += "pop";
                var dm = origin.Spawn(offset, _msg);
                dm.SetColor(Color.green);
                break;

            case TextType.danger:
                _msg += "danger";
                var dm2 = origin.Spawn(offset, _msg);
                dm2.SetColor(new Color32(255, 60, 60, 255));
                break;

            case TextType.mana:
                _msg += "mana";
                var dm3 = origin.Spawn(offset, _msg);
                dm3.SetColor(new Color32(50, 208, 255, 255));
                break;

            case TextType.gold:
                _msg += "gold";
                var dm4 = origin.Spawn(offset, _msg);
                dm4.SetColor(Color.yellow);
                break;

            case TextType.exp:
                _msg += "exp";
                var dm5 = origin.Spawn(offset, _msg);
                dm5.SetColor(Color.white);
                break;

            case TextType.hp:
                _msg += "hp";
                var dm6 = origin.Spawn(offset, _msg);
                dm6.SetColor(Color.red);
                break;
        }
    }

    public void ShowDM_MSG(string _value, Vector3 _pos, Color _color, int _sizeOption = 0)
    {
        DamageNumber origin = _sizeOption == 0 ? dm_small : dm_large;

        var dm5 = origin.Spawn(_pos, _value);
        dm5.SetColor(_color);
        dm5.SetScale(1.4f);
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
        Init_DefaultSetting();
        UI_Main.Start_Main();

        _DefaultSetting = true;
    }

    public void NewGame_Init()
    {
        if (_DefaultSetting)
        {
            return;
        }

        Init_Statistics();

        //UserData.Instance.SetData(PrefsKey.NewGameTimes, UserData.Instance.GetDataInt(PrefsKey.NewGameTimes) + 1);
        //UserData.Instance.NewGameConfig();
        //EventManager.Instance.NewGameReset();


        ActiveFloor_Basement = 4;
        ActiveFloor_Technical = 0;
        DungeonRank = 1;

        Player_Mana = 100;
        Player_Gold = 200;
        AP_MAX = 2;
        Player_AP = 0;

        UserData.Instance.isClear = false;
        NewGamePlus();

        Init_BasementFloor();
        Init_DefaultSetting();
        UI_Main.Start_Main();
        Init_DayResult();
        ExpansionConfirm(false);
        GameManager.Technical.Expantion_Technical();

        if (UserData.Instance.FileConfig.PlayRounds > 1)
        {
            StartCoroutine(NewGameInit_SecondTime());
            Technical_Expansion(1);

            //? 이벤트 시드
            if (UserData.Instance.FileConfig.GameMode == Define.ModeSelect.Endless)
            {
                RandomEventManager.Instance.Init_RE_Seed_Endless();
            }
            else
            {
                RandomEventManager.Instance.Init_RE_Seed(UserData.Instance.FileConfig.Difficulty);
            }
        }
        else
        {
            StartCoroutine(NewGameInit_FirstTime());
        }



        _DefaultSetting = true;
    }

    void NewGamePlus()
    {
        if (UserData.Instance.FileConfig.Buff_ApBonusOne)
        {
            AP_Bonus++;
            //AP_MAX++;
        }
        if (UserData.Instance.FileConfig.Buff_ApBonusTwo)
        {
            AP_Bonus++;
            //AP_MAX++;
        }

        if (UserData.Instance.FileConfig.Buff_PopBonus)
        {
            PopularityOfDungeon += 100;
        }
        if (UserData.Instance.FileConfig.Buff_DangerBonus)
        {
            DangerOfDungeon += 100;
        }

        if (UserData.Instance.FileConfig.Buff_ManaBonus)
        {
            Player_Mana += 500;
        }
        if (UserData.Instance.FileConfig.Buff_GoldBonus)
        {
            Player_Gold += 500;
        }
        if (UserData.Instance.FileConfig.Buff_ManaBonus1000)
        {
            Player_Mana += 1000;
        }
        if (UserData.Instance.FileConfig.Buff_GoldBonus1000)
        {
            Player_Gold += 1000;
        }

        if (UserData.Instance.FileConfig.Buff_Starting_4F)
        {
            ActiveFloor_Basement = 5;
            ActiveFloor_Technical = 2;
        }


        StartCoroutine(Wait_NewGamePlus());
    }

    IEnumerator Wait_NewGamePlus()
    {
        yield return null;


        //? 칭호
        GameManager.Title.Active_Title(TitleGroup.NoviceDungeon);


        //? 유닛
        if (UserData.Instance.FileConfig.Unit_BloodySlime)
        {
            GameManager.Monster.CreateMonster("BloodySlime", true);
        }
        if (UserData.Instance.FileConfig.Unit_FlameGolem)
        {
            GameManager.Monster.CreateMonster("FlameGolem", true);
        }
        if (UserData.Instance.FileConfig.Unit_HellHound)
        {
            GameManager.Monster.CreateMonster("HellHound", true);
        }
        if (UserData.Instance.FileConfig.Unit_Pixie)
        {
            GameManager.Monster.CreateMonster("Pixie", true);
        }
        if (UserData.Instance.FileConfig.Unit_Salinu)
        {
            GameManager.Monster.CreateMonster("Salinu", true);
        }
        if (UserData.Instance.FileConfig.Unit_Griffin)
        {
            GameManager.Monster.CreateMonster("Griffin", true);
        }
        if (UserData.Instance.FileConfig.Unit_Lilith)
        {
            GameManager.Monster.CreateMonster("Lilith", true);
        }

        if (UserData.Instance.FileConfig.Unit_Rena)
        {
            GameManager.Monster.CreateMonster("Rena");
        }

        if (UserData.Instance.FileConfig.Unit_Ravi)
        {
            GameManager.Monster.CreateMonster("Ravi");
        }
        if (UserData.Instance.FileConfig.Unit_Lievil)
        {
            GameManager.Monster.CreateMonster("Lievil");
        }
        if (UserData.Instance.FileConfig.Unit_Rideer)
        {
            GameManager.Monster.CreateMonster("Rideer");
        }


        //? 아티팩트
        GameManager.Artifact.AddArtifact(ArtifactLabel.DungeonMaster_Temp);

        if (UserData.Instance.FileConfig.Arti_Hero)
        {
            GameManager.Artifact.AddArtifact(ArtifactLabel.ProofOfHero);
        }
        if (UserData.Instance.FileConfig.Arti_Decay)
        {
            GameManager.Artifact.AddArtifact(ArtifactLabel.TouchOfDecay);
        }
        if (UserData.Instance.FileConfig.Arti_Pop)
        {
            GameManager.Artifact.AddArtifact(ArtifactLabel.OrbOfPopularity);
        }
        if (UserData.Instance.FileConfig.Arti_Danger)
        {
            GameManager.Artifact.AddArtifact(ArtifactLabel.OrbOfDanger);
        }
        if (UserData.Instance.FileConfig.Arti_DownDanger)
        {
            GameManager.Artifact.AddArtifact(ArtifactLabel.MarbleOfReassurance);
        }
        if (UserData.Instance.FileConfig.Arti_DownPop)
        {
            GameManager.Artifact.AddArtifact(ArtifactLabel.MarbleOfOblivion);
        }

        if (UserData.Instance.FileConfig.Arti_Lv_1)
        {
            GameManager.Artifact.AddArtifact(ArtifactLabel.LvBook_1);
        }
        if (UserData.Instance.FileConfig.Arti_Lv_2)
        {
            GameManager.Artifact.AddArtifact(ArtifactLabel.LvBook_2);
        }
        if (UserData.Instance.FileConfig.Arti_Lv_3)
        {
            GameManager.Artifact.AddArtifact(ArtifactLabel.LvBook_3);
        }
    }





    IEnumerator NewGameInit_FirstTime()
    {
        yield return StartCoroutine(Instantiate_DayOne());
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

    IEnumerator NewGameInit_SecondTime()
    {
        yield return StartCoroutine(Instantiate_DayOne());
        EventManager.Instance.FirstPortalAppearSkip();
        if (UserData.Instance.FileConfig.Buff_Starting_4F)
        {
            EventManager.Instance.EntranceMove_3to4_Skip();
        }
    }

    IEnumerator Instantiate_DayOne()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        Floor_Initializer.NewGame_Init();
        Start_Entrance();
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
            //Debug.Log("플레이어 찾음(Placement)");
            var ppp = GameManager.Placement.Find_Placement("Player");
            var unitPlayer = ppp.GetComponent<Player>();
            if (unitPlayer.State != Monster.MonsterState.Placement) //? Disable Check에서 상태로 교체, 이젠 패배하면 standBy가 됨
            {
                if (tile2.Original != null)
                {
                    GameManager.Placement.PlacementClear_Completely(tile2.Original);
                }
                GameManager.Placement.PlacementConfirm(unitPlayer, info2);
            }

            _player = ppp.gameObject;
            unitPlayer.HP_Damaged = 0;
            unitPlayer.State = Monster.MonsterState.Placement;
            return;
        }


        //? 처음생성(아예 Player가 없는경우 - 새게임 or 로드게임)
        var player = GameManager.Placement.CreatePlacementObject("Player", info2, PlacementType.Monster);
        var component = player as Player;
        component.Player_FirstInit();
        component.State = Monster.MonsterState.Placement;
        GameManager.Placement.PlacementConfirm(player, info2);

        _player = player.GetObject();
    }


    #region Save / Load

    //public void GetPropertyValue(out int _pop, out int _danger, out int _currentAP)
    //{
    //    _pop = this.pop;
    //    _danger = this.danger;
    //    _currentAP = currentAP;
    //}


    public void SetLoadData(DataManager.SaveData data)
    {
        //? 통계 로드
        Load_Statistics(data.statistics);

        //? 게임 데이터 로드
        Load_MainData(data.mainData);

        if (UserData.Instance.FileConfig.Buff_ApBonusOne)
        {
            AP_Bonus++;
        }
        if (UserData.Instance.FileConfig.Buff_ApBonusTwo)
        {
            AP_Bonus++;
        }

        ExpansionConfirm(false);
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



    public CurrentGameData Save_MainData()
    {
        var saveData = new CurrentGameData();

        saveData.turn = Turn;

        saveData.DungeonLV = DungeonRank;
        saveData.FameOfDungeon = pop;
        saveData.DangerOfDungeon = danger;

        saveData.Player_Mana = Player_Mana;
        saveData.Player_Gold = Player_Gold;

        saveData.AP_MAX = AP_MAX;
        saveData.Player_AP = currentAP;


        //? 얘넨 깊은 복사가 되고있음(각각의 값들을 다 새로 복사중)
        saveData.CurrentDay = new Save_DayResult(CurrentDay);
        saveData.DayResultList = new Save_DayResult[DayList.Count];
        for (int i = 0; i < DayList.Count; i++)
        {
            saveData.DayResultList[i] = new Save_DayResult(DayList[i]);
        }

        saveData.ActiveFloor_Basement = ActiveFloor_Basement;
        saveData.ActiveFloor_Technical = ActiveFloor_Technical;

        saveData.addSlotCount = AddUnitSlotCount;

        return saveData;
    }

    public void Load_MainData(CurrentGameData data)
    {
        Turn = data.turn;

        DungeonRank = data.DungeonLV;
        pop = data.FameOfDungeon;
        danger = data.DangerOfDungeon;

        Player_Mana = data.Player_Mana;
        Player_Gold = data.Player_Gold;

        AP_MAX = data.AP_MAX;
        Player_AP = data.Player_AP;

        CurrentDay = new DayResult(data.CurrentDay);
        DayList = new List<DayResult>();
        foreach (var item in data.DayResultList)
        {
            DayList.Add(new DayResult(item));
        }

        ActiveFloor_Basement = (data.ActiveFloor_Basement);
        ActiveFloor_Technical = (data.ActiveFloor_Technical);

        AddUnitSlotCount = data.addSlotCount;
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
            ExpansionConfirm(true);

            if (ActiveFloor_Basement == 5)
            {
                Technical_Expansion();
                var fa = Floor[4].GetFloorObjectList(Define.TileType.Facility);
                foreach (var item in fa)
                {
                    if (item.Original is RemoveableObstacle)
                    {
                        var ro = item.Original as RemoveableObstacle;
                        ro.Show_Sprite();
                    }
                }
            }
            if (ActiveFloor_Basement == 6)
            {
                Technical_Expansion();
                var fa = Floor[5].GetFloorObjectList(Define.TileType.Facility);
                foreach (var item in fa)
                {
                    if (item.Original is RemoveableObstacle)
                    {
                        var ro = item.Original as RemoveableObstacle;
                        ro.Show_Sprite();
                    }
                }
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
    public void Technical_Expansion(int floor)
    {
        //if (ActiveFloor_Technical < floor)
        //{
        //    ActiveFloor_Technical++;
        //    GameManager.Technical.Expantion_Technical();
        //}
        for (; ActiveFloor_Technical < floor; ActiveFloor_Technical++)
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
    public int DungeonRank { get; set; } = 1;
    //public string DungeonRank_Alphabet { get { return ((Define.DungeonRank)_dungeonRank).ToString(); } }
    public void Dungeon_RankUP()
    {
        DungeonRank++;
        AP_MAX = DungeonRank + 1;
    }

    public int Player_Mana { get; private set; }
    public int Player_Gold { get; private set; }

    public int AP_MAX { get; private set; }

    public int AP_Bonus { get; set; }

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
        public int Gold_Get_Technical;
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

                case EventType.Technical:
                    Gold_Get_Technical += value;
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
        public int NPC_Defeat;
        public int NPC_Satisfaction;
        public int NPC_NonSatisfaction;
        public int NPC_Empty;
        public int NPC_Runaway;


        public void AddVisit(int value) { NPC_Visit += value; }
        public void AddPrisoner(int value) { NPC_Prisoner += value; }
        public void AddDefeatNPC(int value) { NPC_Defeat += value; }
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
        public void AddDefeatMonster(int value) { Monster_Defeat += value; }
        public void AddLvUp(int value) { Monster_LvUp += value; }
        public void AddTrait(int value) { Monster_Trait += value; }
        public void AddEvolution(int value) { Monster_Evolution += value; }



        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ던전
        public int GetPopularity;
        public int GetDanger;
        public void AddPop(int _value) { GetPopularity += _value; }
        public void AddDanger(int _value) { GetDanger += _value; }

        public void AddPop_Directly(int _value) { Main.Instance.PopularityOfDungeon += _value; }
        public void AddDanger_Directly(int _value) { Main.Instance.DangerOfDungeon += _value; }

        public DayResult()
        {

        }
        public DayResult(Save_DayResult result)
        {
            CopyFields(result, this);
        }
        private void CopyFields(object source, object destination)
        {
            // Source와 Destination의 필드를 가져와 같은 이름의 필드를 매핑
            foreach (FieldInfo field in source.GetType().GetFields())
            {
                FieldInfo destField = destination.GetType().GetField(field.Name);
                if (destField != null)
                {
                    destField.SetValue(destination, field.GetValue(source));
                }
            }
        }
    }


    void Init_DayResult()
    {
        CurrentDay = new DayResult();
        CurrentDay.SetOrigin(Player_Mana, Player_Gold, PopularityOfDungeon, DangerOfDungeon, DungeonRank);
    }




    void DayOver_Dayresult()
    {
        //? 누적 통계 추가
        UserData.Instance.CurrentPlayerData.statistics.Update_DayOver(new Save_DayResult(CurrentDay));

        DayList.Add(CurrentDay);
        //AddScore(CurrentDay);

        PopularityOfDungeon += CurrentDay.GetPopularity;
        DangerOfDungeon += CurrentDay.GetDanger;

        EventManager.Instance.TurnOver();

        Player_AP = AP_MAX + AP_Bonus;
        if (GameManager.Technical.Get_Technical<ApOrb>() != null)
        {
            Player_AP++;
        }
        if (RandomEventManager.Instance.Check_Current_ContinueEvent(RandomEventManager.ContinueRE.AP_Up))
        {
            Player_AP += (Player_AP / 2);
        }
        if (RandomEventManager.Instance.Check_Current_ContinueEvent(RandomEventManager.ContinueRE.AP_Down))
        {
            Player_AP = 1;
        }

        //? 위가 적용 아래가 새로교체
        Init_DayResult();

        StartCoroutine(Show_DayResult());
    }

    IEnumerator Show_DayResult()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        //? 데모 종료시점. 저장하기전에 해야함.

#if STEAM_DEMO_BUILD
        if (Turn == 20)
        {
            DEMO_Ending();
            yield break;
        }
#endif

        if (Turn == 30 && UserData.Instance.FileConfig.GameMode == Define.ModeSelect.Story)
        {
            Managers.Dialogue.ShowDialogueUI(DialogueName.Day30_Over, Player);
            yield break;
        }



        Managers.UI.Popup_Reservation(() =>
        {
            var ui = Managers.UI.ShowPopUp<UI_DayResult>();
            ui.TextContents(DayList[Turn - 1], CurrentDay);
        });

        StartCoroutine(AutoSave());
    }

    #endregion


    #region 각종 통계 (세이브파일 및 DayResult 기준, 그리고 DayResult에 없는 통계들도 포함함
    public int GetTotalMana()
    {
        int mana = 0;

        foreach (var item in DayList)
        {
            mana += item.Mana_Get_Artifacts;
            mana += item.Mana_Get_Bonus;
            mana += item.Mana_Get_Etc;
            mana += item.Mana_Get_Facility;
            mana += item.Mana_Get_Monster;
        }
        if (CurrentDay != null)
        {
            mana += CurrentDay.Mana_Get_Artifacts;
            mana += CurrentDay.Mana_Get_Bonus;
            mana += CurrentDay.Mana_Get_Etc;
            mana += CurrentDay.Mana_Get_Facility;
            mana += CurrentDay.Mana_Get_Monster;
        }

        return mana;
    }
    public int UseTotalMana()
    {
        int use = 0;

        foreach (var item in DayList)
        {
            use += item.Mana_Use_Etc;
            use += item.Mana_Use_Facility;
            use += item.Mana_Use_Monster;
            use += item.Mana_Use_Technical;
        }
        if (CurrentDay != null)
        {
            use += CurrentDay.Mana_Use_Etc;
            use += CurrentDay.Mana_Use_Facility;
            use += CurrentDay.Mana_Use_Monster;
            use += CurrentDay.Mana_Use_Technical;
        }

        return use;
    }



    public int GetTotalGold()
    {
        int gold = 0;

        foreach (var item in DayList)
        {
            gold += item.Gold_Get_Bonus;
            gold += item.Gold_Get_Etc;
            gold += item.Gold_Get_Facility;
            gold += item.Gold_Get_Monster;
            gold += item.Gold_Get_Technical;
        }
        if (CurrentDay != null)
        {
            gold += CurrentDay.Gold_Get_Bonus;
            gold += CurrentDay.Gold_Get_Etc;
            gold += CurrentDay.Gold_Get_Facility;
            gold += CurrentDay.Gold_Get_Monster;
            gold += CurrentDay.Gold_Get_Technical;
        }

        return gold;
    }

    public int UseTotalGold() //? 길드에서 사용한 골드는 기록이 안되있어서 안쓰는게 좋을듯. 뭐 추가하면 하는데, 딱히 필요없는 수치라서
    {
        int use = 0;

        foreach (var item in DayList)
        {
            use += item.Gold_Use_Etc;
            use += item.Gold_Use_Facility;
            use += item.Gold_Use_Monster;
            use += item.Gold_Use_Technical;
        }
        if (CurrentDay != null)
        {
            use += CurrentDay.Mana_Use_Etc;
            use += CurrentDay.Mana_Use_Facility;
            use += CurrentDay.Mana_Use_Monster;
            use += CurrentDay.Mana_Use_Technical;
        }

        return use;
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
            kill += item.NPC_Defeat;
        }
        if (CurrentDay != null)
        {
            kill += CurrentDay.NPC_Defeat;
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



    public Statistics CurrentStatistics { get; set; }

    void Init_Statistics()
    {
        CurrentStatistics = new Statistics();
    }

    void Load_Statistics(Statistics _data)
    {
        if (_data == null)
        {
            Init_Statistics();
        }
        else
        {
            CurrentStatistics = _data.DeepCopy();
        }
    }


    #endregion 각종 통계









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



    #region Camera

    CameraControl mainCam;

    public void ShowGuild()
    {
        //var cam = Camera.main.GetComponent<CameraControl>();
        mainCam.ChasingTarget(Dungeon, 1);
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
                RandomEventManager.Instance.TurnStart();
                EventManager.Instance.TurnStart();
                GameManager.NPC.TurnStart();
                GameManager.Monster.MonsterTurnStartEvent();
                GameManager.Facility.TurnStartEvent();
                GameManager.Artifact.TurnStartEvent_Artifact();
            }
            else
            {
                Init_Player(); //? 플레이어 없으면 재소환
                NightEvent();
                DayMonsterEvent();
                GameManager.Monster.MonsterTurnOverEvent();
                GameManager.Facility.TurnOverEvent();
                GameManager.Title.TurnOverEvent_Title();

                //? 대사 이벤트 등 턴 이벤트
                Main_TurnOverEvent();
                //? 결과창과 저장
                DayOver_Dayresult();
                //? 엔딩변경 (어차피 이거 로드할 떄 부르니까 턴종료 마지막에 하는게 맞음)
                ChangeEggState();

                //? 현재 통계
                CurrentStatistics.Update_DayResult(this);
                CurrentStatistics.Show_CurrentLog();

                //? 메인 UI 업데이트
                UI_Main.TurnOverEvent();
                DayChangeAnimation();

                //? 사운드 리셋
                SoundManager.Instance.Reset_MainBGM();
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
        //DayChangeAnimation();
    }




    void Main_TurnStartEvent()
    {
        UI_EventBox.AddEventText($"※{Turn}{UserData.Instance.LocaleText("Event_DayStart")}※");

        switch (Turn)
        {

            case 1: //? 원래 1일차 종료부터 가능했던 정보확인을 1일차 시작때부터 할 수 있도록 변경
                UI_Main.Active_Floor();
                //if (UserData.Instance.FileConfig.PlayRounds == 1) //? 초회차면 겜시작시 메뉴얼
                //{
                //    Managers.UI.ShowPopUp<UI_Manual>();
                //}
                //GameManager.NPC.AddEventNPC(NPC_Type_RandomEvent.Romys.ToString(), 5, NPC_Typeof.NPC_Type_RandomEvent);
                //GameManager.NPC.AddEventNPC(NPC_Type_SubEvent.Venom.ToString(), 2, NPC_Typeof.NPC_Type_SubEvent);
                //GameManager.NPC.AddEventNPC(NPC_Type_SubEvent.Judgement.ToString(), 4, NPC_Typeof.NPC_Type_SubEvent);
                //GameManager.NPC.AddEventNPC(NPC_Type_SubEvent.Judgement.ToString(), 10, NPC_Typeof.NPC_Type_SubEvent);
                break;

            case 3: //? FirstAdv Event
                GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_FirstAdventurer.ToString(), 7, NPC_Typeof.NPC_Type_MainEvent);
                break;

            case 7: //? RedHair Event 
                GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_RedHair.ToString(), 10, NPC_Typeof.NPC_Type_MainEvent);
                break;


            //case 20:
            //    GuildManager.Instance.AddInstanceGuildNPC(GuildNPC_LabelName.DeathMagician);
            //    EventManager.Instance.Add_GuildQuest_Special(10001, true);
            //    break;


            case 30:
                break;
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

        if (UserData.Instance.FileConfig.PlayRounds > 1) //? 초회차가 아니면 턴종료 이벤트는 스킵
        {
            return;
        }

        switch (Turn)
        {
            case 1:
                Debug.Log("1일차 종료 이벤트 - 시설배치");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Facility, Player);
                UI_Main.Active_Button(UI_Management.ButtonEvent._1_Facility);
                UI_Main.Active_Button(UI_Management.ButtonEvent._6_DungeonEdit);
                UI_Main.Active_Button(UI_Management.ButtonEvent._5_Quest);
                JournalManager.Instance.AddJournal(1);

                StartCoroutine(Wait_AP_Tutorial());
                break;

            case 2:
                Debug.Log("2일차 종료 이벤트 - 몬스터");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Monster, Player);
                UI_Main.Active_Button(UI_Management.ButtonEvent._3_Management);
                break;

            case 3:
                Debug.Log("3일차 종료 이벤트 - 테크니컬");
                Technical_Expansion(1);
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Technical, Player);
                break;

            case 4:
                Debug.Log("4일차 종료 이벤트 - 비밀방");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Egg, Player);
                break;

            case 5:
                Debug.Log("5일차 종료 이벤트 - 수정");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Orb, Player);
                break;

            case 6:
                Debug.Log("6일차 종료 이벤트 - 길드");
                Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_Guild, Player);
                UI_Main.Active_Button(UI_Management.ButtonEvent._4_Guild);
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
    void DEMO_Ending()
    {
        Managers.UI.Popup_Reservation(() =>
        {
            Managers.UI.ShowPopUp<UI_DEMO_15DAY>("_UI_DEMO_CLEAR");
        });

        //Managers.Resource.Instantiate($"UI/_UI_DEMO_CLEAR");
    }


    #endregion



    #region Monster or Unit


    public int AddUnitSlotCount { get; set; }

    void DayMonsterEvent()
    {
        //Debug.Log($"지금턴은 : {Turn}");
        if (Turn == 30) return;

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
                        item.monster.AddCollectionPoint();
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
    Animator ani_Background;
    Animator ani_Dungeon;

    void Init_DefaultSetting()
    {
        ani_MainUI = UI_Main.GetComponent<Animator>();

        //ani_Background = GameObject.Find("SkyBackground").GetComponent<Animator>();
        ani_Background = GameObject.Find("NonScript").GetComponent<Animator>();

        ani_Dungeon = GameObject.Find("DungeonSprite").GetComponent<Animator>();
        mainCam = Camera.main.GetComponent<CameraControl>();
    }


    public void DayChangeAnimation()
    {
        ani_MainUI.SetBool("Management", Management);
        ani_Background.SetBool("Management", Management);
        //ani_Dungeon.SetBool("Management", Management);
    }


    public void Dungeon_Animation_RandomEvent(RandomEventManager.CurrentRandomEventContent content)
    {
        if (DungeonAnimCor != null)
        {
            StopCoroutine(DungeonAnimCor);
            DungeonAnimCor = null;
        }

        StartCoroutine(DungeonAnim_Wait_Msg(content));
    }

    Coroutine DungeonAnimCor;
    IEnumerator DungeonAnim_Wait_Msg(RandomEventManager.CurrentRandomEventContent content)
    {
        mainCam.ChasingTarget(Dungeon, 1.0f);
        yield return new WaitForSeconds(1.0f);

        switch (content.eventValue)
        {
            case RandomEventValue.Good:
                ani_Dungeon.Play("Dungeon_Event_Green_Show");
                break;

            case RandomEventValue.Bad:
                ani_Dungeon.Play("Dungeon_Event_Red_Show");
                break;

            case RandomEventValue.Normal:
                ani_Dungeon.Play("Dungeon_Event_Blue_Show");
                break;

            case RandomEventValue.Special:
                ani_Dungeon.Play("Dungeon_Event_Purple_Show");
                break;
        }

        yield return null;

        mainCam.CameraShake(1.0f, 0.3f);

        //? 애니메이션 재생이 끝날때까지 기달
        yield return new WaitUntil(() => {
            // 매 프레임마다 상태 정보를 새로 가져옴
            AnimatorStateInfo info = ani_Dungeon.GetCurrentAnimatorStateInfo(0);
            return info.normalizedTime >= 1.0f;
        });


        UI_SystemMessage msg = null;

        if (UserData.Instance.FileConfig.firstCheck_RandomEvent == false) //? 랜덤이벤트가 처음이라면
        {
            UserData.Instance.FileConfig.firstCheck_RandomEvent = true;
            //? 첫랜덤이벤트 다이어로그 호출하기

            Managers.Dialogue.ShowDialogueUI(DialogueName.Tutorial_RandomEvent, Player);

            Managers.UI.Popup_Reservation(() =>
            {
                msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
                msg.Message = RandomEventManager.Instance.GetData(content.ID).description;
            });
        }
        else
        {
            msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = RandomEventManager.Instance.GetData(content.ID).description;
        }

        yield return null;

        //? 시스템메세지가 끝날떄까지 기달
        yield return new WaitUntil(() => msg == null);

        switch (content.eventValue)
        {
            case RandomEventValue.Good:
                ani_Dungeon.Play("Dungeon_Idle_Green");
                break;

            case RandomEventValue.Bad:
                ani_Dungeon.Play("Dungeon_Idle_Red");
                break;

            case RandomEventValue.Normal:
                ani_Dungeon.Play("Dungeon_Idle_Blue");
                break;

            case RandomEventValue.Special:
                ani_Dungeon.Play("Dungeon_Idle_Purple");
                break;
        }

        DungeonAnimCor = null;
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
            if (i == (int)Define.DungeonFloor.Egg)
            {
                Floor[i].Hidden = true;
                Floor[i].LabelName = $"{UserData.Instance.LocaleText("숨겨진곳")}";
            }
            else
            {
                Floor[i].LabelName = $"{UserData.Instance.LocaleText("지하")} {i} {UserData.Instance.LocaleText("층")}";
            }

            Floor[i].Init_Floor();
        }
    }


    void Start_Entrance()
    {
        for (int i = 1; i < ActiveFloor_Basement; i++)
        {
            Floor[i].Init_Entrance();
        }
    }

    void ExpansionConfirm(bool init_Entrance)
    {
        for (int i = 1; i < Floor.Length; i++)
        {
            Floor[i].gameObject.SetActive(false);
        }

        for (int i = 1; i < ActiveFloor_Basement; i++)
        {
            Floor[i].gameObject.SetActive(true);
        }

        if (init_Entrance)
        {
            Start_Entrance();
        }


        mainCam.LimitRefresh();
    }

    //public void DungeonExpansionUI()
    //{
    //    if (Floor.Length > ActiveFloor_Basement)
    //    {
    //        var legacy = FindAnyObjectByType<UI_Expansion_Floor>();
    //        if (legacy != null)
    //        {
    //            Destroy(legacy.gameObject);
    //        }

    //        var ui = Managers.Resource.Instantiate("UI/PopUp/Element/UI_Expansion_Floor");
    //        ui.transform.position = Floor[ActiveFloor_Basement].transform.position + new Vector3(0, 5, 0);

    //        ui.GetComponent<UI_Expansion_Floor>().SetContents(ActiveFloor_Basement, 200, 200, 2);
    //    }
    //}



    public void ResetCurrentAction()
    {
        Debug.Log("ResetCurrentAction");

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

    public void ChangeEggState()
    {
        Debug.Log($"{Turn}일차 종료\nTotal Mana : {GetTotalMana()}\nTotal Gold : {GetTotalGold()}");

        if (UserData.Instance.FileConfig.GameMode == Define.ModeSelect.Endless)
        {
            EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Endless"));
            return;
        }



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
        //? 엔딩 우선순위 = 드래곤 / 마왕 / 히로인 / 토끼 / 멍멍이


        //? 2회차 이상
        if (UserData.Instance.FileConfig.PlayRounds > 1)
        {
            //? 1순위 - 마왕
            if (GameManager.Facility.GetFacilityCount<Devil_Statue>() >= 5)
            {
                CurrentEndingState = Endings.Demon;
                EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Demon"));
                return;
            }

            //? 2순위 - 용사
            // 특수시설 - 신전 건축 - 10000마나를 기부했다면 (일단은 5천)
            if (GameManager.Facility.GetFacilityCount<Devil_Statue>() == 0 && GameManager.Technical.Get_Technical<Temple>() != null &&
                GameManager.Technical.Get_Technical<Temple>().InteractionCounter >= 5)
            {
                CurrentEndingState = Endings.Hero;
                EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Hero"));
                return;
            }
        }


        //? ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ 1회차부터 가능

        //? 1순위 - 드래곤 or 마왕
        if (DangerOfDungeon > PopularityOfDungeon && DangerOfDungeon >= 500)
        {
            CurrentEndingState = Endings.Dragon;
            EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Dragon"));
            return;
        }


        //? 2순위 - 히로인
        if (GameManager.Monster.Check_ExistUnit<Heroine>())
        {
            if (GameManager.Monster.GetMonster<Heroine>().UnitDialogueEvent.ClearCheck((int)UnitDialogueEventLabel.Heroin_Root_Ture))
            {
                CurrentEndingState = Endings.Cat;
                EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Cat"));
                return;
            }
        }

        //? 3순위 - 토끼 - 위험도가 0일때를 상정하지 않아서 안되고있었다... 그냥 +1로 나누자. 
        if ((PopularityOfDungeon / (DangerOfDungeon + 1)) >= 3)
        {
            CurrentEndingState = Endings.Ravi;
            EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Ravi"));
            return;
        }



        //? 가장 후순위 - 아무것도 걸리지 않았으면 노말엔딩(멍멍이)
        CurrentEndingState = Endings.Dog;
        EggObj.GetComponent<SpecialEgg>().SetEggData(GameManager.Facility.GetData("Egg_Dog"));
    }



    #endregion



}

public class CurrentGameData
{
    // 세이브 슬롯에 표시할 게임정보
    public int turn;
    //public int Final_Score;

    // 게임 진행상황
    public int DungeonLV;
    public int FameOfDungeon;
    public int DangerOfDungeon;

    public int Player_Mana;
    public int Player_Gold;
    public int Player_AP;
    public int AP_MAX;

    public Save_DayResult CurrentDay;
    public Save_DayResult[] DayResultList;

    // Floor 정보
    public int ActiveFloor_Basement; //? 확장된 계층정보
    public int ActiveFloor_Technical; //? 특수시설 계층

    // 유닛 슬롯 정보
    public int addSlotCount; //? 일반 유닛 말고 특수유닛 숫자(레나, 우투리, 랜덤이벤트 6명 해서 총 8)
}


public class Statistics //? 각종 통계, 나중에 도전과제 업적으로도 충분히 사용 가능 / 값타입 필드만 사용
{
    //? 기본 정보 - 총획득마나/총사용마나  방문자/도망자/승리(킬)/만족  유닛통계  인기도 위험도 등등
    public int Total_Mana;
    public int Total_Gold;

    public int Total_Visit;
    public int Total_Stisfaction;
    public int Total_Defeat;

    //? 상호작용 횟수 통계
    public int Interaction_Herb;
    public int Interaction_Mineral;
    public int Interaction_Treasure; //? 보물상자 / 무기 등 상세분류로 나눌지말지는 고민. 일단은 Treasure = 1개인 상태
    public int Interaction_Trap;
    public int Interaction_Secret;      //? 숨겨진 방 입장 횟수 (전이포탈 상호작용)

    public int Hightest_Unit_Size;
    public int Highest_Unit_Lv;


    //? 몇개있는지 체크
    public int Amenity;


    //? 기타 필요한거...라기엔 그냥 잡다한 통계 전부 업데이트 해도 될 것 같음.
    public int highTurn;        //? 최대 턴 (무한모드용)


    //? 저장하는 타이밍에 업데이트 되야할 수치가 있으면 업데이트하기
    public void Update_ToSave(CurrentGameData currentData)
    {
        highTurn = Mathf.Max(highTurn, currentData.turn);
    }



    public void Update_DayResult(Main main)
    {
        Total_Mana = main.GetTotalMana();
        Total_Gold = main.GetTotalGold();

        Total_Visit = main.GetTotalVisit();
        Total_Stisfaction = main.GetTotalSatisfaction();
        Total_Defeat = main.GetTotalKill();
    }


    public void Show_CurrentLog()
    {
        string log = "";
        foreach (var field in GetType().GetFields())
        {
            log += $"{field.Name}: {field.GetValue(this)}\n";
        }
        Debug.Log(log);
    }


    //? 필드값을 하나라도 바꾸면 아래 함수를 호출해야함. 
    public void OnChangeFieldValue() //? 여기서 업적 및 도전과제 등을 하면 되는데,,, 그냥 Statistics를 가져다 쓰는게 나을지도 모르겠네
    {

    }

    //public void SetBoolValue(string boolName, bool value)
    //{
    //    // 필드 정보를 가져옴
    //    var field = this.GetType().GetField(boolName);

    //    if (field != null && field.FieldType == typeof(bool))
    //    {
    //        field.SetValue(this, value);
    //    }
    //    else
    //    {
    //        Debug.LogError("Invalid field name or type: " + boolName);
    //    }
    //}

    public Statistics DeepCopy()
    {
        //? 아래 메서드는 어디까지나 필드를 얕은복사 하는 메서드임. 다만 현재 모든 필드값이 값타입이라 값복사가 될뿐임.
        Statistics newConfig = (Statistics)this.MemberwiseClone();
        return newConfig;
    }
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
    public int Gold_Get_Technical;
    public int Gold_Get_Etc;
    public int Gold_Get_Bonus;

    public int Gold_Use_Facility;
    public int Gold_Use_Monster;
    public int Gold_Use_Etc;
    public int Gold_Use_Technical;


    public int NPC_Visit;
    public int NPC_Prisoner;
    public int NPC_Defeat;
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
        CopyFields(result, this);
    }

    private void CopyFields(object source, object destination)
    {
        // Source와 Destination의 필드를 가져와 같은 이름의 필드를 매핑
        foreach (FieldInfo field in source.GetType().GetFields())
        {
            FieldInfo destField = destination.GetType().GetField(field.Name);
            if (destField != null)
            {
                destField.SetValue(destination, field.GetValue(source));
            }
        }
    }

}

