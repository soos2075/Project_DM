using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TechnicalManager
{
    public void Init()
    {
        Init_LocalData();
        FloorInit();
    }


    #region SO_Data
    SO_Technical[] so_data;
    Dictionary<string, SO_Technical> Technical_Dictionary { get; set; } = new Dictionary<string, SO_Technical>();

    void Init_LocalData()
    {
        so_data = Resources.LoadAll<SO_Technical>("Data/Technical");
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

            item.action = () => CreateAction(item.keyName);

            Technical_Dictionary.Add(item.keyName, item);
        }
    }

    public List<SO_Technical> GetTechnicalList(int _DungeonRank = 1)
    {
        List<SO_Technical> list = new List<SO_Technical>();

        foreach (var item in so_data)
        {
            if (item.UnlockRank <= _DungeonRank)
            {
                list.Add(item);
            }
        }

        list.Sort((a, b) => a.id.CompareTo(b.id));
        return list;
    }

    public SO_Technical GetData(string _keyName)
    {
        SO_Technical facil = null;
        if (Technical_Dictionary.TryGetValue(_keyName, out facil))
        {
            return facil;
        }

        Debug.Log($"{_keyName}: Data Not Exist");
        return null;
    }


    #endregion





    Prison _prison;
    public Prison Prison 
    { 
        get
        {
            if (_prison == null)
            {
                foreach (var item in currentTechnicalList)
                {
                    if (item.GetType() == typeof(Prison))
                    {
                        _prison = item as Prison;
                    }
                }
                //_prison = GameManager.FindObjectOfType<Prison>();
            }

            return _prison;
        } 
        set { _prison = value; }
    }

    DonationBox _donation;
    public DonationBox Donation
    {
        get
        {
            if (_donation == null)
            {
                foreach (var item in currentTechnicalList)
                {
                    if (item.GetType() == typeof(DonationBox))
                    {
                        _donation = item as DonationBox;
                    }
                }
            }
            return _donation;
        }
        set { _donation = value; }
    }

    public Transform Donation_Pos { get; set; }


    #region 건설구역 / Floor
    public TechnicalFloor[] Floor_Technical { get; set; }
    void FloorInit()
    {
        Floor_Technical = Main.FindObjectsOfType<TechnicalFloor>();
        System.Array.Sort(Floor_Technical, (a, b) =>
        {
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        });

        for (int i = 0; i < Floor_Technical.Length; i++)
        {
            Floor_Technical[i].FloorIndex = i;

            GameObject go = Managers.Resource.Instantiate($"UI/PopUp/Technical/UI_Technical", Floor_Technical[i].transform);
            var ui = go.GetComponent<UI_Technical>();
            ui.Parent = Floor_Technical[i];
            ui.Init();

            Floor_Technical[i].gameObject.SetActive(false);
        }
    }

    public void Expantion_Technical()
    {
        for (int i = 0; i < Main.Instance.ActiveFloor_Technical; i++)
        {
            if (Floor_Technical[i].gameObject.activeSelf == false)
            {
                Floor_Technical[i].gameObject.SetActive(true);
            }
        }
    }
    #endregion


    #region 실제 액티브 객체
    public List<Technical> currentTechnicalList = new List<Technical>();

    void CreateAction(string _keyName)
    {
        var data = GetData(_keyName);
        if (data == null)
        {
            return;
        }

        var tech = Create(data);

        tech.transform.position = Main.Instance.CurrentTechnical.transform.position + new Vector3(0.25f, -0.75f, 0);
        tech.transform.SetParent(Main.Instance.CurrentTechnical.transform);

        Main.Instance.CurrentTechnical.Current = tech;
        tech.parent = Main.Instance.CurrentTechnical;

        Managers.UI.CloseAll();
    }

    Technical Create(SO_Technical data)
    {
        var obj = Managers.Resource.Instantiate($"Technical/{data.prefabPath}");
        var tech = obj.GetComponent<Technical>();
        tech.Data = GetData(data.keyName);

        currentTechnicalList.Add(tech);
        return tech;
    }

    public void RemoveTechnical(Technical technical)
    {
        technical.RemoveTechnical();
        currentTechnicalList.Remove(technical);

        technical.parent.Current = null;
        Managers.Resource.Destroy(technical.gameObject);
    }
    #endregion



    #region 세이브 데이터
    public Save_TechnicalData[] GetSaveData_Technical()
    {
        Save_TechnicalData[] savedata = new Save_TechnicalData[currentTechnicalList.Count];


        for (int i = 0; i < savedata.Length; i++)
        {
            var newData = new Save_TechnicalData();
            newData.SetData(currentTechnicalList[i]);
            savedata[i] = newData;
        }

        return savedata;
    }

    public void Load_TechnicalData(Save_TechnicalData[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            var tech = Create(GetData(data[i].keyName));

            tech.transform.position = Floor_Technical[data[i].LocationIndex].transform.position + new Vector3(0.25f, -0.75f, 0);
            tech.transform.SetParent(Floor_Technical[data[i].LocationIndex].transform);

            Floor_Technical[data[i].LocationIndex].Current = tech;
            tech.parent = Floor_Technical[data[i].LocationIndex];

            tech.InstanceDate = data[i].InstanceDate;
        }
    }

    #endregion
}
public class Save_TechnicalData
{
    public string keyName;

    public int LocationIndex;

    public int InstanceDate;

    public void SetData(Technical data)
    {
        keyName = data.Data.keyName;
        LocationIndex = data.parent.FloorIndex;
        InstanceDate = data.InstanceDate;
    }
}
