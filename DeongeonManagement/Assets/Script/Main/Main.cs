using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
       
    }

    void Start()
    {
        ManagementInit();

        MonsterInit();
        BasementFloorInit();
        NPCInit();
        AnimationInit();
    }

    void Update()
    {

    }








    #region Management
    public int Player_Mana { get; set; }
    public int Player_Gold { get; set; }
    public int Player_AP { get; set; }


    public List<DayResult> _dayList;

    public DayResult CurrentDay;


    public class DayResult
    {
        public int Mana;
        public int Gold;
        public int Prisoner;
        public int Kill;


        public DayResult()
        {

        }

        public void AddMana(int value)
        {
            Mana += value;
        }
        public void AddGold(int value)
        {
            Gold += value;
        }
        public void AddPrisoner(int value)
        {
            Prisoner += value;
        }
        public void AddKill(int value)
        {
            Kill += value;
        }
    }


    void ManagementInit()
    {
        Player_Mana = 100;
        Player_Gold = 50;
        Player_AP = 10;

        _dayList = new List<DayResult>();
    }



    void DayStart()
    {
        if (CurrentDay != null)
        {
            _dayList.Add(CurrentDay);
        }
        CurrentDay = new DayResult();
    }

    void DayOver()
    {
        if (CurrentDay != null)
        {
            Managers.UI.ShowPopUp<UI_DayResult>();
        }
    }

    #endregion





    #region Day
    public int Turn { get; set; } = 0;


    private bool _Management = true;
    public bool Management
    {
        get { return _Management; }
        set
        {
            _Management = value;
            if (_Management == false)
            {
                Turn++;
                DayStart();
            }
            else
            {
                DayOver();
            }
        }
    }


    public void DayChange()
    {
        Managers.UI.CloseAll();

        Management = !Management;

        DayChangeAnimation();
    }





    #endregion







    #region Animation
    Animator ani_MainUI;
    Animator ani_Sky;
    VerticalLayoutGroup layout;

    void AnimationInit()
    {
        ani_MainUI = FindObjectOfType<UI_Management>().GetComponent<Animator>();
        ani_Sky = FindObjectOfType<SpriteAnimation>().GetComponent<Animator>();
        layout = FindObjectOfType<UI_Management>().GetComponentInChildren<VerticalLayoutGroup>();
    }


    public void DayChangeAnimation() 
    {
        layout.enabled = false;

        ani_MainUI.SetBool("Management", Management);
        ani_Sky.SetBool("Management", Management);
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
            var n = NPCs.Dequeue();
            n.Departure(guild.position, dungeonEntrance.position);
            //UI_EventBox.AddEventText($"{n.Name} {n.Name_Index} (이)가 길드에서 출발");
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
