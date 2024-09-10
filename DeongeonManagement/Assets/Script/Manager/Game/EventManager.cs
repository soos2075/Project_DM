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
        AddDialogueAction();
        AddEventAction();
        AddForQuestAction();
        RegistDayEvent();
    }




    public void TurnStart()
    {
        Debug.Log(CurrentClearEventData.ShowClearData());


        CurrentTurn = Main.Instance.Turn;
        CurrentQuestAction?.Invoke();

        TurnStart_EventSchedule();
        Add_ReservationGuildQuest();

        AddDayEventByCondition();

        //? �����̺�Ʈ �߻��κ�
        OneDayAfter();
        var TodayEvent = GetDayEvent();
        if (TodayEvent != null)
        {
            Run_DayEventAction(TodayEvent.EventIndex);
        }
    }
    public void TurnOver()
    {
        TurnOverEventReserve?.Invoke();
        TurnOverEventReserve = null;

        TurnOver_EventSchedule();
    }


    Action TurnOverEventReserve;
    public void AddTurnOverEventReserve(Action action)
    {
        if (TurnOverEventReserve == null)
        {
            TurnOverEventReserve = action;
        }
        else
        {
            TurnOverEventReserve += action;
        }
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
        //CurrentClearEventData.Init_List();
    }

    public void NewGameReset()
    {
        QuestDataReset();
        GuildManager.Instance.Init_CurrentGuildData(); //? CurrentGuildData�� ���µǼ� �ٽ� ä��� ����
    }


    void TurnStart_EventSchedule()
    {
        switch (CurrentTurn)
        {
            case 8:
                Add_GuildQuest_Special(2102, false);
                break;
        }
    }
    void TurnOver_EventSchedule()
    {
        switch (CurrentTurn)
        {
            case 11:
                Remove_GuildQuest(2102);
                RemoveQuestAction(772102);
                break;

            default:
                Add_Daily(2100);
                break;
        }
    }



    #region ���ǿ� ���� DayEvent �߰��ϴ� �� (���Ͻ��۽� �ߵ�)

    void AddDayEventByCondition()
    {
        int pop = Main.Instance.PopularityOfDungeon;
        int danger = Main.Instance.DangerOfDungeon;

        if (danger >= 300)
        {
            DayEventLabel currentEvent = DayEventLabel.BloodSong_Appear;
            if (CurrentClearEventData.Check_AlreadyClear(currentEvent) == false && Check_AlreadyReserve(currentEvent) == false)
            {
                AddDayEvent(currentEvent, priority: 1, embargo: 0, delay: 0);
            }
        }
    }


    #endregion




    #region �̺�Ʈ ����Ʈ ���� ��� ���� (���̺����ϰ� ������. UserData.SavefileConfig������)
    public ClearEventData CurrentClearEventData { get; set; }
    public class ClearEventData
    {
        public HashSet<DayEventLabel> Clear_DayEventList;
        public HashSet<DialogueName> Clear_DialogueEventList;
        //List<int> Clear_EventList;


        public ClearEventData()
        {
            Clear_DayEventList = new HashSet<DayEventLabel>();
            Clear_DialogueEventList = new HashSet<DialogueName>();
        }

        public ClearEventData DeepCopy()
        {
            var data = new ClearEventData();
            data.Clear_DayEventList = new HashSet<DayEventLabel>(Clear_DayEventList);
            data.Clear_DialogueEventList = new HashSet<DialogueName>(Clear_DialogueEventList);
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

        CurrentTurn = LoadData.turn;

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
            if (item.InstanceQuestList.Count > 0)
            {
                switch (GuildManager.Instance.GetData(item.Original_Index).DayOption)
                {
                    case Guild_DayOption.Special:
                        foreach (var instNPC in GuildManager.Instance.Instance_GuildNPC)
                        {
                            if ((int)instNPC == item.Original_Index)
                            {
                                Debug.Log($"��� ����Ʈ �߻��� : {item.Original_Index}");
                                return true;
                            }
                        }
                        break;

                    case Guild_DayOption.Always:
                        Debug.Log($"��� ����Ʈ �߻��� : {item.Original_Index}");
                        return true;

                    case Guild_DayOption.Odd:
                        if (CurrentTurn % 2 == 1)
                        {
                            Debug.Log($"��� ����Ʈ �߻��� : {item.Original_Index}");
                            return true;
                        }
                        break;

                    case Guild_DayOption.Even:
                        if (CurrentTurn % 2 == 0)
                        {
                            Debug.Log($"��� ����Ʈ �߻��� : {item.Original_Index}");
                            return true;
                        }
                        break;

                    case Guild_DayOption.Multiple_3:
                        if (CurrentTurn % 3 == 0)
                        {
                            Debug.Log($"��� ����Ʈ �߻��� : {item.Original_Index}");
                            return true;
                        }
                        break;

                    case Guild_DayOption.Multiple_4:
                        if (CurrentTurn % 4 == 0)
                        {
                            Debug.Log($"��� ����Ʈ �߻��� : {item.Original_Index}");
                            return true;
                        }
                        break;

                    case Guild_DayOption.Multiple_5:
                        if (CurrentTurn % 5 == 0)
                        {
                            Debug.Log($"��� ����Ʈ �߻��� : {item.Original_Index}");
                            return true;
                        }
                        break;

                    case Guild_DayOption.Multiple_7:
                        if (CurrentTurn % 7 == 0)
                        {
                            Debug.Log($"��� ����Ʈ �߻��� : {item.Original_Index}");
                            return true;
                        }
                        break;
                }
            }
        }

        //? ���� �ɼ� ����Ʈ�� 2���̻��� �� ���˸�
        foreach (var item in CurrentGuildData)
        {
            if (item.Original_Index == 2000)
            {
                if (item.OptionList.Count >= 2)
                {
                    return true;
                }
            }
        }

        return false;
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
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_RetiredHero.ToString(), 9);
        });

        DayEventActionRegister.Add(DayEventLabel.Goblin_Appear, () => {
            Debug.Log("��� ù���� �̺�Ʈ");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin_Leader.ToString(), 9);
        });

        DayEventActionRegister.Add(DayEventLabel.Goblin_Party, () => {
            Debug.Log("��� ��Ƽ �̺�Ʈ");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin_Leader2.ToString(), 3);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 3.5f);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 4);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 4.5f);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.Event_Goblin.ToString(), 5);
        });

        DayEventActionRegister.Add(DayEventLabel.Catastrophe, () => {
            Debug.Log("������ ��� �̺�Ʈ");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Catastrophe.ToString(), 10);
        });

        DayEventActionRegister.Add(DayEventLabel.BloodSong_Appear, () => {
            Debug.Log("Blood Song �̺�Ʈ");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Tanker_A.ToString(), 10f);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Warrior_A.ToString(), 10.5f);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Wizard_A.ToString(), 11f);
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Blood_Elf_A.ToString(), 11.5f);
        });


        DayEventActionRegister.Add(DayEventLabel.Guild_Raid_1, () => {
            Debug.Log("��� ����� 1 �̺�Ʈ");
            Day25Event_Direction();
        });

        DayEventActionRegister.Add(DayEventLabel.Guild_Raid_2, () => {
            Debug.Log("��� ����� 2 �̺�Ʈ");
            Day30Event_Direction();
        });
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




    #region ��� ����
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
    public List<int> CurrentQuestAction_forSave { get; set; } = new List<int>();


    public void AddQuestAction(int _index)
    {
        CurrentQuestAction += GetQuestAction(_index);
        CurrentQuestAction_forSave.Add(_index);

        if (Managers.Scene.GetCurrentScene() == SceneName._2_Management)
        {
            FindAnyObjectByType<UI_Management>().QuestNotice();
        }
        else if (Managers.Scene.GetCurrentScene() == SceneName._3_Guild)
        {
            GuildManager.Instance.AddBackAction(() => FindAnyObjectByType<UI_Management>().QuestNotice());
        }
    }
    public void RemoveQuestAction(int _index)
    {
        CurrentQuestAction -= GetQuestAction(_index);
        CurrentQuestAction_forSave.Remove(_index);
    }




    //? ��忡�� ��ȭ�� ������ Action
    Dictionary<int, Action> GuildNPCAction = new Dictionary<int, Action>();

    //? ��Ÿ ��ȭ�� ����ǰ� �ٷ� ������ Action
    Dictionary<string, Action> EventAction = new Dictionary<string, Action>();

    //? �ܼ��� CurrentQuestEvent�� add / remove �ϴ� �뵵�θ� ���Ǿ����. �� GuildAction�� �ߺ��Ǽ� ���� ����, ���������� ���� ���� ����.
    Dictionary<int, Action> forQuestAction = new Dictionary<int, Action>();

    void AddForQuestAction()  //? ������ ȣ���� �׼�
    {

        forQuestAction.Add(1100, () =>
        {
            Debug.Log("����Ʈ - ��������� Ȱ��ȭ");
            GameManager.NPC.AddEventNPC(NPC_Type_Hunter.Hunter_Slime.ToString(), 12);
        });

        forQuestAction.Add(1101, () =>
        {
            Debug.Log("����Ʈ - ��� Ȱ��ȭ");
            GameManager.NPC.AddEventNPC(NPC_Type_Hunter.Hunter_EarthGolem.ToString(), 13);
        });


        forQuestAction.Add(1140, () =>
        {
            Debug.Log("������ ��� �����");
        });

        forQuestAction.Add(1141, () =>
        {
            Debug.Log("���ӵǴ� ���");
            GameManager.NPC.AddEventNPC(NPC_Type_MainEvent.EM_Catastrophe.ToString(), 10);
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

        forQuestAction.Add(774020, () =>
        {
            GameManager.NPC.AddEventNPC(NPC_Type_SubEvent.Heroine.ToString(), 3, NPC_Typeof.NPC_Type_SubEvent);
        });

        forQuestAction.Add(777010, () =>
        {
            Debug.Log("1�� ��巹�̵� �غ���");
        });

        forQuestAction.Add(777020, () =>
        {
            Debug.Log("2�� ��巹�̵� �غ���");
        });
    }
    void AddDialogueAction() //? ��ȭ�� ���ؼ� ȣ���ϴ°�. �ڵ�󿡴� ���� Dialogue�� Index�θ� ������
    {
        GuildNPCAction.Add(2100, () =>
        {
            Managers.Dialogue.ActionReserve(() => 
            {
                int ranPop = UnityEngine.Random.Range(10, 20 + CurrentTurn);
                var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
                msg.Message = $"{ranPop} {UserData.Instance.LocaleText("Message_Get_Pop")}";
                GuildManager.Instance.AddBackAction(() =>
                {
                    Main.Instance.CurrentDay.AddPop(ranPop);
                    //Debug.Log($"������ �α⵵�� {ranPop} �ö����ϴ�.");
                });
            });
        });

        GuildNPCAction.Add(2102, () => { AddQuestAction(772102); });



        GuildNPCAction.Add(1100, () => { AddQuestAction(1100); });
        GuildNPCAction.Add(1101, () => { AddQuestAction(1101); });
        GuildNPCAction.Add(1140, () => { AddQuestAction(1140); });
        //GuildNPCAction.Add(1151, () => { AddQuestAction(1151); });


        GuildNPCAction.Add(4040, () =>
        {
            GuildManager.Instance.AddBackAction(() =>
            {
                Debug.Log("������ �շ�");
                GameManager.Monster.Resize_MonsterSlot();
                var data = GameManager.Monster.GetData("Heroine");
                var mon = GameManager.Placement.CreatePlacementObject(data.prefabPath, null, PlacementType.Monster) as Monster;
                mon.MonsterInit();
                mon.Initialize_Status();
                mon.AddCollectionPoint();
                GameManager.Monster.AddMonster(mon);
            });
        });

        GuildNPCAction.Add(7010, () => { AddQuestAction(777010); });
        GuildNPCAction.Add(7020, () => { AddQuestAction(777020); });
    }

    //? ��� Main Event�� �����ؾ���
    void AddEventAction()
    {
        EventAction.Add("Tutorial_Orb", () => {
            Transform child = Main.Instance.Player.GetComponentInChildren<SpriteRenderer>().transform;
            child.localScale = new Vector3(-1, 1, 1);

            Camera.main.GetComponent<CameraControl>().ChasingTarget(Main.Instance.EggObj.transform, 2);
        });

        EventAction.Add("Tutorial_Orb_Over", () => {
            Transform child = Main.Instance.Player.GetComponentInChildren<SpriteRenderer>().transform;
            child.localScale = Vector3.one;
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



        EventAction.Add("RedHair_Return", () =>
        {
            Debug.Log("RedHair_Return - Goblin ����");
            AddDayEvent(DayEventLabel.Goblin_Appear, priority: 0, embargo: 9, delay: 1);
        });

        EventAction.Add("Goblin_Satisfiction", () =>
        {
            Debug.Log("��� ���� - ��� ��Ƽ �̺�Ʈ ����");
            AddDayEvent(DayEventLabel.Goblin_Party, priority: 0, embargo: 12, delay: 0);
            AddQuestAction(1150); //? �����Ƽ �ٷ� ����Ʈ�� �߰�

            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 2, true);
        });

        EventAction.Add("Goblin_Pass", () =>
        {
            Debug.Log("��� Die or Empty - ������ ��� �̺�Ʈ");
            AddDayEvent(DayEventLabel.Catastrophe, priority: 0, embargo: 12, delay: 0);
            Add_GuildQuest_Special(3014); //? �������� �ҽĵ�� ����Ʈ �߰� - ���� 1140���� ����
        });


        EventAction.Add("Catastrophe_Refeat", () =>
        {
            Debug.Log("������ ��� - �ذ��� �� ���� ���� �̺�Ʈ");
            RemoveQuestAction(1140);
            AddQuestAction(1141); //? �ذ�Ǳ������� 1141�� ���ӹ߻��Ǵ� �̺�Ʈ(����Ʈ���͸���)
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
        Debug.Log("Ŭ���� �� �ӽ����� �κ� / �������� �� ���Ѹ�� Ȱ��ȭ�� �� �ٽ� Ȱ��ȭ�ؾ���");
        //Temp_saveData = Managers.Data.SaveCurrentData("Clear_Temp");

        yield return new WaitForSecondsRealtime(1);
        Managers.Scene.LoadSceneAsync(SceneName._7_NewEnding, false);
    }

    #region MainEvent �󼼳���

    void Day25Event_Direction()
    {
        var Dungeon = Main.Instance.Dungeon;
        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);


        List<NPC> sol1List = new List<NPC>();
        List<NPC> sol2List = new List<NPC>();

        var cap_A = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Captine_A.ToString());
        cap_A.transform.position = Dungeon.transform.position + (Vector3.right * 1.5f);
        GameManager.Placement.Visible(cap_A);

        for (int i = 0; i < 7; i++)
        {
            var sol_1 = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Soldier1.ToString());
            sol_1.transform.position = Dungeon.transform.position + (Vector3.right * 0.5f * i) + Vector3.right * 2.5f;
            sol_1.Anim_State = NPC.animState.left;
            sol_1.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol_1);
            sol1List.Add(sol_1);
        }

        var cap_B = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Captine_B.ToString());
        cap_B.transform.position = Dungeon.transform.position + (Vector3.right * -1.5f);
        cap_B.Anim_State = NPC.animState.left;
        cap_B.Anim_State = NPC.animState.Idle;
        GameManager.Placement.Visible(cap_B);

        for (int i = 0; i < 7; i++)
        {
            var sol_1 = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Soldier2.ToString());
            sol_1.transform.position = Dungeon.transform.position + (Vector3.right * -0.5f * i) + Vector3.right * -2.5f;
            sol_1.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol_1);
            sol2List.Add(sol_1);
        }

        Managers.Dialogue.ShowDialogueUI(DialogueName.Guild_Raid_1, cap_A.transform);
        StartCoroutine(Wait_Day25_Dialogue(cap_A, cap_B, sol1List, sol2List));
    }
    IEnumerator Wait_Day25_Dialogue(NPC cap_A, NPC cap_B, List<NPC> sol1, List<NPC> sol2)
    {
        var Dungeon = Main.Instance.Dungeon;
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
        var Dungeon = Main.Instance.Dungeon;

        GameManager.NPC.CustomStage = true;
        UserData.Instance.GameMode = Define.GameMode.Stop;

        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteIn, 1, true);


        List<NPC> bloodSong = new List<NPC>();
        //? ���ǳ뷡 ��Ƽ�� ����
        {
            var party = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Blood_Tanker_B.ToString());
            party.transform.position = Dungeon.transform.position + (Vector3.left * 6.5f);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }
        {
            var party = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Blood_Warrior_B.ToString());
            party.transform.position = Dungeon.transform.position + (Vector3.left * 7);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }
        {
            var party = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Blood_Wizard_B.ToString());
            party.transform.position = Dungeon.transform.position + (Vector3.left * 7.5f);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }
        {
            var party = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Blood_Elf_B.ToString());
            party.transform.position = Dungeon.transform.position + (Vector3.left * 8);
            GameManager.Placement.Visible(party);
            bloodSong.Add(party);
        }

        //? ����� ����
        var Cap_A = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Captine_A.ToString());
        Cap_A.transform.position = Dungeon.transform.position + (Vector3.right * 1);
        Cap_A.Anim_State = NPC.animState.right;
        Cap_A.Anim_State = NPC.animState.Ready;
        GameManager.Placement.Visible(Cap_A);

        var Cap_B = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Captine_B.ToString());
        Cap_B.transform.position = Dungeon.transform.position + (Vector3.right * 5);
        Cap_B.Anim_State = NPC.animState.right;
        Cap_B.Anim_State = NPC.animState.Ready;
        GameManager.Placement.Visible(Cap_B);

        var Captine_C = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Captine_BlueKnight.ToString());
        Captine_C.transform.position = Dungeon.transform.position + (Vector3.left * 1.5f);
        GameManager.Placement.Visible(Captine_C);


        List<NPC> sol1List = new List<NPC>();
        List<NPC> sol2List = new List<NPC>();
        List<NPC> sol3List = new List<NPC>();

        for (int i = 0; i < 5; i++)
        {
            var sol = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Soldier1.ToString());
            sol.transform.position = Dungeon.transform.position + (Vector3.right * 0.5f * i) + Vector3.right * 2.0f;
            sol.Anim_State = NPC.animState.left;
            sol.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol);
            sol1List.Add(sol);
        }
        for (int i = 0; i < 5; i++)
        {
            var sol = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Soldier2.ToString());
            sol.transform.position = Dungeon.transform.position + (Vector3.right * 0.5f * i) + Vector3.right * 6.0f;
            sol.Anim_State = NPC.animState.left;
            sol.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol);
            sol2List.Add(sol);
        }
        for (int i = 0; i < 5; i++)
        {
            var sol = GameManager.NPC.InstantiateNPC_Event(NPC_Type_MainEvent.EM_Soldier3.ToString());
            sol.transform.position = Dungeon.transform.position + (Vector3.left * 0.5f * i) + Vector3.left * 2.5f;
            sol.Anim_State = NPC.animState.right;
            sol.Anim_State = NPC.animState.Ready;

            GameManager.Placement.Visible(sol);
            sol3List.Add(sol);
        }

        Managers.Dialogue.ShowDialogueUI(DialogueName.Guild_Raid_2, Captine_C.transform);
        StartCoroutine(Wait_Day30_Dialogue(Cap_A, Cap_B, Captine_C, sol1List, sol2List, sol3List, bloodSong));
    }
    IEnumerator Wait_Day30_Dialogue(NPC cap_A, NPC cap_B, NPC cap_C, List<NPC> sol1, List<NPC> sol2, List<NPC> sol3, List<NPC> bloodSong)
    {
        var Dungeon = Main.Instance.Dungeon;

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
        Vector2Int eggExit = new Vector2Int(11, 2);

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
    void FirstPortalAppearSkip()
    {
        Vector2Int portalIndex = new Vector2Int(2, 2);
        Vector2Int eggExit = new Vector2Int(11, 2);

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
        Vector2Int portal_5 = new Vector2Int(6, 19);

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



    public bool TryRankUp(int fame, int danger)
    {
        UI_Management mainUI = FindAnyObjectByType<UI_Management>();

        if (Main.Instance.DungeonRank == 1 && fame + danger >= 100)
        {
            mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Facility, true);
            //mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Summon, true);
            mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Monster, true);
            return true;
        }

        if (Main.Instance.DungeonRank == 2 && fame + danger >= 400)
        {
            mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Facility, true);
            //mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Summon, true);
            mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Monster, true);
            return true;
        }

        //? �� ������ �α⵵�� ���赵 ���̰� 300 �̻��� �� ��ũ�� �� �� �ֵ��� (�Ѵٿø��°� �������� ����)
        //? ���� ��ġ�� �ʹ� Ŀ���� �� ���� �����ص���.


        return false;
    }


    public void RankUpEvent()
    {
        FindObjectOfType<Player>().Level_Stat(Main.Instance.DungeonRank);
        GameManager.Monster.Resize_MonsterSlot();

        //if (Main.Instance.DungeonRank >= (int)Define.DungeonRank.D && Main.Instance.ActiveFloor_Basement < 5)
        //{
        //    FindAnyObjectByType<UI_Management>().SetNotice(UI_Management.OverlayImages.OverlayImage_Dungeon, true);
        //}
        //else if (Main.Instance.DungeonRank >= (int)Define.DungeonRank.C && Main.Instance.ActiveFloor_Basement < 6)
        //{
        //    FindAnyObjectByType<UI_Management>().SetNotice(UI_Management.OverlayImages.OverlayImage_Dungeon, true);
        //}
    }

}

public enum DayEventLabel
{
    Test1 = 0,
    Test2 = 1,
    Test3 = 2,

    Goblin_Appear = 93,
    Goblin_Party = 100,

    Catastrophe = 140,

    RetiredHero = 153,

    BloodSong_Appear = 200,

    Guild_Raid_1 = 250,
    Guild_Raid_2 = 300,
}




