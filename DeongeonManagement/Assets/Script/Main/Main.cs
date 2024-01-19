using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    #region singleton
    private static Main _instance;
    public static Main Instance { get { Init(); return _instance; } }

    static void Init()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<Main>();
            if (_instance == null)
            {
                var go = new GameObject(name: "@Main");
                _instance = go.AddComponent<Main>();
                DontDestroyOnLoad(go);
            }
        }
    }
    #endregion


    void Start()
    {
        MonsterInit();
        BasementFloorInit();
    }

    void Update()
    {

    }



    #region Monster

    public Monster[] Monsters { get; set; }

    public int TrainingCount { get; set; } = 2;


    void MonsterInit()
    {
        Monsters = new Monster[5];
    }

    public bool MaximumCheck()
    {
        foreach (var monster in Monsters)
        {
            if (monster == null)
            {
                return true;
            }
        }

        Debug.Log("Monster Maximum");
        return false;
    }
    public void AddMonster(Monster mon)
    {
        for (int i = 0; i < Monsters.Length; i++)
        {
            if (Monsters[i] == null)
            {
                //mon.SetStatus();
                Monsters[i] = mon;
                break;
            }
        }
    }
    #endregion Monster






    #region Floor

    public BasementFloor[] Floor { get; set; }

    public BasementFloor CurrentFloor { get; set; }

    void BasementFloorInit()
    {
        Floor = FindObjectsOfType<BasementFloor>();
        System.Array.Reverse(Floor);




    }



    #endregion
}
