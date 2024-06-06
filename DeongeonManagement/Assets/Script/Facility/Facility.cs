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
    }

    #endregion



    public SO_Facility Data { get; set; }

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
    }

    protected float durationTime;
    protected int ap_value;
    protected int hp_value;
    protected int mp_value;
    protected int gold_value;
    protected int pop_value;
    protected int danger_value;



    //? �ϳ��� Ŭ������ ����Ÿ���� �������ϴ� ���(������ / ���� / ���ķ� �߰��� �۽Ǹ�Ƽ��.
    //? ���������δ� �Ʒ� FacilityType�� Ŭ������ �Ǿ���. ���� ���� ����, ������ ������ ���� �̷�������.
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
    }
    public FacilityEventType EventType { get; set; }
    public int InteractionOfTimes { get; set; }
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

        yield return new WaitForSeconds(durationTime);

        int applyMana = Mathf.Clamp(mp, 0, npc.Mana); //? ���� ����ȸ������ npc�� ���� ���� �̻����� ���� ����. - �޹��� ������

        npc.ActionPoint -= ap;
        npc.Mana -= applyMana;
        npc.HP -= hp;


        //? �ִ�ġ �̻����� ȸ����Ű�� ���� ������ ���� -= �ϴ� �κп��� Clamp ���ָ� ��

        if (applyMana > 0)
        {
            Main.Instance.CurrentDay.AddMana(applyMana);
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
    }

    protected enum Target
    {
        Nothing,
        Main,
        Sub,
        Weak,
        Invalid,
    }
    string[] GetTargetTypeString(Target target)
    {
        if (Data == null) return null;

        string[] types = null;

        switch (target)
        {
            case Target.Main:
                types = Data.main?.Split(',');
                break;

            case Target.Sub:
                types = Data.sub?.Split(',');
                break;

            case Target.Weak:
                types = Data.weak?.Split(',');
                break;

            case Target.Invalid:
                types = Data.invalid?.Split(',');
                break;
        }

        return types;
    }

    protected Type[] GetTargetType(Target target)
    {
        string[] names = GetTargetTypeString(target);
        if (names == null) 
        {
            Debug.Log("Target Type Not Exist");
            return null;
        }

        Type[] types = new Type[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            Type type = Type.GetType(names[i]);
            types[i] = type;
        }

        return types;
    }
    protected Target GetTarget(NPC npc)
    {
        Type npcType = npc.GetType();

        for (int i = 1; i < Enum.GetNames(typeof(Target)).Length; i++)
        {
            Type[] types = GetTargetType((Target)i);
            // Ÿ�� ����Ʈ�� ������ �ѱ��
            if (types == null) continue;

            foreach (var item in types)
            {
                if (npcType == item)
                {
                    return (Target)i;
                }
            }
        }

        return Target.Nothing;
    }



}
