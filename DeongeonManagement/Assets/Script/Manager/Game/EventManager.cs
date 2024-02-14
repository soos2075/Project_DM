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
                break;

            case 3:
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
