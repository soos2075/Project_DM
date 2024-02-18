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
        FacilityInit();
    }
    //protected void Update()
    //{

    //}


    #region IPlacementable
    public Define.PlacementType PlacementType { get; set; }
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
    public void Load_Data(int times)
    {
        InteractionOfTimes = times;
    }
    #endregion






    public enum FacilityType
    {
        Herb,
        Mineral,

        RestZone,
        Treasure,
        Artifact,

        Entrance,
        Exit,
        Portal,

        Trap,

        Special,

        NPCEvent, //? npc�� ��ȣ�ۿ��ϴ� �̺�Ʈ

        PlayerEvent, //? �÷��̾� ���� �̺�Ʈ(npc�� ���� ��ȣ�ۿ� ����)
    }
    public abstract FacilityType Type { get; set; }
    public abstract int InteractionOfTimes { get; set; }
    public abstract string Name { get; set; }
    public string Name_prefab { get; set; }

    public abstract void FacilityInit();

    public abstract Coroutine NPC_Interaction(NPC npc);

    public virtual Coroutine NPC_Interaction_Portal(NPC npc, out int floor)
    {
        floor = 0;
        return null;
    }


    protected Coroutine Cor_Facility;

    protected IEnumerator FacilityEvent(NPC npc, float durationTime, string text, int ap = 0, int mp = 0, int hp = 0)
    {
        UI_EventBox.AddEventText($"��{npc.Name_KR} (��)�� {PlacementInfo.Place_Floor.Name_KR}���� {text}");

        yield return new WaitForSeconds(durationTime);

        int applyMana = Mathf.Clamp(mp, 0, npc.Mana); //? ���� ����ȸ������ npc�� ���� ���� �̻����� ���� ����. - �޹��� ������

        npc.ActionPoint -= ap;
        npc.Mana -= applyMana;
        npc.HP -= hp;


        //? �ִ�ġ �̻����� ȸ����Ű�� ���� ������ ���� -= �ϴ� �κп��� Clamp ���ָ� ��

        if (Type != FacilityType.RestZone) //? �޽����� ���� ������ �÷��̾��� �������� ���̳ʽ��Ǹ� �ȵǴϱ�
        {
            Main.Instance.CurrentDay.AddMana(applyMana);
        }
        

        Cor_Facility = null;
        OverCor(npc);
        ClearCheck();
    }

    protected virtual void OverCor(NPC npc)
    {
        
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
