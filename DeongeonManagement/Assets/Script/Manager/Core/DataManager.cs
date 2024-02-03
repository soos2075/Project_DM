using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;

public class DataManager
{

    public void Init()
    {

    }




    void SaveLoadInit()
    {

    }

    void MonsterDataInit()
    {

    }
    void NPCDataInit()
    {

    }



    #region SaveClass
    public class SaveData
    {
        // ���̺� ���� ����
        public int saveIndex;
        public string dateTime;

        // ���̺� ���Կ� ǥ���� ��������
        public int turn;
        public int Final_Score;

        // ���� �����Ȳ
        public int FameOfDungeon;
        public int DangerOfDungeon;

        public int Player_Mana;
        public int Player_Gold;
        public int Player_AP;

        public int Prisoner;
        public Main.DayResult CurrentDay;

        // Floor ����
        public int ActiveFloor_Basement; //? Ȯ��� ��������
        public int ActiveFloor_Technical; //? Ư���ü� ����


        // Monster ���� - �Ϸ�
        public Save_MonsterData[] monsterList;

        // Facility ���� = �Ϸ�
        public Save_FacilityData[] facilityList;

        // Technical ���� - �Ϸ�
        public Save_TechnicalData[] tachnicalList;

    }

    //private SaveData tempData;
    #endregion






    #region SaveLoad

    //private int _fileIndex = 0;
    public void SaveToJson(string fileName, int index)
    {
        //? ������ ������ ���� ���
        SaveData saveData = new SaveData();

        saveData.saveIndex = index;
        saveData.dateTime = System.DateTime.Now.ToString("F");

        saveData.turn = Main.Instance.Turn;
        saveData.Final_Score = Main.Instance.Final_Score;

        saveData.FameOfDungeon = Main.Instance.FameOfDungeon;
        saveData.DangerOfDungeon = Main.Instance.DangerOfDungeon;
        saveData.Player_Mana = Main.Instance.Player_Mana;
        saveData.Player_Gold = Main.Instance.Player_Gold;
        saveData.Player_AP = Main.Instance.Player_AP;
        saveData.Prisoner = Main.Instance.Prisoner;
        saveData.CurrentDay = Main.Instance.CurrentDay;

        saveData.ActiveFloor_Basement = Main.Instance.ActiveFloor_Basement;
        saveData.ActiveFloor_Technical = Main.Instance.ActiveFloor_Technical;


        saveData.monsterList = GameManager.Monster.GetSaveData_Monster();
        saveData.tachnicalList = GameManager.Technical.GetSaveData_Technical();
        saveData.facilityList = GameManager.Facility.GetSaveData_Facility();






        //? ���Ϸ� ����
        string jsonData = JsonConvert.SerializeObject(saveData);
        var result = FileOperation(FileMode.Create, $"{Application.dataPath}/{fileName}.json", jsonData);
        Debug.Log(result);
    }

    public void LoadToStorage(string fileName)
    {
        //? ����� ���� �о��
        var _fileData = FileOperation(FileMode.Open, $"{Application.dataPath}/{fileName}.json");
        SaveData loadData = JsonConvert.DeserializeObject<SaveData>(_fileData);



        //? �ҷ��� ������ ����
        Main.Instance.SetLoadData(loadData);

        GameManager.Monster.Load_MonsterData(loadData.monsterList);
        GameManager.Technical.Load_TechnicalData(loadData.tachnicalList);
        GameManager.Facility.Load_FacilityData(loadData.facilityList);
    }

    public string GetDateToFile(string fileName)
    {
        var _fileData = FileOperation(FileMode.Open, $"{Application.dataPath}/{fileName}.json");
        SaveData loadData = JsonConvert.DeserializeObject<SaveData>(_fileData);

        return loadData.dateTime;
    }




    //? dataPath = Asset���� /// persistentDataPath = C:\Users\USER\AppData\LocalLow\SeonghyunKim\DefenseGame_alpha
    //? �ȵ���̵忡�� �� �޶���. �Ͽ�ư ���ٰ����Ѱ�δ� persistentDataPath�� �����
    string FileOperation(FileMode fileMode, string path, string jsonData = null)
    {
        FileStream fileStream;
        byte[] data;

        switch (fileMode)
        {
            case FileMode.Create:
                fileStream = new FileStream(path, FileMode.Create);
                data = Encoding.UTF8.GetBytes(jsonData);
                fileStream.Write(data, 0, data.Length);
                fileStream.Close();

                return $"FileSave : {path}";

            case FileMode.Open:
                fileStream = new FileStream(path, FileMode.Open);
                data = new byte[fileStream.Length];
                fileStream.Read(data, 0, data.Length);
                fileStream.Close();

                return Encoding.UTF8.GetString(data);

        }
        return $"�����߻� : {path}";
    }



    public bool SaveFileSearch(string searchFileName)
    {
        string searchName;
        FileInfo fileInfo;

        searchName = $"{Application.dataPath}/{searchFileName}.json";
        fileInfo = new FileInfo(searchName);
        return fileInfo.Exists;
    }


    public void DeleteSaveFile(string targetFile)
    {
        if (SaveFileSearch(targetFile))
        {
            File.Delete($"{Application.dataPath}/{targetFile}.json");
            Debug.Log(targetFile + " Delete Complete");
        }
        else
            Debug.Log(targetFile + " �� �������� �ʽ��ϴ�.");
    }





    #endregion

}
