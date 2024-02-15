using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
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
        Init();
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    private void Start()
    {
        
    }
    public void Init()
    {
        DialogueAction = new Dictionary<int, Action>();
        EventAction = new Dictionary<string, Action>();

        AddDialogueAction();
        AddEventAction();
    }

    Dictionary<int, Action> DialogueAction;
    Dictionary<string, Action> EventAction;


    void AddDialogueAction()
    {
        DialogueAction.Add(2100, () => GuildManager.Instance.AddBackAction(() => Main.Instance.CurrentDay.Fame += 15));
    }
    void AddEventAction()
    {
        EventAction.Add("DungeonLevelUp", () => DungeonLvUp());

        EventAction.Add("EggAppear", () => {
            var tile = Main.Instance.Floor[3].GetRandomTile();
            Main.Instance.Floor[3].TileMap.TryGetValue(new Vector2Int(12, 3), out tile);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[3], tile);

            var obj = GameManager.Facility.CreateFacility_OnlyOne("Exit", info, true);
        });

        EventAction.Add("EggEntrance", () =>
        {
            var tile = Main.Instance.Floor[2].GetRandomTile();
            Main.Instance.Floor[2].TileMap.TryGetValue(new Vector2Int(0, 0), out tile);
            PlacementInfo info = new PlacementInfo(Main.Instance.Floor[2], tile);

            var obj = GameManager.Facility.CreateFacility_OnlyOne("EggEntrance", info, true);
        });


        EventAction.Add("EggMessage", () =>
        {
            //Time.timeScale = 1;
            //Managers.UI.ClosePopUp();
            //Managers.Dialogue.currentDialogue = null;
            var message = Managers.UI.ShowPopUp<UI_SystemMessage>();
            message.Message = "숨겨진 방으로 향하는 통로가 생겼습니다.\n\n이후로 이 방에 누군가 침입해 알이 발견당하면 게임오버가 됩니다.";
        });

    }

    public Action GetAction(int dialogueID)
    {
        Action action = null;
        DialogueAction.TryGetValue(dialogueID, out action);
        return action;
    }

    public Action GetAction(string eventName)
    {
        Action action = null;
        EventAction.TryGetValue(eventName, out action);
        return action;
    }



    public bool TryRankUp(int fame, int danger)
    {
        if (Main.Instance.DungeonRank == 1 && fame + danger >= 200)
        {
            DungeonLvUp();
            return true;
        }
        else
        {
            return false;
        }

        if (Main.Instance.DungeonRank == 2 && fame + danger >= 500)
        {
            DungeonLvUp();
            return true;
        }
        else
        {
            return false;
        }
    }
    void DungeonLvUp()
    {
        Main.Instance.DungeonRank++;
        DungeonLvApply();
    }
    void DungeonLvApply()
    {
        switch (Main.Instance.DungeonRank)
        {
            case 1:
                break;

            case 2:
                GameManager.Technical.Level_2();
                FindObjectOfType<Player>().Level_Stat(Main.Instance.DungeonRank);
                Main.Instance.AddAP();
                GameManager.Content.AddLevel2();
                Main.Instance.DungeonExpansionUI();
                Camera.main.GetComponent<CameraControl>().LimitRefresh();
                break;

            case 3:
                Debug.Log("3랭크 = 레벨최대");
                break;
        }
    }

}

public class QuestData
{
    public SO_DialogueData Data;
    public int DataID;
    public string DataName;
    public int OriginIndex;
    public Action QuestAction;
}
