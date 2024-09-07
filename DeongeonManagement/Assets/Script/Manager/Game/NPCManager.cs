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
            Debug.Log("이벤트 스테이지");
            return;
        }

        //? 퀘스트 헌터 등 이벤트로 등장하는 적들
        if (EventNPCAction != null)
        {
            EventNPCAction.Invoke();
            EventNPCAction = null;
        }

        //? 최대 등장 벨류
        Max_NPC_Value = Calculation_MaxValue();
        Debug.Log($"Max Value = {Max_NPC_Value}");

        //? 등장 확률
        WeightUpdate_Danger();
        string weight = "";
        foreach (var item in Weight_NPC)
        {
            weight += $"{item.Key}:{item.Value}\n";
        }
        Debug.Log($"NPC Weight = \n{weight}");

        //? 실제 인스턴트 생성
        for (Current_Value = 0; Current_Value < Max_NPC_Value;)
        {
            var npc = InstantiateNPC_Normal(WeightPicker().ToString());
            if (npc == null) break;
        }
        int normalNPC = Instance_NPC_List.Count;
        Debug.Log($"생성된 일반 NPC = {Instance_NPC_List.Count}");


        //? 희귀 NPC (보너스 역할)
        //int MaxRareCount = 5;
        Instantiate_UniqueNPC();
        Debug.Log($"생성된 유니크 NPC = {Instance_NPC_List.Count - normalNPC}");


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

    void Init_NPC_Weight() //? 가장 처음 한번만 호출
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

    void WeightUpdate_Danger() //? 매 턴이 시작될 때 갱신
    {
        Weight_Reset();
        int danger = Mathf.Clamp(Main.Instance.DangerOfDungeon, (Main.Instance.Turn * 10), Main.Instance.DangerOfDungeon);

        if (danger <= 30)
        {
            AddWeightPoint(herb0: 15, miner0: 15);
            return;
        }

        //? 가중치 순서 //       herb0   miner0  adv0    elf  wizard  herb1   miner1  adv1

        //? 누적 value 30 /       15      15
        if (danger > 30)
        {
            AddWeightPoint(herb0: 15, miner0: 15);
            danger -= 30;
        }
        //? 누적 value 50 /       20      20      10
        if (danger > 20)
        {
            AddWeightPoint(herb0: 5, miner0: 5, adv0: 10);
            danger -= 20;
        }
        //? 누적 value 100 /      35      35      25      5
        if (danger > 50)
        {
            AddWeightPoint(herb0: 15, miner0: 15, adv0: 15, elf: 5);
            danger -= 50;
        }
        //? 누적 value 150        50      50      30      15      5
        if (danger > 50)
        {
            AddWeightPoint(herb0: 15, miner0: 15, adv0: 5, elf: 10, wizard: 5);
            danger -= 50;
        }
        //? 누적 value 200        60      60      40      30      10
        if (danger > 50)
        {
            AddWeightPoint(herb0: 10, miner0: 10, adv0: 10, elf: 15, wizard: 5);
            danger -= 50;
        }
        //? 누적 value 250        60      60      50      40      20      10      10
        if (danger > 50)
        {
            AddWeightPoint(herb1: 10, miner1: 10, adv0: 10, elf: 10, wizard: 10);
            danger -= 50;
        }
        //? 누적 value 300        60      60      50      50      30      20      20      10
        if (danger > 50)
        {
            AddWeightPoint(herb1: 10, miner1: 10, adv1: 10, elf: 10, wizard: 10);
            danger -= 50;
        }
        //? 누적 value 400        60      60      50      50      50      50      50      30
        if (danger > 100)
        {
            AddWeightPoint(herb1: 30, miner1: 30, adv1: 20, wizard: 20);
            danger -= 100;
        }
        //? 누적 value 500        60      60      50      90      90      50      50      50
        if (danger > 100)
        {
            AddWeightPoint(adv1: 20, elf: 40, wizard: 40);
            danger -= 100;
        }

        //? 500위험도를 썼고, 남은 위험도는 어떡하지? 여기서부턴 그냥 인기도로 치환해버릴까?
        //? 뭐 새로운 적도 추가하거나 할 수 있으니 일단은 보류


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


    NPC_Type_Normal WeightPicker() //? 0~1의 랜덤값에 전체 가중치의 합을 곱해줌. 그리고 그값으로 픽하면 됨. 반환값은 랭크 단계
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

        Debug.Log("잘못된 랭크");
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



    NPC InstantiateNPC_Normal(string keyName) //? 얘는 Value값을 더하고 Event애들은 안더함
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
            Debug.Log($"NPC_Data 없음 : {keyName.ToString()}");
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
            Debug.Log($"NPC_Data 없음 : {keyName}");
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
            Debug.Log($"NPC_Data 없음 : {keyName}");
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
            yield return new WaitUntil(() => Managers.UI._popupStack.Count == 0); //? 진화창같은거 떠있으면 좀만 기둘려

            if (Main.Instance.Management == true) // 만약 이미 다른코드로 인해 이미 매니지먼트턴이 됐다면 코루틴 브레이크
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

            Debug.Log("모든 npc가 비활성화됨");
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


    //? 이벤트나 분기 등으로 추가할 타입의 적
    Goblin,

}
public enum NPC_Type_Unique //? 등장 랭크 포인트를 안먹음
{
    ManaGoblin,     //? 마나 보너스 몹 - 체력 15, 마나 ㅈㄴ많음. 행동횟수 ㅈㄴ많음
    GoldLizard,     //? 골드 보너스 몹 - 체력 적당히, 방어 많이 높음, 행동횟수 적음(한 10정도), 반사데미지 강함. 잡으면 골드와 적당한 경험치
    PumpkinHead,    //? 경험치 보너스 몹 - 체력 적당히, 스탯 적당히, 잡으면 많은 경험치
    Santa,          //? 마나 보너스 몹 - 잡으면 위험도가 많이 오름. 층을 이동할 때 마다 마나를 많이 얻음. 상호작용은 따로 없음 
    DungeonThief,   //? 마나와 경험치 - 던전털이범 - 체력 15, 경험치 ㅈㄴ많음. 행동횟수 3회, 회피율 95%고정(luk를 99로), 퍼실리티 무시, 몬스터는 싸움, 잡으면 골드
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