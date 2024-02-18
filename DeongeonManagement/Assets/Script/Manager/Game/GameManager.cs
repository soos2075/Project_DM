using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { Init(); return _instance; } }



    PlacementManager _placement = new PlacementManager();
    ContentManager _content = new ContentManager();
    FacilityManager _facility = new FacilityManager();
    TechnicalManager _technical = new TechnicalManager();
    MonsterManager _monster = new MonsterManager();
    NPCManager _npc = new NPCManager();



    public static PlacementManager Placement { get { return Instance._placement; } }
    public static ContentManager Content { get { return Instance._content; } }
    public static FacilityManager Facility { get { return Instance._facility; } }
    public static TechnicalManager Technical { get { return Instance._technical; } }
    public static MonsterManager Monster { get { return Instance._monster; } }
    public static NPCManager NPC { get { return Instance._npc; } }



    static void Init()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<GameManager>();
            if (_instance == null)
            {
                var go = new GameObject(name: "@GameManager");
                _instance = go.AddComponent<GameManager>();
                //DontDestroyOnLoad(go);
            }
        }
    }


    void Awake()
    {
        Init();
        _placement.Init();
        _content.Init();
        _facility.Init();
        _technical.Init();
        _monster.Init();
        _npc.Init();
    }

    void Start()
    {
        
    }
}
