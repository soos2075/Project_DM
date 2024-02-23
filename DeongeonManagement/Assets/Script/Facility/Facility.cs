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
        SetFacilityBool();
        FacilityInit();
    }
    //protected void Update()
    //{

    //}


    #region IPlacementable
    public PlacementType PlacementType { get; set; }
    public PlacementState PlacementState { get; set; }
    public PlacementInfo PlacementInfo { get; set; }
    public GameObject GetObject()
    {
        return this.gameObject;
    }
    public virtual void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;
    }
    public string Name_KR { get { return Name; } }

    public string Detail_KR { get; protected set; }
    #endregion


    #region SaveLoad
    public void Load_Data(Save_FacilityData _data)
    {
        InteractionOfTimes = _data.interactionTimes;
        OptionIndex = _data.OptionIndex;
    }

    #endregion


    //? �ϳ��� Ŭ������ ����Ÿ���� �������ϴ� ���(������ / ���� / ���ķ� �߰��� �۽Ǹ�Ƽ��.
    //? ���������δ� �Ʒ� FacilityType�� Ŭ������ �Ǿ���. ���� ���� ����, ������ ������ ���� �̷�������.
    public virtual int OptionIndex { get; set; }
    public bool isOnlyOne { get; set; } = false;
    public bool isClearable { get; set; } = true;

    public virtual void SetFacilityBool()
    {
        isOnlyOne = false;
        isClearable = true;
    }
    public enum FacilityEventType
    {

        NPC_Interaction, // �Ϲ� ��ȣ�ۿ� / 1ĭ �������� ��ȣ�ۿ���

        //NPC_Portal,

        NPC_Event, //? Ư�� ��ȣ�ۿ� Ȥ�� �̺�Ʈ / �̵��ؼ� ��ȣ�ۿ���

        Player_Event, //? �÷��̾� ���� �̺�Ʈ(npc�� ���� ��ȣ�ۿ� ����)

        Non_Interaction, //? �ƹ����� ��ȣ�ۿ����������� Ÿ���� �����ؾ���. �̰� ���߿� Ÿ��ui��ü�� ���ִ°͵� ����ϵ�.
    }
    public abstract FacilityEventType Type { get; set; }
    public abstract int InteractionOfTimes { get; set; }
    public abstract string Name { get; set; }
    public string Name_prefab { get; set; }

    public abstract void FacilityInit();

    public abstract Coroutine NPC_Interaction(NPC npc);


    protected Coroutine Cor_Facility { get; set; }

    protected IEnumerator FacilityEvent(NPC npc, float durationTime, string text, int ap = 0, int mp = 0, int hp = 0)
    {
        UI_EventBox.AddEventText($"��{npc.Name_KR} (��)�� {PlacementInfo.Place_Floor.Name_KR}���� {text}");

        PlacementState = PlacementState.Busy;

        yield return new WaitForSeconds(durationTime);

        int applyMana = Mathf.Clamp(mp, 0, npc.Mana); //? ���� ����ȸ������ npc�� ���� ���� �̻����� ���� ����. - �޹��� ������

        npc.ActionPoint -= ap;
        npc.Mana -= applyMana;
        npc.HP -= hp;


        //? �ִ�ġ �̻����� ȸ����Ű�� ���� ������ ���� -= �ϴ� �κп��� Clamp ���ָ� ��

        if (applyMana > 0)
        {
            Main.Instance.CurrentDay.AddMana(applyMana);
            var dm = Main.Instance.dmMesh_dungeon.Spawn(transform.position, $"+{applyMana} mana");
            dm.SetColor(Color.blue);
        }

        OverCor(npc);
    }

    protected virtual void OverCor(NPC npc)
    {
        Cor_Facility = null;
        PlacementState = PlacementState.Standby;
        ClearCheck();
    }


    protected void ClearCheck()
    {
        if (InteractionOfTimes <= 0 && PlacementInfo != null)
        {
            //UI_EventBox.AddEventText($"{Name} (��)�� �����");
            //Managers.Placement.PlacementClear(this);
            //Managers.Placement.PlacementClear_Completely(this);

            GameManager.Facility.RemoveFacility(this);
        }
    }





}
