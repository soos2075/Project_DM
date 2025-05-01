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
        //? 턴 시작시 발생하는 이벤트가 있다면 발생

        int turn = Main.Instance.Turn;

        foreach (var item in CurrentEventList)
        {
            //? 지속형이면 매일 턴 시작시에 갱신되도록 (안그럼 시작과 끝날 때 서로 다른 함수 2개가 필요함)
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

            //? Once가 뒤인 이유는 던전 애니메이션을 얘로 갱신해야되서
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

    //? 이벤트 내용 전에 대충 그럴싸한 말을 추가할까? 할 때 쓸거
    public const string GoodEvent = "";
    public const string NormalEvent = "";
    public const string BadEvent = "";




    public Dictionary<int, Action> RandomEventActionDict { get; set; } = new Dictionary<int, Action>();


    void Init_RandomEvent_Normal() //? 실제로 실행될 액션
    {
        //? 101 ~ : n기의 유닛 회복 또는 전투 불능
        RandomEventActionDict.Add(101, () => { Debug.Log("Action : 101"); RandomUnitStateEvent(true, 30); });
        RandomEventActionDict.Add(102, () => { Debug.Log("Action : 102"); RandomUnitStateEvent(false, 2); });
        RandomEventActionDict.Add(103, () => { Debug.Log("Action : 103"); RandomUnitStateEvent(false, 4); });
        RandomEventActionDict.Add(104, () => { Debug.Log("Action : 104"); RandomUnitStateEvent(false, 6); });
        RandomEventActionDict.Add(105, () => { Debug.Log("Action : 105"); RandomUnitStateEvent(false, 8); });
        RandomEventActionDict.Add(106, () => { Debug.Log("Action : 106"); RandomUnitStateEvent(true, 3); });
        RandomEventActionDict.Add(107, () => { Debug.Log("Action : 107"); RandomUnitStateEvent(true, 6); });


        //? Herb 이벤트
        RandomEventActionDict.Add(111, () => { Debug.Log("Action : 111"); RandomFacilityInteractionEvent<Herb>(150); });
        RandomEventActionDict.Add(112, () => { Debug.Log("Action : 112"); RandomFacilityInteractionEvent<Herb>(200); });
        RandomEventActionDict.Add(113, () => { Debug.Log("Action : 113"); RandomFacilityInteractionEvent<Herb>(50); });
        RandomEventActionDict.Add(114, () => { Debug.Log("Action : 114"); RandomFacilityRemoveEvent<Herb>(50); });
        //RandomEventActionDict.Add(115, () => { Debug.Log("Action : 115"); RandomFacilityRemoveEvent<Herb>(100); });
        RandomEventActionDict.Add(115, () => { Debug.Log("Action : 115"); RandomFacilityInteractionEvent<Herb>(0); });

        //? Mineral 이벤트
        RandomEventActionDict.Add(121, () => { Debug.Log("Action : 121"); RandomFacilityInteractionEvent<Mineral>(150); });
        RandomEventActionDict.Add(122, () => { Debug.Log("Action : 122"); RandomFacilityInteractionEvent<Mineral>(200); });
        RandomEventActionDict.Add(123, () => { Debug.Log("Action : 123"); RandomFacilityInteractionEvent<Mineral>(50); });
        RandomEventActionDict.Add(124, () => { Debug.Log("Action : 124"); RandomFacilityRemoveEvent<Mineral>(50); });
        //RandomEventActionDict.Add(125, () => { Debug.Log("Action : 125"); RandomFacilityRemoveEvent<Mineral>(100); });
        RandomEventActionDict.Add(125, () => { Debug.Log("Action : 125"); RandomFacilityInteractionEvent<Mineral>(0); });


        //? Stat + 이벤트
        RandomEventActionDict.Add(131, () => { Debug.Log("Action : 131"); UnitStatEvent(Define.StatType.Lv, 1); });
        RandomEventActionDict.Add(132, () => { Debug.Log("Action : 132"); UnitStatEvent(Define.StatType.ATK, 2); });
        RandomEventActionDict.Add(133, () => { Debug.Log("Action : 133"); UnitStatEvent(Define.StatType.DEF, 1); });
        RandomEventActionDict.Add(134, () => { Debug.Log("Action : 134"); UnitStatEvent(Define.StatType.AGI, 1); });
        RandomEventActionDict.Add(135, () => { Debug.Log("Action : 135"); UnitStatEvent(Define.StatType.LUK, 1); });
        RandomEventActionDict.Add(136, () => { Debug.Log("Action : 136"); UnitStatEvent(Define.StatType.ALL, 1); });

        //? Stat - 이벤트
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
        //? 만남이벤트
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

    //? n개의 유닛 회복 또는 전투불능
    void RandomUnitStateEvent(bool heal, int count)
    {
        if (heal) //? 랜덤유닛 회복
        {
            var injuryList = GameManager.Monster.GetMonsterAll(Monster.MonsterState.Injury);
            Util.ListShuffle(injuryList);

            for (int i = 0; i < count; i++)
            {
                if (injuryList.Count <= i) break; //? 회복해야할 유닛이 반복회수보다 적으면 브레이크

                injuryList[i].Recover(0);
            }
        }
        else //? 랜덤유닛 전투불능
        {
            var monsterAll = GameManager.Monster.GetMonsterAll(Monster.MonsterState.Placement);
            monsterAll.AddRange(GameManager.Monster.GetMonsterAll(Monster.MonsterState.Standby));
            Util.ListShuffle(monsterAll);

            for (int i = 0; i < count; i++)
            {
                if (monsterAll.Count <= i) break; //? 대상 유닛수가 반복회수보다 적으면 브레이크

                monsterAll[i].HP_Damaged = monsterAll[i].HP_Final;
                if (monsterAll[i].State == Monster.MonsterState.Placement)
                {
                    monsterAll[i].MonsterOutFloor();
                }
                monsterAll[i].State = Monster.MonsterState.Injury;
            }
        }
    }

    //? 유닛 스탯관련

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


    //? 특정타입의 facility 사용횟수 증감
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


    //? 특정타입의 facility 소멸
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
    //? 특정타입의 facility 창조
    void RandomFacilityCreatEvent<T>(float Percentage) where T : Facility
    {

    }




    #endregion



    public class CurrentRandomEventContent
    {
        public int ID;

        //? 이벤트 순서
        public int index;

        //? 이벤트 시작 날짜
        public int startDay;

        //? 이벤트 종료 날짜
        public int endDay;

        //? 발동형 / 지속형
        public RandomEventType type;

        //?  이벤트 종류
        public RandomEventValue eventValue;


        public CurrentRandomEventContent()
        {

        }


        public CurrentRandomEventContent DeepCopy()
        {
            //? 아래 메서드는 어디까지나 필드를 얕은복사 하는 메서드임. 다만 현재 모든 필드값이 값타입이라 값복사가 될뿐임.
            CurrentRandomEventContent newConfig = (CurrentRandomEventContent)this.MemberwiseClone();
            return newConfig;
        }

        public void Check_EventInfo()
        {
            Debug.Log($"{index}번 째 이벤트 정보 \nID : {ID} \nStartDay : {startDay} \nEndDay : {endDay} \nType : {type}");
        }


    }

    public (int _id, int _startDay) Get_NextRandomEventID(int currentTurn, List<CurrentRandomEventContent> eventList)
    {
        foreach (var item in eventList)
        {
            if (item.startDay < currentTurn) //? endDay로 안하는건 지속형이여도 이미 발동된건 의미가 없으니까
            {
                continue;
            }
            //? 일단 순차적으로 되있을테니 바로 다음꺼 리턴하긴 하는데, 만약 나중이벤트가 걸린다면 바로 다음이벤트인지 체크가 필요 (날짜계산으로)

            return (item.ID, item.startDay);
        }
        return (-1, -1);
    }




    //? 이번 회차에 사용 될 랜덤 이벤트 시드
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


    //? 새 게임 시드 만들기 - 2회차 이상 게임 시 호출하면 댐
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


        //? 뽑은거 적당히 분배하기

        //? 하나의 이벤트 다음 이벤트 최대 간격 (30이 아닌이유는 1,20,30일차는 빼야되서) (최소는 2일로)
        int maxInterval = (27 / re.Count);
        int startDay = 1;
        int counter = 0;


        foreach (var item in re)
        {
            var content = new CurrentRandomEventContent();

            counter++;

            int ranInterval = UnityEngine.Random.Range(2, maxInterval + 1);
            int tempStart = startDay + ranInterval;
            while (tempStart == 20) //? 20일이면 다시
            {           //? 이벤트 개수가 10개가 넘어갈 때 max는 2로 고정, 따라서 랜덤값도 2로 고정이기때문에 20에서 무한루프가 걸릴 수 있음
                        //? 따라서 루프에서는 2일이 아닌 3일까지도 가능하게끔 변경함. 이렇게 해도 startDay는 2일뒤로 되있기 때문에 2일 연속 이벤트가 발생할수도 있긴한데
                        //? 뭐 어쩔 수 없는 부분? 그렇다고 이벤트를 최대 9개까지로 제한하는건 살짝 아쉬우니까
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

            //? 디버그
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

    //? 1. 이벤트가 담긴 통에서 랜덤하게 몇개를 뽑아서 리스트를 만듦 2. 점수에 따라서 무한반복

    //? 뽑기통
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

    //? 값에 맞게 랜덤이벤트를 뽑아오는 함수
    List<SO_RandomEventContent> Pick_RE(int resultValue, int minPick, int maxPick, RandomEventPool addType)
    {
        AddPicker(addType);

        int tempValue = -10000;
        int re_Counter = 0;
        int counter = 0;


        while (ValueCheck(tempValue, resultValue) == false)
        {
            //? 뽑기통 셔플
            Util.ListShuffle(PickerList);
            //? 이벤트 개수
            re_Counter = UnityEngine.Random.Range(minPick, maxPick);
            //? 뽑힌 이벤트 값 계산
            tempValue = Calculation_RE_Value(re_Counter);
            //? 반복횟수
            counter++;
            Debug.Log($"{counter}번 째 시도 : {re_Counter}개 이벤트의 값 = {tempValue}");
        }

        var tempList = new List<SO_RandomEventContent>();

        for (int i = 0; i < re_Counter; i++)
        {
            tempList.Add(PickerList[i]);
        }

        return tempList;
    }

    bool ValueCheck(int _value, int _target)  //? +- 10정도의 유도리는 줘도 되려나?
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


