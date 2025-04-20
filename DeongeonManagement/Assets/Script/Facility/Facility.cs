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
    public event Action OnDataChangeEvent;
    public void FacilityRemoveEvent()
    {
        if (OnRemoveEvent != null)
        {
            OnRemoveEvent();
            OnRemoveEvent = null;
        }
    }

    public void DataChangeEvent()
    {
        if (OnDataChangeEvent != null)
        {
            OnDataChangeEvent();
            OnDataChangeEvent = null;
        }
    }

    //private void OnDestroy()
    //{
        
    //}

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
    public virtual void Load_Data(Save_FacilityData _data)
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

    public string categoryName;
    public string labelName;

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
        NPC_Interaction, // �Ϲ� ��ȣ�ۿ� / 1ĭ �������� ��ȣ�ۿ���

        NPC_Event, //? Ư�� ��ȣ�ۿ� Ȥ�� �̺�Ʈ / �̵��ؼ� ��ȣ�ۿ���

        Player_Event, //? �÷��̾� ���� �̺�Ʈ(npc�� ���� ��ȣ�ۿ� ����)

        Non_Interaction, //? �ƹ����� ��ȣ�ۿ����������� Ÿ���� �����ؾ���. �̰� ���߿� Ÿ��ui��ü�� ���ִ°͵� ����ϵ�.

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
        UI_EventBox.AddEventText($"��{npc.Name_Color} {text}");

        PlacementState = PlacementState.Busy;

        bool isLastInteraction = false;
        if (InteractionOfTimes <= 0)
        {
            isLastInteraction = true;
        }

        if (npc.TraitCheck(TraitGroup.Swiftness))
        {
            durationTime /= 2;
        }


        yield return new WaitForSeconds(durationTime);
        //? ���� ���ʽ�
        float trait_Ratio = npc.TraitCheck(TraitGroup.Overflow) ? 1.2f : 1;
        //? ������ ���ʽ�
        int manabonus = GameManager.Buff.ManaAdd_Facility;
        //? ���� ���
        int tempMP = (int)(mp * trait_Ratio) + manabonus;
        //? npc�� ������ �� �ִ� �ִ밪 ���
        int applyMana = Mathf.Clamp(tempMP, tempMP, npc.Mana);

        npc.Change_ActionPoint(-ap);
        npc.Change_Mana(-applyMana);
        npc.Change_HP(-hp);
        //? �ִ�ġ �̻����� ȸ����Ű�� ���� ������ ���� -= �ϴ� �κп��� Clamp ���ָ� ��

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
        }
        if (npc.GetComponentInChildren<SpriteRenderer>(true).enabled)
        {
            npc.SetPriorityList_Update();
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
        List<TraitGroup> tagList = _npc.Data.NPC_TraitList;
        List<TraitGroup> bonus = Data.bonusTarget;
        List<TraitGroup> weak = Data.weakTarget;
        List<TraitGroup> invalid = Data.invalidTarget;

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

