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
        UI_EventBox.AddEventText($"●{npc.Name_KR} (이)가 {PlacementInfo.Place_Floor.Name_KR}에서 {text}");

        yield return new WaitForSeconds(durationTime);

        npc.ActionPoint -= ap;
        npc.Mana -= mp;
        npc.HP -= hp;

        //? 최대치 이상으로 회복시키고 싶지 않으면 위에 -= 하는 부분에서 Clamp 해주면 됨

        if (Type != FacilityType.RestZone) //? 휴식으로 차는 마나는 플레이어의 마나에서 마이너스되면 안되니까
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
            //UI_EventBox.AddEventText($"{Name} (이)가 사라짐");
            //Managers.Placement.PlacementClear(this);
            Managers.Placement.PlacementClear_Completely(this);
        }
    }


}
