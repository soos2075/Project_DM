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

        //Placement(Main.Instance.CurrentFloor);
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
        Entrance,
        Exit,
    }


    public abstract FacilityType Type { get; set; }
    public abstract int InteractionOfTimes { get; set; }


    public BasementFloor Place_Floor { get; set; }
    public BasementTile Place_Tile { get; set; }
    public Define.PlacementType PlacementType { get; set; }

    SpriteRenderer facilityRenderer;


    public abstract void FacilityInit();

    public abstract Coroutine NPC_Interaction(NPC npc);





    void Placement_Random(BasementFloor place)
    {
        PlacementConfirm(place, place.GetRandomTile(this));
        Debug.Log($"{name} �� {Place_Floor} - {Place_Tile.index} �� ��ġ��.");
    }


    public void PlacementConfirm(BasementFloor place_floor, BasementTile place_tile)
    {
        Place_Floor = place_floor;
        Place_Tile = place_tile;
        Place_Tile.SetPlacement(this);

        transform.position = Place_Tile.worldPosition;
        Visible();
    }

    public void PlacementClear()
    {
        //Debug.Log($"{name} �� {Place_Floor} - {Place_Tile.index}���� ��Ȱ��ȭ");

        Place_Tile.ClearPlacement();
        Place_Floor = null;
        Place_Tile = null;

        Disable();
    }

    protected void Visible()
    {
        if (facilityRenderer == null) return;
        facilityRenderer.enabled = true;
    }
    protected void Disable()
    {
        if (facilityRenderer == null) return;
        facilityRenderer.enabled = false;
    }
}
