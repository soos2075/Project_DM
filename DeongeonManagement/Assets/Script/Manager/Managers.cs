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


    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static UIManager UI { get { return Instance._ui; } }




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

    void Start()
    {
        Init();
    }

    void Update()
    {

    }




}
