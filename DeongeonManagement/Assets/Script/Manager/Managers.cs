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


    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static UIManager UI { get { return Instance._ui; } }

    public static PlacementManager Placement { get { return Instance._placement; } }
    public static SpriteManager Sprite { get { return Instance._sprite; } }




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
    }

    void Start()
    {

    }

    //void Update()
    //{

    //}




}
