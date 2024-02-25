using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager
{
    public void Init()
    {
        FillIndex();
        Init_Data();
        Init_EventNPCData();

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
        Debug.Log($"최대 적 랭크 = {rankList.Count}");

        //Instance_NPC_List = new List<NPC>();

        for (Current_Value = 0; Current_Value < Max_NPC_Value;)
        {
            InstantiateNPC((NPCType)WeightRandomPicker());
        }

        Debug.Log($"생성된 적 숫자 = {Instance_NPC_List.Count}");



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
        ActiveNPC(index);
    }


    System.Action EventNPCAction { get; set; }

    public void AddEventNPC(NPCType type, float time)
    {
        EventNPCAction += () => Main.Instance.StartCoroutine(EventNPC(type, time));

        //if (EventNPCAction == null)
        //{
        //    EventNPCAction = () => Main.Instance.StartCoroutine(EventNPC(type, time));
        //}
        //else
        //{
        //    EventNPCAction += () => Main.Instance.StartCoroutine(EventNPC(type, time));
        //}
    }
    public void AddEventNPC(QuestHunter.HunterType _hunter, float time)
    {
        EventNPCAction += () => Main.Instance.StartCoroutine(EventNPC(_hunter, time));
    }
    IEnumerator EventNPC(NPCType type, float time)
    {
        yield return new WaitForSeconds(1);
        InstantiateNPC(type);
        Main.Instance.StartCoroutine(ActiveNPC(Instance_NPC_List.Count - 1, time));
    }
    IEnumerator EventNPC(QuestHunter.HunterType _hunter, float time)
    {
        yield return new WaitForSeconds(1);
        InstantiateNPC_Event(NPCType.Hunter, _hunter);
        Main.Instance.StartCoroutine(ActiveNPC(Instance_NPC_List.Count - 1, time));
    }



    public enum NPCType //? Prefab 이름과 동일해야함. 추가로 사전의 이름까지도.
    {
        //? 가중치 랜덤으로 뽑을 NPC들 / rankWeightedList 의 Lenght와 동일해야함. + 순서도 0부터 순차적으로 증가
        Herbalist_0 = 0,
        Miner_0 = 1,
        Adventurer_0 = 2,

        Herbalist_1 = 3,
        Miner_1 = 4,
        Adventurer_1 = 5,



        //? 이벤트 NPC들.  순서는 자유, rankWeightedList와 관련없음.
        Hunter = 1000,



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
        NPC_Data data = null;
        if (NPCDatas.TryGetValue(rank.ToString(), out data))
        {
            var obj = GameManager.Placement.CreatePlacementObject($"NPC/{data.PrefabName}", null, PlacementType.NPC);
            NPC _npc = obj as NPC;
            _npc.SetData(data, RandomPicker());

            int _value = data.Rank;
            Debug.Log($"{_value}랭크 생성");
            Current_Value += _value;
            Instance_NPC_List.Add(_npc);
        }
        else
        {
            Debug.Log($"NPC_Data 없음 : {rank.ToString()}");
        }
    }
    void InstantiateNPC_Event(NPCType _name, QuestHunter.HunterType _type)
    {
        NPC_Data data = null;
        if (NPCDatas.TryGetValue(_name.ToString(), out data))
        {
            var obj = GameManager.Placement.CreatePlacementObject($"NPC/{data.PrefabName}", null, PlacementType.NPC);
            QuestHunter _hunter = obj as QuestHunter;
            _hunter.Hunter = _type;
            _hunter.SetData(data, -1);
            Debug.Log($"{_type}헌터 생성");
            Instance_NPC_List.Add(_hunter);
        }
        else
        {
            Debug.Log($"NPC_Data 없음 : {_name.ToString()}");
        }
    }



    public void ActiveNPC(int index)
    {
        Instance_NPC_List[index].Departure(guild.position, dungeonEntrance.position);
    }

    public void InactiveNPC(NPC npc)
    {
        GameManager.Placement.PlacementClear(npc);

        Remove_NPC_List.Add(npc);
        npc.gameObject.SetActive(false);

        TurnOverCheck();
    }




    void TurnOverCheck()
    {
        if (Instance_NPC_List.Count == Remove_NPC_List.Count)
        {
            foreach (var item in Remove_NPC_List)
            {
                Managers.Resource.Destroy(item.gameObject);
            }
            Remove_NPC_List.Clear();
            Instance_NPC_List.Clear();

            Debug.Log("모든 npc가 비활성화됨");
            Main.Instance.DayChange();
        }

        //if (Instance_NPC_List.Count == 0)
        //{
        //    Debug.Log("모든 npc가 비활성화됨");
        //    Main.Instance.DayChange();
        //}
    }




    [System.Obsolete]
    public void TestCreate(string name)
    {
        var adv = GameManager.Placement.CreatePlacementObject($"NPC/{name}", null, PlacementType.NPC);
        var npc = adv as NPC;
        npc.Departure(guild.position, dungeonEntrance.position);
    }








    #region Calculation
    void Calculation_MaxNPC()
    {
        int ofFame = Main.Instance.PopularityOfDungeon / 10;

        Max_NPC_Value = Mathf.Clamp(Main.Instance.Turn + ofFame, 5, 5 + (Main.Instance.Turn * 2) + 50);

        Debug.Log("테스트모드!!!!!!!!!!!!!빌드전수정필");
    }



    int[] rankWeightedList = new int[] { 20, 20, 60, 40, 40, 100 };
    List<int> rankList;


    void Calculation_Rank() //? 위험도에 따라 나올 수 있는 적들이 달라짐. 아무리 위험도가 높아도 약한 적이 나오기는 함. 다만 점점 줄어들뿐.
    {
        rankList = new List<int>();

        int _danger = Mathf.Clamp(Main.Instance.DangerOfDungeon, 30 + (Main.Instance.Turn * 5), Main.Instance.DangerOfDungeon);

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

    int WeightRandomPicker() //? 0~1의 랜덤값에 전체 가중치의 합을 곱해줌. 그리고 그값으로 픽하면 됨. 반환값은 랭크 단계
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
        Debug.Log("잘못된 랭크");
        return 0;
    }
    #endregion


    public Dictionary<string, NPC_Data> NPCDatas { get; } = new Dictionary<string, NPC_Data>();


    void Init_Data()
    {
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Herbalist_0";
            npc.PrefabName = "Herbalist";
            npc.Name_Kr = "약초꾼";
            npc.Detail = "던전에서 나오는 약초나 풀들을 채취해서 마을과 길드에 공급해주는 역할을 합니다.";

            npc.Rank = 1;
            npc.ATK = 6;
            npc.DEF = 3;
            npc.AGI = 3;
            npc.LUK = 3;
            npc.HP = 20;
            npc.HP_MAX = 20;

            npc.ActionPoint = 2;
            npc.Mana = 30;
            npc.Speed_Ground = 1.5f;
            npc.ActionDelay = 0.7f;

            NPCDatas.Add(npc.DictName, npc);
        }
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Herbalist_1";
            npc.PrefabName = "Herbalist";
            npc.Name_Kr = "숙련된 약초꾼";
            npc.Detail = "약초꾼을 업으로 오랜기간 일한 숙련자입니다.";

            npc.Rank = 3;
            npc.ATK = 12;
            npc.DEF = 6;
            npc.AGI = 6;
            npc.LUK = 6;
            npc.HP = 40;
            npc.HP_MAX = 40;

            npc.ActionPoint = 4;
            npc.Mana = 80;
            npc.Speed_Ground = 1.55f;
            npc.ActionDelay = 0.7f;

            NPCDatas.Add(npc.DictName, npc);
        }

        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Miner_0";
            npc.PrefabName = "Miner";
            npc.Name_Kr = "광부";
            npc.Detail = "던전에서 생성되는 다양한 광물을 캐내서 마을에 공급해주는 역할을 합니다.";

            npc.Rank = 1;
            npc.ATK = 10;
            npc.DEF = 4;
            npc.AGI = 1;
            npc.LUK = 1;
            npc.HP = 30;
            npc.HP_MAX = 30;

            npc.ActionPoint = 4;
            npc.Mana = 25;
            npc.Speed_Ground = 1.4f;
            npc.ActionDelay = 0.5f;

            NPCDatas.Add(npc.DictName, npc);
        }
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Miner_1";
            npc.PrefabName = "Miner";
            npc.Name_Kr = "숙련된 광부";
            npc.Detail = "광물캐기 숙련자입니다. 더 오래 효율적으로 일 할 수 있습니다.";

            npc.Rank = 3;
            npc.ATK = 20;
            npc.DEF = 8;
            npc.AGI = 2;
            npc.LUK = 2;
            npc.HP = 60;
            npc.HP_MAX = 60;

            npc.ActionPoint = 8;
            npc.Mana = 30;
            npc.Speed_Ground = 1.45f;
            npc.ActionDelay = 0.5f;

            NPCDatas.Add(npc.DictName, npc);
        }


        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Adventurer_0";
            npc.PrefabName = "Adventurer";
            npc.Name_Kr = "견습 모험가";
            npc.Detail = "모험가가 된지 얼마 안된 새내기 모험가입니다. 나름 모험가라서 자원보단 몬스터와 보물에 관심이 있습니다.";

            npc.Rank = 2;
            npc.ATK = 14;
            npc.DEF = 2;
            npc.AGI = 6;
            npc.LUK = 5;
            npc.HP = 50;
            npc.HP_MAX = 50;

            npc.ActionPoint = 3;
            npc.Mana = 40;
            npc.Speed_Ground = 1.6f;
            npc.ActionDelay = 0.6f;

            NPCDatas.Add(npc.DictName, npc);
        }
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Adventurer_1";
            npc.PrefabName = "Adventurer";
            npc.Name_Kr = "이름있는 모험가";
            npc.Detail = "꽤 이름있는 모험가입니다. 전투력과 상황판단능력 등 새내기 모험가와는 비교할 수 없어요.";

            npc.Rank = 6;
            npc.ATK = 30;
            npc.DEF = 5;
            npc.AGI = 9;
            npc.LUK = 8;
            npc.HP = 100;
            npc.HP_MAX = 100;

            npc.ActionPoint = 6;
            npc.Mana = 60;
            npc.Speed_Ground = 1.6f;
            npc.ActionDelay = 0.7f;

            NPCDatas.Add(npc.DictName, npc);
        }
    }


    void Init_EventNPCData()
    {
        {
            NPC_Data npc = new NPC_Data();

            npc.DictName = "Hunter_0";
            npc.PrefabName = "Hunter";
            npc.Name_Kr = "헌터";
            npc.Detail = "길드의 퀘스트를 받고 출동한 토벌대";

            npc.Rank = 3;
            npc.ATK = 40;
            npc.DEF = 15;
            npc.AGI = 7;
            npc.LUK = 7;
            npc.HP = 150;
            npc.HP_MAX = 150;

            npc.ActionPoint = 100;
            npc.Mana = 100;
            npc.Speed_Ground = 1.0f;
            npc.ActionDelay = 1.0f;

            NPCDatas.Add(npc.DictName, npc);
        }
    }


}



public class NPC_Data
{
    public int Rank { get; set; } //? 소환시에 랭크만큼의 Value를 빼줌. 강한적만 너무 많이 나오는거 방지용.

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



    public float Speed_Ground { get; set; } //? 클수록 빠름
    public float ActionDelay { get; set; } //? 작을수록 빠름

}