using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityManager
{
    public void Init()
    {
        Init_LocalData();

    }

    #region SO_Data
    SO_Facility[] so_data;
    Dictionary<string, SO_Facility> Facility_Dictionary { get; set; } = new Dictionary<string, SO_Facility>();

    void Init_LocalData()
    {
        so_data = Resources.LoadAll<SO_Facility>("Data/Facility");
        foreach (var item in so_data)
        {
            string[] datas = null;
            switch (UserData.Instance.Language)
            {
                case Define.Language.EN:
                    Managers.Data.ObjectsLabel_EN.TryGetValue(item.id, out datas);
                    break;

                case Define.Language.KR:
                    Managers.Data.ObjectsLabel_KR.TryGetValue(item.id, out datas);
                    break;
            }
            if (datas == null)
            {
                Debug.Log($"{item.id} : CSV Data Not Exist");
                continue;
            }

            item.labelName = datas[0];
            item.detail = datas[1];

            Facility_Dictionary.Add(item.keyName, item);
        }
    }


    public SO_Facility GetData(string _keyName)
    {
        SO_Facility facil = null;
        if (Facility_Dictionary.TryGetValue(_keyName, out facil))
        {
            return facil;
        }

        Debug.Log($"{_keyName}: Data Not Exist");
        return null;
    }
    #endregion




    public Action TurnStartAction { get; set; }
    public Action TurnOverAction { get; set; }
    public void TurnStartEvent()
    {
        TurnStartAction?.Invoke();
        TurnStartAction = null;
    }
    public void TurnOverEvent()
    {
        TurnOverAction?.Invoke();
        TurnOverAction = null;
    }



    public List<Facility> facilityList = new List<Facility>();


    public IPlacementable CreateFacility(string _keyName, PlacementInfo info)
    {
        SO_Facility data = GetData(_keyName);
        if (data != null)
        {
            var newObj = GameManager.Placement.CreatePlacementObject(data.prefabPath, info, PlacementType.Facility);
            GameManager.Placement.PlacementConfirm(newObj, info, true);
            var facil = newObj as Facility;
            facil.Data = data;
            facil.SetData();

            facilityList.Add(facil);
            return newObj;
        }

        Debug.Log($"{_keyName}: Create Fail");
        return null;
    }
    public IPlacementable CreateFacility_OnlyOne(string _keyName, PlacementInfo info)
    {
        SO_Facility data = GetData(_keyName);
        if (data != null)
        {
            var newObj = GameManager.Placement.CreateOnlyOne(data.prefabPath, info, PlacementType.Facility);
            GameManager.Placement.PlacementConfirm(newObj, info, true);
            var facil = newObj as Facility;
            facil.Data = data;
            facil.SetData();

            facilityList.Add(newObj as Facility);
            return newObj;
        }

        Debug.Log($"{_keyName}: Create Fail");
        return null;
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
                    var obj = CreateFacility_OnlyOne(data[i].keyName, new PlacementInfo(floor, tile));
                    var facil = obj as Facility;
                    facil.Load_Data(data[i]);
                }
            }
            else
            {
                BasementFloor floor = Main.Instance.Floor[data[i].floorIndex];
                BasementTile tile = null;
                if (floor.TileMap.TryGetValue(data[i].posIndex, out tile))
                {
                    var obj = CreateFacility(data[i].keyName, new PlacementInfo(floor, tile));
                    var facil = obj as Facility;
                    facil.Load_Data(data[i]);
                }
            }
        }
    }
    #endregion


}



public class Save_FacilityData
{
    public string keyName;
    public string prefabPath;

    public int interactionTimes;
    //public int OptionIndex;

    public int floorIndex;
    public Vector2Int posIndex;

    public bool isOnlyOne;

    //public bool isUnchange; //? 필요하면 나중에 쓰자. 타일 형태가 안바뀌는놈. 근데 이동 후 상호작용은 전부 OnlyOne이라서 아직은 필요가없음

    public void SetData(Facility facility)
    {
        keyName = facility.Data.keyName;
        prefabPath = facility.name;

        interactionTimes = facility.InteractionOfTimes;
        //OptionIndex = facility.OptionIndex;

        floorIndex = facility.PlacementInfo.Place_Floor.FloorIndex;
        posIndex = facility.PlacementInfo.Place_Tile.index;

        isOnlyOne = facility.isOnlyOne;
    }
}