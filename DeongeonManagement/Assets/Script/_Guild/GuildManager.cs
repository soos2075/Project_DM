using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildManager : MonoBehaviour
{
    #region singleton
    private static GuildManager _instance;
    public static GuildManager Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<GuildManager>();
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
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
    #endregion


    void Start()
    {
        GuildEnter();
    }

    public void GuildEnter()
    {
        NPC_Dict = new Dictionary<int, Interaction_Guild>();
        DungeonBackAction = new List<Action>();
        Dict_Init();
    }


    Dictionary<int, Interaction_Guild> NPC_Dict;


    void Dict_Init()
    {
        var npc = FindObjectsOfType<Interaction_Guild>();

        foreach (var item in npc)
        {
            NPC_Dict.Add(item.Original_Index, item);
        }
    }

    public Interaction_Guild GetInteraction(int id)
    {
        Interaction_Guild target = null;

        if (NPC_Dict.TryGetValue(id, out target))
        {
            return target;
        }
        return null;
    }


    public List<Action> DungeonBackAction { get; set; }

    public void AddBackAction(Action action)
    {
        DungeonBackAction.Add(action);
    }
}
