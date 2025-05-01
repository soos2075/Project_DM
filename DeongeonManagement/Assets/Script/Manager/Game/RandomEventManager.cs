using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    #region Singleton
    private static RandomEventManager _instance;
    public static RandomEventManager Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<RandomEventManager>();
            if (_instance == null)
            {
                GameObject go = new GameObject { name = "@RandomEventManager" };
                _instance = go.AddComponent<RandomEventManager>();
            }
            DontDestroyOnLoad(_instance);
        }
    }


    private void Awake()
    {
        Initialize();
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    #endregion

    private void Start()
    {
        Managers.Data.OnAddressablesComplete += () => Init_LocalData();
        Init_RandomEvent();
    }
    public void Init_RandomEvent()
    {
        Init_RandomEvent_Normal();
        Init_RandomEvent_Infinity();
        Init_RandomEvent_Normal_Continue();
        Init_RandomEvent_Visit();
    }



    public void TurnStart()
    {
        //? �� ���۽� �߻��ϴ� �̺�Ʈ�� �ִٸ� �߻�

        int turn = Main.Instance.Turn;

        foreach (var item in CurrentEventList)
        {
            //? �������̸� ���� �� ���۽ÿ� ���ŵǵ��� (�ȱ׷� ���۰� ���� �� ���� �ٸ� �Լ� 2���� �ʿ���)
            if (item.type == RandomEventType.Continuous && item.startDay <= turn && turn < item.endDay)
            {
                RandomEventActionDict[item.ID].Invoke();
                Main.Instance.Dungeon_Animation_RandomEvent(item);

                //Managers.UI.Popup_Reservation(() =>
                //{
                //    var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
                //    msg.Message = GetData(item.ID).description;
                //});
            }

            //? Once�� ���� ������ ���� �ִϸ��̼��� ��� �����ؾߵǼ�
            if (item.type == RandomEventType.Once && item.startDay == turn)
            {
                RandomEventActionDict[item.ID].Invoke();
                Main.Instance.Dungeon_Animation_RandomEvent(item);

                //Managers.UI.Popup_Reservation(() =>
                //{
                //    var msg = Managers.UI.ShowPopUp<UI_SystemMessage>();
                //    msg.Message = GetData(item.ID).description;
                //});
            }
        }
    }

    //? �̺�Ʈ ���� ���� ���� �׷����� ���� �߰��ұ�? �� �� ����
    public const string GoodEvent = "";
    public const string NormalEvent = "";
    public const string BadEvent = "";




    public Dictionary<int, Action> RandomEventActionDict { get; set; } = new Dictionary<int, Action>();


    void Init_RandomEvent_Normal() //? ������ ����� �׼�
    {
        //? 101 ~ : n���� ���� ȸ�� �Ǵ� ���� �Ҵ�
        RandomEventActionDict.Add(101, () => { Debug.Log("Action : 101"); RandomUnitStateEvent(true, 30); });
        RandomEventActionDict.Add(102, () => { Debug.Log("Action : 102"); RandomUnitStateEvent(false, 2); });
        RandomEventActionDict.Add(103, () => { Debug.Log("Action : 103"); RandomUnitStateEvent(false, 4); });
        RandomEventActionDict.Add(104, () => { Debug.Log("Action : 104"); RandomUnitStateEvent(false, 6); });
        RandomEventActionDict.Add(105, () => { Debug.Log("Action : 105"); RandomUnitStateEvent(false, 8); });
        RandomEventActionDict.Add(106, () => { Debug.Log("Action : 106"); RandomUnitStateEvent(true, 3); });
        RandomEventActionDict.Add(107, () => { Debug.Log("Action : 107"); RandomUnitStateEvent(true, 6); });


        //? Herb �̺�Ʈ
        RandomEventActionDict.Add(111, () => { Debug.Log("Action : 111"); RandomFacilityInteractionEvent<Herb>(150); });
        RandomEventActionDict.Add(112, () => { Debug.Log("Action : 112"); RandomFacilityInteractionEvent<Herb>(200); });
        RandomEventActionDict.Add(113, () => { Debug.Log("Action : 113"); RandomFacilityInteractionEvent<Herb>(50); });
        RandomEventActionDict.Add(114, () => { Debug.Log("Action : 114"); RandomFacilityRemoveEvent<Herb>(50); });
        //RandomEventActionDict.Add(115, () => { Debug.Log("Action : 115"); RandomFacilityRemoveEvent<Herb>(100); });
        RandomEventActionDict.Add(115, () => { Debug.Log("Action : 115"); RandomFacilityInteractionEvent<Herb>(0); });

        //? Mineral �̺�Ʈ
        RandomEventActionDict.Add(121, () => { Debug.Log("Action : 121"); RandomFacilityInteractionEvent<Mineral>(150); });
        RandomEventActionDict.Add(122, () => { Debug.Log("Action : 122"); RandomFacilityInteractionEvent<Mineral>(200); });
        RandomEventActionDict.Add(123, () => { Debug.Log("Action : 123"); RandomFacilityInteractionEvent<Mineral>(50); });
        RandomEventActionDict.Add(124, () => { Debug.Log("Action : 124"); RandomFacilityRemoveEvent<Mineral>(50); });
        //RandomEventActionDict.Add(125, () => { Debug.Log("Action : 125"); RandomFacilityRemoveEvent<Mineral>(100); });
        RandomEventActionDict.Add(125, () => { Debug.Log("Action : 125"); RandomFacilityInteractionEvent<Mineral>(0); });


        //? Stat + �̺�Ʈ
        RandomEventActionDict.Add(131, () => { Debug.Log("Action : 131"); UnitStatEvent(Define.StatType.Lv, 1); });
        RandomEventActionDict.Add(132, () => { Debug.Log("Action : 132"); UnitStatEvent(Define.StatType.ATK, 2); });
        RandomEventActionDict.Add(133, () => { Debug.Log("Action : 133"); UnitStatEvent(Define.StatType.DEF, 1); });
        RandomEventActionDict.Add(134, () => { Debug.Log("Action : 134"); UnitStatEvent(Define.StatType.AGI, 1); });
        RandomEventActionDict.Add(135, () => { Debug.Log("Action : 135"); UnitStatEvent(Define.StatType.LUK, 1); });
        RandomEventActionDict.Add(136, () => { Debug.Log("Action : 136"); UnitStatEvent(Define.StatType.ALL, 1); });

        //? Stat - �̺�Ʈ
        RandomEventActionDict.Add(142, () => { Debug.Log("Action : 142"); UnitStatEvent(Define.StatType.ATK, -2); });
        RandomEventActionDict.Add(143, () => { Debug.Log("Action : 143"); UnitStatEvent(Define.StatType.DEF, -1); });
        RandomEventActionDict.Add(144, () => { Debug.Log("Action : 144"); UnitStatEvent(Define.StatType.AGI, -1); });
        RandomEventActionDict.Add(145, () => { Debug.Log("Action : 145"); UnitStatEvent(Define.StatType.LUK, -1); });
        RandomEventActionDict.Add(146, () => { Debug.Log("Action : 146"); UnitStatEvent(Define.StatType.ALL, -1); });
    }
    void Init_RandomEvent_Infinity()
    {

    }

    void Init_RandomEvent_Normal_Continue()
    {

    }

    void Init_RandomEvent_Visit()
    {
        //? �����̺�Ʈ
        RandomEventActionDict.Add(2011, () =>
        {
            Debug.Log("Action : 2011");
            GameManager.NPC.AddEventNPC(NPC_Type_RandomEvent.Mastia.ToString(), 5, NPC_Typeof.NPC_Type_RandomEvent);
        });

        RandomEventActionDict.Add(2012, () =>
        {
            Debug.Log("Action : 2012");
            GameManager.NPC.AddEventNPC(NPC_Type_RandomEvent.Karen.ToString(), 5, NPC_Typeof.NPC_Type_RandomEvent);
        });

        RandomEventActionDict.Add(2013, () =>
        {
            Debug.Log("Action : 2013");
            GameManager.NPC.AddEventNPC(NPC_Type_RandomEvent.Stan.ToString(), 5, NPC_Typeof.NPC_Type_RandomEvent);
        });

        RandomEventActionDict.Add(2014, () =>
        {
            Debug.Log("Action : 2014");
            GameManager.NPC.AddEventNPC(NPC_Type_RandomEvent.Euh.ToString(), 5, NPC_Typeof.NPC_Type_RandomEvent);
        });

        RandomEventActionDict.Add(2015, () =>
        {
            Debug.Log("Action : 2015");
            GameManager.NPC.AddEventNPC(NPC_Type_RandomEvent.Romys.ToString(), 5, NPC_Typeof.NPC_Type_RandomEvent);
        });

        RandomEventActionDict.Add(2016, () =>
        {
            Debug.Log("Action : 2016");
            GameManager.NPC.AddEventNPC(NPC_Type_RandomEvent.Siri.ToString(), 5, NPC_Typeof.NPC_Type_RandomEvent);
        });


    }



    #region Real Action Detail

    //? n���� ���� ȸ�� �Ǵ� �����Ҵ�
    void RandomUnitStateEvent(bool heal, int count)
    {
        if (heal) //? �������� ȸ��
        {
            var injuryList = GameManager.Monster.GetMonsterAll(Monster.MonsterState.Injury);
            Util.ListShuffle(injuryList);

            for (int i = 0; i < count; i++)
            {
                if (injuryList.Count <= i) break; //? ȸ���ؾ��� ������ �ݺ�ȸ������ ������ �극��ũ

                injuryList[i].Recover(0);
            }
        }
        else //? �������� �����Ҵ�
        {
            var monsterAll = GameManager.Monster.GetMonsterAll(Monster.MonsterState.Placement);
            monsterAll.AddRange(GameManager.Monster.GetMonsterAll(Monster.MonsterState.Standby));
            Util.ListShuffle(monsterAll);

            for (int i = 0; i < count; i++)
            {
                if (monsterAll.Count <= i) break; //? ��� ���ּ��� �ݺ�ȸ������ ������ �극��ũ

                monsterAll[i].HP_Damaged = monsterAll[i].HP_Final;
                if (monsterAll[i].State == Monster.MonsterState.Placement)
                {
                    monsterAll[i].MonsterOutFloor();
                }
                monsterAll[i].State = Monster.MonsterState.Injury;
            }
        }
    }

    //? ���� ���Ȱ���

    void UnitStatEvent(Define.StatType _type, int _value)
    {
        var monsterAll = GameManager.Monster.GetMonsterAll();

        foreach (var mon in monsterAll)
        {
            switch (_type)
            {
                case Define.StatType.Lv:
                    mon.LevelUp(true);
                    break;

                default:
                    mon.AddStat_Public(_type, _value);
                    break;
            }
        }
    }


    //? Ư��Ÿ���� facility ���Ƚ�� ����
    void RandomFacilityInteractionEvent<T>(float Percentage) where T : Facility
    {
        var list = GameManager.Facility.GetFacility<T>();
        float perValue = (Percentage / 100f);

        foreach (var item in list)
        {
            item.InteractionOfTimes = Mathf.RoundToInt(item.InteractionOfTimes * perValue);
            if (item.InteractionOfTimes <= 0)
            {
                item.InteractionOfTimes = 1;
            }
        }
    }


    //? Ư��Ÿ���� facility �Ҹ�
    void RandomFacilityRemoveEvent<T>(float Percentage) where T : Facility
    {
        var list = GameManager.Facility.GetFacility<T>();
        float perValue = (Percentage / 100f);

        foreach (var item in list)
        {
            if (UnityEngine.Random.value < perValue)
            {
                GameManager.Facility.RemoveFacility(item);
            }
        }
    }
    //? Ư��Ÿ���� facility â��
    void RandomFacilityCreatEvent<T>(float Percentage) where T : Facility
    {

    }




    #endregion



    public class CurrentRandomEventContent
    {
        public int ID;

        //? �̺�Ʈ ����
        public int index;

        //? �̺�Ʈ ���� ��¥
        public int startDay;

        //? �̺�Ʈ ���� ��¥
        public int endDay;

        //? �ߵ��� / ������
        public RandomEventType type;

        //?  �̺�Ʈ ����
        public RandomEventValue eventValue;


        public CurrentRandomEventContent()
        {

        }


        public CurrentRandomEventContent DeepCopy()
        {
            //? �Ʒ� �޼���� �������� �ʵ带 �������� �ϴ� �޼�����. �ٸ� ���� ��� �ʵ尪�� ��Ÿ���̶� �����簡 �ɻ���.
            CurrentRandomEventContent newConfig = (CurrentRandomEventContent)this.MemberwiseClone();
            return newConfig;
        }

        public void Check_EventInfo()
        {
            Debug.Log($"{index}�� ° �̺�Ʈ ���� \nID : {ID} \nStartDay : {startDay} \nEndDay : {endDay} \nType : {type}");
        }


    }

    public (int _id, int _startDay) Get_NextRandomEventID(int currentTurn, List<CurrentRandomEventContent> eventList)
    {
        foreach (var item in eventList)
        {
            if (item.startDay < currentTurn) //? endDay�� ���ϴ°� �������̿��� �̹� �ߵ��Ȱ� �ǹ̰� �����ϱ�
            {
                continue;
            }
            //? �ϴ� ���������� �������״� �ٷ� ������ �����ϱ� �ϴµ�, ���� �����̺�Ʈ�� �ɸ��ٸ� �ٷ� �����̺�Ʈ���� üũ�� �ʿ� (��¥�������)

            return (item.ID, item.startDay);
        }
        return (-1, -1);
    }




    //? �̹� ȸ���� ��� �� ���� �̺�Ʈ �õ�
    public List<CurrentRandomEventContent> CurrentEventList { get; set; } = new List<CurrentRandomEventContent>();

    #region Save / Load
    public List<CurrentRandomEventContent> Save_RE_Seed()
    {
        var newData = new List<CurrentRandomEventContent>();

        foreach (var item in CurrentEventList)
        {
            newData.Add(item.DeepCopy());
        }
        return newData;
    }

    public void Load_RE_Seed(List<CurrentRandomEventContent> loadData)
    {
        if (loadData == null)
        {
            Debug.Log("Load RE Data Nothing");
            return;
        }

        CurrentEventList = new List<CurrentRandomEventContent>();
        foreach (var item in loadData)
        {
            CurrentEventList.Add(item.DeepCopy());
            item.Check_EventInfo();
        }
    }
    #endregion


    //? �� ���� �õ� ����� - 2ȸ�� �̻� ���� �� ȣ���ϸ� ��
    public void Init_RE_Seed(Define.DifficultyLevel difficulty)
    {
        CurrentEventList = new List<CurrentRandomEventContent>();

        List<SO_RandomEventContent> re = null;
        switch (difficulty)
        {
            case Define.DifficultyLevel.Easy:
                re = Pick_RE(30, 5, 10, RandomEventPool.Normal);
                break;

            case Define.DifficultyLevel.Normal:
                re = Pick_RE(0, 5, 10, RandomEventPool.Normal);
                break;

            case Define.DifficultyLevel.Hard:
                re = Pick_RE(-30, 5, 10, RandomEventPool.Normal);
                break;

            case Define.DifficultyLevel.VeryHard:
                re = Pick_RE(-60, 7, 12, RandomEventPool.Normal);
                break;

            case Define.DifficultyLevel.Master:
                re = Pick_RE(-100, 8, 13, RandomEventPool.Normal);
                break;
        }


        //? ������ ������ �й��ϱ�

        //? �ϳ��� �̺�Ʈ ���� �̺�Ʈ �ִ� ���� (30�� �ƴ������� 1,20,30������ ���ߵǼ�) (�ּҴ� 2�Ϸ�)
        int maxInterval = (27 / re.Count);
        int startDay = 1;
        int counter = 0;


        foreach (var item in re)
        {
            var content = new CurrentRandomEventContent();

            counter++;

            int ranInterval = UnityEngine.Random.Range(2, maxInterval + 1);
            int tempStart = startDay + ranInterval;
            while (tempStart == 20) //? 20���̸� �ٽ�
            {           //? �̺�Ʈ ������ 10���� �Ѿ �� max�� 2�� ����, ���� �������� 2�� �����̱⶧���� 20���� ���ѷ����� �ɸ� �� ����
                        //? ���� ���������� 2���� �ƴ� 3�ϱ����� �����ϰԲ� ������. �̷��� �ص� startDay�� 2�ϵڷ� ���ֱ� ������ 2�� ���� �̺�Ʈ�� �߻��Ҽ��� �ֱ��ѵ�
                        //? �� ��¿ �� ���� �κ�? �׷��ٰ� �̺�Ʈ�� �ִ� 9�������� �����ϴ°� ��¦ �ƽ���ϱ�
                ranInterval = UnityEngine.Random.Range(2, maxInterval + 2);  
                tempStart = startDay + ranInterval;
            }


            startDay += maxInterval;

            content.ID = item.ID;
            content.index = counter;
            content.startDay = tempStart;
            content.endDay = tempStart + item.continuousDays;
            content.type = item.type;
            content.eventValue = item.eventValue;

            CurrentEventList.Add(content);

            //? �����
            content.Check_EventInfo();
        }
    }

    public void Init_RE_Seed_InfinityMode()
    {
        CurrentEventList = new List<CurrentRandomEventContent>();
    }



    #region SO_Data
    SO_RandomEventContent[] so_data;

    public void Init_LocalData()
    {
        so_data = Resources.LoadAll<SO_RandomEventContent>("Data/RandomEvent");
        foreach (var item in so_data)
        {
            string datas = Managers.Data.GetTextData_RandomEvent(item.ID);

            if (datas == null)
            {
                Debug.Log($"RandomEvent ID [{item.ID}] : CSV Data Not Exist");
                continue;
            }

            item.description = datas;
        }
    }

    public SO_RandomEventContent GetData(int _id)
    {
        foreach (var item in so_data)
        {
            if (item.ID == _id)
            {
                return item;
            }
        }
        Debug.Log($"RE_{_id}: Data Not Exist");
        return null;
    }
    #endregion



    #region Random Event Picker

    //? 1. �̺�Ʈ�� ��� �뿡�� �����ϰ� ��� �̾Ƽ� ����Ʈ�� ���� 2. ������ ���� ���ѹݺ�

    //? �̱���
    List<SO_RandomEventContent> PickerList;

    void AddPicker(RandomEventPool addType)
    {
        PickerList = new List<SO_RandomEventContent>();

        foreach (var item in so_data)
        {
            if (item.pool == addType)
            {
                PickerList.Add(item);
            }
        }
    }

    //? ���� �°� �����̺�Ʈ�� �̾ƿ��� �Լ�
    List<SO_RandomEventContent> Pick_RE(int resultValue, int minPick, int maxPick, RandomEventPool addType)
    {
        AddPicker(addType);

        int tempValue = -10000;
        int re_Counter = 0;
        int counter = 0;


        while (ValueCheck(tempValue, resultValue) == false)
        {
            //? �̱��� ����
            Util.ListShuffle(PickerList);
            //? �̺�Ʈ ����
            re_Counter = UnityEngine.Random.Range(minPick, maxPick);
            //? ���� �̺�Ʈ �� ���
            tempValue = Calculation_RE_Value(re_Counter);
            //? �ݺ�Ƚ��
            counter++;
            Debug.Log($"{counter}�� ° �õ� : {re_Counter}�� �̺�Ʈ�� �� = {tempValue}");
        }

        var tempList = new List<SO_RandomEventContent>();

        for (int i = 0; i < re_Counter; i++)
        {
            tempList.Add(PickerList[i]);
        }

        return tempList;
    }

    bool ValueCheck(int _value, int _target)  //? +- 10������ �������� �൵ �Ƿ���?
    {
        if (_value == _target + 10 || _value == _target || _value == _target - 10)
        {
            return true;
        }
        return false;
    }


    int Calculation_RE_Value(int index)
    {
        int sum = 0;

        for (int i = 0; i < index; i++)
        {
            sum += PickerList[i].value;
        }

        return sum;
    }
    #endregion


}

public enum RandomEventPool
{
    Normal,
    Infinity,
}
public enum RandomEventType
{
    Once,
    Continuous,
}

public enum RandomEventValue
{
    Good,
    Bad,
    Normal,
    Special,
}


