using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Facility : MonoBehaviour, Interface.IPlacementable
{
    protected void Awake()
    {
        PlacementType = Define.PlacementType.Facility;
    }
    protected void Start()
    {
        FacilityInit();

        facilityRenderer = GetComponentInChildren<SpriteRenderer>();

        Placement(Main.Instance.CurrentFloor);
    }
    protected void Update()
    {

    }


    public enum FacilityType
    {
        Herb,
        Mineral,
        RestZone,
        Trap,

    }


    public abstract FacilityType Type { get; set; }
    public abstract int InteractionOfTimes { get; set; }


    public BasementFloor Place_Floor { get; set; }
    public BasementTile Place_Tile { get; set; }
    public Define.PlacementType PlacementType { get; set; }

    SpriteRenderer facilityRenderer;


    public abstract void FacilityInit();

    public abstract Coroutine NPC_Interaction(NPC npc);





    public void Placement(BasementFloor place)
    {
        PlacementConfirm(place, place.GetRandomTile(this));
        Debug.Log($"{name} 가 {Place_Floor} - {Place_Tile.index} 에 배치됨.");
    }


    protected void PlacementConfirm(BasementFloor place_floor, BasementTile place_tile)
    {
        Place_Floor = place_floor;
        Place_Tile = place_tile;
        Place_Tile.SetPlacement(this);

        transform.position = Place_Tile.worldPosition;
        Visible();
    }

    public void PlacementClear()
    {
        Debug.Log($"{name} 가 {Place_Floor} - {Place_Tile.index}에서 비활성화");

        Place_Tile.ClearPlacement();
        Place_Floor = null;
        Place_Tile = null;

        Disable();
    }

    protected void Visible()
    {
        facilityRenderer.enabled = true;
    }
    protected void Disable()
    {
        facilityRenderer.enabled = false;
    }
}
