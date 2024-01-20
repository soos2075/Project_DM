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
                Monsters[i] = mon;
                break;
            }
        }
    }
    #endregion Monster









    #region NPC
    public List<NPC> NPCs;
    int NPC_index;

    Transform guild;
    Transform dungeonEnterance;

    void NPCInit()
    {
        NPCs = new List<NPC>();

        guild = Util.FindChild<Transform>(gameObject, "Guild");
        dungeonEnterance = Util.FindChild<Transform>(gameObject, "Dungeon");

        AddNPC();
        AddNPC();
        AddNPC();


        Invoke("ActiveNPC", 3);
    }


    public void AddNPC()
    {
        var npc = Managers.Resource.Instantiate("NPC/Herbalist", transform);
        NPCs.Add(npc.GetComponent<NPC>());
    }


    public void ActiveNPC()
    {
        if (NPC_index < NPCs.Count)
        {
            NPCs[NPC_index].Departure(guild.position, dungeonEnterance.position);
            NPC_index++;
        }
    }



    #endregion





    #region Floor

    public BasementFloor[] Floor { get; set; }

    public BasementFloor CurrentFloor { get; set; }

    void BasementFloorInit()
    {
        Floor = FindObjectsOfType<BasementFloor>();
        System.Array.Sort(Floor, (a, b) =>
        {
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        });

    }



    #endregion
}
