using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager
{
    public void Init()
    {
        Init_LocalData();
        Init_TitleArray();

        ////? �׽�Ʈ
        //Titles[0].Active();
    }

    #region SO_Data
    SO_Title[] so_data;

    Dictionary<string, SO_Title> Title_Dictionary { get; set; }

    public void Init_LocalData()
    {
        Title_Dictionary = new Dictionary<string, SO_Title>();
        so_data = Resources.LoadAll<SO_Title>("Data/Title");

        foreach (var item in so_data)
        {
            //(string, string) datas = ("", "");
            string[] datas = Managers.Data.GetTextData_Title(item.id);

            if (datas == null)
            {
                Debug.Log($"{item.id} : CSV Data Not Exist");
                continue;
            }

            item.Title = datas[0];
            item.Detail = datas[1];
            item.Effect = datas[2];
            item.Acquire = datas[3];

            Title_Dictionary.Add(item.keyName, item);
        }
    }


    public SO_Title GetData(string _keyName)
    {
        SO_Title title = null;
        if (Title_Dictionary.TryGetValue(_keyName, out title))
        {
            return title;
        }

        Debug.Log($"{_keyName}: Data Not Exist");
        return null;
    }



    #endregion






    #region Save Load

    public List<DungeonTitle> Save_TitlesData()
    {
        List<DungeonTitle> saveData = new List<DungeonTitle>();
        foreach (var item in Titles)
        {
            saveData.Add(item.DeepCopy());
        }

        return saveData;
    }
    public void Load_TitlesData(List<DungeonTitle> data)
    {
        if (data != null)
        {
            Titles = new DungeonTitle[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                Titles[i] = data[i].DeepCopy();
                Titles[i].SetData(so_data[i]);
            }
        }
    }
    #endregion





    #region ���� �ν��Ͻ� GameData

    public class DungeonTitle
    {
        [Newtonsoft.Json.JsonIgnore]
        public SO_Title Data;

        //? ID�� ��ü
        public string keyName;

        //? enum
        public TitleGroup group;

        //? Ȱ��ȭ ����
        public bool isActive;


        public DungeonTitle()
        {

        }
        public DungeonTitle(SO_Title _data)
        {
            Data = _data;
            group = (TitleGroup)_data.id;
            keyName = _data.keyName;
        }

        public void Active()
        {
            isActive = true;
            AddCollectionPoint();
        }
        public void Inactive()
        {
            isActive = false;
        }

        public void AddCollectionPoint()
        {
            var collection = CollectionManager.Instance.Get_Collection(Data);
            if (collection != null)
            {
                collection.AddPoint();
            }
        }


        public DungeonTitle DeepCopy()
        {
            var newTitle = new DungeonTitle();
            newTitle.keyName = keyName;
            newTitle.group = group;
            newTitle.isActive = isActive;

            return newTitle;
        }
        public void SetData(SO_Title _data)
        {
            Data = _data;
        }
    }

    DungeonTitle[] Titles { get; set; }

    void Init_TitleArray()
    {
        //? SO_Artifact ������ŭ�� size�� ��̸� ����
        int size = so_data.Length;
        Titles = new DungeonTitle[size];

        for (int i = 0; i < size; i++)
        {
            Titles[i] = new DungeonTitle(so_data[i]);
        }
    }

    DungeonTitle Get_InstanceTitle(string _keyName)
    {
        foreach (var item in Titles)
        {
            if (item.keyName == _keyName)
            {
                return item;
            }
        }

        return null;
    }
    DungeonTitle Get_InstanceTitle(int _id)
    {
        foreach (var item in Titles)
        {
            if (item.Data.id == _id)
            {
                return item;
            }
        }

        return null;
    }
    public DungeonTitle Get_InstanceTitle(TitleGroup _id)
    {
        foreach (var item in Titles)
        {
            if (item.group == _id)
            {
                return item;
            }
        }

        return null;
    }

    #endregion ���� �ν��Ͻ� GameData



    #region ����ȣ�� (���� Buff ����)

    //void Update_CurrentTitleBuff()
    //{
    //    var valueList = Enum.GetValues(typeof(TitleGroup));
    //    foreach (var item in valueList)
    //    {
    //        TitleGroup title = (TitleGroup)item;

    //        if (Get_InstanceTitle(title).isActive)
    //        {
    //            switch (title)
    //            {
    //                case TitleGroup.NoviceDungeon:
    //                    break;
    //                case TitleGroup.Herb_1:
    //                    GameManager.Buff.Title_Herb_1 = 30;
    //                    break;
    //                case TitleGroup.Herb_2:
    //                    GameManager.Buff.Title_Herb_2 = 10;
    //                    break;
    //                case TitleGroup.Herb_3:
    //                    GameManager.Buff.Title_Herb_3 = 20;
    //                    break;
    //                case TitleGroup.Mineral_1:
    //                    break;
    //                case TitleGroup.Mineral_2:
    //                    break;
    //                case TitleGroup.Mineral_3:
    //                    break;
    //                case TitleGroup.Treasure_1:
    //                    break;
    //                case TitleGroup.Treasure_2:
    //                    break;
    //                case TitleGroup.Treasure_3:
    //                    break;
    //                case TitleGroup.Trap_1:
    //                    break;
    //                case TitleGroup.Trap_2:
    //                    break;
    //                case TitleGroup.Trap_3:
    //                    break;
    //                case TitleGroup.ManyMonster_1:
    //                    break;
    //                case TitleGroup.StrongMonster_1:
    //                    break;
    //                case TitleGroup.StrongMonster_2:
    //                    break;
    //                case TitleGroup.StrongMonster_3:
    //                    break;
    //                case TitleGroup.Amenity_1:
    //                    break;
    //                case TitleGroup.Amenity_2:
    //                    break;
    //                case TitleGroup.Marauder_1:
    //                    break;
    //                case TitleGroup.Marauder_2:
    //                    break;
    //                case TitleGroup.Secret_1:
    //                    break;
    //            }
    //        }
    //    }
    //}


    #endregion




    #region �ܺ�ȣ��

    //? �������� Ÿ��Ʋ �����Լ�
    public void CreateTitleBar(string _keyName, Transform parents, bool tooltip = true)
    {
        var content = Managers.Resource.Instantiate("UI/PopUp/Title/Title_Content", parents);
        content.GetComponent<UI_Title_Content>().Set_TitleData(GetData(_keyName), tooltip);
    }


    public List<DungeonTitle> GetCurrentTitle()
    {
        List<DungeonTitle> titleL = new List<DungeonTitle>();

        foreach (var item in Titles)
        {
            if (item.isActive)
            {
                titleL.Add(item);
            }
        }

        return titleL;
    }

    


    public void Active_Title(string _keyName)
    {
        var title = Get_InstanceTitle(_keyName);
        if (title != null)
        {
            title.Active();
        }
    }
    public void Active_Title(int _id)
    {
        var title = Get_InstanceTitle(_id);
        if (title != null)
        {
            title.Active();
        }
    }
    public void Active_Title(TitleGroup _id)
    {
        Active_Title((int)_id);
    }


    //public void TurnStartEvent_Title()
    //{

    //}
    public void TurnOverEvent_Title() //? Īȣ�� ��� Ÿ�̹�...�� �Ͻ��� ������ ���������� ���� ����̱� �ѵ� �ϴ��� �׽�Ʈ������
    {
        Update_Title(); //? ���� ȣ���� �ϱ� �ص� �� ����ÿ� ���� ȣ��Ǳ� �ؾ��ҵ�

        var stat = Main.Instance.CurrentStatistics;

        //? ��й� üũ
        if (!Get_InstanceTitle(TitleGroup.Secret_1).isActive)
        {
            if (stat.Interaction_Secret >= 5) Get_InstanceTitle(TitleGroup.Secret_1).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Secret_2).isActive)
        {
            if (stat.Interaction_Secret >= 25) Get_InstanceTitle(TitleGroup.Secret_2).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Secret_3).isActive)
        {
            if (stat.Interaction_Secret >= 100) Get_InstanceTitle(TitleGroup.Secret_3).Active();
        }

        //? ���ʷ� üũ
        if (!Get_InstanceTitle(TitleGroup.Herb_1).isActive)
        {
            if (stat.Interaction_Herb >= 100) Get_InstanceTitle(TitleGroup.Herb_1).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Herb_2).isActive)
        {
            if (stat.Interaction_Herb >= 500) Get_InstanceTitle(TitleGroup.Herb_2).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Herb_3).isActive)
        {
            if (stat.Interaction_Herb >= 2000) Get_InstanceTitle(TitleGroup.Herb_3).Active();
        }

        //? ������ üũ
        if (!Get_InstanceTitle(TitleGroup.Mineral_1).isActive)
        {
            if (stat.Interaction_Mineral >= 100) Get_InstanceTitle(TitleGroup.Mineral_1).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Mineral_2).isActive)
        {
            if (stat.Interaction_Mineral >= 500) Get_InstanceTitle(TitleGroup.Mineral_2).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Mineral_3).isActive)
        {
            if (stat.Interaction_Mineral >= 2000) Get_InstanceTitle(TitleGroup.Mineral_3).Active();
        }


        //? ���� üũ
        if (!Get_InstanceTitle(TitleGroup.Treasure_1).isActive)
        {
            if (stat.Interaction_Treasure >= 10) Get_InstanceTitle(TitleGroup.Treasure_1).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Treasure_2).isActive)
        {
            if (stat.Interaction_Treasure >= 50) Get_InstanceTitle(TitleGroup.Treasure_2).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Treasure_3).isActive)
        {
            if (stat.Interaction_Treasure >= 200) Get_InstanceTitle(TitleGroup.Treasure_3).Active();
        }


        //? ���� üũ
        if (!Get_InstanceTitle(TitleGroup.Trap_1).isActive)
        {
            if (stat.Interaction_Trap >= 10) Get_InstanceTitle(TitleGroup.Trap_1).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Trap_2).isActive)
        {
            if (stat.Interaction_Trap >= 50) Get_InstanceTitle(TitleGroup.Trap_2).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Trap_3).isActive)
        {
            if (stat.Interaction_Trap >= 200) Get_InstanceTitle(TitleGroup.Trap_3).Active();
        }


        //? ���� üũ
        if (!Get_InstanceTitle(TitleGroup.ManyMonster_1).isActive)
        {
            if (stat.Hightest_Unit_Size >= 10) Get_InstanceTitle(TitleGroup.ManyMonster_1).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.StrongMonster_1).isActive)
        {
            if (stat.Highest_Unit_Lv >= 25) Get_InstanceTitle(TitleGroup.StrongMonster_1).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.StrongMonster_2).isActive)
        {
            if (stat.Highest_Unit_Lv >= 50) Get_InstanceTitle(TitleGroup.StrongMonster_2).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.StrongMonster_3).isActive)
        {
            if (stat.Highest_Unit_Lv >= 80) Get_InstanceTitle(TitleGroup.StrongMonster_3).Active();
        }


        //? ���ǽü� üũ
        if (!Get_InstanceTitle(TitleGroup.Amenity_1).isActive)
        {
            if (GameManager.Facility.GetFacilityCount<Wells>() >= 1) Get_InstanceTitle(TitleGroup.Amenity_1).Active();
        }
        if (!Get_InstanceTitle(TitleGroup.Amenity_2).isActive)
        {
            if (GameManager.Facility.GetFacilityCount<Wells>() >= 5) Get_InstanceTitle(TitleGroup.Amenity_2).Active();
        }
    }

    public void Update_Title() //? Ư�� ��Ȳ���� �ٷ� ���� �� ��...�ϱ� ��忡�� ���ƿ��� ���� ����� �̷� �� ȣ���ϸ� �ɵ�.
    {
        if (!Get_InstanceTitle(TitleGroup.Marauder_1).isActive)
        {
            if (GameManager.Monster.GetMonster<Heroine>() != null || GameManager.Monster.GetMonster<Utori>() != null)
            {
                Get_InstanceTitle(TitleGroup.Marauder_1).Active();
            }
        }
        if (!Get_InstanceTitle(TitleGroup.Marauder_2).isActive)
        {
            if (GameManager.Monster.GetMonster<Heroine>() != null && GameManager.Monster.GetMonster<Utori>() != null)
            {
                Get_InstanceTitle(TitleGroup.Marauder_2).Active();
            }
        }
    }



    #endregion �ܺ�ȣ��
}

public enum TitleGroup
{
    NoviceDungeon = 0,

    Herb_1 = 4,
    Herb_2,
    Herb_3,

    Mineral_1 = 7,
    Mineral_2,
    Mineral_3,

    Treasure_1 = 10,
    Treasure_2,
    Treasure_3,

    Trap_1 = 20,
    Trap_2,
    Trap_3,

    ManyMonster_1 = 30,

    StrongMonster_1 = 40,
    StrongMonster_2 = 41,
    StrongMonster_3 = 42,

    Amenity_1 = 50,
    Amenity_2 = 51,

    Marauder_1 = 60,
    Marauder_2 = 61,

    Secret_1 = 70,
    Secret_2,
    Secret_3,
}
