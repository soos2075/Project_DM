using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager
{
    public void Init()
    {
        FillIndex();
        Init_Data();
        Init_QuestNPC();
        Init_EventNPC();

        guild = Util.FindChild<Transform>(Main.Instance.gameObject, "Guild");
        dungeonEntrance = Util.FindChild<Transform>(Main.Instance.gameObject, "Dungeon");

        Managers.Scene.BeforeSceneChangeAction = () => StopAllMoving();
    }

    void StopAllMoving()
    {
        if (Instance_NPC_List.Count == 0) return;

        foreach (var item in Instance_NPC_List)
        {
            item.StopAllCoroutines();
        }
    }




    public Transform guild;
    public Transform dungeonEntrance;

    int Max_NPC_Value { get; set; }
    int Current_Value { get; set; }

    public List<NPC> Instance_NPC_List { get; set; } = new List<NPC>();
    public List<NPC> Instance_EventNPC_List { get; set; } = new List<NPC>();
    public List<NPC> Remove_NPC_List { get; set; } = new List<NPC>();


    public void TurnStart()
    {
        if (EventNPCAction != null)
        {
            EventNPCAction.Invoke();
            EventNPCAction = null;
        }

        Calculation_MaxNPC();
        Debug.Log($"Max Value = {Max_NPC_Value}");

        Calculation_Rank();
        Debug.Log($"�ִ� �� ��ũ = {rankList.Count}");

        //Instance_NPC_List = new List<NPC>();

        for (Current_Value = 0; Current_Value < Max_NPC_Value;)
        {
            InstantiateNPC((NPCType)WeightRandomPicker());
        }

        Debug.Log($"������ �� ���� = {Instance_NPC_List.Count}");



        if (Instance_NPC_List.Count <= 7)
        {
            for (int i = 0; i < Instance_NPC_List.Count; i++)
            {
                float ranValue = Random.Range(1f, 8f);
                Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
            }
        }
        else
        {
            for (int i = 0; i < 7; i++)
            {
                float ranValue = Random.Range(1f, 8f);
                Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
            }

            if (Instance_NPC_List.Count <= 15)
            {
                for (int i = 7; i < Instance_NPC_List.Count; i++)
                {
                    float ranValue = Random.Range(8f, 14f);
                    Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
                }
            }
            else
            {
                for (int i = 7; i < 15; i++)
                {
                    float ranValue = Random.Range(8f, 14f);
                    Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
                }

                for (int i = 15; i < Instance_NPC_List.Count; i++)
                {
                    float ranValue = Random.Range(14f, 20f);
                    Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
                }
            }
        }
    }

    IEnumerator ActiveNPC(int index, float delay)
    {
        yield return new WaitForSeconds(delay);

        Instance_NPC_List[index].Departure(guild.position, dungeonEntrance.position);
    }
    IEnumerator ActiveNPC(NPC _eventNPC, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var item in Instance_EventNPC_List)
        {
            if (item == _eventNPC)
            {
                _eventNPC.Departure(guild.position, dungeonEntrance.position);
            }
        }
    }


    System.Action EventNPCAction { get; set; }

    public void AddEventNPC(NPCType type, float time)
    {
        EventNPCAction += () =>
        {
            var npc = InstantiateNPC_Event(type);
            Main.Instance.StartCoroutine(ActiveNPC(npc, time));
        };
    }




    public enum NPCType
    {
        //? ����ġ �������� ���� NPC�� / rankWeightedList�� enum�� ����
        Herbalist0_1 = 0,
        Miner0_1 = 1,
        Adventurer0_1 = 2,

        Herbalist0_2,
        Miner0_2,
        Adventurer0_2,


        Herbalist1_1,
        Miner1_1,
        Adventurer1_1,

        Herbalist1_2,
        Miner1_2,
        Adventurer1_2,



        //? �̺�Ʈ NPC��.  ������ ����, rankWeightedList�� ���þ���. index�� ���� Ÿ���� enum ���� ���ƾ���.
        //? Dict_Key�� enum string���� �����ؾ���
        Hunter_Slime = 1100,
        Hunter_EarthGolem = 1101,

        Event_Day3 = 2000,
        Event_Day8,
        Event_Day15,
        Event_Day23,
        Event_Day30,

    }

    bool[] NameIndex;

    void FillIndex()
    {
        NameIndex = new bool[100];
        for (int i = 0; i < 100; i++)
        {
            NameIndex[i] = true;
        }
    }
    int RandomPicker()
    {
        int count = 0;
        while (count < 100)
        {
            count++;
            int pick = Random.Range(0, 100);
            if (NameIndex[pick])
            {
                NameIndex[pick] = false;
                return pick;
            }
        }
        return count;
    }


    void InstantiateNPC(NPCType rank)
    {
        string Dict_Key = rank.ToString().Substring(0, rank.ToString().IndexOf('_'));
        //Debug.Log(Dict_Key);

        NPC_Data data = null;
        if (NPCDatas.TryGetValue(Dict_Key, out data))
        {
            var obj = GameManager.Placement.CreatePlacementObject($"NPC/{data.PrefabName}", null, PlacementType.NPC);
            NPC _npc = obj as NPC;
            _npc.SetData(data, RandomPicker());

            int _value = data.Rank;
            Debug.Log($"{_value}��ũ ����");
            Current_Value += _value;
            Instance_NPC_List.Add(_npc);
        }
        else
        {
            Debug.Log($"NPC_Data ���� : {rank.ToString()}");
        }
    }
    NPC InstantiateNPC_Event(NPCType _name)
    {
        NPC_Data data = null;
        if (NPCDatas.TryGetValue(_name.ToString(), out data))
        {
            var obj = GameManager.Placement.CreatePlacementObject($"NPC/{data.PrefabName}", null, PlacementType.NPC);
            NPC _npc = obj as NPC;
            _npc.SetData(data, -1);
            _npc.EventID = (int)_name;
            //Instance_NPC_List.Add(_npc);
            Instance_EventNPC_List.Add(_npc);
            return _npc;
        }
        else
        {
            Debug.Log($"�̺�Ʈ ������ ���� : {_name.ToString()}");
            return null;
        }
    }



    public void InactiveNPC(NPC npc)
    {
        GameManager.Placement.PlacementClear(npc);

        Remove_NPC_List.Add(npc);
        npc.gameObject.SetActive(false);

        GameManager.Instance.StartCoroutine(TurnOverCheck());
    }




    IEnumerator TurnOverCheck()
    {
        yield return null;

        if (Instance_NPC_List.Count + Instance_EventNPC_List.Count == Remove_NPC_List.Count)
        {
            yield return new WaitUntil(() => Managers.UI._popupStack.Count == 0); //? ��ȭâ������ �������� ���� ��ѷ�

            foreach (var item in Remove_NPC_List)
            {
                Managers.Resource.Destroy(item.gameObject);
            }
            Remove_NPC_List.Clear();
            Instance_NPC_List.Clear();
            Instance_EventNPC_List.Clear();

            Debug.Log("��� npc�� ��Ȱ��ȭ��");
            Main.Instance.DayChange();
        }
    }







    #region Calculation
    void Calculation_MaxNPC()
    {
        int ofFame = Main.Instance.PopularityOfDungeon / 10;

        Max_NPC_Value = Mathf.Clamp(Main.Instance.Turn + ofFame, 5, 5 + (Main.Instance.Turn * 2));

        //Debug.Log("�׽�Ʈ���!!!!!!!!!!!!!������������");
    }


    // 1,2,3, 4,5,6 = ��ũ 1 // 10���� ��ũ2
    int[] rankWeightedList = new int[] { 
        15, 15, 20, 25, 25, 30,  // 1��ũ
        15, 15, 20, 25, 25, 30 }; // 2��ũ
    List<int> rankList;


    void Calculation_Rank() //? ���赵�� ���� ���� �� �ִ� ������ �޶���. �ƹ��� ���赵�� ���Ƶ� ���� ���� ������� ��. �ٸ� ���� �پ���.
    {
        rankList = new List<int>();

        int _danger = Mathf.Clamp(Main.Instance.DangerOfDungeon, 15 + (Main.Instance.Turn * 5), Main.Instance.DangerOfDungeon);

        for (int i = 0; i < rankWeightedList.Length; i++)
        {
            _danger -= rankWeightedList[i];
            rankList.Add(rankWeightedList[i]);
            if (_danger <= 0)
            {
                rankList[i] += _danger;
                break;
            }
        }
    } 

    int WeightRandomPicker() //? 0~1�� �������� ��ü ����ġ�� ���� ������. �׸��� �װ����� ���ϸ� ��. ��ȯ���� ��ũ �ܰ�
    {
        int weightMax = 0;
        foreach (var item in rankList)
        {
            weightMax += item;
        }

        float randomValue = Random.value * weightMax;
        int currentWeight = 0;


        for (int i = 0; i < rankList.Count; i++)
        {
            currentWeight += rankList[i];
            if (currentWeight > randomValue)
            {
                return i;
            }
        }
        Debug.Log("�߸��� ��ũ");
        return 0;
    }
    #endregion


    public Dictionary<string, NPC_Data> NPCDatas { get; } = new Dictionary<string, NPC_Data>();


    void Init_Data()
    {
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Herbalist0";
            npc.PrefabName = "Herbalist";
            npc.Name_Kr = "���ʲ�";
            npc.Detail = "�������� ������ ���ʳ� Ǯ���� ä���ؼ� ������ ��忡 �������ִ� ������ �մϴ�.";

            npc.Rank = 1;
            npc.ATK = 6;
            npc.DEF = 3;
            npc.AGI = 3;
            npc.LUK = 3;
            npc.HP = 30;
            npc.HP_MAX = 30;

            npc.ActionPoint = 2;
            npc.Mana = 80;
            npc.Speed_Ground = 1.5f;
            npc.ActionDelay = 0.7f;

            NPCDatas.Add(npc.DictName, npc);
        }
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Herbalist1";
            npc.PrefabName = "Herbalist";
            npc.Name_Kr = "���õ� ���ʲ�";
            npc.Detail = "���ʲ��� ������ �����Ⱓ ���� �������Դϴ�.";

            npc.Rank = 5;
            npc.ATK = 12;
            npc.DEF = 6;
            npc.AGI = 6;
            npc.LUK = 6;
            npc.HP = 80;
            npc.HP_MAX = 80;

            npc.ActionPoint = 4;
            npc.Mana = 300;
            npc.Speed_Ground = 1.55f;
            npc.ActionDelay = 0.7f;

            NPCDatas.Add(npc.DictName, npc);
        }

        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Miner0";
            npc.PrefabName = "Miner";
            npc.Name_Kr = "����";
            npc.Detail = "�������� �����Ǵ� �پ��� ������ ĳ���� ������ �������ִ� ������ �մϴ�.";

            npc.Rank = 1;
            npc.ATK = 10;
            npc.DEF = 4;
            npc.AGI = 1;
            npc.LUK = 1;
            npc.HP = 45;
            npc.HP_MAX = 45;

            npc.ActionPoint = 4;
            npc.Mana = 80;
            npc.Speed_Ground = 1.4f;
            npc.ActionDelay = 0.5f;

            NPCDatas.Add(npc.DictName, npc);
        }
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Miner1";
            npc.PrefabName = "Miner";
            npc.Name_Kr = "���õ� ����";
            npc.Detail = "����ĳ�� �������Դϴ�. �� ���� ȿ�������� �� �� �� �ֽ��ϴ�.";

            npc.Rank = 5;
            npc.ATK = 20;
            npc.DEF = 8;
            npc.AGI = 2;
            npc.LUK = 2;
            npc.HP = 100;
            npc.HP_MAX = 100;

            npc.ActionPoint = 8;
            npc.Mana = 300;
            npc.Speed_Ground = 1.45f;
            npc.ActionDelay = 0.5f;

            NPCDatas.Add(npc.DictName, npc);
        }


        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Adventurer0";
            npc.PrefabName = "Adventurer";
            npc.Name_Kr = "�߽� ���谡";
            npc.Detail = "���谡�� ���� �� �ȵ� ������ ���谡�Դϴ�. ���� ���谡�� �ڿ����� ���Ϳ� ������ ������ �ֽ��ϴ�.";

            npc.Rank = 3;
            npc.ATK = 13;
            npc.DEF = 2;
            npc.AGI = 6;
            npc.LUK = 5;
            npc.HP = 50;
            npc.HP_MAX = 50;

            npc.ActionPoint = 3;
            npc.Mana = 100;
            npc.Speed_Ground = 1.6f;
            npc.ActionDelay = 0.8f;

            NPCDatas.Add(npc.DictName, npc);
        }
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Adventurer1";
            npc.PrefabName = "Adventurer";
            npc.Name_Kr = "�̸��ִ� ���谡";
            npc.Detail = "�� �̸��ִ� ���谡�Դϴ�. ���� �ɷ°� ��Ȳ�Ǵ� �ɷ� �� ������ ���谡�ʹ� ���� �� �����.";

            npc.Rank = 8;
            npc.ATK = 22;
            npc.DEF = 6;
            npc.AGI = 9;
            npc.LUK = 8;
            npc.HP = 120;
            npc.HP_MAX = 120;

            npc.ActionPoint = 6;
            npc.Mana = 400;
            npc.Speed_Ground = 1.6f;
            npc.ActionDelay = 0.8f;

            NPCDatas.Add(npc.DictName, npc);
        }
    }


    void Init_QuestNPC()
    {
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Hunter_Slime";
            npc.PrefabName = "Hunter";
            npc.Name_Kr = "������ ����";
            npc.Detail = "����� ����Ʈ�� �ް� �⵿�� ���� ����";

            npc.Rank = 3;
            npc.ATK = 20;
            npc.DEF = 15;
            npc.AGI = 7;
            npc.LUK = 7;
            npc.HP = 150;
            npc.HP_MAX = 150;

            npc.ActionPoint = 100;
            npc.Mana = 100;
            npc.Speed_Ground = 0.9f;
            npc.ActionDelay = 1.0f;

            NPCDatas.Add(npc.DictName, npc);
        }

        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Hunter_EarthGolem";
            npc.PrefabName = "Hunter";
            npc.Name_Kr = "�ް�� ����";
            npc.Detail = "��忡�� ���� ���� ��Ѱ��� �ŷ��ȴٴ� ���� ��� �� �ް�� ��� ����";

            npc.Rank = 2;
            npc.ATK = 16;
            npc.DEF = 4;
            npc.AGI = 10;
            npc.LUK = 10;
            npc.HP = 80;
            npc.HP_MAX = 80;

            npc.ActionPoint = 100;
            npc.Mana = 100;
            npc.Speed_Ground = 0.9f;
            npc.ActionDelay = 1.0f;

            NPCDatas.Add(npc.DictName, npc);
        }
    }

    void Init_EventNPC()
    {
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Event_Day3";
            npc.PrefabName = "EventNPC";
            npc.Name_Kr = "�߽����谡";
            npc.Detail = "���谡�� ���� �� �ȵ� ������ ���谡�Դϴ�. ���� ���谡�� �ڿ����� ���Ϳ� ������ ������ �ֽ��ϴ�.";

            npc.Rank = 1;
            npc.ATK = 10;
            npc.DEF = 1;
            npc.AGI = 1;
            npc.LUK = 1;
            npc.HP = 25;
            npc.HP_MAX = 25;

            npc.ActionPoint = 5;
            npc.Mana = 90;
            npc.Speed_Ground = 1.0f;
            npc.ActionDelay = 1.0f;

            NPCDatas.Add(npc.DictName, npc);
        }

        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Event_Day8";
            npc.PrefabName = "EventNPC";
            npc.Name_Kr = "����ռ��� �߽����谡";
            npc.Detail = "�ʺ��� ������ �����Ͽ� �ֺ��� ������ �������ϴ� �߽� ���谡";

            npc.Rank = 3;
            npc.ATK = 25;
            npc.DEF = 1;
            npc.AGI = 1;
            npc.LUK = 1;
            npc.HP = 80;
            npc.HP_MAX = 80;

            npc.ActionPoint = 15;
            npc.Mana = 150;
            npc.Speed_Ground = 0.8f;
            npc.ActionDelay = 1.0f;

            NPCDatas.Add(npc.DictName, npc);
        }

        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Event_Day15";
            npc.PrefabName = "EventNPC";
            npc.Name_Kr = "������ ����";
            npc.Detail = "����ġ�� ������ ���� ��� ������ �����Ϸ��� ���� ���� ����� ���谡";

            npc.Rank = 7;
            npc.ATK = 35;
            npc.DEF = 15;
            npc.AGI = 5;
            npc.LUK = 10;
            npc.HP = 120;
            npc.HP_MAX = 120;

            npc.ActionPoint = 100;
            npc.Mana = 500;
            npc.Speed_Ground = 0.8f;
            npc.ActionDelay = 1.0f;

            NPCDatas.Add(npc.DictName, npc);
        }
    }


}



public class NPC_Data
{
    public int Rank { get; set; } //? ��ȯ�ÿ� ��ũ��ŭ�� Value�� ����. �������� �ʹ� ���� �����°� ������.

    public string DictName { get; set; }
    public string PrefabName { get; set; }
    public string Name_Kr { get; set; }
    public string Detail { get; set; }
    public int LV { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int AGI { get; set; }
    public int LUK { get; set; }



    public int HP { get; set; }
    public int HP_MAX { get; set; }
    public int ActionPoint { get; set; }
    public int Mana { get; set; }



    public float Speed_Ground { get; set; } //? Ŭ���� ����
    public float ActionDelay { get; set; } //? �������� ����

}