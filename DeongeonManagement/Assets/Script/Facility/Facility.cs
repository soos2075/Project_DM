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

        NPCEvent, //? npc와 상호작용하는 이벤트

        PlayerEvent, //? 플레이어 전용 이벤트(npc는 절대 상호작용 ㄴㄴ)
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
        UI_EventBox.AddEventText($"●{npc.Name_KR} (이)가 {PlacementInfo.Place_Floor.Name_KR}에서 {text}");

        yield return new WaitForSeconds(durationTime);

        int applyMana = Mathf.Clamp(mp, 0, npc.Mana); //? 높은 마나회수여도 npc가 가진 마나 이상으로 얻진 못함. - 앵벌이 방지용

        npc.ActionPoint -= ap;
        npc.Mana -= applyMana;
        npc.HP -= hp;


        //? 최대치 이상으로 회복시키고 싶지 않으면 위에 -= 하는 부분에서 Clamp 해주면 됨

        if (Type != FacilityType.RestZone) //? 휴식으로 차는 마나는 플레이어의 마나에서 마이너스되면 안되니까
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
            //UI_EventBox.AddEventText($"{Name} (이)가 사라짐");
            //Managers.Placement.PlacementClear(this);
            //Managers.Placement.PlacementClear_Completely(this);

            GameManager.Facility.RemoveFacility(this);
        }
    }





}
