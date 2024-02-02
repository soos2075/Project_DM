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
    public string Name_KR { get { return Name; } }
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
        Event,
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

    protected IEnumerator FacilityEvent(NPC npc, float durationTime, string text, int ap = 1, int mp = 1, int hp = 0)
    {
        UI_EventBox.AddEventText($"��{npc.Name_KR} (��)�� {PlacementInfo.Place_Floor.Name_KR}���� {text}");

        yield return new WaitForSeconds(durationTime);

        npc.ActionPoint -= ap;
        npc.Mana -= mp;
        npc.HP -= hp;

        //? �ִ�ġ �̻����� ȸ����Ű�� ���� ������ ���� -= �ϴ� �κп��� Clamp ���ָ� ��

        if (Type != FacilityType.RestZone) //? �޽����� ���� ������ �÷��̾��� �������� ���̳ʽ��Ǹ� �ȵǴϱ�
        {
            Main.Instance.CurrentDay.AddMana(mp);
        }
        

        Cor_Facility = null;

        ClearCheck();
    }

    protected void ClearCheck()
    {
        if (InteractionOfTimes <= 0 && PlacementInfo != null)
        {
            //UI_EventBox.AddEventText($"{Name} (��)�� �����");
            //Managers.Placement.PlacementClear(this);
            Managers.Placement.PlacementClear_Completely(this);
        }
    }


}
