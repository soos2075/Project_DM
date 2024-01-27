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



    public enum FacilityType
    {
        Herb,
        Mineral,
        RestZone,
        Trap,
        Entrance,
        Exit,
    }
    public abstract FacilityType Type { get; set; }
    public abstract int InteractionOfTimes { get; set; }
    public abstract string Name { get; set; }

    public abstract void FacilityInit();

    public abstract Coroutine NPC_Interaction(NPC npc);


    protected Coroutine Cor_Facility;

    protected IEnumerator FacilityEvent(NPC npc, float durationTime, int ap, int mp, string text)
    {
        UI_EventBox.AddEventText($"●{npc.Name_KR} (이)가 {PlacementInfo.Place_Floor.Name_KR}에서 {text}");

        yield return new WaitForSeconds(durationTime);

        npc.ActionPoint -= ap;
        npc.Mana -= mp;
        Main.Instance.CurrentDay.AddMana(mp); 


        Cor_Facility = null;
        ClearCheck();
    }

    protected void ClearCheck()
    {
        if (InteractionOfTimes <= 0)
        {
            //UI_EventBox.AddEventText($"{Name} (이)가 사라짐");
            Managers.Placement.PlacementClear(this);
        }
    }


}
