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
        CurrentQuestEvent?.Invoke();
    }


    public int CurrentTurn { get; set; }


    //? ������� �����(��忡�� ��ȭ�� npc���� ����Ʈ �������� �������)
    public List<GuildNPC_Data> CurrentGuildData { get; set; }

    //? ��尡�� �߰����Ѿ� �� ����Ʈ ����Ʈ
    public List<int> GuildQuestAdd { get; set; } = new List<int>();


    //? ���� �������� ����Ʈ ��� - ���� ���� ����� Action
    public Action CurrentQuestEvent { get; private set; }

    //? ���� �������� ����Ʈ ���(dataManager���� ��������θ� ���)
    public List<int> CurrentQuestEvent_ForSave { get; set; } = new List<int>();


    public void Load_QuestEvent(List<int> _loadData)
    {
        CurrentQuestEvent = null;
        CurrentQuestEvent_ForSave.Clear();

        if (_loadData == null)
        {
            return;
        }
        foreach (var item in _loadData)
        {
            AddQuestAction(item);
        }
    }

    public void AddQuestAction(int _index)
    {
        CurrentQuestEvent += GetQuestAction(_index);
        CurrentQuestEvent_ForSave.Add(_index);
    }
    public void RemoveQuestAction(int _index)
    {
        CurrentQuestEvent -= GetQuestAction(_index);
        CurrentQuestEvent_ForSave.Remove(_index);
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
            Time.timeScale = 1;
            Managers.UI.ClosePopUp();
            Managers.Dialogue.currentDialogue = null;

            int ranPop = UnityEngine.Random.Range(10, 20);
            var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
            msg.Message = $"������ �α⵵�� {ranPop} �ö����ϴ�.";
            FindAnyObjectByType<GuildManager>().AddBackAction(() =>
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
        EventAction.Add("EggAppear", () => {
            var tile = Main.Instance.Floor[3].GetRandomTile();
            Main.Instance.Floor[3].TileMap.TryGetValue(new Vector2Int(12, 3), out tile);

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
            message.Message = "������ ������ ���ϴ� ��ΰ� ������ϴ�.\n\n���ķ� �� �濡 ������ ħ���� ���� �߰ߴ��ϸ� ���ӿ����� �˴ϴ�.";
        });


        EventAction.Add("Ending", () =>
        {
            StartCoroutine(WaitEnding(1));
        });

    }
    IEnumerator WaitEnding(float _time)
    {
        var fade = Managers.UI.ClearAndShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.WhiteOut, 1);
        UserData.Instance.GameClear();

        yield return new WaitForSecondsRealtime(_time);
        Managers.Scene.LoadSceneAsync(SceneName._5_Ending, false);
    }

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
        if (Main.Instance.DungeonRank == 1 && fame + danger >= 200)
        {
            return true;
        }

        if (Main.Instance.DungeonRank == 2 && fame + danger >= 500)
        {
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


    public void Reset_Singleton()
    {
        CurrentGuildData?.Clear();
        GuildQuestAdd?.Clear();
        CurrentQuestEvent = null;
        CurrentQuestEvent_ForSave?.Clear();
        CurrentTurn = 0;
    }
}

