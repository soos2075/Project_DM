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
        DialogueAction.Add(2100, () => GuildManager.Instance.AddBackAction(() => Main.Instance.FameOfDungeon += 15));
    }
    void AddEventAction()
    {
        EventAction.Add("DungeonLevelUp", () => Main.Instance.Dungeon_Lv++);
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
}

public class QuestData
{
    public SO_DialogueData Data;
    public int DataID;
    public string DataName;
    public int OriginIndex;
    public Action QuestAction;
}
