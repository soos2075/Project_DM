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
        CurrentTurn = Main.Instance.Turn;
        CurrentQuestAction?.Invoke();

        TurnStart_EventSchedule();
        Add_ReservationQuest();

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
        TurnOver_EventSchedule();
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
    }



    #region ��� �湮 �˸� ǥ��
    public bool CheckGuildNotice()
    {
        var guildData = CurrentGuildData;
        if (CurrentGuildData == null)
        {
            Debug.Log("���� ��� ������ ����");
            return false;
        }

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
            GameManager.NPC.AddEventNPC(EventNPCType.Event_RetiredHero, 9);
        });

        //DayEventActionRegister.Add(DayEventLabel.Test1, () => Debug.Log("�׽�Ʈ1 �����̺�Ʈ ����"));
        //DayEventActionRegister.Add(DayEventLabel.Test2, () => Debug.Log("�׽�Ʈ2 �����̺�Ʈ ����"));
        //DayEventActionRegister.Add(DayEventLabel.Test3, () => Debug.Log("�׽�Ʈ3 �����̺�Ʈ ����"));

        DayEventActionRegister.Add(DayEventLabel.Goblin_Appear, () => {
            Debug.Log("��� ù���� �̺�Ʈ");
            GameManager.NPC.AddEventNPC(EventNPCType.Event_Goblin_Leader, 9);
        });

        DayEventActionRegister.Add(DayEventLabel.Goblin_Party, () => {
            Debug.Log("��� ��Ƽ �̺�Ʈ");
            GameManager.NPC.AddEventNPC(EventNPCType.Event_Goblin_Leader2, 3);
            GameManager.NPC.AddEventNPC(EventNPCType.Event_Goblin, 3.5f);
            GameManager.NPC.AddEventNPC(EventNPCType.Event_Goblin, 4);
            GameManager.NPC.AddEventNPC(EventNPCType.Event_Goblin, 4.5f);
            GameManager.NPC.AddEventNPC(EventNPCType.Event_Goblin, 5);
        });

        DayEventActionRegister.Add(DayEventLabel.Catastrophe, () => {
            Debug.Log("������ ��� �̺�Ʈ");
            GameManager.NPC.AddEventNPC(EventNPCType.Event_Catastrophe, 10);
        });
    }


    void Run_DayEventAction(DayEventLabel dayEventName)
    {
        Action act = null;
        if (DayEventActionRegister.TryGetValue(dayEventName, out act))
        {
            act.Invoke();
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


    //? ������ ��带 �ǰ��ٰ� ����Ʈ�� �߰��Ǹ� �ȵǱ� ������ ��¥ �����ϱ�
    public List<Quest_Reservation> Reservation_Quest { get; set; } = new List<Quest_Reservation>();
    public void ReservationToQuest(int day, int questIndex)
    {
        Reservation_Quest.Add(new Quest_Reservation(day, questIndex));
    }

    public void Add_ReservationQuest() //? ���� ���� �ٲ𶧸���
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
            GameManager.NPC.AddEventNPC(EventNPCType.Hunter_Slime, 12);
        });

        forQuestAction.Add(1101, () =>
        {
            Debug.Log("����Ʈ - ��� Ȱ��ȭ");
            GameManager.NPC.AddEventNPC(EventNPCType.Hunter_EarthGolem, 13);
        });


        forQuestAction.Add(1140, () =>
        {
            Debug.Log("������ ��� �����");
        });

        forQuestAction.Add(1141, () =>
        {
            Debug.Log("���ӵǴ� ���");
            GameManager.NPC.AddEventNPC(EventNPCType.Event_Catastrophe, 10);
        });

        forQuestAction.Add(1150, () =>
        {
            Debug.Log("��� ��Ƽ �湮 �����");
        });

        forQuestAction.Add(1151, () =>
        {
            Debug.Log("������ ���� �湮 �����");
        });

        forQuestAction.Add(772102, () =>
        {
            Debug.Log("���� ������ �湮Ȯ�� 3��!!");
            GameManager.NPC.Event_Herb = true;
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
    }


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
            StartCoroutine(EntranceMove_2to4());
        });



        //? Diglogue�� ���� ȣ��
        EventAction.Add("Day8_Event_Die", () => 
        {
            Debug.Log("����ռ����谡 �й� �̺�Ʈ - ������ ���� �̺�Ʈ ����");
            AddDayEvent(DayEventLabel.RetiredHero, priority: 0, embargo: 10, delay: 0);
            GuildManager.Instance.AddInstanceGuildNPC(GuildNPC_LabelName.RetiredHero);

            //? ���谡 �й��ϸ� �����̾��� �ٷ� �̺�Ʈ �߰��ϱ�(�ϴ� BIC ���� �ӽ÷� �ϴ°ű���)
            //ReservationToQuest(1, 15010); 
            Add_GuildQuest_Special(15010);

            var e8 = GameObject.Find("Event_Day8");
            if (e8 != null)
            {
                GameManager.Placement.Disable(e8.GetComponent<EventNPC>());

                var fade = Managers.UI.ShowPopUp<UI_Fade>();
                fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 2, true);
            }
        });

        EventAction.Add("Hero_Die", () =>
        {
            Debug.Log("���� �й� �̺�Ʈ");
            var e8 = GameObject.Find("Event_RetiredHero");
            if (e8 != null)
            {
                GameManager.Placement.Disable(e8.GetComponent<EventNPC>());

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
            Debug.Log("������ ��ȭ2");
            var fade = Managers.UI.ShowPopUp<UI_Fade>();
            fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 1, true);

            var player = GameObject.Find("Player");
            //player.GetComponentInChildren<SpriteRenderer>().transform.localScale = new Vector3(-1, 1, 1);
            player.transform.position = GuildHelper.Instance.GetPos(GuildHelper.Pos.Table2).position;

            FindAnyObjectByType<UI_DialogueBubble>().Bubble_MoveToTarget(player.transform);
        });



        EventAction.Add("Day8_ReturnEvent", () =>
        {
            Debug.Log("����ռ����谡 ���� �̺�Ʈ - ������丮 ����");
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
            AddDayEvent(DayEventLabel.Catastrophe, priority: 0, embargo: 15, delay: 0);
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

        GameObject player = GameObject.Find("Player");
        GameObject floor3 = GameObject.Find("BasementFloor_3");

        cam.ChasingTarget(floor3.transform.position + new Vector3(-6, -3, 0), 1.5f);
        yield return new WaitForSeconds(2);
        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].TileMap.TryGetValue(new Vector2Int(0, 0), out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
        }

        yield return new WaitForSeconds(2);
        cam.ChasingTarget(player.transform.position + new Vector3(6, 0, 0), 1.5f);
        yield return new WaitForSeconds(2);
        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Egg].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Egg].TileMap.TryGetValue(new Vector2Int(12, 2), out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Egg], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("Exit", info);
        }

        yield return new WaitForSeconds(2);
        Managers.Dialogue.AllowPerfectSkip = true;
    }

    void FirstPortalAppearSkip()
    {
        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].TileMap.TryGetValue(new Vector2Int(0, 0), out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
        }
        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Egg].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Egg].TileMap.TryGetValue(new Vector2Int(12, 2), out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Egg], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("Exit", info);
        }
    }


    IEnumerator EntranceMove_2to4()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        if (Managers.Dialogue.GetState() == DialogueManager.DialogueState.None)
        {
            EntranceMove_2to4_Skip();
            yield break;
        }

        Managers.Dialogue.AllowPerfectSkip = false;

        Camera.main.GetComponent<CameraControl>().ChasingTarget(new Vector3(0, -15, 0), 2);
        yield return new WaitForSecondsRealtime(2);

        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].TileMap.TryGetValue(new Vector2Int(0, 0), out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
        }
        yield return new WaitForSecondsRealtime(2);

        Camera.main.GetComponent<CameraControl>().ChasingTarget(new Vector3(0, -18, 0), 2);
        yield return new WaitForSecondsRealtime(2);

        {
            var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].GetRandomTile();
            Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].TileMap.TryGetValue(new Vector2Int(1, 15), out tile);
            // �ε����Ͽ��� �׽�Ʈ�� ��
            if (tile.Original != null)
            {
                GameManager.Facility.RemoveFacility(tile.Original as Facility);
            }

            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
        }
        yield return new WaitForSecondsRealtime(2);

        Camera.main.GetComponent<CameraControl>().ChasingTarget(Main.Instance.Player, 2);
        yield return new WaitForSecondsRealtime(2);

        var ent = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].Entrance;
        var exi = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].Exit;

        Managers.Dialogue.AllowPerfectSkip = true;
    }


    void EntranceMove_2to4_Skip()
    {
        if (Managers.Dialogue.GetState() == DialogueManager.DialogueState.None)
        {
            {
                var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].GetRandomTile();
                Main.Instance.Floor[(int)Define.DungeonFloor.Floor_3].TileMap.TryGetValue(new Vector2Int(0, 0), out tile);
                GameManager.Facility.RemoveFacility(tile.Original as Facility);
            }
            {
                var tile = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].GetRandomTile();
                Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].TileMap.TryGetValue(new Vector2Int(1, 15), out tile);
                // �ε����Ͽ��� �׽�Ʈ�� ��
                if (tile.Original != null)
                {
                    GameManager.Facility.RemoveFacility(tile.Original as Facility);
                }

                PlacementInfo info = new PlacementInfo(Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4], tile);
                var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
            }

            var ent = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].Entrance;
            var exi = Main.Instance.Floor[(int)Define.DungeonFloor.Floor_4].Exit;
        }
    }

    #endregion





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
            mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Summon, true);
            return true;
        }

        if (Main.Instance.DungeonRank == 2 && fame + danger >= 400)
        {
            mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Facility, true);
            mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Summon, true);
            return true;
        }

        return false;
    }


    public void RankUpEvent()
    {
        FindObjectOfType<Player>().Level_Stat(Main.Instance.DungeonRank);
        Camera.main.GetComponent<CameraControl>().LimitRefresh();
        GameManager.Monster.Resize_MonsterSlot();

        if (Main.Instance.DungeonRank >= 2)
        {
            Main.Instance.DungeonExpansionUI();
        }
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

}




