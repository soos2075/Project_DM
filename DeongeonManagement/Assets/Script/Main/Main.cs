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
        ManagementInit();
        BasementFloorInit();
        AnimationInit();

        StartCoroutine(NextStart());

    }



    void StartNextFrame()
    {
        CurrentFloor = Floor[3];
        var obj = Managers.Placement.CreateOnlyOne("Facility/Special_MagicEgg", null, Define.PlacementType.Facility);
        PlacementInfo info = new PlacementInfo(CurrentFloor, CurrentFloor.GetRandomTile(obj));

        Managers.Placement.PlacementConfirm(obj, info, true);

        Managers.UI.ShowPopUp<UI_Dialogue>();
    }

    IEnumerator NextStart()
    {
        yield return new WaitForEndOfFrame();
        StartNextFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Time.timeScale == 1);
        var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
        message.Message = "던전에 다양한 시설과 몬스터들을 배치하여 모험가들에게 마나를 얻으세요!\n\n" +
            "마나는 기본적으로 모험가들이 던전에서 행하는 모든 행동에서 얻을 수 있어요.";

    }


    #region Management

    public int Final_Score { get; private set; }

    void AddScore(DayResult day)
    {
        int score = day.Get_Mana;
        score += day.Get_Gold;
        score += day.Get_Prisoner * 50;
        score += day.Get_Kill * 100;

        Final_Score += score;
    }

    public int FameOfDungeon { get; set; }
    public int DangerOfDungeon { get; set; }


    public int Player_Mana { get; set; }
    public int Player_Gold { get; set; }
    public int Player_AP { get; set; }

    public int Prisoner { get; set; }


    public List<DayResult> _dayList;

    public DayResult CurrentDay { get; set; }


    public class DayResult
    {
        public int Origin_Mana;
        public int Origin_Gold;
        public int Origin_Prisoner;

        public DayResult(int mana, int gold, int prisoner)
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
        Player_Mana = 7777;
        Player_Gold = 3333;
        Player_AP = 100;

        _dayList = new List<DayResult>();

        CurrentDay = new DayResult(Player_Mana, Player_Gold, Prisoner);
    }




    void DayOver()
    {
        _dayList.Add(CurrentDay);
        AddScore(CurrentDay);

        var ui = Managers.UI.ShowPopUp<UI_DayResult>();
        ui.TextContents(_dayList[Turn - 1]);

        Player_Mana += CurrentDay.Get_Mana;
        Player_Gold += CurrentDay.Get_Gold;
        Player_AP = 3;

        //? 위가 적용 아래가 새로교체

        CurrentDay = new DayResult(Player_Mana, Player_Gold, Prisoner);
    }

    #endregion




    #region TechnicalEvent
    public List<Action<int>> DayActions { get; set; } = new List<Action<int>>();
    public List<Action<int>> NightActions { get; set; } = new List<Action<int>>();

    public UI_Technical CurrentTechnical { get; set; }

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
                Managers.NPC.TurnStart();
            }
            else
            {
                DangerOfDungeon += 10;
                FindObjectOfType<UI_Management>().DayOver();
                DayOver();
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
                Managers.Technical.Level_2();
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
            CurrentFloor = Floor[3];
            var obj = Managers.Placement.CreateOnlyOne("Facility/Exit", null, Define.PlacementType.Facility);
            PlacementInfo info = new PlacementInfo(CurrentFloor, CurrentFloor.GetRandomTile(obj));

            Managers.Placement.PlacementConfirm(obj, info, true);
        }
        

        yield return new WaitForSecondsRealtime(1);

        Camera.main.transform.position = new Vector3(Floor[2].transform.position.x, Floor[2].transform.position.y, -10);
        Camera.main.orthographicSize = 3;

        yield return new WaitForSecondsRealtime(1);
        {
            CurrentFloor = Floor[2];

            var obj = Managers.Placement.CreateOnlyOne("Facility/EggEntrance", null, Define.PlacementType.Facility);
            PlacementInfo info = new PlacementInfo(CurrentFloor, CurrentFloor.GetRandomTile(obj));

            Managers.Placement.PlacementConfirm(obj, info, true);
        }
        yield return new WaitForSecondsRealtime(1);

        var dia = Managers.UI.ShowPopUp<UI_Dialogue>();
        var data = DialogueManager.Instance.GetDialogue("EggAppear");

        dia.data = data;
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
        }

        SetHiddenFloor();
    }

    void SetHiddenFloor()
    {
        Floor[3].Hidden = true;
        //Floor[3].Remove_Entrance();
        //Managers.Placement.PlacementClear_Completely(Floor[3].Exit);
        //Managers.Placement.PlacementClear_Completely(Floor[3].Entrance);
    }

    #endregion

}
