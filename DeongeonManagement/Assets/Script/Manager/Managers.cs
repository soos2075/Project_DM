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

    PlacementManager _placement = new PlacementManager();
    SpriteManager _sprite = new SpriteManager();
    ContentManager _content = new ContentManager();
    TechnicalManager _technical = new TechnicalManager();
    MonsterManager _monster = new MonsterManager();


    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static UIManager UI { get { return Instance._ui; } }

    public static PlacementManager Placement { get { return Instance._placement; } }
    public static SpriteManager Sprite { get { return Instance._sprite; } }
    public static ContentManager Content { get { return Instance._content; } }
    public static TechnicalManager Technical { get { return Instance._technical; } }
    public static MonsterManager Monster { get { return Instance._monster; } }



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
        _technical.Init();
        _monster.Init();
    }

    void Start()
    {

    }

    //void Update()
    //{

    //}




}
