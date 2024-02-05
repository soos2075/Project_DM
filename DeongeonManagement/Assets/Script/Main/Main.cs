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

    private void Awake()
    {
       
    }

    void Start()
    {
        Debug.Log("왜이게먼저?");
        NewGame_Init();
        Default_Init();
    }

    bool _DefaultSetting = false;

    public void Default_Init()
    {
        if (_DefaultSetting)
        {
            return;
        }


        BasementFloorInit();
        _dayList = new List<DayResult>();
        AnimationInit();
        FindObjectOfType<UI_Management>().Start_Main();

        _DefaultSetting = true;
    }

    public void NewGame_Init()
    {
        if (_DefaultSetting)
        {
            return;
        }

        ActiveFloor_Basement = 4;
        ActiveFloor_Technical = 1;
        Dungeon_Lv = 2;

        BasementFloorInit();
        _dayList = new List<DayResult>();
        AnimationInit();
        FindObjectOfType<UI_Management>().Start_Main();

        ManagementInit();

        Expansion_BasementFloor();
        GameManager.Technical.Expantion_Technical();

        StartCoroutine(NextStart());

        _DefaultSetting = true;
    }




    IEnumerator NextStart()
    {
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
        var tile = Floor[3].GetRandomTile();
        PlacementInfo info = new PlacementInfo(Floor[3], tile);

        GameManager.Facility.CreateFacility_OnlyOne("Special_MagicEgg", info, true);

        Managers.Dialogue.ShowDialogueUI("Prologue");
    }





    #region Load Data
    public void SetLoadData(DataManager.SaveData data)
    {
        Turn = data.turn;
        Final_Score = data.Final_Score;
        FameOfDungeon = data.FameOfDungeon;
        DangerOfDungeon = data.DangerOfDungeon;
        Player_Mana = data.Player_Mana;
        Player_Gold = data.Player_Gold;
        Player_AP = data.Player_AP;
        Prisoner = data.Prisoner;
        CurrentDay = data.CurrentDay;

        ActiveFloor_Basement = (data.ActiveFloor_Basement);
        ActiveFloor_Technical = (data.ActiveFloor_Technical);
        Expansion_BasementFloor();
        GameManager.Technical.Expantion_Technical();

        FindObjectOfType<UI_Management>().Texts_Refresh();
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
            Expansion_BasementFloor();
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
    public int FameOfDungeon { get { return _fame; } set { _fame = Mathf.Clamp(value, 0, value); } }
    int _danger;
    public int DangerOfDungeon { get { return _danger; } set { _danger = Mathf.Clamp(value, 0, value); } }


    public int Dungeon_Lv { get; set; }
    public int Player_Mana { get; private set; }
    public int Player_Gold { get; private set; }

    public int Player_AP { get; set; }
    public int Prisoner { get; set; }


    public List<DayResult> _dayList;

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
    }


    void ManagementInit()
    {
        Player_Mana = 300;
        Player_Gold = 200;
        Player_AP = 3;



        CurrentDay = new DayResult();
        CurrentDay.SetOrigin(Player_Mana, Player_Gold, Prisoner);
    }




    void DayOver_Dayresult()
    {
        _dayList.Add(CurrentDay);
        AddScore(CurrentDay);

        var ui = Managers.UI.ShowPopUp<UI_DayResult>();
        ui.TextContents(_dayList[Turn - 1]);

        Player_Mana += CurrentDay.Get_Mana;
        Player_Gold += CurrentDay.Get_Gold;
        Player_AP = 3;

        //? 위가 적용 아래가 새로교체

        CurrentDay = new DayResult();
        CurrentDay.SetOrigin(Player_Mana, Player_Gold, Prisoner);
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
                DangerOfDungeon += 10;
                FindObjectOfType<UI_Management>().Texts_Refresh();
                DayOver_Dayresult();
                TurnOverEvent();
                NightEvent();
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
                //Debug.Log("3일차 시작 이벤트 발생");
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
                //Debug.Log("1일차 종료 이벤트 발생");
                
                break;

            case 2:
                Debug.Log("2일차 종료 이벤트 발생");
                GameManager.Technical.Level_2();
                break;

            case 5:
                Debug.Log("3일차 종료 이벤트 발생");
                StartCoroutine(TurnEvent_EggAppear());
                break;


            default:
                break;
        }
    }


    IEnumerator TurnEvent_EggAppear()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2);

        Managers.UI.CloseAll();
        FindObjectOfType<UI_EventBox>().BoxActive(false);

        Camera.main.transform.position = new Vector3(Floor[3].transform.position.x, Floor[3].transform.position.y, -10);
        Camera.main.orthographicSize = 3;

        yield return new WaitForSecondsRealtime(1);
        {
            var tile = Floor[3].GetRandomTile();
            PlacementInfo info = new PlacementInfo(Floor[3], tile);

            var obj = GameManager.Facility.CreateFacility_OnlyOne("Exit", info, true);
        }
        

        yield return new WaitForSecondsRealtime(1);

        Camera.main.transform.position = new Vector3(Floor[2].transform.position.x, Floor[2].transform.position.y, -10);
        Camera.main.orthographicSize = 3;

        yield return new WaitForSecondsRealtime(1);
        {
            var tile = Floor[2].GetRandomTile();
            PlacementInfo info = new PlacementInfo(Floor[3], tile);

            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info, true);
        }
        yield return new WaitForSecondsRealtime(1);

        var dia = Managers.UI.ShowPopUp<UI_Dialogue>();
        var data = Managers.Dialogue.GetDialogue("EggAppear");

        dia.Data = data;
    }


    #endregion




   





   







    #region Animation
    Animator ani_MainUI;
    Animator ani_Sky;
    VerticalLayoutGroup layout;

    void AnimationInit()
    {
        ani_MainUI = FindObjectOfType<UI_Management>().GetComponent<Animator>();
        ani_Sky = FindObjectOfType<SpriteAnimation>().GetComponent<Animator>();
        layout = FindObjectOfType<UI_Management>().GetComponentInChildren<VerticalLayoutGroup>();
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

    void BasementFloorInit()
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



    void Expansion_BasementFloor()
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
        DungeonExpansionUI();
    }

    void DungeonExpansionUI()
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
