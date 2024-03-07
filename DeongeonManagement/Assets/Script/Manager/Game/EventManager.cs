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


    //? 길드정보 저장용(길드에서 변화된 npc들의 퀘스트 정보들이 들어있음)
    public List<GuildNPC_Data> CurrentGuildData { get; set; }

    //? 길드가면 추가시켜야 될 퀘스트 리스트
    public List<int> GuildQuestAdd { get; set; } = new List<int>();


    //? 현재 진행중인 퀘스트 목록 - 실제 매턴 실행될 Action
    public Action CurrentQuestEvent { get; private set; }

    //? 현재 진행중인 퀘스트 목록(dataManager에서 저장용으로만 사용)
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




    //? 길드에서 대화로 실행할 Action
    Dictionary<int, Action> GuildNPCAction = new Dictionary<int, Action>();

    //? 기타 대화가 종료되고 바로 실행할 Action
    Dictionary<string, Action> EventAction = new Dictionary<string, Action>();

    //? 단순히 CurrentQuestEvent에 add / remove 하는 용도로만 사용되어야함. 또 GuildAction과 중복되서 사용될 수도, 독립적으로 사용될 수도 있음.
    Dictionary<int, Action> forQuestAction = new Dictionary<int, Action>();

    void AddForQuestAction() 
    {
        forQuestAction.Add(1100, () =>
        {
            Debug.Log("퀘스트 - 슬라임토벌 활성화");
            GameManager.NPC.AddEventNPC(NPCManager.NPCType.Hunter_Slime, 12);
        });

        forQuestAction.Add(1101, () =>
        {
            Debug.Log("퀘스트 - 어스골렘 활성화");
            GameManager.NPC.AddEventNPC(NPCManager.NPCType.Hunter_EarthGolem, 13);
        });

        forQuestAction.Add(1102, () =>
        {
            Debug.Log("1102 퀘스트");
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
            msg.Message = $"던전의 인기도가 {ranPop} 올랐습니다.";
            FindAnyObjectByType<GuildManager>().AddBackAction(() =>
            {
                Main.Instance.CurrentDay.AddPop(ranPop);
                //Debug.Log($"던전의 인기도가 {ranPop} 올랐습니다.");
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
            message.Message = "숨겨진 방으로 향하는 통로가 생겼습니다.\n\n이후로 이 방에 누군가 침입해 알이 발견당하면 게임오버가 됩니다.";
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

