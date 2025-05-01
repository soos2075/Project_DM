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

        //? �����̺�Ʈ �߻��κ�
        OneDayAfter();
        var TodayEvent = GetDayEvent(); //? �����Ȱ� ���ÿ� DayList���� �����ϴϱ� ��������� ȣ���ؾ��� / 29�ϳ� ���¿� ���� ������

        if (CurrentTurn == 30 && Main.Instance.CurrentEndingState == Endings.Demon)
        {
            //? 30�� �̺�Ʈ�� ������ ��ü
            Last_Judgment();
            ClearQuestAction(7710003);
        }
        else if (CurrentTurn == 30 && Main.Instance.CurrentEndingState == Endings.Hero)
        {
            //? 30�� �̺�Ʈ�� ������ ��ü
            Hero_Final();
        }
        else if (TodayEvent != null)
        {
            Run_DayEventAction(TodayEvent.EventIndex); //? �갡 ���� invoke�� �ϴ� ���̱� ������ �길 ���ϸ� ��
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
        GuildManager.Instance.Init_CurrentGuildData(); //? CurrentGuildData�� ���µǼ� �ٽ� ä��� ����
    }



    #region �� ���� / ����� ���ǿ� ���� ���� �̺�Ʈ �߰��ϴ°�
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
        //? �α⵵ ��� �̺�Ʈ
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


        //? 2ȸ�� �̻� / �α⵵���赵���� 1200�̻��� �� 
        if (UserData.Instance.FileConfig.PlayRounds > 1 && Danger + Pop >= 1200 &&
            CurrentClearEventData.Check_AlreadyClear(DayEventLabel.Catastrophe) == false && Check_AlreadyReserve(DayEventLabel.Catastrophe) == false)
        {
            AddDayEvent(DayEventLabel.Catastrophe, priority: 1, embargo: 0, delay: 3);
            Add_GuildQuest_Special(3014); //? �������� �ҽĵ�� ����Ʈ �߰� - ���� 1140���� ����
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


    #region �̺�Ʈ ����Ʈ ���� ������ (���̺����ϰ� ������. UserData.SavefileConfig������)
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


    #region ��� �湮 �˸� ǥ��
    public bool CheckGuildNotice()
    {
        var guildData = CurrentGuildData;
        if (CurrentGuildData == null)
        {
            Debug.Log("���� ��� ������ ����");
            return false;
        }

        //? Ȱ��ȭ �� ������ ��������Ʈ�� �������
        foreach (var item in guildData)
        {
            //? ����Ʈ ������ �ѱ��
            if (item.InstanceQuestList.Count == 0)
            {
                continue;
            }

            //? ���ܸ���̸� �ѱ��
            if (GuildManager.Instance.Delete_GuildNPC.Contains((GuildNPC_LabelName)item.Original_Index))
            {
                continue;
            }

            //? ������ ���ԵǴ� NPC�� �ְ�
            if (GuildManager.Instance.Instance_GuildNPC.Contains((GuildNPC_LabelName)item.Original_Index))
            {
                Debug.Log($"��� ����Ʈ �߻��� : {item.InstanceQuestList[0]}");
                return true;
            }

            switch (GuildManager.Instance.GetData(item.Original_Index).DayOption)
            {
                case Guild_DayOption.Special:
                    break;

                case Guild_DayOption.Always:
                    if (GuildManager.Instance.GetData(item.Original_Index).FirstDay <= CurrentTurn)
                    {
                        Debug.Log($"��� ����Ʈ �߻��� : {item.Original_Index}");
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

        //? ���� �ɼ� ����Ʈ�� 2���̻��� �� ���˸�
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
            Debug.Log($"��� ����Ʈ �߻��� : {data.Original_Index + data.InstanceQuestList[0]}");
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

        //? ����Ʈ�� �켱����. ���ڰ� �������� �켱������ ����
        public int Priority;

        //? �ߵ���⳯¥ (��������� ����. �ּ� 3�� �Ŀ� �߻��ϴ� �̺�Ʈ or �ּ� ���̻� �ߵ��ϴ� �̺�Ʈ)
        //? Embargo�� ��(��¥)���� - �ּ� ���� �� ���� �̻��� �� �̺�Ʈ�� �߻�
        public int Embargo;
        //? Delay�� ��ϵǰ��������� ���� 1�� �پ��µ� �ּ� �����̸� �ֱ� �����̰� 0���� �������� ����
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

        if (DayEventList.Count == 0) //? ��ϵ� �̺�Ʈ�� ������ Null
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


        if (CurrentAbleEvent.Count == 0) //? ��ϵ� �̺�Ʈ�߿� ���� ���� ������ �̺�Ʈ�� ������ Null
        {
            return null;
        }
        else
        {
            DayEvent temp = CurrentAbleEvent[0];
            foreach (var item in CurrentAbleEvent)
            {
                if (item.Priority < temp.Priority) //? ���� �켱������ �� ���� ���� ����Ʈ�� ������ ��ü
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
            Debug.Log("������ ���� �̺�Ʈ");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_RetiredHero.ToString(), 9, NPC_Typeof.NPC_Type_MainEvent);
        });

        DayEventActionRegister.Add(DayEventLabel.Goblin_Appear, () => {
            Debug.Log("��� ù���� �̺�Ʈ");
            GameManager.NPC.Set_UniqueChance(NPC_Type_Unique.ManaGoblin, 0.05f);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin_Leader.ToString(), 9, NPC_Typeof.NPC_Type_MainEvent);
        });

        DayEventActionRegister.Add(DayEventLabel.Goblin_Party, () => {
            Debug.Log("��� ��Ƽ �̺�Ʈ");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin_Leader.ToString(), 3, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 3.5f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 4, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 4.5f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 5, NPC_Typeof.NPC_Type_MainEvent);
        });

        DayEventActionRegister.Add(DayEventLabel.Orc_Party, () => {
            Debug.Log("��ũ ��Ƽ �̺�Ʈ");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Orc.ToString(), 3, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Orc.ToString(), 3.5f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Orc.ToString(), 4, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Orc.ToString(), 4.5f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Orc.ToString(), 5, NPC_Typeof.NPC_Type_MainEvent);
        });

        DayEventActionRegister.Add(DayEventLabel.Catastrophe, () => {
            Debug.Log("������ ��� �̺�Ʈ");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Catastrophe.ToString(), 10, NPC_Typeof.NPC_Type_MainEvent);
        });

        DayEventActionRegister.Add(DayEventLabel.BloodSong_Appear, () => {
            Debug.Log("Blood Song �̺�Ʈ");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Tanker_A.ToString(), 10f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Warrior_A.ToString(), 10.5f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Wizard_A.ToString(), 11f, NPC_Typeof.NPC_Type_MainEvent);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Elf_A.ToString(), 11.5f, NPC_Typeof.NPC_Type_MainEvent);
        });


        DayEventActionRegister.Add(DayEventLabel.Guild_Raid_1, () => {
            Debug.Log("��� ����� 1 �̺�Ʈ");
            Guild_Raid_First();
            ClearQuestAction(777010);
            Clear_GuildQuest(7010);
        });

        DayEventActionRegister.Add(DayEventLabel.Guild_Raid_2, () => {
            Debug.Log("��� ����� 2 �̺�Ʈ");
            Guild_Raid_Second();
            ClearQuestAction(777020);
            Clear_GuildQuest(7020);
        });

        DayEventActionRegister.Add(DayEventLabel.Forest_Raid_1, () => {
            Debug.Log("�����̵� 1 �̺�Ʈ");
            Forest_Raid_1();
            ClearQuestAction(778010);
            Clear_GuildQuest(8010);
        });

        DayEventActionRegister.Add(DayEventLabel.Forest_Raid_2, () => {
            Debug.Log("�����̵� 2 �̺�Ʈ");
            Forest_Raid_2();
            ClearQuestAction(778020);
            Clear_GuildQuest(8020);
        });

        //DayEventActionRegister.Add(DayEventLabel.Last_Judgment, () => {
        //    Debug.Log("���տ��� ���İ���");
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


    #region ��� �̺�Ʈ �׼� ���� (��� ����)
    public int CurrentTurn { get; set; }

    //? ������� ����� - ��� ������ �����͸� �������ְ� �� ����ÿ��� �����������. ����� �ҷ����⿡�� �����ϰ� ���
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


    //? ������ ��带 �ǰ��ٰ� ����Ʈ�� �߰��Ǹ� �ȵǱ� ������ ��¥ �����ϱ�
    public List<Quest_Reservation> Reservation_Quest { get; set; } = new List<Quest_Reservation>();
    public void ReservationToQuest(int day, int questIndex)
    {
        Reservation_Quest.Add(new Quest_Reservation(day, questIndex));
    }

    public void Add_ReservationGuildQuest() //? ���� ���� �ٲ𶧸���
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


    //? ��尡�� �߰����Ѿ� �� ����Ʈ ����Ʈ - �α⵵ �ø��� �� ���� ���� �ʱ�ȭ �Ǵ� �׸� (��� ����Ʈ �߻� �˸��� ����)
    public List<int> AddQuest_Daily { get; set; } = new List<int>();

    public void Add_Daily(int index)
    {
        AddQuest_Daily.Add(index);
        AddQuest_Daily = Util.ListDistinct(AddQuest_Daily);
    }

    //? ���� �������� ����Ʈ ��� - ���� ���� ����� Action
    public Action CurrentQuestAction { get; private set; }

    //? ���� �������� ����Ʈ ���(dataManager���� ��������θ� ���)
    public HashSet<int> CurrentQuestAction_forSave { get; set; } = new HashSet<int>();

    
    //? �̰� Ŀ���ҽ������������� ȣ���ϰ� �־ �θ��������� ���װ� ������ / NPC Manager���� �ؾ���
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




    //? ��忡�� ��ȭ�� ������ Action
    Dictionary<int, Action> GuildNPCAction = new Dictionary<int, Action>();

    //? ��Ÿ ��ȭ�� ����ǰ� �ٷ� ������ Action
    Dictionary<string, Action> EventAction = new Dictionary<string, Action>();

    //? �ܼ��� CurrentQuestEvent�� add / remove �ϴ� �뵵�θ� ���Ǿ����. �� GuildAction�� �ߺ��Ǽ� ���� ����, ���������� ���� ���� ����.
    Dictionary<int, Action> forQuestAction = new Dictionary<int, Action>();

    void Init_ForQuestAction()  //? ������ ȣ���� �׼�
    {

        forQuestAction.Add(1100, () =>
        {
            Debug.Log("����Ʈ - ��������� Ȱ��ȭ");
            GameManager.NPC.AddEventNPC(NPC_Type_Hunter.Hunter_Slime.ToString(), 12, NPC_Typeof.NPC_Type_Hunter);
        });

        forQuestAction.Add(1101, () =>
        {
            Debug.Log("����Ʈ - ��� Ȱ��ȭ");
            GameManager.NPC.AddEventNPC(NPC_Type_Hunter.Hunter_EarthGolem.ToString(), 13, NPC_Typeof.NPC_Type_Hunter);
        });


        forQuestAction.Add(1140, () =>
        {
            Debug.Log("������ ��� �����");
        });

        forQuestAction.Add(1141, () =>
        {
            Debug.Log("���ӵǴ� ���");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Catastrophe.ToString(), 10, NPC_Typeof.NPC_Type_MainEvent);
        });

        forQuestAction.Add(771141, () =>
        {
            Debug.Log("�ذ��� �Ǹ���");
        });

        forQuestAction.Add(1150, () =>
        {
            Debug.Log("��� ��Ƽ �湮 �����");
        });

        forQuestAction.Add(1151, () =>
        {
            Debug.Log("������ ���� �湮 �����");
        });




        //? ����Ʈ ��Ͽ�
        forQuestAction.Add(772102, () =>
        {
            Debug.Log("���� ������ �湮Ȯ�� 3��!!");
            GameManager.NPC.Event_Herb = true;
        });
        forQuestAction.Add(772103, () =>
        {
            Debug.Log("���� ������ �湮Ȯ�� 3��!!");
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
            Debug.Log("1�� ��巹�̵� �غ���");
        });

        forQuestAction.Add(777020, () =>
        {
            Debug.Log("2�� ��巹�̵� �غ���");
        });

        forQuestAction.Add(778010, () =>
        {
            Debug.Log("1�� ���ǽ��� �غ���");
        });

        forQuestAction.Add(778020, () =>
        {
            Debug.Log("2�� ���ǽ��� �غ���");
        });

        forQuestAction.Add(7710003, () =>
        {
            Debug.Log("������ ���� �غ���");
        });

        forQuestAction.Add(7712000, () =>
        {
            Debug.Log("����Ʈ��");
            GameManager.NPC.AddEventNPC(NPC_Type_SubEvent.Lightning.ToString(), 3, NPC_Typeof.NPC_Type_SubEvent);
        });
    }
    void Init_DialogueAction() //? ��ȭ�� ���ؼ� ȣ���ϴ°�. �ڵ�󿡴� ���� Dialogue�� Index�θ� ������
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
                //    //Debug.Log($"������ �α⵵�� {ranPop} �ö����ϴ�.");
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
                Debug.Log("������ �շ�");
                GameManager.Monster.Resize_AddOne();
                var mon = GameManager.Monster.CreateMonster("Heroine", false, true);
                GuildManager.Instance.AddDeleteGuildNPC(GuildNPC_LabelName.Heroine);
            });
        });

        GuildNPCAction.Add(7010, () => { AddQuestAction(777010); });
        GuildNPCAction.Add(7020, () => { AddQuestAction(777020); });
        GuildNPCAction.Add(8010, () => { AddQuestAction(778010); });
        GuildNPCAction.Add(8020, () => { AddQuestAction(778020); });

        //? ����Ʈ�� ���̼�
        GuildNPCAction.Add(12010, () =>
        {
            AddQuestAction(7712000);
            GuildManager.Instance.GetInteraction(12000).Remove_Option(0); //? Ȯ��� �� ���� �߰��Ǵ� ����Ʈ�� ���⼭ �����������(�����������ϱ�)
        });


        //? �׷����Ͽ�� ��ȭ ����Ʈ �߰�
        GuildNPCAction.Add(100701, () => { Add_GuildQuest_Special(5010); });
        //? �׷����Ͽ�� ���� NPC ����Ʈ �߰�(�� �����ϴ°�)
        GuildNPCAction.Add(5010, () => { Add_GuildQuest_Special(5199, false); Add_GuildQuest_Special(5299, false); });
    }

    //? ��� Main Event�� �����ؾ���
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



        //? Diglogue�� ���� ȣ��
        EventAction.Add("Racing_Certificate", () =>
        {
            GuildManager.Instance.AddBackAction(() =>
            {
                GameManager.Artifact.AddArtifact(ArtifactLabel.Racing);

                var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
                message.DelayTime = 2;
                //? �ű� ��Ƽ��Ʈ
                message.Message = $"{UserData.Instance.LocaleText("New")}{UserData.Instance.LocaleText("��Ƽ��Ʈ")} : " +
                $"{GameManager.Artifact.GetData("Racing").labelName}";
            });
        });




        EventAction.Add("RedHair_Defeat", () =>
        {
            Debug.Log("RedHair - RetiredHero �̺�Ʈ ����");
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
            Debug.Log("���� �й� �̺�Ʈ");
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
            Debug.Log("���� ��� ��ȭ");
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
            Debug.Log("���� ��� ��ȭ2");
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
            Debug.Log("������ ��ȭ2");
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
            Debug.Log("������ ��ȭ3");
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
            Debug.Log("������ ������");
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
            Debug.Log("���ΰ� ����");
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
            Debug.Log("������ ����");

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
            Debug.Log("������ ���� ��Ʈ Ȯ��");

            GameManager.Monster.GetMonster<Heroine>().UnitDialogueEvent.ClearEvent((int)UnitDialogueEventLabel.Heroin_Root_Ture);
            Main.Instance.ChangeEggState();
        });

        EventAction.Add("Peddler_Join", () =>
        {
            Debug.Log("ġŲ ���� ��ȭ");

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
                Debug.Log("ġŲ �շ�");
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
            Debug.Log("RedHair_Return - Goblin ����");
            AddDayEvent(DayEventLabel.Goblin_Appear, priority: 0, embargo: 10, delay: 0);
        });

        EventAction.Add("Goblin_Satisfiction", () =>
        {
            Debug.Log("��� ���� - ��� ��Ƽ �̺�Ʈ ����");
            AddDayEvent(DayEventLabel.Goblin_Party, priority: 0, embargo: 14, delay: 0);
            AddQuestAction(1150); //? �����Ƽ �ٷ� ����Ʈ�� �߰�

            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 2, true);
        });

        EventAction.Add("Goblin_Pass", () =>
        {
            Debug.Log("��� Die or Empty - ��ũ��Ƽ");
            AddDayEvent(DayEventLabel.Orc_Party, priority: 0, embargo: 14, delay: 0);


            //AddDayEvent(DayEventLabel.Catastrophe, priority: 0, embargo: 12, delay: 0);
            //Add_GuildQuest_Special(3014); //? �������� �ҽĵ�� ����Ʈ �߰� - ���� 1140���� ����
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
            Debug.Log("������ ��� - �ذ��� �� ���� ���� �̺�Ʈ");
            ClearQuestAction(1140);
            AddQuestAction(1141); //? �ذ�Ǳ������� 1141�� ���ӹ߻��Ǵ� �̺�Ʈ(����Ʈ���͸���)

            GuildManager.Instance.AddInstanceGuildNPC(GuildNPC_LabelName.DeathMagician);
            Add_GuildQuest_Special(10001, true);
        });

        EventAction.Add("DeathMagician_Catastrophe", () =>
        {
            Debug.Log("������ ��� - ���������� - �Ǹ��� ����");
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
            Debug.Log("������ ��� - ����");

            GameManager.Technical.Get_Technical<BarrierOfSealing>().Set_Seal();
            GameManager.Technical.Get_Technical<BarrierOfSealing>().AddCollectionPoint();
        });


        EventAction.Add("DeathMagician_DevilStatue", () =>
        {
            Debug.Log("���������� - ���ΰ� ���� ��Ʈ");
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
            Debug.Log("���ΰ� ���� ��Ʈ Ȯ�� / ��Ƽ��Ʈ �ް� ��� �̺�Ʈ ����");
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            player.GetComponentInChildren<SpriteRenderer>().flipX = true;
            player.GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.DeathMagician).position;
            GuildManager.Instance.RemoveInstanceGuildNPC(GuildNPC_LabelName.DeathMagician);


            //? ��Ƽ��Ʈ �߰�
            GuildManager.Instance.AddBackAction(() =>
            {
                GameManager.Artifact.AddArtifact(ArtifactLabel.TouchOfDecay);
            });
            //? ����̺�Ʈ �߰� (����Ʈ�� ��ϵ� �����̺�Ʈ��)
            AddQuestAction(7710003);
        });



        EventAction.Add("Dog_Artifact", () =>
        {
            Debug.Log("�ź��� �� ����");

            var saveData = Managers.Data.GetData("Temp_GuildSave");

            if (saveData.mainData.Player_Gold < 800) //? ��尡 �����ϴٸ� �ý��� �޼��� Ȥ�� ������ ��ȭ
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
            Debug.Log("���� ���� ����");

            var saveData = Managers.Data.GetData("Temp_GuildSave");

            if (saveData.mainData.Player_Gold < 500) //? ��尡 �����ϴٸ� �ý��� �޼��� Ȥ�� ������ ��ȭ
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
            Debug.Log("���� ID ����");
            GuildManager.Instance.AddBackAction(() =>
            {
                GameManager.Artifact.AddArtifact(ArtifactLabel.Pearl); //? �߰��� ��Ƽ��Ʈ�� �ٲٸ� ��
            });
        });


        EventAction.Add("Monster_Yes", () =>
        {
            Debug.Log("���͸Ŵ��� - Yes");
            GameManager.Monster.State = MonsterManager.SelectState.Yes;
        });
        EventAction.Add("Monster_No", () =>
        {
            Debug.Log("���͸Ŵ��� - No");
            GameManager.Monster.State = MonsterManager.SelectState.No;
        });

        EventAction.Add("Gold400_Yes", () =>
        {
            if (Main.Instance.Player_Gold < 400)
            {
                Debug.Log("������ - Yes_Fail");
                GameManager.Monster.State = MonsterManager.SelectState.Yes_Fail;
                Managers.Dialogue.ShowDialogueUI((int)UnitDialogueEventLabel.Utori_NoGold);
            }
            else
            {
                Debug.Log("���͸Ŵ��� - Yes");
                GameManager.Monster.State = MonsterManager.SelectState.Yes;
            }
        });

        EventAction.Add("Gold4000_Yes2", () =>
        {
            if (Main.Instance.Player_Gold < 4000)
            {
                Debug.Log("������ - Yes_Fail");
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

                Debug.Log("�ý��� �޼��� - ���� ���� �̺�Ʈ�� ���� ����");
                var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();

                var data = RandomEventManager.Instance.Get_NextRandomEventID(CurrentTurn, saveData.currentRandomEventList);
                if (data._id < 0)
                {
                    msg.Set_Text(UserData.Instance.LocaleText("�ƹ��Ͼ���"));
                    return;
                }
                else
                {
                    msg.Set_Text($"{data._startDay - CurrentTurn}{UserData.Instance.LocaleText("~�� ��")}, \n" +
                        $"{RandomEventManager.Instance.GetData(data._id).description} ");
                }
            });
        });




        //EventAction.Add("Ending", () =>
        //{
        //    StartCoroutine(WaitEnding(1));
        //});

        //? ��������
        EventAction.Add("Ending", () =>
        {
            StartCoroutine(NewEnding());
        });
    }


    IEnumerator NewEnding()
    {
        var fade = Managers.UI.ClearAndShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteOut, 1);


        // ���⼭ ������ �����͸� �̸� ����־����. ������ 7NewEnding���� GameClear�� �Ѿ������ ������
        //? �ӽ÷� ���� ����(�������)
        //Debug.Log("Ŭ���� �� �ӽ����� �κ� / �������� �� ���Ѹ�� Ȱ��ȭ�� �� �ٽ� Ȱ��ȭ�ؾ���");

        //? ������ Ŭ���� �Ǵ� Ÿ�̹�
        Temp_saveData = Managers.Data.SaveCurrentData("Clear_Temp");
        UserData.Instance.GameClear(Temp_saveData);


        yield return null;
        //? �������̺� ����������� - �̰� ���߿� ���Ѹ���� �� ���Ѹ�� ����� �������ε� ���Ѹ�� �ֱ������� ����
        Temp_saveData = null;

        yield return new WaitForSecondsRealtime(1);
        Managers.Scene.LoadSceneAsync(SceneName._7_NewEnding, false);
    }

    #region MainEvent �󼼳���

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
        //? ���ǳ뷡 ��Ƽ�� ����
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

        //? ����� ����
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
        Debug.Log("���տ��� - ������ ���� �̺�Ʈ ����");

        //? �����غ�
        var Dungeon = Main.Instance.Dungeon;
        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);

        //? �� ��ȯ
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

        //? ��ȭ���� / ������ �� �̵�
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
        Debug.Log("��翣�� - ������ �̺�Ʈ ����");

        //? �����غ�
        var Dungeon = Main.Instance.Dungeon;
        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);

        //? �� ��ȯ
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

        //? ��ȭ���� / ������ �� �̵�
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
            // �ε����Ͽ��� �׽�Ʈ�� ��
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
                // �ε����Ͽ��� �׽�Ʈ�� ��
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




    //? Ŭ����ÿ��� �ӽ÷� �����. �̰� ���� �� SaveLoad������ ��� ����ؼ� ������. (�ε����� Ŭ���� �ε�����). Ŭ���� ó�� �Ŀ� ������
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




