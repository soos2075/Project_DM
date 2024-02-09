using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TechnicalManager
{
    public void Init()
    {
        currentTechnicalList = new List<Technical>();

        FloorInit();
        AddContents();
    }



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








    public List<Technical> currentTechnicalList;

    public List<TechnicalData> TechnicalDataList { get; set; }



    void CreateAction(string path)
    {
        var obj = Managers.Resource.Instantiate($"Technical/{path}");
        obj.transform.position = Main.Instance.CurrentTechnical.transform.position + new Vector3(0.25f, -0.75f, 0);
        obj.transform.SetParent(Main.Instance.CurrentTechnical.transform);

        var tech = obj.GetComponent<Technical>();
        tech.Data = GetData(path);

        Main.Instance.CurrentTechnical.Current = tech;
        tech.parent = Main.Instance.CurrentTechnical;

        Managers.UI.CloseAll();

        currentTechnicalList.Add(tech);
    }
    Technical CreateAction(string path, int floor)
    {
        var obj = Managers.Resource.Instantiate($"Technical/{path}");
        obj.transform.position = Floor_Technical[floor].transform.position + new Vector3(0.25f, -0.75f, 0);
        obj.transform.SetParent(Floor_Technical[floor].transform);

        var tech = obj.GetComponent<Technical>();
        tech.Data = GetData(path);
        tech.InstanceDate = Main.Instance.Turn;

        Floor_Technical[floor].Current = tech;
        tech.parent = Floor_Technical[floor];

        currentTechnicalList.Add(tech);

        return tech;
    }

    public void RemoveTechnical(Technical technical)
    {
        currentTechnicalList.Remove(technical);

        technical.parent.Current = null;



        Managers.Resource.Destroy(technical.gameObject);
    }





    #region 사전 데이터


    public TechnicalData GetData(string technicalName)
    {
        foreach (var item in TechnicalDataList)
        {
            if (item.contentName == technicalName)
            {
                return item;
            }
        }
        Debug.Log($"{technicalName} 데이터를 찾지 못함");
        return null;
    }


    void AddContents()
    {
        TechnicalDataList = new List<TechnicalData>();

        {
            TechnicalData content = new TechnicalData("HerbFarm");
            content.SetName("허브농장", "2일마다 주변의 층에 약초를 공급해줍니다. 가끔 좋은 약초가 나올때도 있어요!");
            content.SetCondition(50, 50, 1);
            content.sprite = Managers.Sprite.GetSprite("HerbFarm");
            content.action = (data) => CreateAction("HerbFarm");

            TechnicalDataList.Add(content);
        }
    }
    public void Level_2()
    {
        {
            TechnicalData content = new TechnicalData("HerbFarm");
            content.SetName("레벨2 건물", "레벨 2가 되면 추가되는 시설 예정 / 던전레벨 시스템 만들고나서 수정필요");
            content.SetCondition(50, 50, 1);
            content.sprite = Managers.Sprite.GetSprite("HerbFarm");
            content.action = (data) => CreateAction("HerbFarm");

            TechnicalDataList.Add(content);
        }
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
            var tech = CreateAction(data[i].Name_Technical, data[i].LocationIndex);
            tech.InstanceDate = data[i].InstanceDate;
        }
    }

    #endregion
}

public class TechnicalData
{
    public Action<PointerEventData> action;

    public Sprite sprite;
    public string name_Placement;
    public string name_Detail;

    public int need_Mana;
    public int need_Gold;
    public int need_LV;

    public string contentName;
    public string prefabPath;

    public TechnicalData(string _contentName)
    {
        contentName = _contentName;
    }

    public void SetName(string title, string box)
    {
        name_Placement = title;
        name_Detail = box;
    }
    public void SetCondition(int mana, int gold, int lv)
    {
        need_Mana = mana;
        need_Gold = gold;
        need_LV = lv;
    }
}

public class Save_TechnicalData
{
    public string Name_Technical;

    public int LocationIndex;

    public int InstanceDate;

    public void SetData(Technical data)
    {
        Name_Technical = data.Data.contentName;
        LocationIndex = data.parent.FloorIndex;
        InstanceDate = data.InstanceDate;
    }
}
