using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    #region Singleton
    private static EventManager _instance;
    public static EventManager Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<EventManager>();
            if (_instance == null)
            {
                GameObject go = new GameObject { name = "@EventManager" };
                _instance = go.AddComponent<EventManager>();
            }
            DontDestroyOnLoad(_instance);
        }
    }


    private void Awake()
    {
        Initialize();
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    #endregion

    private void Start()
    {
        Init_Event();
    }
    public void Init_Event()
    {
        Init_DialogueAction();
        Init_EventAction();
        Init_ForQuestAction();
        RegistDayEvent();
    }


    int Pop { get; set; }
    int Danger { get; set; }

    public void TurnStart()
    {
        CurrentTurn = Main.Instance.Turn;
        Pop = Main.Instance.PopularityOfDungeon;
        Danger = Main.Instance.DangerOfDungeon;

        //CurrentQuestAction?.Invoke();

        TurnStart_EventSchedule();
        Add_ReservationGuildQuest();

        AddDayEventByCondition_Start();

        //? 시작이벤트 발생부분
        OneDayAfter();
        var TodayEvent = GetDayEvent(); //? 가져옴과 동시에 DayList에서 삭제하니까 여기까지는 호출해야함 / 29일날 상태에 따라 결정됨

        if (CurrentTurn == 30 && Main.Instance.CurrentEndingState == Endings.Demon)
        {
            //? 30일 이벤트를 셀프로 대체
            Last_Judgment();
            ClearQuestAction(7710003);
        }
        else if (CurrentTurn == 30 && Main.Instance.CurrentEndingState == Endings.Hero)
        {
            //? 30일 이벤트를 셀프로 대체
            Hero_Final();
        }
        else if (TodayEvent != null)
        {
            Run_DayEventAction(TodayEvent.EventIndex); //? 얘가 실제 invoke를 하는 곳이기 때문에 얘만 안하면 댐
        }
    }
    public void TurnOver()
    {
        Pop = Main.Instance.PopularityOfDungeon;
        Danger = Main.Instance.DangerOfDungeon;

        TurnOverEventReserve?.Invoke();
        TurnOverEventReserve = null;

        TurnOver_EventSchedule();

        if (TryRankUp(Pop, Danger))
        {
            Main.Instance.Dungeon_RankUP();
            RankUpEvent();
        }

        AddDayEventByCondition_Over();
    }


    Action TurnOverEventReserve;
    public void AddTurnOverEventReserve(Action action)
    {
        TurnOverEventReserve += action;
    }



    void QuestDataReset()
    {
        CurrentTurn = 0;
        CurrentGuildData = new List<GuildNPC_Data>();
        AddQuest_Daily = new List<int>();
        CurrentQuestAction = null;
        CurrentQuestAction_forSave.Clear();
        DayEventList = new List<DayEvent>();

        Reservation_Quest = new List<Quest_Reservation>();
        CurrentClearEventData = new ClearEventData();
        TurnOverEventReserve = null;
        Managers.Dialogue.CurrentReserveAction = null;
    }

    public void NewGameReset()
    {
        QuestDataReset();
        GuildManager.Instance.Init_CurrentGuildData(); //? CurrentGuildData가 리셋되서 다시 채우는 역할
    }



    #region 턴 시작 / 종료시 조건에 따라 여러 이벤트 추가하는곳
    void TurnStart_EventSchedule()
    {
        switch (CurrentTurn)
        {
            case 8:
                Add_GuildQuest_Special(2102, false);
                break;

            case 15:
                Add_GuildQuest_Special(2103, false);
                break;
        }
    }
    void TurnOver_EventSchedule()
    {
        switch (CurrentTurn)
        {
            case 11:
                Remove_GuildQuest(2102);
                ClearQuestAction(772102);
                GameManager.NPC.Event_Herb = false;
                break;

            case 18:
                Remove_GuildQuest(2103);
                ClearQuestAction(772103);
                GameManager.NPC.Event_Mineral = false;
                break;

            //default:
            //    Add_Daily(2100);
            //    break;
        }
        //? 인기도 상승 이벤트
        Add_Daily(2100);
    }

    void AddDayEventByCondition_Start()
    {
        if (Danger >= 450)
        {
            DayEventLabel currentEvent = DayEventLabel.BloodSong_Appear;
            if (CurrentClearEventData.Check_AlreadyClear(currentEvent) == false && Check_AlreadyReserve(currentEvent) == false)
            {
                AddDayEvent(currentEvent, priority: 1, embargo: 0, delay: 0);
            }
        }
    }

    void AddDayEventByCondition_Over()
    {
        TryAddUnique(Pop, Danger);

        if (Danger >= 500 && Danger > Pop && CurrentTurn >= 15 && Main.Instance.CurrentEndingState == Endings.Dragon &&
        CurrentClearEventData.Check_AlreadyClear(DialogueName.Dragon_First) == false)
        {
            Managers.Dialogue.ActionReserve(() =>
            {
                Managers.Dialogue.ShowDialogueUI(DialogueName.Dragon_First, Main.Instance.Player);
            });
        }

        if (Danger >= 800 && Danger > Pop && CurrentTurn >= 15 && Main.Instance.CurrentEndingState == Endings.Dragon &&
            CurrentClearEventData.Check_AlreadyClear(DialogueName.Dragon_First) && 
            CurrentClearEventData.Check_AlreadyClear(DialogueName.Dragon_Second) == false &&
            Main.Instance.CurrentEndingState != Endings.Cat)
        {
            Managers.Dialogue.ActionReserve(() =>
            {
                Managers.Dialogue.ShowDialogueUI(DialogueName.Dragon_Second, Main.Instance.Player);
            });
        }


        //? 2회차 이상 / 인기도위험도합이 1200이상일 때 
        if (UserData.Instance.FileConfig.PlayRounds > 1 && Danger + Pop >= 1200 &&
            CurrentClearEventData.Check_AlreadyClear(DayEventLabel.Catastrophe) == false && Check_AlreadyReserve(DayEventLabel.Catastrophe) == false)
        {
            AddDayEvent(DayEventLabel.Catastrophe, priority: 1, embargo: 0, delay: 3);
            Add_GuildQuest_Special(3014); //? 길드원한테 소식듣는 퀘스트 추가 - 이후 1140으로 연결
        }
    }




    void TryAddUnique(int fame, int danger)
    {
        if (fame >= 300)
        {
            GameManager.NPC.Set_UniqueChance(NPC_Type_Unique.Santa, 0.05f);
        }

        if (fame >= 600)
        {
            GameManager.NPC.Set_UniqueChance(NPC_Type_Unique.GoldLizard, 0.05f);
        }

        if (danger >= 300)
        {
            GameManager.NPC.Set_UniqueChance(NPC_Type_Unique.DungeonThief, 0.05f);
        }

        if (danger >= 600)
        {
            GameManager.NPC.Set_UniqueChance(NPC_Type_Unique.PumpkinHead, 0.05f);
        }
    }

    bool TryRankUp(int fame, int danger)
    {
        if (Main.Instance.DungeonRank == 1 && fame + danger >= 100)
        {
            UserData.Instance.FileConfig.Notice_Facility = true;
            UserData.Instance.FileConfig.Notice_Monster = true;
            UserData.Instance.FileConfig.Notice_Summon = true;
            return true;
        }

        if (Main.Instance.DungeonRank == 2 && fame + danger >= 500)
        {
            UserData.Instance.FileConfig.Notice_Facility = true;
            UserData.Instance.FileConfig.Notice_Monster = true;
            UserData.Instance.FileConfig.Notice_Summon = true;
            return true;
        }

        if (Main.Instance.DungeonRank == 3 && fame + danger >= 1200)
        {
            UserData.Instance.FileConfig.Notice_Facility = true;
            UserData.Instance.FileConfig.Notice_Monster = true;
            UserData.Instance.FileConfig.Notice_Summon = true;
            return true;
        }

        if (Main.Instance.DungeonRank == 4 && CurrentClearEventData.Check_AlreadyClear(DialogueName.Catastrophe_Seal))
        {
            UserData.Instance.FileConfig.Notice_Facility = true;
            return true;
        }


        //if (Main.Instance.DungeonRank == (int)Define.DungeonRank.C && danger >= 500 && CurrentClearEventData.Check_AlreadyClear(DialogueName.Dragon_First))
        //{
        //    UserData.Instance.FileConfig.Notice_Facility = true;
        //    UserData.Instance.FileConfig.Notice_Monster = true;
        //    UserData.Instance.FileConfig.Notice_Summon = true;
        //    return true;
        //}

        return false;
    }


    public void RankUpEvent()
    {
        FindObjectOfType<Player>().Player_RankUp(Main.Instance.DungeonRank);
        GameManager.Monster.Resize_MonsterSlot();

        if (Main.Instance.DungeonRank >= (int)Define.DungeonRank.D && Main.Instance.ActiveFloor_Basement < 5)
        {
            UserData.Instance.FileConfig.Notice_DungeonEdit = true;
            UserData.Instance.FileConfig.Notice_Ex4 = true;
        }
        else if (Main.Instance.DungeonRank >= (int)Define.DungeonRank.C && Main.Instance.ActiveFloor_Basement < 6)
        {
            UserData.Instance.FileConfig.Notice_DungeonEdit = true;
            UserData.Instance.FileConfig.Notice_Ex5 = true;
        }
    }

    #endregion


    #region 이벤트 퀘스트 관련 데이터 (세이브파일과 연동됨. UserData.SavefileConfig같은거)
    public ClearEventData CurrentClearEventData { get; set; }
    public class ClearEventData
    {
        public HashSet<DayEventLabel> Clear_DayEventList;
        public HashSet<DialogueName> Clear_DialogueEventList;
        public HashSet<int> Clear_QuestList;


        public ClearEventData()
        {
            Clear_DayEventList = new HashSet<DayEventLabel>();
            Clear_DialogueEventList = new HashSet<DialogueName>();
            Clear_QuestList = new HashSet<int>();
        }

        public ClearEventData DeepCopy()
        {
            var data = new ClearEventData();
            data.Clear_DayEventList = new HashSet<DayEventLabel>(Clear_DayEventList);
            data.Clear_DialogueEventList = new HashSet<DialogueName>(Clear_DialogueEventList);
            data.Clear_QuestList = new HashSet<int>(Clear_QuestList);
            return data;
        }


        public void AddClear(DayEventLabel _DayEventName)
        {
            Clear_DayEventList.Add(_DayEventName);
        }
        public void AddClear(DialogueName _EventName)
        {
            Clear_DialogueEventList.Add(_EventName);
        }
        public void AddClear_Quest(int _EventName)
        {
            Clear_QuestList.Add(_EventName);
        }



        public bool Check_AlreadyClear(DayEventLabel _DayEventName)
        {
            foreach (var item in Clear_DayEventList)
            {
                if (item == _DayEventName)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Check_AlreadyClear(DialogueName _EventName)
        {
            foreach (var item in Clear_DialogueEventList)
            {
                if (item == _EventName)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Check_AlreadyClear(int _EventName)
        {
            foreach (var item in Clear_DialogueEventList)
            {
                if ((int)item == _EventName)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Check_AlreadyClear_Quest(int _EventName)
        {
            foreach (var item in Clear_QuestList)
            {
                if (item == _EventName)
                {
                    return true;
                }
            }
            return false;
        }




        public string ShowClearData()
        {
            string data = "";
            foreach (var item in Clear_DayEventList)
            {
                data += $"{item.ToString()}\n";
            }
            foreach (var item in Clear_DialogueEventList)
            {
                data += $"{item.ToString()}\n";
            }
            foreach (var item in Clear_QuestList)
            {
                data += $"{item.ToString()}\n";
            }
            return data;
        }
    }



    #endregion


    #region EventDataSave & Load
    public DataManager.SaveData_EventData Data_SaveEventManager()
    {
        DataManager.SaveData_EventData save = new DataManager.SaveData_EventData();

        if (CurrentGuildData != null)
        {
            save.CurrentGuildData = new List<GuildNPC_Data>();
            foreach (var item in CurrentGuildData)
            {
                save.CurrentGuildData.Add(item.DeepCopy());
            }
        }

        if (AddQuest_Daily != null)
        {
            save.AddQuest_Daily = new List<int>(AddQuest_Daily);
        }

        if (CurrentQuestAction_forSave != null)
        {
            save.CurrentQuestAction_forSave = new List<int>(CurrentQuestAction_forSave);
        }

        if (DayEventList != null)
        {
            save.DayEventList = new List<DayEvent>();
            foreach (var item in DayEventList)
            {
                save.DayEventList.Add(item.DeepCopy());
            }
        }

        if (Reservation_Quest != null)
        {
            save.Reservation_Quest = new List<Quest_Reservation>();
            foreach (var item in Reservation_Quest)
            {
                save.Reservation_Quest.Add(item.DeepCopy());
            }
        }

        if (CurrentClearEventData != null)
        {
            save.CurrentClearEventData = CurrentClearEventData.DeepCopy();
        }

        return save;
    }

    public void Data_LoadEventManager(DataManager.SaveData LoadData)
    {
        if (LoadData.eventData == null) return;

        QuestDataReset();

        CurrentTurn = LoadData.mainData.turn;

        if (LoadData.eventData.CurrentGuildData != null)
        {
            var newData = new List<GuildNPC_Data>();
            foreach (var item in LoadData.eventData.CurrentGuildData)
            {
                newData.Add(item.DeepCopy());
            }
            CurrentGuildData = newData;
        }

        if (LoadData.eventData.AddQuest_Daily != null)
        {
            var newData = new List<int>();
            foreach (var item in LoadData.eventData.AddQuest_Daily)
            {
                newData.Add(item);
            }
            AddQuest_Daily = newData;
        }

        if (LoadData.eventData.CurrentQuestAction_forSave != null)
        {
            foreach (var item in LoadData.eventData.CurrentQuestAction_forSave)
            {
                AddQuestAction(item);
            }
        }

        if (LoadData.eventData.DayEventList != null)
        {
            //DayEventList = new List<DayEvent>(LoadData.eventData.DayEventList);
            var newData = new List<DayEvent>();
            foreach (var item in LoadData.eventData.DayEventList)
            {
                newData.Add(item.DeepCopy());
            }
            DayEventList = newData;
        }

        if (LoadData.eventData.Reservation_Quest != null)
        {
            //Reservation_Quest = new List<Quest_Reservation>(LoadData.eventData.Reservation_Quest);
            var newData = new List<Quest_Reservation>();
            foreach (var item in LoadData.eventData.Reservation_Quest)
            {
                newData.Add(item.DeepCopy());
            }
            Reservation_Quest = newData;
        }

        if (LoadData.eventData.CurrentClearEventData != null)
        {
            CurrentClearEventData = LoadData.eventData.CurrentClearEventData.DeepCopy();
        }
    }
    #endregion


    #region 길드 방문 알림 표시
    public bool CheckGuildNotice()
    {
        var guildData = CurrentGuildData;
        if (CurrentGuildData == null)
        {
            Debug.Log("현재 길드 데이터 없음");
            return false;
        }

        //? 활성화 된 길드원의 메인퀘스트가 있을경우
        foreach (var item in guildData)
        {
            //? 퀘스트 없으면 넘기고
            if (item.InstanceQuestList.Count == 0)
            {
                continue;
            }

            //? 제외목록이면 넘기고
            if (GuildManager.Instance.Delete_GuildNPC.Contains((GuildNPC_LabelName)item.Original_Index))
            {
                continue;
            }

            //? 무조건 포함되는 NPC면 넣고
            if (GuildManager.Instance.Instance_GuildNPC.Contains((GuildNPC_LabelName)item.Original_Index))
            {
                Debug.Log($"길드 퀘스트 발생중 : {item.InstanceQuestList[0]}");
                return true;
            }

            switch (GuildManager.Instance.GetData(item.Original_Index).DayOption)
            {
                case Guild_DayOption.Special:
                    break;

                case Guild_DayOption.Always:
                    if (GuildManager.Instance.GetData(item.Original_Index).FirstDay <= CurrentTurn)
                    {
                        Debug.Log($"길드 퀘스트 발생중 : {item.Original_Index}");
                        return true;
                    }
                    break;

                case Guild_DayOption.Odd:
                    if (Calc_Turn_Check(item, 2, 1)) return true;
                    break;

                case Guild_DayOption.Even:
                    if (Calc_Turn_Check(item, 2, 0)) return true;
                    break;

                case Guild_DayOption.Multiple_3:
                    if (Calc_Turn_Check(item, 3, 0)) return true;
                    break;

                case Guild_DayOption.Multiple_4:
                    if (Calc_Turn_Check(item, 4, 0)) return true;
                    break;

                case Guild_DayOption.Multiple_5:
                    if (Calc_Turn_Check(item, 5, 0)) return true;
                    break;

                case Guild_DayOption.Multiple_7:
                    if (Calc_Turn_Check(item, 7, 0)) return true;
                    break;
            }
        }

        //? 길드원 옵션 퀘스트가 2개이상일 때 길드알림
        foreach (var item in CurrentGuildData)
        {
            if ((GuildNPC_LabelName)item.Original_Index == GuildNPC_LabelName.StaffA)
            {
                if (item.OptionList.Count >= 2)
                {
                    return true;
                }
            }
        }

        return false;
    }


    bool Calc_Turn_Check(GuildNPC_Data data, int day, int conditionInt)
    {
        if (CurrentTurn % day == conditionInt)
        {
            Debug.Log($"길드 퀘스트 발생중 : {data.Original_Index + data.InstanceQuestList[0]}");
            return true;
        }
        else
        {
            return false;
        }
    }


    #endregion


    #region DayEventPriorityQueue

    Dictionary<DayEventLabel, Action> DayEventActionRegister = new Dictionary<DayEventLabel, Action>();

    public class DayEvent
    {
        public DayEventLabel EventIndex;

        //? 퀘스트의 우선순위. 숫자가 낮을수록 우선순위가 높음
        public int Priority;

        //? 발동대기날짜 (여러방법이 있음. 최소 3일 후에 발생하는 이벤트 or 최소 턴이상에 발동하는 이벤트)
        //? Embargo는 턴(날짜)제한 - 최소 턴이 이 숫자 이상일 때 이벤트가 발생
        public int Embargo;
        //? Delay는 등록되고난다음부터 매일 1씩 줄어드는데 최소 딜레이를 주기 위함이고 0보다 작을때만 가능
        public int Delay;

        public DayEvent()
        {

        }
        public DayEvent(DayEventLabel index, int priority, int embargo, int delay)
        {
            EventIndex = index;
            Priority = priority;
            Embargo = embargo;
            Delay = delay;
        }

        public DayEvent DeepCopy()
        {
            var data = new DayEvent(EventIndex, Priority, Embargo, Delay);
            return data;
        }
    }

    public List<DayEvent> DayEventList { get; set; } = new List<DayEvent>();
    bool Check_AlreadyReserve(DayEventLabel _eventName)
    {
        foreach (var item in DayEventList)
        {
            if (item.EventIndex == _eventName)
            {
                return true;
            }
        }
        return false;
    }

    void OneDayAfter()
    {
        foreach (var item in DayEventList)
        {
            item.Delay--;
        }
    }
    DayEvent GetDayEvent()
    {
        List<DayEvent> CurrentAbleEvent = new List<DayEvent>();

        if (DayEventList.Count == 0) //? 등록된 이벤트가 없으면 Null
        {
            return null;
        }
        else
        {
            foreach (var item in DayEventList)
            {
                if (item.Delay <= 0 && item.Embargo <= CurrentTurn)
                {
                    CurrentAbleEvent.Add(item);
                }
            }
        }


        if (CurrentAbleEvent.Count == 0) //? 등록된 이벤트중에 지금 당장 가능한 이벤트가 없으면 Null
        {
            return null;
        }
        else
        {
            DayEvent temp = CurrentAbleEvent[0];
            foreach (var item in CurrentAbleEvent)
            {
                if (item.Priority < temp.Priority) //? 만약 우선순위가 더 낮은 급한 퀘스트가 있으면 교체
                {
                    temp = item;
                }
            }

            DayEventList.Remove(temp);
            return temp;
        }
    }



    public void AddDayEvent(DayEventLabel eventName, int priority, int embargo, int delay)
    {
        DayEventList.Add(new DayEvent(eventName, priority, embargo, delay));
    }

    void RegistDayEvent()
    {
        DayEventActionRegister.Add(DayEventLabel.RetiredHero, () => {
            Debug.Log("은퇴한 영웅 이벤트");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_RetiredHero.ToString(), 9, NPC_Typeof.NPC_Type_MainEvent);
        });

        DayEventActionRegister.Add(DayEventLabel.Goblin_Appear, () => {
            Debug.Log("고블린 첫등장 이벤트");
            GameManager.NPC.Set_UniqueChance(NPC_Type_Unique.ManaGoblin, 0.05f);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin_Leader.ToString(), 9, NPC_Typeof.NPC_Type_MainEvent);
        });

        DayEventActionRegister.Add(DayEventLabel.Goblin_Party, () => {
            Debug.Log("고블린 파티 이벤트");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin_Leader.ToString(), 3, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 3.5f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 4, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 4.5f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 5, NPC_Typeof.NPC_Type_MainEvent);
        });

        DayEventActionRegister.Add(DayEventLabel.Orc_Party, () => {
            Debug.Log("오크 파티 이벤트");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Orc.ToString(), 3, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Orc.ToString(), 3.5f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Orc.ToString(), 4, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Orc.ToString(), 4.5f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Orc.ToString(), 5, NPC_Typeof.NPC_Type_MainEvent);
        });

        DayEventActionRegister.Add(DayEventLabel.Catastrophe, () => {
            Debug.Log("던전의 재앙 이벤트");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Catastrophe.ToString(), 10, NPC_Typeof.NPC_Type_MainEvent);
        });

        DayEventActionRegister.Add(DayEventLabel.BloodSong_Appear, () => {
            Debug.Log("Blood Song 이벤트");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Tanker_A.ToString(), 10f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Warrior_A.ToString(), 10.5f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Wizard_A.ToString(), 11f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Elf_A.ToString(), 11.5f, NPC_Typeof.NPC_Type_MainEvent);
        });


        DayEventActionRegister.Add(DayEventLabel.Guild_Raid_1, () => {
            Debug.Log("길드 토벌대 1 이벤트");
            Guild_Raid_First();
            ClearQuestAction(777010);
            Clear_GuildQuest(7010);
        });

        DayEventActionRegister.Add(DayEventLabel.Guild_Raid_2, () => {
            Debug.Log("길드 토벌대 2 이벤트");
            Guild_Raid_Second();
            ClearQuestAction(777020);
            Clear_GuildQuest(7020);
        });

        DayEventActionRegister.Add(DayEventLabel.Forest_Raid_1, () => {
            Debug.Log("숲레이드 1 이벤트");
            Forest_Raid_1();
            ClearQuestAction(778010);
            Clear_GuildQuest(8010);
        });

        DayEventActionRegister.Add(DayEventLabel.Forest_Raid_2, () => {
            Debug.Log("숲레이드 2 이벤트");
            Forest_Raid_2();
            ClearQuestAction(778020);
            Clear_GuildQuest(8020);
        });

        //DayEventActionRegister.Add(DayEventLabel.Last_Judgment, () => {
        //    Debug.Log("마왕엔딩 최후결전");
        //    Last_Judgment();
        //    ClearQuestAction(7710003);
        //});
    }


    void Run_DayEventAction(DayEventLabel dayEventName)
    {
        Action act = null;
        if (DayEventActionRegister.TryGetValue(dayEventName, out act))
        {
            act.Invoke();
            CurrentClearEventData.AddClear(dayEventName);
        }
    }

    #endregion


    #region 모든 이벤트 액션 관련 (길드 포함)
    public int CurrentTurn { get; set; }

    //? 길드정보 저장용 - 모든 길드관련 데이터를 가지고있고 씬 변경시에도 사라지지않음. 저장과 불러오기에도 동일하게 사용
    public List<GuildNPC_Data> CurrentGuildData { get; set; }

    public void Add_GuildQuest_Special(int index, bool special = true)
    {
        int id = (index / 1000) * 1000;
        int questIndex = index - id;

        foreach (var item in CurrentGuildData)
        {
            if (item.Original_Index == id)
            {
                item.AddQuest(questIndex, special);
            }
        }
    }

    public void Remove_GuildQuest(int index)
    {
        int id = (index / 1000) * 1000;
        int questIndex = index - id;

        foreach (var item in CurrentGuildData)
        {
            if (item.Original_Index == id)
            {
                item.RemoveQuest(questIndex);
            }
        }
    }

    public void Clear_GuildQuest(int index)
    {
        int id = (index / 1000) * 1000;
        int questIndex = index - id;

        foreach (var item in CurrentGuildData)
        {
            if (item.Original_Index == id)
            {
                item.ClearQuest(questIndex);
            }
        }
    }


    //? 같은날 길드를 또간다고 퀘스트가 추가되면 안되기 때문에 날짜 예약하기
    public List<Quest_Reservation> Reservation_Quest { get; set; } = new List<Quest_Reservation>();
    public void ReservationToQuest(int day, int questIndex)
    {
        Reservation_Quest.Add(new Quest_Reservation(day, questIndex));
    }

    public void Add_ReservationGuildQuest() //? 매일 턴이 바뀔때마다
    {
        var removeList = new List<Quest_Reservation>();
        foreach (var item in Reservation_Quest)
        {
            item.days--;
            if (item.days <= 0)
            {
                Add_GuildQuest_Special(item.questIndex);
                removeList.Add(item);
            }
        }

        foreach (var item in removeList)
        {
            Reservation_Quest.Remove(item);
        }
    }


    public class Quest_Reservation
    {
        public int days;
        public int questIndex;

        public Quest_Reservation()
        {

        }

        public Quest_Reservation(int day, int index)
        {
            days = day;
            questIndex = index;
        }

        public Quest_Reservation DeepCopy()
        {
            var data = new Quest_Reservation(days, questIndex);
            return data;
        }
    }


    //? 길드가면 추가시켜야 될 퀘스트 리스트 - 인기도 올리는 것 같이 매일 초기화 되는 항목 (얘는 퀘스트 발생 알림이 없음)
    public List<int> AddQuest_Daily { get; set; } = new List<int>();

    public void Add_Daily(int index)
    {
        AddQuest_Daily.Add(index);
        AddQuest_Daily = Util.ListDistinct(AddQuest_Daily);
    }

    //? 현재 진행중인 퀘스트 목록 - 실제 매턴 실행될 Action
    public Action CurrentQuestAction { get; private set; }

    //? 현재 진행중인 퀘스트 목록(dataManager에서 저장용으로만 사용)
    public HashSet<int> CurrentQuestAction_forSave { get; set; } = new HashSet<int>();

    
    //? 이걸 커스텀스테이지에서도 호출하고 있어서 두마리나오는 버그가 생겼음 / NPC Manager에서 해야함
    public void CurrentQuestInvoke()
    {
        CurrentQuestAction?.Invoke();
    }



    public void AddQuestAction(int _index)
    {
        if (!CurrentQuestAction_forSave.Contains(_index))
        {
            CurrentQuestAction += GetQuestAction(_index);
            CurrentQuestAction_forSave.Add(_index);
            UserData.Instance.FileConfig.Notice_Quest = true;
        }

        //CurrentQuestAction += GetQuestAction(_index);
        //CurrentQuestAction_forSave.Add(_index);

        //UserData.Instance.FileConfig.Notice_Quest = true;
    }
    public void ClearQuestAction(int _index)
    {
        CurrentClearEventData.AddClear_Quest(_index);
        RemoveQuestAction(_index);
    }
    public void RemoveQuestAction(int _index)
    {
        if (CurrentQuestAction_forSave.Contains(_index))
        {
            CurrentQuestAction -= GetQuestAction(_index);
            CurrentQuestAction_forSave.Remove(_index);
            if (CurrentQuestAction_forSave.Count == 0)
            {
                UserData.Instance.FileConfig.Notice_Quest = false;
            }
        }
    }




    //? 길드에서 대화로 실행할 Action
    Dictionary<int, Action> GuildNPCAction = new Dictionary<int, Action>();

    //? 기타 대화가 종료되고 바로 실행할 Action
    Dictionary<string, Action> EventAction = new Dictionary<string, Action>();

    //? 단순히 CurrentQuestEvent에 add / remove 하는 용도로만 사용되어야함. 또 GuildAction과 중복되서 사용될 수도, 독립적으로 사용될 수도 있음.
    Dictionary<int, Action> forQuestAction = new Dictionary<int, Action>();

    void Init_ForQuestAction()  //? 실제로 호출할 액션
    {

        forQuestAction.Add(1100, () =>
        {
            Debug.Log("퀘스트 - 슬라임토벌 활성화");
            GameManager.NPC.AddEventNPC(NPC_Type_Hunter.Hunter_Slime.ToString(), 12, NPC_Typeof.NPC_Type_Hunter);
        });

        forQuestAction.Add(1101, () =>
        {
            Debug.Log("퀘스트 - 어스골렘 활성화");
            GameManager.NPC.AddEventNPC(NPC_Type_Hunter.Hunter_EarthGolem.ToString(), 13, NPC_Typeof.NPC_Type_Hunter);
        });


        forQuestAction.Add(1140, () =>
        {
            Debug.Log("던전의 재앙 대기중");
        });

        forQuestAction.Add(1141, () =>
        {
            Debug.Log("지속되는 재앙");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Catastrophe.ToString(), 10, NPC_Typeof.NPC_Type_MainEvent);
        });

        forQuestAction.Add(771141, () =>
        {
            Debug.Log("해결의 실마리");
        });

        forQuestAction.Add(1150, () =>
        {
            Debug.Log("고블린 파티 방문 대기중");
        });

        forQuestAction.Add(1151, () =>
        {
            Debug.Log("은퇴한 영웅 방문 대기중");
        });




        //? 퀘스트 등록용
        forQuestAction.Add(772102, () =>
        {
            Debug.Log("약초 직업류 방문확률 3배!!");
            GameManager.NPC.Event_Herb = true;
        });
        forQuestAction.Add(772103, () =>
        {
            Debug.Log("광물 직업류 방문확률 3배!!");
            GameManager.NPC.Event_Mineral = true;
        });

        forQuestAction.Add(774020, () =>
        {
            GameManager.NPC.AddEventNPC(NPC_Type_SubEvent.Heroine.ToString(), 7, NPC_Typeof.NPC_Type_SubEvent);
        });
        forQuestAction.Add(774030, () =>
        {
            GameManager.NPC.AddEventNPC(NPC_Type_SubEvent.Heroine.ToString(), 7, NPC_Typeof.NPC_Type_SubEvent);
        });

        forQuestAction.Add(777010, () =>
        {
            Debug.Log("1차 길드레이드 준비중");
        });

        forQuestAction.Add(777020, () =>
        {
            Debug.Log("2차 길드레이드 준비중");
        });

        forQuestAction.Add(778010, () =>
        {
            Debug.Log("1차 숲의습격 준비중");
        });

        forQuestAction.Add(778020, () =>
        {
            Debug.Log("2차 숲의습격 준비중");
        });

        forQuestAction.Add(7710003, () =>
        {
            Debug.Log("최후의 심판 준비중");
        });

        forQuestAction.Add(7712000, () =>
        {
            Debug.Log("라이트닝");
            GameManager.NPC.AddEventNPC(NPC_Type_SubEvent.Lightning.ToString(), 3, NPC_Typeof.NPC_Type_SubEvent);
        });
    }
    void Init_DialogueAction() //? 대화를 통해서 호출하는곳. 코드상에는 없고 Dialogue에 Index로만 존재함
    {
        GuildNPCAction.Add(2100, () =>
        {
            Managers.Dialogue.ActionReserve(() => 
            {
                int ranPop = UnityEngine.Random.Range(10, 20 + CurrentTurn);
                var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
                msg.Message = $"{ranPop} {UserData.Instance.LocaleText("Message_Get_Pop")}";

                Managers.Data.GetData("Temp_GuildSave").mainData.FameOfDungeon += ranPop;
                //GuildManager.Instance.AddBackAction(() =>
                //{
                //    Main.Instance.CurrentDay.AddPop(ranPop);
                //    //Debug.Log($"던전의 인기도가 {ranPop} 올랐습니다.");
                //});
            });
        });

        GuildNPCAction.Add(2102, () => { AddQuestAction(772102); });
        GuildNPCAction.Add(2103, () => { AddQuestAction(772103); });



        GuildNPCAction.Add(1100, () => { AddQuestAction(1100); });
        GuildNPCAction.Add(1101, () => { AddQuestAction(1101); });
        GuildNPCAction.Add(1140, () => { AddQuestAction(1140); });
        //GuildNPCAction.Add(1151, () => { AddQuestAction(1151); });


        GuildNPCAction.Add(4040, () =>
        {
            GuildManager.Instance.AddBackAction(() =>
            {
                Debug.Log("히로인 합류");
                GameManager.Monster.Resize_AddOne();
                var mon = GameManager.Monster.CreateMonster("Heroine", false, true);
                GuildManager.Instance.AddDeleteGuildNPC(GuildNPC_LabelName.Heroine);
            });
        });

        GuildNPCAction.Add(7010, () => { AddQuestAction(777010); });
        GuildNPCAction.Add(7020, () => { AddQuestAction(777020); });
        GuildNPCAction.Add(8010, () => { AddQuestAction(778010); });
        GuildNPCAction.Add(8020, () => { AddQuestAction(778020); });

        //? 라이트닝 레이서
        GuildNPCAction.Add(12010, () =>
        {
            AddQuestAction(7712000);
            GuildManager.Instance.GetInteraction(12000).Remove_Option(0); //? 확장될 때 마다 추가되는 퀘스트라서 여기서 삭제해줘야함(선택지없으니까)
        });


        //? 그레이하운드 진화 퀘스트 추가
        GuildNPCAction.Add(100701, () => { Add_GuildQuest_Special(5010); });
        //? 그레이하운드 관련 NPC 퀘스트 추가(뼈 구매하는거)
        GuildNPCAction.Add(5010, () => { Add_GuildQuest_Special(5199, false); Add_GuildQuest_Special(5299, false); });
    }

    //? 모든 Main Event를 포함해야함
    void Init_EventAction()
    {
        EventAction.Add("GameOver", () => {
            Managers.UI.ClearAndShowPopUp<UI_GameOver>();
        });


        EventAction.Add("DenySkip", () => {
            Managers.Dialogue.AllowPerfectSkip = false;
        });

        EventAction.Add("AllowSkip", () => {
            Managers.Dialogue.AllowPerfectSkip = true;
        });




        EventAction.Add("Dialogue_Close", () => {
            Managers.Dialogue.Close_CurrentDialogue();
        });


        EventAction.Add("Tutorial_Orb", () => {
            Transform child = Main.Instance.Player.GetComponentInChildren<SpriteRenderer>().transform;
            child.localScale = new Vector3(-1, 1, 1);

            Camera.main.GetComponent<CameraControl>().ChasingTarget(Main.Instance.EggObj.transform, 2);
        });

        EventAction.Add("Tutorial_Orb_Over", () => {
            Transform child = Main.Instance.Player.GetComponentInChildren<SpriteRenderer>().transform;
            child.localScale = Vector3.one;
        });

        EventAction.Add("Tutorial_RandomEvent", () => {
            Camera.main.GetComponent<CameraControl>().ChasingTarget(Main.Instance.Dungeon.position, 1.0f);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_Tutorial_RandomEvent");
        });

        



        EventAction.Add("Player_FlipX_True", () =>
        {
            Main.Instance.Player.GetComponentInChildren<SpriteRenderer>().flipX = true;
            Camera.main.GetComponent<CameraControl>().ChasingTarget(Main.Instance.Player.position, 2);

        });

        EventAction.Add("Player_FlipX_False", () =>
        {
            Main.Instance.Player.GetComponentInChildren<SpriteRenderer>().flipX = false;
            Camera.main.GetComponent<CameraControl>().ChasingTarget(Main.Instance.Player.position, 1);
        });

        EventAction.Add("Player_Guild_PosReset", () =>
        {
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.GetComponentInChildren<SpriteRenderer>().flipX = false;
            player.GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Exit).position;

            FindAnyObjectByType<UI_DialogueBubble>().Bubble_MoveToTarget(player.transform);
        });



        EventAction.Add("EggMessage", () =>
        {
            var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
            message.DelayTime = 2;
            message.Message = UserData.Instance.LocaleText("Message_Egg");
        });

        EventAction.Add("FirstEggApprear", () =>
        {
            StartCoroutine(FirstPortalAppear());
        });

        EventAction.Add("Entrance_Move_2to4", () =>
        {
            StartCoroutine(EntranceMove_3to4());
        });



        //? Diglogue를 통해 호출
        EventAction.Add("Racing_Certificate", () =>
        {
            GuildManager.Instance.AddBackAction(() =>
            {
                GameManager.Artifact.AddArtifact(ArtifactLabel.Racing);

                var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
                message.DelayTime = 2;
                //? 신규 아티팩트
                message.Message = $"{UserData.Instance.LocaleText("New")}{UserData.Instance.LocaleText("아티팩트")} : " +
                $"{GameManager.Artifact.GetData("Racing").labelName}";
            });
        });




        EventAction.Add("RedHair_Defeat", () =>
        {
            Debug.Log("RedHair - RetiredHero 이벤트 연계");
            AddDayEvent(DayEventLabel.RetiredHero, priority: 0, embargo: 12, delay: 0);
            GuildManager.Instance.AddInstanceGuildNPC(GuildNPC_LabelName.RetiredHero);

            ReservationToQuest(1, 15010); 
            //Add_GuildQuest_Special(15010);

            var e8 = GameManager.Placement.Find_Placement(NPC_Type_MainEvent.EM_RedHair);
            if (e8 != null)
            {
                GameManager.Placement.Disable(e8.GetComponent<NPC_MainEvent>());
                var fade = Managers.UI.ShowPopUp<UI_Fade>();
                fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 2, true);
            }
        });

        EventAction.Add("Hero_Die", () =>
        {
            Debug.Log("영웅 패배 이벤트");
            var e8 = GameManager.Placement.Find_Placement(NPC_Type_MainEvent.EM_RetiredHero);
            if (e8 != null)
            {
                GameManager.Placement.Disable(e8.GetComponent<NPC_MainEvent>());
                var fade = Managers.UI.ShowPopUp<UI_Fade>();
                fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 2, true);
            }
        });

        EventAction.Add("Hero_Quest_1", () =>
        {
            AddQuestAction(1151);
            Debug.Log("영웅 길드 대화");
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var hero = GameObject.Find("RetiredHero");
            hero.transform.localScale = new Vector3(-1, 1, 1);
            var player = GameObject.Find("Player");
            player.GetComponentInChildren<SpriteRenderer>().transform.localScale = new Vector3(-1, 1, 1);
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Hero).position;

            FindAnyObjectByType<UI_DialogueBubble>().Bubble_MoveToTarget(player.transform);
        });

        EventAction.Add("Hero_Quest_2", () =>
        {
            Debug.Log("영웅 길드 대화2");
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var hero = GameObject.Find("RetiredHero");
            hero.transform.localScale = new Vector3(1, 1, 1);
            var player = GameObject.Find("Player");
            player.GetComponentInChildren<SpriteRenderer>().transform.localScale = new Vector3(1, 1, 1);
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Exit).position;

            FindAnyObjectByType<UI_DialogueBubble>().Bubble_MoveToTarget(player.transform);
        });

        EventAction.Add("Heroine_Quest_2", () =>
        {
            AddQuestAction(774020);
            Debug.Log("히로인 대화2");
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            player.GetComponent<Animator>().Play(Define.ANIM_Idle_Sit);
            //player.GetComponentInChildren<SpriteRenderer>().flipX = true;
            player.GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;

            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Table2).position;
            //FindAnyObjectByType<UI_DialogueBubble>().Bubble_MoveToTarget(player.transform);
        });

        EventAction.Add("Heroine_EndDialogue", () =>
        {
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.GetComponent<Animator>().Play(Define.ANIM_Idle);
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Table2).position + Vector3.up;
        });


        EventAction.Add("Heroine_Quest_3", () =>
        {
            Debug.Log("히로인 대화3");
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Exit).position;
            FindAnyObjectByType<UI_DialogueBubble>().Bubble_MoveToTarget(player.transform);

            ClearQuestAction(774020);
            AddQuestAction(774030);
        });

        EventAction.Add("Heroine_Quest_Prison", () =>
        {
            Debug.Log("히로인 붙잡힘");
            Add_GuildQuest_Special(4040, true);
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 2, true);

            var player = Main.Instance.Player;
            GameObject heroine = Managers.Resource.Instantiate("Monster/Heroine");
            heroine.GetComponent<Heroine>().enabled = false;
            heroine.GetComponentInChildren<SpriteRenderer>().flipX = true;
            heroine.GetComponentInChildren<SpriteRenderer>().sortingOrder = -1;
            Transform prison = GameManager.Technical.Prison.transform;

            player.position = prison.position + new Vector3(-1, -1, 0);
            heroine.transform.position = prison.position + new Vector3(0.5f, 0, 0);

            Camera.main.GetComponent<CameraControl>().ChasingTarget(prison.position, 2);
            FindAnyObjectByType<UI_DialogueBubble>().Bubble_MoveToTarget(heroine.transform);
        });

        EventAction.Add("Heroine_Quest_Prison2", () =>
        {
            Debug.Log("주인공 독백");
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            Managers.Resource.Destroy(GameObject.Find("Heroine"));

            var player = Main.Instance.Player;
            player.position = player.GetComponent<IPlacementable>().PlacementInfo.Place_Tile.worldPosition;

            Camera.main.GetComponent<CameraControl>().ChasingTarget(player.position, 2);
            FindAnyObjectByType<UI_DialogueBubble>().Bubble_MoveToTarget(player.transform);
        });

        EventAction.Add("Heroine_Join", () =>
        {
            Debug.Log("히로인 동료");

            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            player.GetComponent<Animator>().Play(Define.ANIM_Idle_Sit);

            player.GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;

            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Table2).position;
            //FindAnyObjectByType<UI_DialogueBubble>().Bubble_MoveToTarget(player.transform);
        });

        EventAction.Add("Heroine_Ending", () =>
        {
            Debug.Log("히로인 엔딩 루트 확정");

            GameManager.Monster.GetMonster<Heroine>().UnitDialogueEvent.ClearEvent((int)UnitDialogueEventLabel.Heroin_Root_Ture);
            Main.Instance.ChangeEggState();
        });

        EventAction.Add("Peddler_Join", () =>
        {
            Debug.Log("치킨 동료 대화");

            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            player.GetComponent<Animator>().Play(Define.ANIM_Idle);
            player.GetComponentInChildren<SpriteRenderer>().flipX = true;
            player.GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Center_Right).position;

            var peddler = GameObject.Find("Peddler");
            peddler.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Center_Left).position;

            FindAnyObjectByType<UI_DialogueBubble>().Bubble_MoveToTarget(peddler.transform);


            GuildManager.Instance.AddBackAction(() =>
            {
                Debug.Log("치킨 합류");
                GameManager.Monster.Resize_AddOne();
                var mon = GameManager.Monster.CreateMonster("Utori", false, true);
                GuildManager.Instance.AddDeleteGuildNPC(GuildNPC_LabelName.Peddler);
            });
        });

        EventAction.Add("Peddler_Over", () =>
        {
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.GetComponentInChildren<SpriteRenderer>().flipX = false;

            var peddler = GameObject.Find("Peddler");
            peddler.SetActive(false);
        });




        EventAction.Add("RedHair_Return", () =>
        {
            Debug.Log("RedHair_Return - Goblin 연계");
            AddDayEvent(DayEventLabel.Goblin_Appear, priority: 0, embargo: 10, delay: 0);
        });

        EventAction.Add("Goblin_Satisfiction", () =>
        {
            Debug.Log("고블린 만족 - 고블린 파티 이벤트 연계");
            AddDayEvent(DayEventLabel.Goblin_Party, priority: 0, embargo: 14, delay: 0);
            AddQuestAction(1150); //? 고블린파티 바로 퀘스트에 추가

            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 2, true);
        });

        EventAction.Add("Goblin_Pass", () =>
        {
            Debug.Log("고블린 Die or Empty - 오크파티");
            AddDayEvent(DayEventLabel.Orc_Party, priority: 0, embargo: 14, delay: 0);


            //AddDayEvent(DayEventLabel.Catastrophe, priority: 0, embargo: 12, delay: 0);
            //Add_GuildQuest_Special(3014); //? 길드원한테 소식듣는 퀘스트 추가 - 이후 1140으로 연결
        });

        EventAction.Add("Orc_Leader_Left", () =>
        {
            var orc = GameObject.Find("Event_Orc_Leader");
            orc.GetComponent<NPC>().Anim_State = NPC.animState.left;
            orc.GetComponent<NPC>().Anim_State = NPC.animState.Ready;
            orc.GetComponent<Animator>().Play(Define.ANIM_Attack);
        });

        EventAction.Add("Orc_Leader_Right", () =>
        {
            var orc = GameObject.Find("Event_Orc_Leader");
            orc.GetComponent<NPC>().Anim_State = NPC.animState.right;
            orc.GetComponent<NPC>().Anim_State = NPC.animState.Ready;
        });



        EventAction.Add("Catastrophe_Refeat", () =>
        {
            Debug.Log("던전의 재앙 - 해결할 때 까지 지속 이벤트");
            ClearQuestAction(1140);
            AddQuestAction(1141); //? 해결되기전까지 1141은 지속발생되는 이벤트(퀘스트헌터마냥)

            GuildManager.Instance.AddInstanceGuildNPC(GuildNPC_LabelName.DeathMagician);
            Add_GuildQuest_Special(10001, true);
        });

        EventAction.Add("DeathMagician_Catastrophe", () =>
        {
            Debug.Log("던전의 재앙 - 데스매지션 - 실마리 제공");
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.GetComponentInChildren<SpriteRenderer>().flipX = true;
            player.GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.DeathMagician).position;

            AddQuestAction(771141);
            GuildManager.Instance.RemoveInstanceGuildNPC(GuildNPC_LabelName.DeathMagician);
        });

        EventAction.Add("Catastrophe_Sealing", () =>
        {
            Debug.Log("던전의 재앙 - 봉인");

            GameManager.Technical.Get_Technical<BarrierOfSealing>().Set_Seal();
            GameManager.Technical.Get_Technical<BarrierOfSealing>().AddCollectionPoint();
        });


        EventAction.Add("DeathMagician_DevilStatue", () =>
        {
            Debug.Log("데스매지션 - 주인공 마왕 루트");
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.GetComponentInChildren<SpriteRenderer>().flipX = true;
            player.GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.DeathMagician).position;
            GuildManager.Instance.RemoveInstanceGuildNPC(GuildNPC_LabelName.DeathMagician);
        });

        EventAction.Add("DevilStatue_5", () =>
        {
            Debug.Log("주인공 마왕 루트 확정 / 아티팩트 받고 용사 이벤트 출현");
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.GetComponentInChildren<SpriteRenderer>().flipX = true;
            player.GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.DeathMagician).position;
            GuildManager.Instance.RemoveInstanceGuildNPC(GuildNPC_LabelName.DeathMagician);


            //? 아티팩트 추가
            GuildManager.Instance.AddBackAction(() =>
            {
                GameManager.Artifact.AddArtifact(ArtifactLabel.TouchOfDecay);
            });
            //? 용사이벤트 추가 (퀘스트에 등록될 예비이벤트임)
            AddQuestAction(7710003);
        });



        EventAction.Add("Dog_Artifact", () =>
        {
            Debug.Log("신비한 뼈 구입");

            var saveData = Managers.Data.GetData("Temp_GuildSave");

            if (saveData.mainData.Player_Gold < 800) //? 골드가 부족하다면 시스템 메세지 혹은 돈부족 대화
            {
                Managers.Dialogue.ShowDialogueUI(5555);
                return;
            }

            saveData.mainData.Player_Gold -= 800;
            GuildManager.Instance.AddBackAction(() =>
            {
                GameManager.Artifact.AddArtifact(ArtifactLabel.BananaBone);
            });
            Clear_GuildQuest(5199);
            Clear_GuildQuest(5299);
        });



        EventAction.Add("RandomArtifact", () =>
        {
            Debug.Log("랜덤 유물 구매");

            var saveData = Managers.Data.GetData("Temp_GuildSave");

            if (saveData.mainData.Player_Gold < 500) //? 골드가 부족하다면 시스템 메세지 혹은 돈부족 대화
            {
                Managers.Dialogue.ShowDialogueUI(11011);
                return;
            }

            saveData.mainData.Player_Gold -= 500;
            GuildManager.Instance.AddBackAction(() =>
            {
                GameManager.Artifact.Add_RandomArtifact();
            });
        });
        EventAction.Add("Artifact_ID", () =>
        {
            Debug.Log("유물 ID 구매");
            GuildManager.Instance.AddBackAction(() =>
            {
                GameManager.Artifact.AddArtifact(ArtifactLabel.Pearl); //? 추가한 아티팩트로 바꾸면 댐
            });
        });


        EventAction.Add("Monster_Yes", () =>
        {
            Debug.Log("몬스터매니저 - Yes");
            GameManager.Monster.State = MonsterManager.SelectState.Yes;
        });
        EventAction.Add("Monster_No", () =>
        {
            Debug.Log("몬스터매니저 - No");
            GameManager.Monster.State = MonsterManager.SelectState.No;
        });

        EventAction.Add("Gold400_Yes", () =>
        {
            if (Main.Instance.Player_Gold < 400)
            {
                Debug.Log("골드부족 - Yes_Fail");
                GameManager.Monster.State = MonsterManager.SelectState.Yes_Fail;
                Managers.Dialogue.ShowDialogueUI((int)UnitDialogueEventLabel.Utori_NoGold);
            }
            else
            {
                Debug.Log("몬스터매니저 - Yes");
                GameManager.Monster.State = MonsterManager.SelectState.Yes;
            }
        });

        EventAction.Add("Gold4000_Yes2", () =>
        {
            if (Main.Instance.Player_Gold < 4000)
            {
                Debug.Log("골드부족 - Yes_Fail");
                GameManager.Monster.State = MonsterManager.SelectState.Yes_Fail;
                Managers.Dialogue.ShowDialogueUI((int)UnitDialogueEventLabel.Utori_NoGold);
            }
            else
            {
                Debug.Log("State - Yes2");
                GameManager.Monster.State = MonsterManager.SelectState.Yes2;
            }
        });


        EventAction.Add("Soothsayer_Continue", () =>
        {
            GuildHelper.Instance.VIP_Room_Talk(FindAnyObjectByType<PlayerController>().gameObject,
    GuildHelper.Instance.Get_Current_Guild_NPC(GuildNPC_LabelName.Soothsayer),
    DialogueName.Soothsayer_Continue);
        });


        EventAction.Add("Soothsayer_Option", () =>
        {
            var saveData = Managers.Data.GetData("Temp_GuildSave");
            int gold = Mathf.RoundToInt(saveData.mainData.Player_Gold * 0.3f);
            Managers.Dialogue.Show_SelectOption(new int[] { 16199, 16299 }, new string[] { $" ({gold} {UserData.Instance.LocaleText("Gold")})", "" });
        });


        EventAction.Add("Soothsayer_Yes", () =>
        {
            Managers.Dialogue.ActionReserve(() => 
            {
                var saveData = Managers.Data.GetData("Temp_GuildSave");
                int gold = Mathf.RoundToInt(saveData.mainData.Player_Gold * 0.3f);
                saveData.mainData.Player_Gold -= gold;

                Debug.Log("시스템 메세지 - 다음 던전 이벤트에 대한 정보");
                var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();

                var data = RandomEventManager.Instance.Get_NextRandomEventID(CurrentTurn, saveData.currentRandomEventList);
                if (data._id < 0)
                {
                    msg.Set_Text(UserData.Instance.LocaleText("아무일없음"));
                    return;
                }
                else
                {
                    msg.Set_Text($"{data._startDay - CurrentTurn}{UserData.Instance.LocaleText("~일 후")}, \n" +
                        $"{RandomEventManager.Instance.GetData(data._id).description} ");
                }
            });
        });




        //EventAction.Add("Ending", () =>
        //{
        //    StartCoroutine(WaitEnding(1));
        //});

        //? 엔딩관련
        EventAction.Add("Ending", () =>
        {
            StartCoroutine(NewEnding());
        });
    }


    IEnumerator NewEnding()
    {
        var fade = Managers.UI.ClearAndShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteOut, 1);


        // 여기서 저장할 데이터를 미리 들고있어야함. 삭제는 7NewEnding에서 GameClear로 넘어가기전에 삭제함
        //? 임시로 저장 해제(데모버전)
        //Debug.Log("클리어 후 임시저장 부분 / 엔딩저장 및 무한모드 활성화할 때 다시 활성화해야함");

        //? 실제로 클리어 되는 타이밍
        Temp_saveData = Managers.Data.SaveCurrentData("Clear_Temp");
        UserData.Instance.GameClear(Temp_saveData);


        yield return null;
        //? 템프세이브 삭제해줘야함 - 이거 나중에 무한모드할 때 무한모드 저장용 데이터인데 무한모드 넣기전까진 봉인
        Temp_saveData = null;

        yield return new WaitForSecondsRealtime(1);
        Managers.Scene.LoadSceneAsync(SceneName._7_NewEnding, false);
    }

    #region MainEvent 상세내용

    void Guild_Raid_First()
    {
        var Dungeon = Main.Instance.Dungeon;
        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);


        List<NPC> sol1List = new List<NPC>();
        List<NPC> sol2List = new List<NPC>();

        var cap_A = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Captine_A.ToString(), NPC_Typeof.NPC_Type_MainEvent);
        cap_A.transform.position = Dungeon.transform.position + (Vector3.right * 3);
        GameManager.Placement.Visible(cap_A);

        for (int i = 0; i < 7; i++)
        {
            var sol_1 = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Soldier1.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            sol_1.transform.position = Dungeon.transform.position + (Vector3.right * 1 * i) + Vector3.right * 5;
            sol_1.Anim_State = NPC.animState.left;
            sol_1.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol_1);
            sol1List.Add(sol_1);
        }

        var cap_B = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Captine_B.ToString(), NPC_Typeof.NPC_Type_MainEvent);
        cap_B.transform.position = Dungeon.transform.position + (Vector3.right * -3);
        cap_B.Anim_State = NPC.animState.left;
        cap_B.Anim_State = NPC.animState.Idle;
        GameManager.Placement.Visible(cap_B);

        for (int i = 0; i < 7; i++)
        {
            var sol_1 = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Soldier2.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            sol_1.transform.position = Dungeon.transform.position + (Vector3.right * -1 * i) + Vector3.right * -5;
            sol_1.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol_1);
            sol2List.Add(sol_1);
        }

        Managers.Dialogue.ShowDialogueUI(DialogueName.Guild_Raid_1, cap_A.transform);
        StartCoroutine(Wait_Guild_Raid_First(cap_A, cap_B, sol1List, sol2List));
    }
    IEnumerator Wait_Guild_Raid_First(NPC cap_A, NPC cap_B, List<NPC> sol1, List<NPC> sol2)
    {
        var Dungeon = Main.Instance.Dungeon;
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        cap_A.Departure(cap_A.transform.position, Dungeon.position);
        foreach (var item in sol1)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

        float timer = 0;
        while (timer < 6)
        {
            timer += Time.deltaTime;
            yield return UserData.Instance.Wait_GamePlay;
        }

        cap_B.Departure(cap_B.transform.position, Dungeon.position);
        foreach (var item in sol2)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }
    }
    void Guild_Raid_Second()
    {
        var Dungeon = Main.Instance.Dungeon;

        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);


        List<NPC> bloodSong = new List<NPC>();
        //? 피의노래 파티원 생성
        {
            var party = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Blood_Tanker_B.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            party.transform.position = Dungeon.transform.position + (Vector3.left * 13);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }
        {
            var party = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Blood_Warrior_B.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            party.transform.position = Dungeon.transform.position + (Vector3.left * 14);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }
        {
            var party = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Blood_Wizard_B.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            party.transform.position = Dungeon.transform.position + (Vector3.left * 15);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }
        {
            var party = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Blood_Elf_B.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            party.transform.position = Dungeon.transform.position + (Vector3.left * 16);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }

        //? 대장급 생성
        var Cap_A = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Captine_A.ToString(), NPC_Typeof.NPC_Type_MainEvent);
        Cap_A.transform.position = Dungeon.transform.position + (Vector3.right * 2);
        Cap_A.Anim_State = NPC.animState.right;
        Cap_A.Anim_State = NPC.animState.Ready;
        GameManager.Placement.Visible(Cap_A);

        var Cap_B = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Captine_B.ToString(), NPC_Typeof.NPC_Type_MainEvent);
        Cap_B.transform.position = Dungeon.transform.position + (Vector3.right * 10);
        Cap_B.Anim_State = NPC.animState.right;
        Cap_B.Anim_State = NPC.animState.Ready;
        GameManager.Placement.Visible(Cap_B);

        var Captine_C = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Captine_BlueKnight.ToString(), NPC_Typeof.NPC_Type_MainEvent);
        Captine_C.transform.position = Dungeon.transform.position + (Vector3.left * 3);
        GameManager.Placement.Visible(Captine_C);


        List<NPC> sol1List = new List<NPC>();
        List<NPC> sol2List = new List<NPC>();
        List<NPC> sol3List = new List<NPC>();

        for (int i = 0; i < 5; i++)
        {
            var sol = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Soldier1.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            sol.transform.position = Dungeon.transform.position + (Vector3.right * 1 * i) + Vector3.right * 4;
            sol.Anim_State = NPC.animState.left;
            sol.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol);
            sol1List.Add(sol);
        }
        for (int i = 0; i < 5; i++)
        {
            var sol = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Soldier2.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            sol.transform.position = Dungeon.transform.position + (Vector3.right * 1 * i) + Vector3.right * 12;
            sol.Anim_State = NPC.animState.left;
            sol.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol);
            sol2List.Add(sol);
        }
        for (int i = 0; i < 5; i++)
        {
            var sol = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Soldier3.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            sol.transform.position = Dungeon.transform.position + (Vector3.left * 1 * i) + Vector3.left * 5;
            sol.Anim_State = NPC.animState.right;
            sol.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol);
            sol3List.Add(sol);
        }

        Managers.Dialogue.ShowDialogueUI(DialogueName.Guild_Raid_2, Captine_C.transform);
        StartCoroutine(Wait_Guild_Raid_Second(Cap_A, Cap_B, Captine_C, sol1List, sol2List, sol3List, bloodSong));
    }
    IEnumerator Wait_Guild_Raid_Second(NPC cap_A, NPC cap_B, NPC cap_C, List<NPC> sol1, List<NPC> sol2, List<NPC> sol3, List<NPC> bloodSong)
    {
        var Dungeon = Main.Instance.Dungeon;

        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        cap_A.Departure(cap_A.transform.position, Dungeon.position);
        foreach (var item in sol1)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

        float timer = 0;
        while (timer < 6)
        {
            timer += Time.deltaTime;
            yield return UserData.Instance.Wait_GamePlay;
        }

        cap_B.Departure(cap_B.transform.position, Dungeon.position);
        foreach (var item in sol2)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

        timer = 0;
        while (timer < 10)
        {
            timer += Time.deltaTime;
            yield return UserData.Instance.Wait_GamePlay;
        }

        cap_C.Departure(cap_C.transform.position, Dungeon.position);
        foreach (var item in sol3)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

        timer = 0;
        while (timer < 8)
        {
            timer += Time.deltaTime;
            yield return UserData.Instance.Wait_GamePlay;
        }

        foreach (var item in bloodSong)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

    }


    void Forest_Raid_1()
    {
        var Dungeon = Main.Instance.Dungeon;
        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);


        var cap_A = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.Event_Goblin_Leader.ToString(), NPC_Typeof.NPC_Type_MainEvent);
        cap_A.transform.position = Dungeon.transform.position + (Vector3.right * 3);
        GameManager.Placement.Visible(cap_A);

        List<NPC> sol1List = new List<NPC>();
        for (int i = 0; i < 5; i++)
        {
            var group = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.Event_Goblin.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            group.transform.position = Dungeon.transform.position + (Vector3.right * 1 * i) + Vector3.right * 5;
            group.Anim_State = NPC.animState.left;
            group.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(group);
            sol1List.Add(group);
        }

        List<NPC> sol3List = new List<NPC>();
        for (int i = 0; i < 5; i++)
        {
            var group = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.Event_Goblin_Knight.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            group.transform.position = Dungeon.transform.position + (Vector3.right * 1 * i) + Vector3.right * 10;
            group.Anim_State = NPC.animState.left;
            group.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(group);
            sol3List.Add(group);
        }

        List<NPC> sol2List = new List<NPC>();
        for (int i = 0; i < 8; i++)
        {
            var group = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.Event_Orc.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            group.transform.position = Dungeon.transform.position + (Vector3.right * -1 * i) + Vector3.right * -5;
            group.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(group);
            sol2List.Add(group);
        }

        Managers.Dialogue.ShowDialogueUI(DialogueName.Forest_Raid_1, cap_A.transform);
        StartCoroutine(Move_Forest_Raid_1(cap_A, sol1List, sol3List, sol2List));
    }
    IEnumerator Move_Forest_Raid_1(NPC cap_A, List<NPC> group1, List<NPC> group2, List<NPC> group3)
    {
        var Dungeon = Main.Instance.Dungeon;
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        UserData.Instance.GameMode = Define.GameMode.Normal;

        cap_A.Departure(cap_A.transform.position, Dungeon.position);
        foreach (var item in group1)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }
        foreach (var item in group2)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

        float timer = 0;
        while (timer < 8)
        {
            timer += Time.deltaTime;
            yield return UserData.Instance.Wait_GamePlay;
        }
        foreach (var item in group3)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

    }

    void Forest_Raid_2()
    {
        var Dungeon = Main.Instance.Dungeon;
        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);


        var cap_A = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.Event_Orc_Leader.ToString(), NPC_Typeof.NPC_Type_MainEvent);
        cap_A.transform.position = Dungeon.transform.position + (Vector3.right * 3);
        GameManager.Placement.Visible(cap_A);

        List<NPC> sol1List = new List<NPC>();
        for (int i = 0; i < 15; i++)
        {
            var group = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.Event_Orc.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            group.transform.position = Dungeon.transform.position + (Vector3.right * 1 * i) + Vector3.right * 5;
            group.Anim_State = NPC.animState.left;
            group.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(group);
            sol1List.Add(group);
        }

        List<NPC> sol2List = new List<NPC>();
        for (int i = 0; i < 10; i++)
        {
            var group = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.Event_Goblin_Knight.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            group.transform.position = Dungeon.transform.position + (Vector3.right * -0.9f * i) + Vector3.right * -5;
            group.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(group);
            sol2List.Add(group);
        }

        List<NPC> sol3List = new List<NPC>();
        for (int i = 0; i < 10; i++)
        {
            var group = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.Event_Lizard.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            group.transform.position = Dungeon.transform.position + (Vector3.right * -0.9f * i) + Vector3.right * -15;
            group.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(group);
            sol3List.Add(group);
        }



        Managers.Dialogue.ShowDialogueUI(DialogueName.Forest_Raid_2, cap_A.transform);
        StartCoroutine(Move_Forest_Raid_2(cap_A, sol1List, sol2List, sol3List));
    }
    IEnumerator Move_Forest_Raid_2(NPC cap_A, List<NPC> group1, List<NPC> group2, List<NPC> group3)
    {
        var Dungeon = Main.Instance.Dungeon;
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        UserData.Instance.GameMode = Define.GameMode.Normal;

        cap_A.Departure(cap_A.transform.position, Dungeon.position);
        foreach (var item in group1)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }


        float timer = 0;
        while (timer < 12)
        {
            timer += Time.deltaTime;
            yield return UserData.Instance.Wait_GamePlay;
        }
        foreach (var item in group2)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }
        foreach (var item in group3)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }

    }


    void Last_Judgment()
    {
        Debug.Log("마왕엔딩 - 최후의 결전 이벤트 시작");

        //? 사전준비
        var Dungeon = Main.Instance.Dungeon;
        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);

        //? 적 소환
        var hero = GameManager.NPC.InstantiateNPC_Event(NPC_Type_SubEvent.Judgement.ToString(), NPC_Typeof.NPC_Type_SubEvent);
        hero.transform.position = Dungeon.transform.position + (Vector3.left * 3);
        GameManager.Placement.Visible(hero);

        List<NPC> sol1List = new List<NPC>();
        for (int i = 0; i < 5; i++)
        {
            var group = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_KingdomKnight.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            group.transform.position = Dungeon.transform.position + (Vector3.right * 1 * i) + Vector3.right * 5;
            group.Anim_State = NPC.animState.left;
            group.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(group);
            sol1List.Add(group);
        }

        //? 대화시작 / 끝나고 적 이동
        Managers.Dialogue.ShowDialogueUI(DialogueName.The_Judgement, hero.transform);
        StartCoroutine(Move_Last_Judgment(hero, sol1List));
    }
    IEnumerator Move_Last_Judgment(NPC cap, List<NPC> group1)
    {
        var Dungeon = Main.Instance.Dungeon;
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        UserData.Instance.GameMode = Define.GameMode.Normal;

        foreach (var item in group1)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }


        float timer = 0;
        while (timer < 15)
        {
            timer += Time.deltaTime;
            yield return UserData.Instance.Wait_GamePlay;
        }

        cap.Departure(cap.transform.position, Dungeon.position);
    }

    void Hero_Final()
    {
        Debug.Log("용사엔딩 - 마지막 이벤트 시작");

        //? 사전준비
        var Dungeon = Main.Instance.Dungeon;
        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);

        //? 적 소환
        var venom = GameManager.NPC.InstantiateNPC_Event(NPC_Type_SubEvent.Venom.ToString(), NPC_Typeof.NPC_Type_SubEvent);
        venom.transform.position = Dungeon.transform.position + (Vector3.right * 3);
        GameManager.Placement.Visible(venom);
        venom.Anim_State = NPC.animState.left;
        venom.Anim_State = NPC.animState.Ready;

        List<NPC> catastrophe = new List<NPC>();
        for (int i = 0; i < 6; i++)
        {
            var group = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Catastrophe_Clone.ToString(), NPC_Typeof.NPC_Type_MainEvent);
            group.transform.position = Dungeon.transform.position + (Vector3.right * -1.5f * i) + Vector3.right * -3;
            group.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(group);
            catastrophe.Add(group);
        }

        //? 대화시작 / 끝나고 적 이동
        Managers.Dialogue.ShowDialogueUI(DialogueName.The_Venom, venom.transform.Find("_Pos"));
        StartCoroutine(Move_Hero_Final(venom, catastrophe));
    }
    IEnumerator Move_Hero_Final(NPC cap, List<NPC> group1)
    {
        var Dungeon = Main.Instance.Dungeon;
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        UserData.Instance.GameMode = Define.GameMode.Normal;

        foreach (var item in group1)
        {
            item.Departure(item.transform.position, Dungeon.position);
        }


        float timer = 0;
        while (timer < 15)
        {
            timer += Time.deltaTime;
            yield return UserData.Instance.Wait_GamePlay;
        }

        cap.Departure(cap.transform.position, Dungeon.position);
    }

    #endregion





    IEnumerator FirstPortalAppear()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        if (Managers.Dialogue.GetState() == DialogueManager.DialogueState.None)
        {
            FirstPortalAppearSkip();
            yield break;
        }

        Managers.Dialogue.AllowPerfectSkip = false;
        var cam = Camera.main.GetComponent<CameraControl>();

        Vector2Int portalIndex = new Vector2Int(2, 2);
        Vector2Int eggExit = new Vector2Int(12, 2);

        Vector3 floor3 = GetTilePosition(Define.DungeonFloor.Floor_3, portalIndex);
        Vector3 floorEgg = GetTilePosition(Define.DungeonFloor.Egg, eggExit);

        cam.ChasingTarget(floor3, 2);
        yield return new WaitForSeconds(2);

        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].TileMap.TryGetValue(portalIndex, out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
        }

        yield return new WaitForSeconds(2);
        cam.ChasingTarget(floorEgg, 2);
        yield return new WaitForSeconds(2);

        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Egg].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Egg].TileMap.TryGetValue(eggExit, out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Egg], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("Exit", info);
        }

        yield return new WaitForSeconds(2);
        Managers.Dialogue.AllowPerfectSkip = true;
    }
    public void FirstPortalAppearSkip()
    {
        Vector2Int portalIndex = new Vector2Int(2, 2);
        Vector2Int eggExit = new Vector2Int(12, 2);

        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].TileMap.TryGetValue(portalIndex, out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
        }
        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Egg].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Egg].TileMap.TryGetValue(eggExit, out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Egg], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("Exit", info);
        }
    }


    IEnumerator EntranceMove_3to4()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        if (Managers.Dialogue.GetState() == DialogueManager.DialogueState.None)
        {
            EntranceMove_3to4_Skip();
            yield break;
        }

        Managers.Dialogue.AllowPerfectSkip = false;

        var cam = Camera.main.GetComponent<CameraControl>();

        Vector2Int portal_3 = new Vector2Int(2, 2);
        Vector2Int portal_4 = new Vector2Int(2, 2);


        Vector3 floor3 = GetTilePosition(Define.DungeonFloor.Floor_3, portal_3);
        Vector3 floor4 = GetTilePosition(Define.DungeonFloor.Floor_4, portal_4);

        cam.ChasingTarget(floor3, 2);
        yield return new WaitForSecondsRealtime(2);

        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].TileMap.TryGetValue(portal_3, out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
        }

        yield return new WaitForSecondsRealtime(2);
        cam.ChasingTarget(floor4, 2);
        yield return new WaitForSecondsRealtime(2);

        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].TileMap.TryGetValue(portal_4, out tile);
            // 로드파일에서 테스트할 때
            if (tile.Original != null)
            {
                GameManager.Facility.RemoveFacility(tile.Original as Facility);
            }

            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
        }

        yield return new WaitForSecondsRealtime(2);
        cam.ChasingTarget(Main.Instance.Player, 2);
        yield return new WaitForSecondsRealtime(2);

        Managers.Dialogue.AllowPerfectSkip = true;
    }
    void EntranceMove_3to4_Skip()
    {
        if (Managers.Dialogue.GetState() == DialogueManager.DialogueState.None)
        {
            Vector2Int portal_3 = new Vector2Int(2, 2);
            Vector2Int portal_4 = new Vector2Int(2, 2);

            {
                var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].GetRandomTile();
                Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].TileMap.TryGetValue(portal_3, out tile);
                GameManager.Facility.RemoveFacility(tile.Original as Facility);
            }
            {
                var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].GetRandomTile();
                Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].TileMap.TryGetValue(portal_4, out tile);
                // 로드파일에서 테스트할 때
                if (tile.Original != null)
                {
                    GameManager.Facility.RemoveFacility(tile.Original as Facility);
                }

                PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4], tile);
                var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
            }
        }
    }

    public void EntranceMove_4to5()
    {
        Vector2Int portal_4 = new Vector2Int(2, 2);
        Vector2Int portal_5 = new Vector2Int(10, 24);

        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].TileMap.TryGetValue(portal_4, out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
        }
        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_5].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Floor_5].TileMap.TryGetValue(portal_5, out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);

            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Floor_5], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
        }
    }


    #endregion


    Vector3 GetTilePosition(Define.DungeonFloor floor, Vector2Int pos)
    {
        BasementTile tile = null;
        Main.Instance.Floor[(int)floor].TileMap.TryGetValue(pos, out tile);

        return tile.worldPosition;
    }




    //? 클리어시에만 임시로 사용함. 이게 있을 땐 SaveLoad에서도 얘로 대신해서 저장함. (인덱스는 클릭한 인덱스로). 클리어 처리 후엔 삭제됨
    public DataManager.SaveData Temp_saveData { get; set; }


    public void AddCustomAction(string _name, Action _action)
    {
        EventAction.Add(_name, _action);
    }


    public Action GetAction(int dialogueID)
    {
        Action action = null;
        GuildNPCAction.TryGetValue(dialogueID, out action);
        return action;
    }

    public Action GetAction(string eventName)
    {
        Action action = null;
        EventAction.TryGetValue(eventName, out action);
        return action;
    }
    public Action GetQuestAction(int _QuestID)
    {
        Action action = null;
        forQuestAction.TryGetValue(_QuestID, out action);
        return action;
    }



    

}

public enum DayEventLabel
{
    Test1 = 0,
    Test2 = 1,
    Test3 = 2,

    Goblin_Appear = 93,
    Goblin_Party = 100,

    Orc_Party = 101,

    Catastrophe = 140,

    RetiredHero = 153,

    BloodSong_Appear = 200,

    Guild_Raid_1 = 220,
    Guild_Raid_2 = 221,

    Forest_Raid_1 = 230,
    Forest_Raid_2 = 231,

    Last_Judgment = 10003,
}




