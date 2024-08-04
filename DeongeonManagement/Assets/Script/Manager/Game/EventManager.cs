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
    }




    public void TurnStart()
    {
        CurrentTurn = Main.Instance.Turn;
        CurrentQuestAction?.Invoke();

        Add_ReservationQuest();
    }

    public void TurnOver()
    {
        Add_Daily(2100);
    }


    public void QuestDataReset()
    {
        CurrentTurn = 0;
        CurrentGuildData = new List<GuildNPC_Data>();
        AddQuest_Special = new List<int>();
        AddQuest_Daily = new List<int>();
        CurrentQuestAction = null;
        CurrentQuestAction_forSave.Clear();
        DayEventList = new List<DayEvent>();

        Reservation_Quest = new List<Quest_Reservation>();
    }




    public DataManager.SaveData_EventData Data_SaveEventManager()
    {
        DataManager.SaveData_EventData save = new DataManager.SaveData_EventData();

        if (CurrentGuildData != null)
        {
            save.CurrentGuildData = new List<GuildNPC_Data>();

            foreach (var item in CurrentGuildData)
            {
                //var newData = new GuildNPC_Data();
                //newData.SetData_Deep(item.Original_Index, new List<int>(item.InstanceQuestList), new List<int>(item.OptionList));
                //save.CurrentGuildData.Add(newData);

                save.CurrentGuildData.Add(item.DeepCopy());
            }
        }



        if (AddQuest_Special != null)
        {
            save.AddQuest_Special = new List<int>(AddQuest_Special);
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
            save.DayEventList = new List<DayEvent>(DayEventList);
        }

        if (Reservation_Quest != null)
        {
            save.Reservation_Quest = new List<Quest_Reservation>(Reservation_Quest);
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
            CurrentGuildData.AddRange(LoadData.eventData.CurrentGuildData);
        }

        if (LoadData.eventData.AddQuest_Special != null)
        {
            AddQuest_Special.AddRange(LoadData.eventData.AddQuest_Special);
        }

        if (LoadData.eventData.AddQuest_Daily != null)
        {
            AddQuest_Daily.AddRange(LoadData.eventData.AddQuest_Daily);
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
            DayEventList = new List<DayEvent>(LoadData.eventData.DayEventList);
        }

        if (LoadData.eventData.Reservation_Quest != null)
        {
            Reservation_Quest = new List<Quest_Reservation>(LoadData.eventData.Reservation_Quest);
        }
    }



    #region ��� �湮 �˸� ǥ��
    public bool CheckGuildNotice_Wating()
    {
        return AddQuest_Special.Count > 0 ? true : false;
    }


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
                    case Guild_DayOption.Always:
                        Debug.Log($"����Ʈ �߻� : {item.Original_Index}");
                        return true;

                    case Guild_DayOption.Odd:
                        if (CurrentTurn % 2 == 1)
                        {
                            Debug.Log($"����Ʈ �߻� : {item.Original_Index}");
                            return true;
                        }
                        break;

                    case Guild_DayOption.Even:
                        if (CurrentTurn % 2 == 0)
                        {
                            Debug.Log($"����Ʈ �߻� : {item.Original_Index}");
                            return true;
                        }
                        break;

                    case Guild_DayOption.Multiple_3:
                        if (CurrentTurn % 3 == 0)
                        {
                            Debug.Log($"����Ʈ �߻� : {item.Original_Index}");
                            return true;
                        }
                        break;

                    case Guild_DayOption.Multiple_4:
                        if (CurrentTurn % 4 == 0)
                        {
                            Debug.Log($"����Ʈ �߻� : {item.Original_Index}");
                            return true;
                        }
                        break;

                    case Guild_DayOption.Multiple_5:
                        if (CurrentTurn % 5 == 0)
                        {
                            Debug.Log($"����Ʈ �߻� : {item.Original_Index}");
                            return true;
                        }
                        break;

                    case Guild_DayOption.Multiple_7:
                        if (CurrentTurn % 7 == 0)
                        {
                            Debug.Log($"����Ʈ �߻� : {item.Original_Index}");
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

    public class DayEvent
    {
        public int EventIndex;

        //? ����Ʈ�� �켱����. ���ڰ� �������� �켱������ ����
        public int Priority;

        public DayEvent()
        {

        }
        public DayEvent(int index, int priority)
        {
            EventIndex = index;
            Priority = priority;
        }
    }

    public List<DayEvent> DayEventList { get; set; } = new List<DayEvent>();


    public DayEvent GetDayEvent()
    {
        if (DayEventList.Count == 0)
        {
            return null;
        }
        else
        {
            var temp = DayEventList[0];

            foreach (var item in DayEventList)
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

    public void AddDayEvent(int questIndex, int priority)
    {
        DayEventList.Add(new DayEvent(questIndex, priority));
    }

    #endregion





    public int CurrentTurn { get; set; }


    //? ������� �����(��忡�� ��ȭ�� npc���� ����Ʈ �������� �������)
    public List<GuildNPC_Data> CurrentGuildData { get; set; }

    //? ��尡�� �߰����Ѿ� �� ����Ʈ ����Ʈ
    public List<int> AddQuest_Special { get; set; } = new List<int>();

    public void Add_Special(int index)
    {
        AddQuest_Special.Add(index);
        AddQuest_Special = Util.ListDistinct(AddQuest_Special);
    }


    //? ������ ��带 �ǰ��ٰ� ����Ʈ�� �߰��Ǹ� �ȵǱ� ������ ��¥ �����ϱ�
    public List<Quest_Reservation> Reservation_Quest = new List<Quest_Reservation>();
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
                Add_Special(item.questIndex);
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

    void AddForQuestAction() 
    {
        forQuestAction.Add(1100, () =>
        {
            Debug.Log("����Ʈ - ��������� Ȱ��ȭ");
            GameManager.NPC.AddEventNPC(NPCManager.NPCType.Hunter_Slime, 12);
        });

        forQuestAction.Add(1101, () =>
        {
            Debug.Log("����Ʈ - ��� Ȱ��ȭ");
            GameManager.NPC.AddEventNPC(NPCManager.NPCType.Hunter_EarthGolem, 13);
        });

        forQuestAction.Add(1102, () =>
        {
            Debug.Log("1102 ����Ʈ");
        });
    }
    void AddDialogueAction()
    {
        GuildNPCAction.Add(2100, () =>
        {
            UserData.Instance.GameMode = Define.GameMode.Normal;
            //Time.timeScale = 1;
            Managers.UI.ClosePopUp();
            Managers.Dialogue.currentDialogue = null;

            int ranPop = UnityEngine.Random.Range(10, 20 + CurrentTurn);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = $"{ranPop} {UserData.Instance.LocaleText("Message_Get_Pop")}";
            GuildManager.Instance.AddBackAction(() =>
            {
                Main.Instance.CurrentDay.AddPop(ranPop);
                //Debug.Log($"������ �α⵵�� {ranPop} �ö����ϴ�.");
                });
        });

        GuildNPCAction.Add(1100, () =>
        {
            AddQuestAction(1100);
        });

        GuildNPCAction.Add(1101, () =>
        {
            AddQuestAction(1101);
        });

        GuildNPCAction.Add(1102, () =>
        {
            AddQuestAction(1102);
        });
    }
    void AddEventAction()
    {
        //EventAction.Add("Message_Tutorial_AP", () => 
        //{
        //    var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
        //    message.DelayTime = 2;
        //    message.Message = UserData.Instance.LocaleText("Message_Tutorial_AP");
        //});



        EventAction.Add("EggAppear", () => {
            var tile = Main.Instance.Floor[3].GetRandomTile();
            Main.Instance.Floor[3].TileMap.TryGetValue(new Vector2Int(12, 2), out tile);

            GameManager.Facility.RemoveFacility(tile.Original as Facility);

            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("Exit", info);
        });

        EventAction.Add("EggEntrance", () =>
        {
            var tile = Main.Instance.Floor[2].GetRandomTile();
            Main.Instance.Floor[2].TileMap.TryGetValue(new Vector2Int(0, 0), out tile);

            GameManager.Facility.RemoveFacility(tile.Original as Facility);

            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[2], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
        });


        EventAction.Add("EggMessage", () =>
        {
            //Time.timeScale = 1;
            //Managers.UI.ClosePopUp();
            //Managers.Dialogue.currentDialogue = null;
            var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
            message.DelayTime = 2;
            message.Message = UserData.Instance.LocaleText("Message_Egg");
        });

        EventAction.Add("Entrance_Move_2to4", () =>
        {
            StartCoroutine(EntranceMove_2to4());
        });



        //EventAction.Add("Ending", () =>
        //{
        //    StartCoroutine(WaitEnding(1));
        //});

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
            var tile = Main.Instance.Floor[2].GetRandomTile();
            Main.Instance.Floor[2].TileMap.TryGetValue(new Vector2Int(0, 0), out tile);
            GameManager.Facility.RemoveFacility(tile.Original as Facility);
        }
        yield return new WaitForSecondsRealtime(2);

        Camera.main.GetComponent<CameraControl>().ChasingTarget(new Vector3(0, -18, 0), 2);
        yield return new WaitForSecondsRealtime(2);

        {
            var tile = Main.Instance.Floor[4].GetRandomTile();
            Main.Instance.Floor[4].TileMap.TryGetValue(new Vector2Int(1, 15), out tile);
            // �ε����Ͽ��� �׽�Ʈ�� ��
            if (tile.Original != null)
            {
                GameManager.Facility.RemoveFacility(tile.Original as Facility);
            }

            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[4], tile);
            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
        }
        yield return new WaitForSecondsRealtime(2);

        Camera.main.GetComponent<CameraControl>().ChasingTarget(Main.Instance.Player, 2);
        yield return new WaitForSecondsRealtime(2);

        Managers.Dialogue.AllowPerfectSkip = true;
    }


    void EntranceMove_2to4_Skip()
    {
        if (Managers.Dialogue.GetState() == DialogueManager.DialogueState.None)
        {
            {
                var tile = Main.Instance.Floor[2].GetRandomTile();
                Main.Instance.Floor[2].TileMap.TryGetValue(new Vector2Int(0, 0), out tile);
                GameManager.Facility.RemoveFacility(tile.Original as Facility);
            }
            {
                var tile = Main.Instance.Floor[4].GetRandomTile();
                Main.Instance.Floor[4].TileMap.TryGetValue(new Vector2Int(1, 15), out tile);
                // �ε����Ͽ��� �׽�Ʈ�� ��
                if (tile.Original != null)
                {
                    GameManager.Facility.RemoveFacility(tile.Original as Facility);
                }

                PlacementInfo info = new PlacementInfo(Main.Instance.Floor[4], tile);
                var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info);
            }
        }
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

        if (Main.Instance.DungeonRank == 1 && fame + danger >= 150)
        {
            mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Facility, true);
            mainUI.SetNotice(UI_Management.OverlayImages.OverlayImage_Summon, true);
            return true;
        }

        if (Main.Instance.DungeonRank == 2 && fame + danger >= 500)
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




