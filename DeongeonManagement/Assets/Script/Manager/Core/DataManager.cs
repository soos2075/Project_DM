using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System;

public class DataManager
{

    public void Init()
    {
        SaveFileList = new Dictionary<string, SaveData>();
        Scan_File();
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
        public int DungeonLV;
        public int FameOfDungeon;
        public int DangerOfDungeon;

        public int Player_Mana;
        public int Player_Gold;
        public int Player_AP;

        public int Prisoner;
        public Main.DayResult CurrentDay;
        public List<Main.DayResult> DayResultList;

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

    Dictionary<string, SaveData> SaveFileList;


    void Scan_File()
    {
        for (int i = 1; i <= 6; i++)
        {
            if (Managers.Data.SaveFileSearch($"DM_Save_{i}"))
            {
                var data = LoadToStorage($"DM_Save_{i}");

                SaveFileList.Add($"DM_Save_{i}", data);
            }
        }

        if (Managers.Data.SaveFileSearch($"AutoSave"))
        {
            var data = LoadToStorage($"AutoSave");
            SaveFileList.Add($"AutoSave", data);
        }
    }

    void Add_File(SaveData newData, string fileKey)
    {
        SaveData old = null;
        if (SaveFileList.TryGetValue(fileKey, out old))
        {
            SaveFileList.Remove(fileKey);
        }

        SaveFileList.Add(fileKey, newData);
        SaveToStorage(newData, fileKey);
    }




    public void LoadGame(string fileKey)
    {
        SaveData data = null;
        if (SaveFileList.TryGetValue(fileKey, out data))
        {
            LoadFileApply(data);
            Debug.Log($"Load Success : {fileKey}");
        }
        else
        {
            Debug.Log($"Load Fail : {fileKey}");
        }
    }
    
    public SaveData GetData(string fileKey)
    {
        SaveData data = null;
        if (SaveFileList.TryGetValue(fileKey, out data))
        {
            return data;
        }
        return null;
    }




    public void SaveToJson(string fileName, int index)
    {
        if (Main.Instance.Management == false)
        {
            Debug.Log("�������� ����Ұ�");
            return;
        }

        //? ������ ������ ���� ���
        SaveData saveData = new SaveData();

        saveData.saveIndex = index;
        saveData.dateTime = System.DateTime.Now.ToString("F");

        saveData.turn = Main.Instance.Turn;
        saveData.Final_Score = Main.Instance.Final_Score;

        saveData.DungeonLV = Main.Instance.Dungeon_Lv;

        saveData.FameOfDungeon = Main.Instance.FameOfDungeon;
        saveData.DangerOfDungeon = Main.Instance.DangerOfDungeon;
        saveData.Player_Mana = Main.Instance.Player_Mana;
        saveData.Player_Gold = Main.Instance.Player_Gold;
        saveData.Player_AP = Main.Instance.Player_AP;
        saveData.Prisoner = Main.Instance.Prisoner;
        saveData.CurrentDay = Main.Instance.CurrentDay;

        saveData.DayResultList = Main.Instance._dayList;

        saveData.ActiveFloor_Basement = Main.Instance.ActiveFloor_Basement;
        saveData.ActiveFloor_Technical = Main.Instance.ActiveFloor_Technical;


        saveData.monsterList = GameManager.Monster.GetSaveData_Monster();
        saveData.tachnicalList = GameManager.Technical.GetSaveData_Technical();
        saveData.facilityList = GameManager.Facility.GetSaveData_Facility();


        Add_File(saveData, $"{fileName}");
    }


    void SaveToStorage(SaveData data, string fileName)
    {
        //? ���Ϸ� ����
        string jsonData = JsonConvert.SerializeObject(data);
        var result = FileOperation(FileMode.Create, $"{Application.dataPath}/{fileName}.json", jsonData);
        Debug.Log($"Save Sucess : {fileName}");
    }


    SaveData LoadToStorage(string fileName)
    {
        //? ����� ���� �о��
        var _fileData = FileOperation(FileMode.Open, $"{Application.dataPath}/{fileName}.json");
        SaveData loadData = JsonConvert.DeserializeObject<SaveData>(_fileData);

        return loadData;
    }

    void LoadFileApply(SaveData loadData)
    {
        Main.Instance.SetLoadData(loadData);

        GameManager.Monster.Load_MonsterData(loadData.monsterList);
        GameManager.Technical.Load_TechnicalData(loadData.tachnicalList);
        GameManager.Facility.Load_FacilityData(loadData.facilityList);
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



    bool SaveFileSearch(string searchFileName)
    {
        string searchName;
        FileInfo fileInfo;

        searchName = $"{Application.dataPath}/{searchFileName}.json";
        fileInfo = new FileInfo(searchName);
        return fileInfo.Exists;
    }


    void DeleteSaveFile(string targetFile)
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
