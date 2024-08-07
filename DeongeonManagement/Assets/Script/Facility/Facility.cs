using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Facility : MonoBehaviour, IPlacementable
{
    protected void Awake()
    {

    }
    protected void Start()
    {
        Init_FacilityEgo();
        Init_Personal();
    }

    public event Action OnRemoveEvent;
    public void FacilityRemoveEvent()
    {
        if (OnRemoveEvent != null)
        {
            OnRemoveEvent();
            OnRemoveEvent = null;
        }
    }

    private void OnDestroy()
    {
        
    }

    //protected void Update()
    //{

    //}


    #region IPlacementable
    public PlacementType PlacementType { get; set; }
    public PlacementState PlacementState { get; set; }
    [field: SerializeField]
    public PlacementInfo PlacementInfo { get; set; }
    public GameObject GetObject()
    {
        return this.gameObject;
    }
    public virtual void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;
    }
    public virtual void MouseMoveEvent()
    {
        if (Main.Instance.Management == false) return;
    }
    public virtual void MouseExitEvent()
    {
        if (Main.Instance.Management == false) return;
    }

    public virtual void MouseDownEvent()
    {

    }
    public virtual void MouseUpEvent()
    {

    }

    public string Name_Color { get { return Name; } }

    public string Detail_KR { get; protected set; }
    #endregion


    #region SaveLoad
    public void Load_Data(Save_FacilityData _data)
    {
        InteractionOfTimes = _data.interactionTimes;
        //OptionIndex = _data.OptionIndex;
        isInit = _data.isInit;

        instanceIndex = _data.instanceIndex;
    }

    #endregion




    #region Collection

    public void AddCollectionPoint()
    {
        var collection = CollectionManager.Instance.Get_Collection(Data);
        if (collection != null)
        {
            collection.AddPoint();
        }
    }

    #endregion




    public SO_Facility Data { get; set; }

    public string Data_KeyName { get; set; }
    public int instanceIndex { get; set; }

    public void SetData()
    {
        if (Data == null) return;

        EventType = Data.Type;
        Name = Data.labelName;
        Detail_KR = Data.detail;
        InteractionOfTimes = Data.interactionOfTimes;

        durationTime = Data.durationTime;
        ap_value = Data.ap_value;
        mp_value = Data.mp_value;
        gold_value = Data.gold_value;
        hp_value = Data.hp_value;
        pop_value = Data.pop_value;
        danger_value = Data.danger_value;

        CategoryIndex = Data.id;

        isOnlyOne = Data.isOnlyOne;
        isClearable = Data.isClearable;

        Data_KeyName = Data.keyName;
    }

    protected float durationTime;
    protected int ap_value;
    protected int hp_value;
    protected int mp_value;
    protected int gold_value;
    protected int pop_value;
    protected int danger_value;



    public int CategoryIndex { get; set; }
    public int LabelIndex { get; set; }

    public bool isInit { get; set; }


    public bool isOnlyOne { get; set; } = false;
    public bool isClearable { get; set; } = true;

    public virtual void Init_FacilityEgo()
    {
        isOnlyOne = false;
        isClearable = true;
    }
    public enum FacilityEventType
    {
        NPC_Interaction, // 일반 상호작용 / 1칸 떨어져서 상호작용함

        NPC_Event, //? 특수 상호작용 혹은 이벤트 / 이동해서 상호작용함

        Player_Event, //? 플레이어 전용 이벤트(npc는 절대 상호작용 ㄴㄴ)

        Non_Interaction, //? 아무랑도 상호작용하지않지만 타일은 차지해야함. 이거 나중에 타일ui자체도 없애는것도 방법일듯.

        //NPC_Interaction_Wall = 4,
    }
    public FacilityEventType EventType { get; set; }
    public virtual int InteractionOfTimes { get; set; }
    public virtual int IOT_Temp { get; set; } = 0;
    public string Name { get; set; }

    public abstract void Init_Personal();

    public abstract Coroutine NPC_Interaction(NPC npc);


    protected Coroutine Cor_Facility { get; set; }

    protected IEnumerator FacilityEvent(NPC npc, float durationTime, string text, int ap = 0, int mp = 0, int hp = 0)
    {
        UI_EventBox.AddEventText($"●{npc.Name_Color} {text}");

        PlacementState = PlacementState.Busy;

        bool isLastInteraction = false;
        if (InteractionOfTimes <= 0)
        {
            isLastInteraction = true;
        }

        yield return new WaitForSeconds(durationTime);
        int manabonus = mp + GameManager.Buff.FacilityBonus;
        int applyMana = Mathf.Clamp(manabonus, manabonus, npc.Mana); //? 높은 마나회수여도 npc가 가진 마나 이상으로 얻진 못함. - 앵벌이 방지용

        npc.ActionPoint -= ap;
        npc.Mana -= applyMana;
        npc.HP -= hp;

        //? 최대치 이상으로 회복시키고 싶지 않으면 위에 -= 하는 부분에서 Clamp 해주면 됨

        if (applyMana > 0)
        {
            Main.Instance.CurrentDay.AddMana(applyMana, Main.DayResult.EventType.Facility);
            Main.Instance.ShowDM(applyMana, Main.TextType.mana, transform);
        }

        OverCor(npc, isLastInteraction);
    }

    protected virtual void OverCor(NPC npc, bool isRemove)
    {
        Cor_Facility = null;
        PlacementState = PlacementState.Standby;
        if (isRemove)
        {
            GameManager.Facility.RemoveFacility(this);
            if (npc.gameObject.activeInHierarchy)
            {
                npc.SetPriorityListForPublic();
            }
        }

        AddCollectionPoint();
    }

    protected enum Target
    {
        //Nothing,
        Bonus,
        Normal,
        Weak,
        Invalid,
    }

    protected Target TagCheck(NPC _npc)
    {
        List<TagGroup> tagList = _npc.Data.FacilityTagList;
        List<TagGroup> bonus = Data.bonusTarget;
        List<TagGroup> weak = Data.weakTarget;
        List<TagGroup> invalid = Data.invalidTarget;

        foreach (var item in tagList)
        {
            foreach (var item2 in bonus)
            {
                if (item == item2)
                {
                    return Target.Bonus;
                }
            }
            foreach (var item2 in weak)
            {
                if (item == item2)
                {
                    return Target.Weak;
                }
            }
            foreach (var item2 in invalid)
            {
                if (item == item2)
                {
                    return Target.Invalid;
                }
            }
        }

        return Target.Normal;
    }


    //string[] GetTargetTypeString(Target target)
    //{
    //    if (Data == null) return null;

    //    string[] types = null;

    //    switch (target)
    //    {
    //        case Target.Main:
    //            types = Data.main?.Split(',');
    //            break;

    //        case Target.Normal:
    //            types = Data.sub?.Split(',');
    //            break;

    //        case Target.Weak:
    //            types = Data.weak?.Split(',');
    //            break;

    //        case Target.Invalid:
    //            types = Data.invalid?.Split(',');
    //            break;
    //    }

    //    return types;
    //}

    //protected Type[] GetTargetType(Target target)
    //{
    //    string[] names = GetTargetTypeString(target);
    //    if (names == null) 
    //    {
    //        Debug.Log("Target Type Not Exist");
    //        return null;
    //    }

    //    Type[] types = new Type[names.Length];

    //    for (int i = 0; i < names.Length; i++)
    //    {
    //        Type type = Type.GetType(names[i]);
    //        types[i] = type;
    //    }

    //    return types;
    //}
    //protected Target GetTarget(NPC npc)
    //{
    //    Type npcType = npc.GetType();

    //    for (int i = 1; i < Enum.GetNames(typeof(Target)).Length; i++)
    //    {
    //        Type[] types = GetTargetType((Target)i);
    //        // 타겟 리스트가 없으면 넘기기
    //        if (types == null) continue;

    //        foreach (var item in types)
    //        {
    //            if (npcType == item)
    //            {
    //                return (Target)i;
    //            }
    //        }
    //    }

    //    return Target.Nothing;
    //}


    //protected Target GetTarget(TagGroup npc)
    //{
    //    Type npcType = npc.GetType();

    //    for (int i = 1; i < Enum.GetNames(typeof(Target)).Length; i++)
    //    {
    //        Type[] types = GetTargetType((Target)i);
    //        // 타겟 리스트가 없으면 넘기기
    //        if (types == null) continue;

    //        foreach (var item in types)
    //        {
    //            if (npcType == item)
    //            {
    //                return (Target)i;
    //            }
    //        }
    //    }

    //    return Target.Nothing;
    //}



    #region DayEvent_Facility
    protected enum DayType
    {
        Day,
        Night,
    }

    protected void AddTurnEvent(Action<int> _action, DayType dayType)
    {
        switch (dayType)
        {
            case DayType.Day:
                Main.Instance.DayActions.Add(_action);
                break;

            case DayType.Night:
                Main.Instance.NightActions.Add(_action);
                break;

        }
    }
    protected void RemoveTurnEvent(Action<int> _action, DayType dayType)
    {
        switch (dayType)
        {
            case DayType.Day:
                Main.Instance.DayActions.Remove(_action);
                break;

            case DayType.Night:
                Main.Instance.NightActions.Remove(_action);
                break;
        }
    }
    #endregion

}

