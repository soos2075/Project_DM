using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalManager
{
    public void Init()
    {
        Init_LocalData();
    }

    #region SO_Data
    SO_Journal[] so_data;

    public void Init_LocalData()
    {
        so_data = Resources.LoadAll<SO_Journal>("Data/Journal");
        foreach (var item in so_data)
        {
            string[] datas = Managers.Data.GetTextData_Journal(item.ID);

            if (datas == null)
            {
                Debug.Log($"{item.ID} : CSV Data Not Exist");
                continue;
            }

            item.title = datas[0];
            item.description = datas[1];
        }
    }

    public SO_Journal GetData(int _id)
    {
        foreach (var item in so_data)
        {
            if (item.ID == _id)
            {
                return item;
            }
        }
        Debug.Log($"{_id}: Data Not Exist");
        return null;
    }
    #endregion


    #region Save Load

    public List<JournalData> Save_JournalData()
    {
        List<JournalData> saveData = new List<JournalData>();
        foreach (var item in CurrentJournalList)
        {
            saveData.Add(item.DeepCopy());
        }
        return saveData;
    }
    public void Load_JournalData(List<JournalData> data)
    {
        if (data != null)
        {
            var loadData = new List<JournalData>();
            foreach (var item in data)
            {
                loadData.Add(item.DeepCopy());
            }
            CurrentJournalList = loadData;
        }
    }

    #endregion






    #region ��ϵ� ���� (������ ����Ǵ� ����Ʈ�ʹ� ����. ������ ���� ����Ʈ�� ���)

    public class JournalData
    {
        public int ID;

        public int startDay;
        public int endDay;

        public JournalData(int _id)
        {
            ID = _id;
        }


        //? �Ʒ� ������ ��� �ʿ�������� �𸣰���. (�Ʒ����� �����ع����� ��Ÿ�ӿ��� ���ٲ��� �� ������ �ȹٲ�)
        public string title;
        public string detail;

        //? �̰� ������ �̰� �켱���� ����ϰ�, ������ �׳� startDay�� endDay�� ����ؼ� ����ϱ�
        public string dayInfo;


        public JournalData DeepCopy()
        {
            return (JournalData)this.MemberwiseClone();
        }
    }

    public List<JournalData> CurrentJournalList { get; set; } = new List<JournalData>();

    //? ���� ���� ����Ʈ�� ����
    public void AddJournal(int _id)
    {
        CurrentJournalList.Add(new JournalData(_id));
    }
    public void AddJournal(int _id, int _startDay, int _endDay, string _dayInfo = "")
    {
        //var data = GetData(_id);
        var jour = new JournalData(_id);

        jour.startDay = _startDay;
        jour.endDay = _endDay;

        CurrentJournalList.Add(jour);
    }

    public void RemoveJournal(int _id)
    {
        foreach (var item in CurrentJournalList)
        {
            if (item.ID == _id)
            {
                CurrentJournalList.Remove(item);
            }
        }
    }


    #endregion


}
