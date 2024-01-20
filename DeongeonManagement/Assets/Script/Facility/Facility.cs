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
        Initialize();
    }
    protected void Update()
    {

    }
    public BasementFloor Place_Floor { get; set; }
    public BasementTile Place_Tile { get; set; }
    public Define.PlacementType PlacementType { get; set; }

    void Initialize()
    {

    }


    public abstract void Interaction_NPC();





    private void PlacementConfirm(BasementFloor place_floor, BasementTile place_tile)
    {
        Place_Floor = place_floor;
        Place_Tile = place_tile;
        Place_Tile.SetPlacement(this);

        transform.position = Place_Tile.worldPosition;
    }


    public void Placement(BasementFloor place)
    {
        //Debug.Log($"{name} 가 {place} 에 배치됨.");
        PlacementConfirm(place, place.GetRandomTile(this));
    }


    public void PlacementClear()
    {
        //Debug.Log($"{name} 가 대기상태로 들어감.");
        Place_Tile.ClearPlacement();
        Place_Floor = null;
        Place_Tile = null;

    }
}
