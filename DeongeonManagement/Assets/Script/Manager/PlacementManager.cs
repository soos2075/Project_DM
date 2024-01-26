using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager
{
    public Transform Placement_Root { get; set; }


    public void Init()
    {
        GameObject obj = GameObject.Find("@Placement_Root");
        if (obj == null)
        {
            obj = new GameObject { name = "@Placement_Root" };
        }
        Placement_Root = obj.transform;
    }


    public IPlacementable CreateOnlyOne(string path, PlacementInfo info, Define.PlacementType type)
    {
        var newObj = CreatePlacementObject(path, info, type);
        //? 모든 오브젝트 검색
        var objList = Main.Instance.CurrentFloor.GetFloorObjectList();
        foreach (var item in objList)
        {
            if (newObj.GetType() == item.placementable.GetType())
            {
                PlacementClear_Completely(item.placementable);
            }
        }
        return newObj;
    }

    public IPlacementable CreatePlacementObject(string path, PlacementInfo info, Define.PlacementType type)
    {
        var obj = Managers.Resource.Instantiate(path, Placement_Root).GetComponent<IPlacementable>();
        obj.PlacementType = type;
        obj.PlacementInfo = info;
        Disable(obj);
        return obj;
    }

    public void PlacementConfirm(IPlacementable obj, PlacementInfo newPlace, bool isUnique = false)
    {
        obj.PlacementInfo = newPlace;
        if (isUnique)
        {
            obj.PlacementInfo.Place_Tile.SetUnchangeable(obj);
        }
        else
        {
            obj.PlacementInfo.Place_Tile.SetPlacement(obj);
        }
        
        obj.GetObject().transform.position = obj.PlacementInfo.Place_Tile.worldPosition;
        Visible(obj);

        obj.PlacementInfo.Place_Floor.AddObject(obj);
        //Debug.Log($"{obj.GetObject().name} 가 {newPlace.Place_Floor.Name_KR} - {newPlace.Place_Tile.index} 에 배치");
    }

    public void PlacementClear(IPlacementable obj)
    {
        if (obj.PlacementInfo == null)
        {
            return;
        }

        //Debug.Log($"{obj.GetObject().name} 가 {obj.PlacementInfo.Place_Floor.Name_KR} - {obj.PlacementInfo.Place_Tile.index} 에서 해제");

        obj.PlacementInfo.Place_Floor.RemoveObject(obj);
        obj.PlacementInfo.Place_Tile.ClearPlacement();
        obj.PlacementInfo = null;
        Disable(obj);
    }

    public void PlacementClear_Completely(IPlacementable obj)
    {
        obj.PlacementInfo.Place_Tile.ClearAbsolute();
        PlacementClear(obj);

    }



    public void PlacementMove(IPlacementable obj, PlacementInfo newPlace)
    {
        obj.PlacementInfo.Place_Tile.ClearPlacement();
        obj.PlacementInfo = newPlace;
        newPlace.Place_Tile.SetPlacement(obj);
        obj.GetObject().transform.position = obj.PlacementInfo.Place_Tile.worldPosition;
    }




    public PlacementInfo GetRandomPlacement(IPlacementable placementable, BasementFloor floor)
    {
        var ranTile = floor.GetRandomTile(placementable);
        return new PlacementInfo(floor, ranTile);
    }


    public void Visible(IPlacementable obj)
    {
        SpriteRenderer renderer = obj.GetObject().GetComponentInChildren<SpriteRenderer>();
        if (renderer) renderer.enabled = true;
    }

    public void Disable(IPlacementable obj)
    {
        SpriteRenderer renderer = obj.GetObject().GetComponentInChildren<SpriteRenderer>();
        if (renderer) renderer.enabled = false;
    }

}



public class PlacementInfo
{
    public BasementFloor Place_Floor { get; set; }
    public BasementTile Place_Tile { get; set; }


    public PlacementInfo(BasementFloor floor, BasementTile tile)
    {
        Place_Floor = floor;
        Place_Tile = tile;
    }
}
