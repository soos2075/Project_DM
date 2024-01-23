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


    public abstract void FacilityInit();

    public abstract Coroutine NPC_Interaction(NPC npc);


}
