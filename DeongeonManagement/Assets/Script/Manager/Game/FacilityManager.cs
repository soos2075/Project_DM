using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityManager
{
    public void Init()
    {
        facilityList = new List<Facility>();
    }


    public List<Facility> facilityList;



    public IPlacementable CreateFacility(string prefabName, PlacementInfo info, bool isUnChangeable = false)
    {
        var newObj = GameManager.Placement.CreatePlacementObject($"Facility/{prefabName}", info, Define.PlacementType.Facility);
        GameManager.Placement.PlacementConfirm(newObj, info, isUnChangeable);

        facilityList.Add(newObj as Facility);
        return newObj;
    }
    public IPlacementable CreateFacility_OnlyOne(string prefabName, PlacementInfo info, bool isUnChangeable = false)
    {
        var newObj = GameManager.Placement.CreateOnlyOne($"Facility/{prefabName}", info, Define.PlacementType.Facility);
        GameManager.Placement.PlacementConfirm(newObj, info, isUnChangeable);

        facilityList.Add(newObj as Facility);
        return newObj;
    }

    public void RemoveFacility(Facility facility)
    {
        GameManager.Placement.PlacementClear_Completely(facility);

        facilityList.Remove(facility);
    }


    void AllClear()
    {
        foreach (var item in facilityList)
        {
            GameManager.Placement.PlacementClear_Completely(item);
        }
        facilityList.Clear();
    }



    #region SaveLoad
    public Save_FacilityData[] GetSaveData_Facility()
    {
        Save_FacilityData[] saveData = new Save_FacilityData[facilityList.Count];

        for (int i = 0; i < saveData.Length; i++)
        {
            var data = new Save_FacilityData();
            data.SetData(facilityList[i]);

            saveData[i] = data;
        }
        return saveData;
    }

    public void Load_FacilityData(Save_FacilityData[] data)
    {
        AllClear();

        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].isOnlyOne)
            {
                BasementFloor floor = Main.Instance.Floor[data[i].floorIndex];
                BasementTile tile = null;
                if (floor.TileMap.TryGetValue(data[i].posIndex, out tile))
                {
                    CreateFacility_OnlyOne(data[i].prefabName, new PlacementInfo(floor, tile), true);
                }
            }
            else
            {
                BasementFloor floor = Main.Instance.Floor[data[i].floorIndex];
                BasementTile tile = null;
                if (floor.TileMap.TryGetValue(data[i].posIndex, out tile))
                {
                    CreateFacility(data[i].prefabName, new PlacementInfo(floor, tile), true);
                }
            }
        }
    }
    #endregion


}

public class Save_FacilityData
{
    public string prefabName;
    public int interactionTimes;
    public int floorIndex;
    public Vector2Int posIndex;
    public bool isOnlyOne;

    public bool isUnchange; //? 필요하면 나중에 쓰자. 타일 형태가 안바뀌는놈. 근데 이동 후 상호작용은 전부 OnlyOne이라서 아직은 필요가없음



    public void SetData(Facility facility)
    {
        prefabName = facility.name;
        interactionTimes = facility.InteractionOfTimes;
        floorIndex = facility.PlacementInfo.Place_Floor.FloorIndex;
        posIndex = facility.PlacementInfo.Place_Tile.index;

        switch (facility.Type)
        {
            case Facility.FacilityType.Entrance:
                isOnlyOne = true;
                break;
            case Facility.FacilityType.Exit:
                isOnlyOne = true;
                break;
            case Facility.FacilityType.Portal:
                isOnlyOne = true;
                break;
            case Facility.FacilityType.Special:
                isOnlyOne = true;
                break;
            case Facility.FacilityType.Event:
                isOnlyOne = true;
                break;

            default:
                isOnlyOne = false;
                break;
        }
    }
}