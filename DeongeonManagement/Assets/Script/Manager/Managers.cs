using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    public static Managers Instance { get { Init(); return _instance; } }



    ResourceManager _resource = new ResourceManager();
    PoolManager _pool = new PoolManager();
    UIManager _ui = new UIManager();

    DataManager _data = new DataManager();

    PlacementManager _placement = new PlacementManager();
    SpriteManager _sprite = new SpriteManager();
    ContentManager _content = new ContentManager();
    FacilityManager _facility = new FacilityManager();
    TechnicalManager _technical = new TechnicalManager();
    MonsterManager _monster = new MonsterManager();
    NPCManager _npc = new NPCManager();



    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static UIManager UI { get { return Instance._ui; } }

    public static DataManager Data { get { return Instance._data; } }

    public static PlacementManager Placement { get { return Instance._placement; } }
    public static SpriteManager Sprite { get { return Instance._sprite; } }
    public static ContentManager Content { get { return Instance._content; } }
    public static FacilityManager Facility { get { return Instance._facility; } }
    public static TechnicalManager Technical { get { return Instance._technical; } }
    public static MonsterManager Monster { get { return Instance._monster; } }
    public static NPCManager NPC { get { return Instance._npc; } }



    static void Init()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<Managers>();
            if (_instance == null)
            {
                var go = new GameObject(name: "@Managers");
                _instance = go.AddComponent<Managers>();
                DontDestroyOnLoad(go);
            }
        }
    }


    void Awake()
    {
        Init();
        _placement.Init();
        _sprite.Init();
        _content.Init();
        _facility.Init();
        _technical.Init();
        _monster.Init();
        _npc.Init();
    }

    void Start()
    {

    }

    //void Update()
    //{

    //}




}
