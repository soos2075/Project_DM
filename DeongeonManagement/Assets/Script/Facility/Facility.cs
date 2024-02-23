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


    //? 하나의 클래스에 여러타입을 가져야하는 경우(조각상 / 함정 / 이후로 추가할 퍼실리티들.
    //? 최종적으로는 아래 FacilityType이 클래스가 되야함. 허브는 허브로 통합, 광물은 광물로 통합 이런식으로.
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

        NPC_Interaction, // 일반 상호작용 / 1칸 떨어져서 상호작용함

        //NPC_Portal,

        NPC_Event, //? 특수 상호작용 혹은 이벤트 / 이동해서 상호작용함

        Player_Event, //? 플레이어 전용 이벤트(npc는 절대 상호작용 ㄴㄴ)

        Non_Interaction, //? 아무랑도 상호작용하지않지만 타일은 차지해야함. 이거 나중에 타일ui자체도 없애는것도 방법일듯.
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
        UI_EventBox.AddEventText($"●{npc.Name_KR} (이)가 {PlacementInfo.Place_Floor.Name_KR}에서 {text}");

        PlacementState = PlacementState.Busy;

        yield return new WaitForSeconds(durationTime);

        int applyMana = Mathf.Clamp(mp, 0, npc.Mana); //? 높은 마나회수여도 npc가 가진 마나 이상으로 얻진 못함. - 앵벌이 방지용

        npc.ActionPoint -= ap;
        npc.Mana -= applyMana;
        npc.HP -= hp;


        //? 최대치 이상으로 회복시키고 싶지 않으면 위에 -= 하는 부분에서 Clamp 해주면 됨

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
            //UI_EventBox.AddEventText($"{Name} (이)가 사라짐");
            //Managers.Placement.PlacementClear(this);
            //Managers.Placement.PlacementClear_Completely(this);

            GameManager.Facility.RemoveFacility(this);
        }
    }





}
