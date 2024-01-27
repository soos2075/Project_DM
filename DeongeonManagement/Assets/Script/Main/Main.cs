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

    public int Prisoner { get; set; }


    public List<DayResult> _dayList;

    public DayResult CurrentDay { get; set; }


    public class DayResult
    {
        public int Origin_Mana;
        public int Origin_Gold;
        public int Origin_Prisoner;

        public DayResult(int mana, int gold, int prisoner)
        {
            Origin_Mana = mana;
            Origin_Gold = gold;
            Origin_Prisoner = prisoner;
        }

        public int Get_Mana;
        public int Get_Gold;
        public int Get_Prisoner;
        public int Get_Kill;

        public int Use_Mana;
        public int Use_Gold;
        public int Use_Prisoner;
        public int Use_Kill;


        public void AddMana(int value)
        {
            Get_Mana += value;
        }
        public void AddGold(int value)
        {
            Get_Gold += value;
        }
        public void AddPrisoner(int value)
        {
            Get_Prisoner += value;
        }
        public void AddKill(int value)
        {
            Get_Kill += value;
        }


        //? 사용
        public void SubtractMana(int value)
        {
            Use_Mana += value;
            Instance.Player_Mana -= value;
        }
        public void SubtractGold(int value)
        {
            Use_Gold += value;
            Instance.Player_Gold -= value;
        }
        public void SubtractPrisoner(int value)
        {
            Use_Prisoner += value;
        }
        public void SubtractKill(int value)
        {
            Use_Kill += value;
        }
    }


    void ManagementInit()
    {
        Player_Mana = 100;
        Player_Gold = 50;
        Player_AP = 10;

        _dayList = new List<DayResult>();

        CurrentDay = new DayResult(Player_Mana, Player_Gold, Prisoner);
    }




    void DayOver()
    {
        _dayList.Add(CurrentDay);

        var ui = Managers.UI.ShowPopUp<UI_DayResult>();
        ui.TextContents(_dayList[Turn - 1]);

        Player_Mana += CurrentDay.Get_Mana;
        Player_Gold += CurrentDay.Get_Gold;
        Player_AP = 3;

        //? 위가 적용 아래가 새로교체

        CurrentDay = new DayResult(Player_Mana, Player_Gold, Prisoner);
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
