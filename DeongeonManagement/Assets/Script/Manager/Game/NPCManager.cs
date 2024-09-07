using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCManager
{
    public void Init()
    {
        Init_LocalData();
        Init_NPC_Weight();
        Init_UniqueNPC();

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

                case Define.Language.JP:
                    Managers.Data.ObjectsLabel_JP.TryGetValue(item.id, out datas);
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
        RandomPickerReset();

        if (CustomStage)
        {
            CustomStage = false;
            Debug.Log("�̺�Ʈ ��������");
            return;
        }

        //? ����Ʈ ���� �� �̺�Ʈ�� �����ϴ� ����
        if (EventNPCAction != null)
        {
            EventNPCAction.Invoke();
            EventNPCAction = null;
        }

        //? �ִ� ���� ����
        Max_NPC_Value = Calculation_MaxValue();
        Debug.Log($"Max Value = {Max_NPC_Value}");

        //? ���� Ȯ��
        WeightUpdate_Danger();
        string weight = "";
        foreach (var item in Weight_NPC)
        {
            weight += $"{item.Key}:{item.Value}\n";
        }
        Debug.Log($"NPC Weight = \n{weight}");

        //? ���� �ν���Ʈ ����
        for (Current_Value = 0; Current_Value < Max_NPC_Value;)
        {
            var npc = InstantiateNPC_Normal(WeightPicker().ToString());
            if (npc == null) break;
        }
        int normalNPC = Instance_NPC_List.Count;
        Debug.Log($"������ �Ϲ� NPC = {Instance_NPC_List.Count}");


        //? ��� NPC (���ʽ� ����)
        //int MaxRareCount = 5;
        Instantiate_UniqueNPC();
        Debug.Log($"������ ����ũ NPC = {Instance_NPC_List.Count - normalNPC}");


        for (int i = 0; i < Instance_NPC_List.Count; i++)
        {
            float ranValue = Random.Range(1f, Instance_NPC_List.Count + 5);
            Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
        }

        //if (Instance_NPC_List.Count <= 7)
        //{
        //    for (int i = 0; i < Instance_NPC_List.Count; i++)
        //    {
        //        float ranValue = Random.Range(1f, 8f);
        //        Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < 7; i++)
        //    {
        //        float ranValue = Random.Range(1f, 8f);
        //        Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
        //    }

        //    if (Instance_NPC_List.Count <= 15)
        //    {
        //        for (int i = 7; i < Instance_NPC_List.Count; i++)
        //        {
        //            float ranValue = Random.Range(8f, 14f);
        //            Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 7; i < 15; i++)
        //        {
        //            float ranValue = Random.Range(8f, 14f);
        //            Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
        //        }

        //        for (int i = 15; i < Instance_NPC_List.Count; i++)
        //        {
        //            float ranValue = Random.Range(14f, 20f);
        //            Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
        //        }
        //    }
        //}
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

    public void AddEventNPC(string typeName, float time)
    {
        EventNPCAction += () =>
        {
            var npc = InstantiateNPC_Event(typeName);
            Main.Instance.StartCoroutine(ActiveNPC(npc, time));
        };
    }
    public void AddEventNPC(string typeName, float time, NPC_Typeof types)
    {
        EventNPCAction += () =>
        {
            var npc = InstantiateNPC_Custom(typeName, types);
            Main.Instance.StartCoroutine(ActiveNPC(npc, time));
        };
    }


    public bool CustomStage { get; set; }





    #region New Calculation System
    int Calculation_MaxValue()
    {
        int ofFame = Main.Instance.PopularityOfDungeon / 10;
        int ofDanger = Main.Instance.DangerOfDungeon / 25;

        int maxValue = Mathf.Clamp(ofFame + ofDanger, 4 + Main.Instance.Turn, ofFame + ofDanger);
        return maxValue;
    }

    Dictionary<NPC_Type_Normal, int> Weight_NPC = new Dictionary<NPC_Type_Normal, int>();

    void Init_NPC_Weight() //? ���� ó�� �ѹ��� ȣ��
    {
        for (int i = 0; i < System.Enum.GetNames(typeof(NPC_Type_Normal)).Length; i++)
        {
            Weight_NPC.Add((NPC_Type_Normal)i, 0);
        }
    }
    void Weight_Reset()
    {
        SetWeightPoint(NPC_Type_Normal.Herbalist0, 0);
        SetWeightPoint(NPC_Type_Normal.Herbalist1, 0);
        SetWeightPoint(NPC_Type_Normal.Miner0, 0);
        SetWeightPoint(NPC_Type_Normal.Miner1, 0);
        SetWeightPoint(NPC_Type_Normal.Adventurer0, 0);
        SetWeightPoint(NPC_Type_Normal.Adventurer1, 0);
        SetWeightPoint(NPC_Type_Normal.Elf, 0);
        SetWeightPoint(NPC_Type_Normal.Wizard, 0);
    }

    void WeightUpdate_Danger() //? �� ���� ���۵� �� ����
    {
        Weight_Reset();
        int danger = Mathf.Clamp(Main.Instance.DangerOfDungeon, (Main.Instance.Turn * 10), Main.Instance.DangerOfDungeon);

        if (danger <= 30)
        {
            AddWeightPoint(herb0: 15, miner0: 15);
            return;
        }

        //? ����ġ ���� //       herb0   miner0  adv0    elf  wizard  herb1   miner1  adv1

        //? ���� value 30 /       15      15
        if (danger > 30)
        {
            AddWeightPoint(herb0: 15, miner0: 15);
            danger -= 30;
        }
        //? ���� value 50 /       20      20      10
        if (danger > 20)
        {
            AddWeightPoint(herb0: 5, miner0: 5, adv0: 10);
            danger -= 20;
        }
        //? ���� value 100 /      35      35      25      5
        if (danger > 50)
        {
            AddWeightPoint(herb0: 15, miner0: 15, adv0: 15, elf: 5);
            danger -= 50;
        }
        //? ���� value 150        50      50      30      15      5
        if (danger > 50)
        {
            AddWeightPoint(herb0: 15, miner0: 15, adv0: 5, elf: 10, wizard: 5);
            danger -= 50;
        }
        //? ���� value 200        60      60      40      30      10
        if (danger > 50)
        {
            AddWeightPoint(herb0: 10, miner0: 10, adv0: 10, elf: 15, wizard: 5);
            danger -= 50;
        }
        //? ���� value 250        60      60      50      40      20      10      10
        if (danger > 50)
        {
            AddWeightPoint(herb1: 10, miner1: 10, adv0: 10, elf: 10, wizard: 10);
            danger -= 50;
        }
        //? ���� value 300        60      60      50      50      30      20      20      10
        if (danger > 50)
        {
            AddWeightPoint(herb1: 10, miner1: 10, adv1: 10, elf: 10, wizard: 10);
            danger -= 50;
        }
        //? ���� value 400        60      60      50      50      50      50      50      30
        if (danger > 100)
        {
            AddWeightPoint(herb1: 30, miner1: 30, adv1: 20, wizard: 20);
            danger -= 100;
        }
        //? ���� value 500        60      60      50      90      90      50      50      50
        if (danger > 100)
        {
            AddWeightPoint(adv1: 20, elf: 40, wizard: 40);
            danger -= 100;
        }

        //? 500���赵�� ���, ���� ���赵�� �����? ���⼭���� �׳� �α⵵�� ġȯ�ع�����?
        //? �� ���ο� ���� �߰��ϰų� �� �� ������ �ϴ��� ����


        Event_ValueChange();
    }


    void SetWeightPoint(NPC_Type_Normal target, int value)
    {
        Weight_NPC[target] = value;
    }
    void AddWeightPoint(NPC_Type_Normal target, int value)
    {
        Weight_NPC[target] += value;
    }
    void AddWeightPoint(int herb0 = 0, int herb1 = 0, int miner0 = 0, int miner1 = 0, int adv0 = 0, int adv1 = 0, int elf = 0, int wizard = 0)
    {
        AddWeightPoint(NPC_Type_Normal.Herbalist0, herb0);
        AddWeightPoint(NPC_Type_Normal.Herbalist1, herb1);
        AddWeightPoint(NPC_Type_Normal.Miner0, miner0);
        AddWeightPoint(NPC_Type_Normal.Miner1, miner1);
        AddWeightPoint(NPC_Type_Normal.Adventurer0, adv0);
        AddWeightPoint(NPC_Type_Normal.Adventurer1, adv1);
        AddWeightPoint(NPC_Type_Normal.Elf, elf);
        AddWeightPoint(NPC_Type_Normal.Wizard, wizard);
    }
    public void AddWaightPoint_Event(NPC_Type_Normal target, int value)
    {
        AddWeightPoint(target, value);
    }


    public bool Event_Herb { get; set; }
    public bool Event_Mineral { get; set; }
    public bool Event_Monster { get; set; }

    public void Event_ValueChange()
    {
        if (Event_Herb)
        {
            Weight_NPC[NPC_Type_Normal.Herbalist0] *= 3;
            Weight_NPC[NPC_Type_Normal.Herbalist1] *= 3;
            Weight_NPC[NPC_Type_Normal.Elf] *= 3;
        }

        if (Event_Mineral)
        {

        }
        if (Event_Monster)
        {

        }
    }


    NPC_Type_Normal WeightPicker() //? 0~1�� �������� ��ü ����ġ�� ���� ������. �׸��� �װ����� ���ϸ� ��. ��ȯ���� ��ũ �ܰ�
    {
        int weightMax = 0;
        foreach (var item in Weight_NPC)
        {
            weightMax += item.Value;
        }

        float randomValue = Random.value * weightMax;
        int currentWeight = 0;


        foreach (var item in Weight_NPC)
        {
            currentWeight += item.Value;
            if (currentWeight >= randomValue)
            {
                return item.Key;
            }
        }

        Debug.Log("�߸��� ��ũ");
        return 0;
    }




    public Dictionary<NPC_Type_Unique, float> UniqueNPC_Dict;

    void Init_UniqueNPC()
    {
        UniqueNPC_Dict = new Dictionary<NPC_Type_Unique, float>();

        for (int i = 0; i < Enum.GetNames(typeof(NPC_Type_Unique)).Length; i++)
        {
            UniqueNPC_Dict.Add((NPC_Type_Unique)i, 1);
        }
    }

    void Add_UniqueChance(NPC_Type_Unique target, float probability)
    {
        UniqueNPC_Dict[target] = probability;
    }

    void Instantiate_UniqueNPC()
    {
        foreach (var item in UniqueNPC_Dict)
        {
            float ranValue = Random.value;
            if (item.Value >= ranValue)
            {
                InstantiateNPC_Event(item.Key.ToString());
            }
        }
    }




    public Dictionary<NPC_Type_Unique, float> Save_NPCData()
    {
        return UniqueNPC_Dict;
    }
    public void Load_NPCData(Dictionary<NPC_Type_Unique, float> data)
    {
        if (data != null)
        {
            UniqueNPC_Dict = data;
        }
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

    void RandomPickerReset()
    {
        for (int i = 0; i < NameIndex.Length; i++)
        {
            NameIndex[i] = false;
        }
    }



    NPC InstantiateNPC_Normal(string keyName) //? ��� Value���� ���ϰ� Event�ֵ��� �ȴ���
    {
        SO_NPC data = null;
        if (NPC_Dictionary.TryGetValue(keyName, out data))
        {
            var obj = GameManager.Placement.CreatePlacementObject(data.prefabPath, null, PlacementType.NPC);
            NPC _npc = obj as NPC;
            _npc.SetData(data, RandomPicker());
            int _value = data.Rank;
            Current_Value += _value;
            Instance_NPC_List.Add(_npc);
            return _npc;
        }
        else
        {
            Debug.Log($"NPC_Data ���� : {keyName.ToString()}");
            return null;
        }
    }
    public NPC InstantiateNPC_Event(string keyName)
    {
        SO_NPC data = null;
        if (NPC_Dictionary.TryGetValue(keyName, out data))
        {
            var obj = GameManager.Placement.CreatePlacementObject(data.prefabPath, null, PlacementType.NPC);
            NPC _npc = obj as NPC;
            _npc.SetData(data, -1);
            Instance_EventNPC_List.Add(_npc);
            return _npc;
        }
        else
        {
            Debug.Log($"NPC_Data ���� : {keyName}");
            return null;
        }
    }
    public NPC InstantiateNPC_Custom(string keyName, NPC_Typeof addScript)
    {
        SO_NPC data = null;
        if (NPC_Dictionary.TryGetValue(keyName, out data))
        {
            var obj = GameManager.Placement.CreatePlacementObject(data.prefabPath, null, PlacementType.NPC, addScript);
            NPC _npc = obj as NPC;
            _npc.SetData(data, -1);
            Instance_EventNPC_List.Add(_npc);
            return _npc;
        }
        else
        {
            Debug.Log($"NPC_Data ���� : {keyName}");
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

public enum NPC_Typeof
{
    NPC_Type_Normal = 0,
    NPC_Type_MainEvent = 1,
    NPC_Type_SubEvent = 2,
    NPC_Type_Unique = 3,
    NPC_Type_Hunter = 4,
}
public enum NPC_Type_Normal
{
    Herbalist0 = 1000, Herbalist1 = 1001, Herbalist2 = 1002,

    Miner0 = 1100, Miner1 = 1101,

    Adventurer0 = 1200, Adventurer1 = 1201,

    Elf = 1300,

    Wizard = 1400,


    //? �̺�Ʈ�� �б� ������ �߰��� Ÿ���� ��
    Goblin,

}
public enum NPC_Type_Unique //? ���� ��ũ ����Ʈ�� �ȸ���
{
    ManaGoblin,     //? ���� ���ʽ� �� - ü�� 15, ���� ��������. �ൿȽ�� ��������
    GoldLizard,     //? ��� ���ʽ� �� - ü�� ������, ��� ���� ����, �ൿȽ�� ����(�� 10����), �ݻ絥���� ����. ������ ���� ������ ����ġ
    PumpkinHead,    //? ����ġ ���ʽ� �� - ü�� ������, ���� ������, ������ ���� ����ġ
    Santa,          //? ���� ���ʽ� �� - ������ ���赵�� ���� ����. ���� �̵��� �� ���� ������ ���� ����. ��ȣ�ۿ��� ���� ���� 
    DungeonThief,   //? ������ ����ġ - �������̹� - ü�� 15, ����ġ ��������. �ൿȽ�� 3ȸ, ȸ���� 95%����(luk�� 99��), �۽Ǹ�Ƽ ����, ���ʹ� �ο�, ������ ���
}

public enum NPC_Type_MainEvent
{
    EM_FirstAdventurer = 1903,
    EM_RedHair = 1908,

    Event_Goblin = 1910,
    Event_Goblin_Leader = 1911,
    Event_Goblin_Leader2 = 1912,

    EM_Catastrophe = 1914,
    EM_RetiredHero = 1915,

    EM_Blood_Warrior_A = 1920,
    EM_Blood_Tanker_A,
    EM_Blood_Wizard_A,
    EM_Blood_Elf_A,

    EM_Blood_Warrior_B,
    EM_Blood_Tanker_B,
    EM_Blood_Wizard_B,
    EM_Blood_Elf_B,

    EM_Captine_A = 1930,
    EM_Captine_B = 1931,
    EM_Captine_BlueKnight = 1932,

    EM_Soldier1 = 1941,
    EM_Soldier2,
    EM_Soldier3,
}
public enum NPC_Type_SubEvent
{
    Heroine = 1916,
    DungeonRacer = 1917,
}
public enum NPC_Type_Hunter
{
    Hunter_Slime = 1800,
    Hunter_EarthGolem = 1801,
}