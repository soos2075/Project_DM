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
    Dictionary<string, SO_Facility> Facility_Dictionary { get; set; }

    public void Init_LocalData()
    {
        Facility_Dictionary = new Dictionary<string, SO_Facility>();

        so_data = Resources.LoadAll<SO_Facility>("Data/Facility");
        foreach (var item in so_data)
        {
            if (item.id < 0)
            {
                Facility_Dictionary.Add(item.keyName, item);
                continue;
            }

            string[] datas = null;
            switch (UserData.Instance.Language)
            {
                case Define.Language.EN:
                    Managers.Data.ObjectsLabel_EN.TryGetValue(item.id, out datas);
                    break;

                case Define.Language.KR:
                    Managers.Data.ObjectsLabel_KR.TryGetValue(item.id, out datas);
                    break;

                case Define.Language.JP:
                    Managers.Data.ObjectsLabel_JP.TryGetValue(item.id, out datas);
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

    #region 실제 인스턴트

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
            facil.instanceIndex = RandomInstanceIndex();

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
            facil.instanceIndex = RandomInstanceIndex();

            facilityList.Add(newObj as Facility);
            return newObj;
        }

        Debug.Log($"{_keyName}: Create Fail");
        return null;
    }


    public void RemoveFacility(Facility facility)
    {
        GameManager.Placement.PlacementClear_Completely(facility);

        facility.FacilityRemoveEvent();

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



    int RandomInstanceIndex()
    {
        int ran = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        while (Check_Index(ran) == false)
        {
            ran = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        return ran;
    }

    bool Check_Index(int value)
    {
        foreach (var item in facilityList)
        {
            if (item.instanceIndex == value)
            {
                return false;
            }
        }
        return true;
    }


    public Facility GetInstanceOfIndex(int index)
    {
        foreach (var item in facilityList)
        {
            if (item.instanceIndex == index)
            {
                return item;
            }
        }

        return null;
    }


    #endregion







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

    public bool isInit;

    public int interactionTimes;
    //public int OptionIndex;

    public int floorIndex;
    public Vector2Int posIndex;

    public bool isOnlyOne;

    public int instanceIndex;

    public string categoryName;
    public string labelName;

    public void SetData(Facility facility)
    {
        keyName = facility.Data_KeyName;
        prefabPath = facility.name;

        isInit = facility.isInit;

        interactionTimes = facility.InteractionOfTimes;
        //OptionIndex = facility.OptionIndex;

        floorIndex = facility.PlacementInfo.Place_Floor.FloorIndex;
        posIndex = facility.PlacementInfo.Place_Tile.index;

        isOnlyOne = facility.isOnlyOne;

        instanceIndex = facility.instanceIndex;

        categoryName = facility.categoryName;
        labelName = facility.labelName;
    }
}