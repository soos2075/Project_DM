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
    SceneManagerEx _scene = new SceneManagerEx();

    SpriteManager _sprite = new SpriteManager();
    DialogueManager _dialogue = new DialogueManager();



    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static DataManager Data { get { return Instance._data; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }

    public static SpriteManager Sprite { get { return Instance._sprite; } }
    public static DialogueManager Dialogue { get { return Instance._dialogue; } }





    static void Init()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<Managers>();
            if (_instance == null)
            {
                var go = new GameObject(name: "@CoreManagers");
                _instance = go.AddComponent<Managers>();
                DontDestroyOnLoad(go);
            }
        }
    }


    void Awake()
    {
        Init();

        _data.Init();
        _sprite.Init();
        _scene.Init();
        _dialogue.Init();
    }

    void Start()
    {


    }

    //void Update()
    //{

    //}




}
