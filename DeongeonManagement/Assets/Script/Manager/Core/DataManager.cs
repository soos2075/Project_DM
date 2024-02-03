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
        // 세이브 슬롯 정보
        public int saveIndex;
        public string dateTime;

        // 세이브 슬롯에 표시할 게임정보
        public int turn;
        public int Final_Score;

        // 게임 진행상황
        public int FameOfDungeon;
        public int DangerOfDungeon;

        public int Player_Mana;
        public int Player_Gold;
        public int Player_AP;

        public int Prisoner;
        public Main.DayResult CurrentDay;

        // Floor 정보
        public int ActiveFloor_Basement; //? 확장된 계층정보
        public int ActiveFloor_Technical; //? 특수시설 계층


        // Monster 정보 - 완료
        public Save_MonsterData[] monsterList;

        // Facility 정보 = 완료
        public Save_FacilityData[] facilityList;

        // Technical 정보 - 완료
        public Save_TechnicalData[] tachnicalList;

    }

    //private SaveData tempData;
    #endregion






    #region SaveLoad

    //private int _fileIndex = 0;
    public void SaveToJson(string fileName, int index)
    {
        //? 저장할 정보를 몽땅 기록
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






        //? 파일로 저장
        string jsonData = JsonConvert.SerializeObject(saveData);
        var result = FileOperation(FileMode.Create, $"{Application.dataPath}/{fileName}.json", jsonData);
        Debug.Log(result);
    }

    public void LoadToStorage(string fileName)
    {
        //? 저장된 파일 읽어옴
        var _fileData = FileOperation(FileMode.Open, $"{Application.dataPath}/{fileName}.json");
        SaveData loadData = JsonConvert.DeserializeObject<SaveData>(_fileData);



        //? 불러온 데이터 적용
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




    //? dataPath = Asset폴더 /// persistentDataPath = C:\Users\USER\AppData\LocalLow\SeonghyunKim\DefenseGame_alpha
    //? 안드로이드에선 또 달라짐. 하여튼 접근가능한경로는 persistentDataPath를 써야함
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
        return $"오류발생 : {path}";
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
            Debug.Log(targetFile + " 이 존재하지 않습니다.");
    }





    #endregion

}
