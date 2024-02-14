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


    bool ConfirmCheck(int mana, int gold, int lv, int ap)
    {
        if (Main.Instance.Player_Mana < mana)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "������ �����մϴ�";
            return false;
        }
        if (Main.Instance.Player_Gold < gold)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "��尡 �����մϴ�";
            return false;
        }
        if (Main.Instance.DungeonRank < lv)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "���� ����� �����մϴ�";
            return false;
        }
        if (Main.Instance.Player_AP < ap)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "�ൿ���� �����մϴ�";
            return false;
        }


        Main.Instance.CurrentDay.SubtractMana(mana);
        Main.Instance.CurrentDay.SubtractGold(gold);
        Main.Instance.Player_AP -= ap;
        return true;
    }

    void CreateAction(TechnicalData data)
    {
        if (ConfirmCheck(data.need_Mana, data.need_Gold, data.need_LV, data.need_AP) == false)
        {
            return;
        }

        var obj = Managers.Resource.Instantiate($"Technical/{data.prefabPath}");
        obj.transform.position = Main.Instance.CurrentTechnical.transform.position + new Vector3(0.25f, -0.75f, 0);
        obj.transform.SetParent(Main.Instance.CurrentTechnical.transform);

        var tech = obj.GetComponent<Technical>();
        tech.Data = GetData(data.prefabPath);

        Main.Instance.CurrentTechnical.Current = tech;
        tech.parent = Main.Instance.CurrentTechnical;

        Managers.UI.CloseAll();

        currentTechnicalList.Add(tech);

        if (data.contentName == "DonationBox")
        {
            Managers.Resource.Instantiate($"Technical/DonationBox_Entrance");
        }
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
        technical.RemoveTechnical();
        currentTechnicalList.Remove(technical);

        technical.parent.Current = null;
        Managers.Resource.Destroy(technical.gameObject);
    }





    #region ���� ������


    public TechnicalData GetData(string technicalName)
    {
        foreach (var item in TechnicalDataList)
        {
            if (item.contentName == technicalName)
            {
                return item;
            }
        }
        Debug.Log($"{technicalName} �����͸� ã�� ����");
        return null;
    }


    void AddContents()
    {
        TechnicalDataList = new List<TechnicalData>();

        {
            TechnicalData content = new TechnicalData("HerbFarm");
            content.SetName("������", "2�ϸ��� Ȱ��ȭ �� ��� ���� ���ʸ� �������ݴϴ�. ���� ���� ���ʰ� ���ö��� �־��!");
            content.SetCondition(200, 100, 1, 1);
            content.prefabPath = "HerbFarm";
            content.sprite = Managers.Sprite.GetSprite("Object/HerbFarm");
            content.action = (data) => CreateAction(content);

            TechnicalDataList.Add(content);
        }
    }
    public void Level_2()
    {
        {
            TechnicalData content = new TechnicalData("DonationBox");
            content.SetName("�����", "���� �Ա��� ������� ��ġ�մϴ�. ��� ���� ��������� ����Ḧ �޴°ſ���. " +
                "���� �α��ִ� �������� ����Ḧ �޽��ϴٸ�, ���� �츮 ������ ����Ḧ ������ ���� ���� ��ġ�� �������?");
            content.SetCondition(300, 0, 2, 2);
            content.prefabPath = "DonationBox";
            content.sprite = Managers.Sprite.GetSprite("Object/DonationBox");
            content.action = (data) => CreateAction(content);

            TechnicalDataList.Add(content);
        }

        {
            TechnicalData content = new TechnicalData("Prison");
            content.SetName("����", "���谡�� ���η� ���� �� �ִ� ������ ��ġ�մϴ�. ����Ʈ�� �Ϻ� ���谡�鿡�Լ� �������� ��带 �� �� �����ſ���.");
            content.SetCondition(200, 0, 2, 2);
            content.prefabPath = "Prison";
            content.sprite = Managers.Sprite.GetSprite("Object/Prison");
            content.action = (data) => CreateAction(content);

            TechnicalDataList.Add(content);
        }
    }
    #endregion





    #region ���̺� ������
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
    public int need_AP;

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
    public void SetCondition(int mana, int gold, int lv, int ap = 0)
    {
        need_Mana = mana;
        need_Gold = gold;
        need_LV = lv;
        need_AP = ap;
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
