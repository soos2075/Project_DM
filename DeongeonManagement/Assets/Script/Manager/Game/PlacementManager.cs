using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager
{
    public Transform Placement_Root { get { return Init(); } }


    public Transform Init()
    {
        GameObject obj = GameObject.Find("@Placement_Root");
        if (obj == null)
        {
            obj = new GameObject { name = "@Placement_Root" };
        }
        return obj.transform;
    }


    public IPlacementable CreateOnlyOne(string path, PlacementInfo info, Define.PlacementType type)
    {
        var newObj = CreatePlacementObject(path, info, type);
        //? 모든 오브젝트 검색
        var objList = info.Place_Floor.GetFloorObjectList();
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

    public void PlacementConfirm(IPlacementable obj, PlacementInfo newPlace, bool isUnchange = false)
    {
        obj.PlacementInfo = newPlace;
        if (isUnchange)
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
    public void PlacementMove_NPC(NPC npc, PlacementInfo newPlace, float duration)
    {
        Managers.Instance.StartCoroutine(MoveUpdate(npc, npc.PlacementInfo.Place_Tile, newPlace.Place_Tile, duration));

        npc.PlacementInfo.Place_Tile.ClearPlacement();
        npc.PlacementInfo = newPlace;
        newPlace.Place_Tile.SetPlacement(npc);
        //npc.GetObject().transform.position = npc.PlacementInfo.Place_Tile.worldPosition;
    }

    IEnumerator MoveUpdate(NPC npc, BasementTile startPos, BasementTile endPos, float duration)
    {
        Vector3 dir = endPos.worldPosition - startPos.worldPosition;
        Debug.Log(dir);
        if (dir.x > 0)
        {
            //? 무브 오른쪽
            npc.Anim_State = NPC.moveState.right;
        }
        else if (dir.x < 0)
        {
            //? 왼쪽
            npc.Anim_State = NPC.moveState.left;
        }
        else if (dir.y > 0)
        {
            //? 위
            npc.Anim_State = NPC.moveState.back;
        }
        else if (dir.y < 0)
        {
            //? 아래
            npc.Anim_State = NPC.moveState.front;
        }


        float dis = Vector3.Distance(startPos.worldPosition, endPos.worldPosition);

        float moveValue = dis / duration;
        float timer = 0;

        while (timer < duration)// && dis > 0.05f)
        {
            yield return null;
            timer += Time.deltaTime;
            npc.transform.position += dir.normalized * moveValue * Time.deltaTime;
            //dis = Vector3.Distance(npc.transform.position, endPos.worldPosition);
        }

        npc.transform.position = endPos.worldPosition;
    }




    //public PlacementInfo GetRandomPlacement(IPlacementable placementable, BasementFloor floor)
    //{
    //    var ranTile = floor.GetRandomTile(placementable);
    //    return new PlacementInfo(floor, ranTile);
    //}


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


[System.Serializable]
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
