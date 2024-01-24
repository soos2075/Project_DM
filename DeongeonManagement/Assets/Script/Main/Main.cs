using System;
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
        NPCInit();
        AnimationInit();
    }

    void Update()
    {

    }




    #region Day

    public bool Management = true;


    Animator ani_MainUI;
    void AnimationInit()
    {
        ani_MainUI = FindObjectOfType<UI_Management>().GetComponent<Animator>();
    }


    public void ManagementStart()
    {
        Management = true;
        ani_MainUI.SetBool("Hide", false);
    }
    public void ManagementOver()
    {
        Management = false;
        ani_MainUI.SetBool("Hide", true);
    }




    #endregion













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
                Monsters[i] = mon;
                break;
            }
        }
    }
    #endregion Monster









    #region NPC
    public Queue<NPC> NPCs;

    public Transform guild;
    public Transform dungeonEntrance;

    void NPCInit()
    {
        NPCs = new Queue<NPC>();

        guild = Util.FindChild<Transform>(gameObject, "Guild");
        dungeonEntrance = Util.FindChild<Transform>(gameObject, "Dungeon");


    }


    public void AddAndActive(string npcName)
    {
        AddNPC(npcName);
        ActiveNPC();
    }

    public void AddNPC(string npcName)
    {
        var npc = Managers.Placement.CreatePlacementObject($"NPC/{npcName}", null, Define.PlacementType.NPC);

        NPCs.Enqueue(npc as NPC);
    }


    public void ActiveNPC()
    {
        if (NPCs.Count > 0)
        {
            NPCs.Dequeue().Departure(guild.position, dungeonEntrance.position);
        }
    }

    public void InactiveNPC(NPC npc)
    {
        Managers.Placement.PlacementClear(npc);
        //Managers.Resource.Destroy(npc.gameObject);
    }


    #endregion





    #region Floor

    public BasementFloor[] Floor { get; set; }

    public BasementFloor CurrentFloor { get; set; }

    public BasementTile CurrentTile { get; set; }
    public Action CurrentAction { get; set; }

    public Vector2Int[] CurrentBoundary { get; set; } = Define.Boundary_Cross_1;

    void BasementFloorInit()
    {
        Floor = FindObjectsOfType<BasementFloor>();
        System.Array.Sort(Floor, (a, b) =>
        {
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        });

        for (int i = 0; i < Floor.Length; i++)
        {
            Floor[i].FloorIndex = i;
        }
    }



    #endregion
}
