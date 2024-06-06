using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager
{
    public void Init()
    {
        Init_LocalData();

        Managers.Scene.BeforeSceneChangeAction = () => StopAllMoving();
    }

    #region SO_Data
    SO_NPC[] so_data;
    Dictionary<string, SO_NPC> NPC_Dictionary { get; set; }

    public void Init_LocalData()
    {
        NPC_Dictionary = new Dictionary<string, SO_NPC>();
        so_data = Resources.LoadAll<SO_NPC>("Data/NPC");
        foreach (var item in so_data)
        {
            string[] datas = null;
            switch (UserData.Instance.Language)
            {
                case Define.Language.EN:
                    Managers.Data.ObjectsLabel_EN.TryGetValue(item.id, out datas);
                    break;

                case Define.Language.KR:
                    Managers.Data.ObjectsLabel_KR.TryGetValue(item.id, out datas);
                    break;
            }
            if (datas == null)
            {
                Debug.Log($"{item.id} : CSV Data Not Exist");
                continue;
            }

            item.labelName = datas[0];
            item.detail = datas[1];

            NPC_Dictionary.Add(item.keyName, item);
        }
    }


    public SO_NPC GetData(string _keyName)
    {
        SO_NPC npc = null;
        if (NPC_Dictionary.TryGetValue(_keyName, out npc))
        {
            return npc;
        }

        Debug.Log($"{_keyName}: Data Not Exist");
        return null;
    }
    #endregion





    void StopAllMoving()
    {
        if (Instance_NPC_List.Count == 0) return;

        foreach (var item in Instance_NPC_List)
        {
            item.StopAllCoroutines();
        }
    }


    int Max_NPC_Value { get; set; }
    int Current_Value { get; set; }

    public List<NPC> Instance_NPC_List { get; set; } = new List<NPC>();
    public List<NPC> Instance_EventNPC_List { get; set; } = new List<NPC>();
    public List<NPC> Remove_NPC_List { get; set; } = new List<NPC>();


    public void TurnStart()
    {
        if (CustomStage)
        {
            CustomStage = false;
            Debug.Log("�̺�Ʈ ��������");
            return;
        }


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
            var npc = InstantiateNPC((NPCType)WeightRandomPicker());
            if (npc == null)
            {
                break;
            }
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

        Instance_NPC_List[index].Departure(Main.Instance.Guild.position, Main.Instance.Dungeon.position);
    }
    IEnumerator ActiveNPC(NPC _eventNPC, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var item in Instance_EventNPC_List)
        {
            if (item == _eventNPC)
            {
                _eventNPC.Departure(Main.Instance.Guild.position, Main.Instance.Dungeon.position);
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


    public bool CustomStage { get; set; }





    public enum NPCType
    {
        //? ����ġ �������� ���� NPC�� / rankWeightedList�� enum�� ����

        // 1����Ŭ
        Herbalist0_1 = 0,
        Miner0_1 = 1,
        Adventurer0_1 = 2,

        // 2����Ŭ
        Herbalist0_2,
        Miner0_2,
        Adventurer0_2,
        Elf_1 = 6,
        Wizard_1 = 7,

        // 3����Ŭ
        Herbalist0_3,
        Miner0_3,
        Adventurer0_3,
        Elf_2,
        Wizard_2,

        // 4����Ŭ
        Herbalist1_1,
        Miner1_1,
        Adventurer1_1,

        // 5����Ŭ
        Herbalist1_2,
        Miner1_2,
        Adventurer1_2,


        DarkElf_1,
        DarkWizard_1,


        //? �̺�Ʈ NPC��.  ������ ����, rankWeightedList�� ���þ���. index�� ���� Ÿ���� enum ���� ���ƾ���.
        //? Dict_Key�� enum string���� �����ؾ���
        Hunter_Slime = 1100,
        Hunter_EarthGolem = 1101,

        Event_Day3 = 2000,
        Event_Day8,
        Event_Day15,


        A_Warrior,
        A_Tanker,
        A_Wizard,
        A_Elf,

        B_Warrior,
        B_Tanker,
        B_Wizard,
        B_Elf,

        Captine_A,
        Captine_B,
        Captine_C,

        Event_Soldier1,
        Event_Soldier2,
        Event_Soldier3,
    }


    #region Calculation
    void Calculation_MaxNPC()
    {
        int ofFame = Main.Instance.PopularityOfDungeon / 10;

        Max_NPC_Value = Mathf.Clamp(Main.Instance.Turn + ofFame, 5, 5 + (Main.Instance.Turn * 2));

        //Debug.Log("�׽�Ʈ���!!!!!!!!!!!!!������������");
        //Max_NPC_Value = ofFame;
    }


    //? ��ũ -> 1��ũ ���ʲ�, ����, ���谡, Elf, Wizard ���� -> 2��ũ ���ʲ�, ����, ���谡, DarkElf, DarkWizard ����
    int[] rankWeightedList = new int[] {
        13, 12, 10,             // 1��ũ ���ʲ�, ����, ���谡
        17, 18, 10, 10, 10,     // 1��ũ ���ʲ�, ����, ���谡, Elf, Wizard
        15, 15, 15, 20, 20,      // 1��ũ ���ʲ�, ����, ���谡, Elf, Wizard
        15, 15, 10,             // 2��ũ ���ʲ�, ����, ���谡
        25, 25, 20 };           // 2��ũ ���ʲ�, ����, ���谡
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


    bool[] NameIndex { get; set; } = new bool[100];

    int RandomPicker()
    {
        int count = 0;
        while (count < 100)
        {
            count++;
            int pick = Random.Range(0, 100);
            if (NameIndex[pick] == false)
            {
                NameIndex[pick] = true;
                return pick;
            }
        }
        return count;
    }


    NPC InstantiateNPC(NPCType rank)
    {
        string Dict_Key = rank.ToString().Substring(0, rank.ToString().IndexOf('_'));
        //Debug.Log(Dict_Key);

        SO_NPC data = null;
        if (NPC_Dictionary.TryGetValue(Dict_Key, out data))
        {
            var obj = GameManager.Placement.CreatePlacementObject(data.prefabPath, null, PlacementType.NPC);
            obj.GetObject().name = Dict_Key;

            NPC _npc = obj as NPC;
            _npc.SetData(data, RandomPicker());

            int _value = data.Rank;
            //Debug.Log($"{_value}��ũ ����");
            Current_Value += _value;
            Instance_NPC_List.Add(_npc);
            return _npc;
        }
        else
        {
            Debug.Log($"NPC_Data ���� : {rank.ToString()}");
            return null;
        }
    }
    public NPC InstantiateNPC_Event(NPCType _name)
    {
        SO_NPC data = null;
        if (NPC_Dictionary.TryGetValue(_name.ToString(), out data))
        {
            var obj = GameManager.Placement.CreatePlacementObject(data.prefabPath, null, PlacementType.NPC);
            NPC _npc = obj as NPC;
            _npc.SetData(data, -1);
            _npc.EventID = (int)_name;
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

            if (Main.Instance.Management == true) // ���� �̹� �ٸ��ڵ�� ���� �̹� �Ŵ�����Ʈ���� �ƴٸ� �ڷ�ƾ �극��ũ
            {
                yield break;
            }

            foreach (var item in Remove_NPC_List)
            {
                Managers.Resource.Destroy(item.gameObject);
            }
            Remove_NPC_List.Clear();
            Instance_NPC_List.Clear();
            Instance_EventNPC_List.Clear();

            Debug.Log("��� npc�� ��Ȱ��ȭ��");
            Main.Instance.DayChange_Over();
        }
    }




}
