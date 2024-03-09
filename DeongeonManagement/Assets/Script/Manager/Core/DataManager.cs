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
        Scan_File();
        Init_Object_CSV();
    }


    #region CSV Data Parsing
    public Dictionary<int, string[]> ObjectsLabel_KR = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> ObjectsLabel_EN = new Dictionary<int, string[]>();

    void Init_Object_CSV()
    {
        CSV_File_Parsing_Object("Object/Object_KR", ObjectsLabel_KR);
        CSV_File_Parsing_Object("Object/Object_EN", ObjectsLabel_EN);

        CSV_File_Parsing_Dialogue("Dialogue/Dialogue_KR", Dialogue_KR);
        CSV_File_Parsing_Dialogue("Dialogue/Dialogue_EN", Dialogue_EN);
        CSV_File_Parsing_Dialogue("Dialogue/Dialogue_JP", Dialogue_JP);
    }




    // 0 x / 1 : id / 2 : Label / 3 : Detail / 4 : Option1 / 5 : Option2 / 6: Option3
    void CSV_File_Parsing_Object(string _filePath, Dictionary<int, string[]> _dict) 
    {
        var obj_kr = FileOperation(FileMode.Open, $"{Application.dataPath}/Data/{_filePath}.csv");
        var spl_n = obj_kr.Split('\n');
        
        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1]))
            {
                continue;
            }

            string[] datas = new string[] { spl_comma[2], spl_comma[3], spl_comma[4], spl_comma[5], spl_comma[6] };


            for (int k = 0; k < datas.Length; k++)
            {
                if (datas[k].Contains('\\'))
                {
                    var split = datas[k].Split('\\');
                    datas[k] = string.Join("\n", split);
                }
            }

            _dict.Add(int.Parse(spl_comma[1]), datas);
        }
    }


    public Dictionary<DialogueName, DialogueData> Dialogue_KR = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_EN = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_JP = new Dictionary<DialogueName, DialogueData>();

    void CSV_File_Parsing_Dialogue(string _filePath, Dictionary<DialogueName, DialogueData> _dict)
    {
        var obj_kr = FileOperation(FileMode.Open, $"{Application.dataPath}/Data/{_filePath}.csv");
        var spl_n = obj_kr.Split('\n');

        for (int i = 6; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1]))
            {
                continue;
            }


            int id = int.Parse(spl_comma[1]);
            //DialogueName keyName = (DialogueName)Enum.Parse(typeof(DialogueName), spl_comma[2]);

            var dialogue = new DialogueData();
            dialogue.id = id;
            dialogue.Type = (DialogueData.DialogueType)Enum.Parse(typeof(DialogueData.DialogueType), spl_comma[3]);
            dialogue.dialogueName = spl_comma[4];

            while (string.IsNullOrEmpty(spl_comma[6]) == false)
            {
                int index = int.Parse(spl_comma[6]);
                string optionString = spl_comma[7];
                string mainText = spl_comma[8];

                if (mainText.Contains('\\'))
                {
                    var split = mainText.Split('\\');
                    mainText = string.Join("\n", split);
                }
                if (mainText.Contains('-'))
                {
                    var split = mainText.Split('-');
                    mainText = string.Join(',', split);
                }

                var textData = new DialogueData.TextData(optionString, mainText);
                dialogue.TextDataList.Add(textData);
                //Debug.Log(mainText);

                i++;
                spl_comma = spl_n[i].Split(',');
                if (spl_comma.Length < 2)
                {
                    break;
                }
            }

            _dict.Add((DialogueName)id, dialogue);
        }
    }



    #endregion






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
        public int DungeonLV;
        public int FameOfDungeon;
        public int DangerOfDungeon;

        public int Player_Mana;
        public int Player_Gold;
        public int Player_AP;
        public int AP_MAX;

        public int Prisoner;

        public Save_DayResult CurrentDay;
        public Save_DayResult[] DayResultList;

        //public Main.DayResult CurrentDay;
        //public List<Main.DayResult> DayResultList;

        // Floor 정보
        public int ActiveFloor_Basement; //? 확장된 계층정보
        public int ActiveFloor_Technical; //? 특수시설 계층


        // Monster 정보 - 완료
        public Save_MonsterData[] monsterList;

        // Facility 정보 = 완료
        public Save_FacilityData[] facilityList;

        // Technical 정보 - 완료
        public Save_TechnicalData[] tachnicalList;


        //? 값타입으로 싹 바꿔야된다 이거
        // Guild 정보
        public List<GuildNPC_Data> guildNPCList;

        // 조건채운 퀘스트 목록(길드에 추가되기전)
        public List<int> guildQuestList;

        // 현재 진행중인 퀘스트 목록
        public List<int> currentQuestList;
    }

    //private SaveData tempData;
    #endregion


    #region SaveLoad

    Dictionary<string, SaveData> SaveFileList = new Dictionary<string, SaveData>();


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
            LoadGuildData(data);
            Debug.Log($"Load Success : {fileKey}");
        }
        else
        {
            Debug.Log($"Load Fail : {fileKey}");
        }
    }
    //? 길드에서 돌아올 땐 길드관련 데이터를 받으면 안되므로 이걸 호출
    public void LoadGame_ToGuild(string fileKey)
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
            Debug.Log("낮동안은 저장불가");
            return;
        }

        //? 저장할 정보를 몽땅 기록
        SaveData saveData = new SaveData();


        saveData.saveIndex = index;
        saveData.dateTime = System.DateTime.Now.ToString("F");

        saveData.turn = Main.Instance.Turn;
        saveData.Final_Score = Main.Instance.Final_Score;

        saveData.DungeonLV = Main.Instance.DungeonRank;

        saveData.Player_Mana = Main.Instance.Player_Mana;
        saveData.Player_Gold = Main.Instance.Player_Gold;

        saveData.AP_MAX = Main.Instance.AP_MAX;

        //saveData.FameOfDungeon = Main.Instance.PopularityOfDungeon;
        //saveData.DangerOfDungeon = Main.Instance.DangerOfDungeon;
        //saveData.Player_AP = Main.Instance.Player_AP;
        int _fame;
        int _danger;
        int _ap;
        Main.Instance.GetPropertyValue(out _fame, out _danger, out _ap);
        saveData.FameOfDungeon = _fame;
        saveData.DangerOfDungeon = _danger;
        saveData.Player_AP = _ap;

        saveData.Prisoner = Main.Instance.Prisoner;

        saveData.CurrentDay = new Save_DayResult(Main.Instance.CurrentDay);
        saveData.DayResultList = new Save_DayResult[Main.Instance.DayList.Count];
        for (int i = 0; i < Main.Instance.DayList.Count; i++)
        {
            saveData.DayResultList[i] = new Save_DayResult(Main.Instance.DayList[i]);
        }

        saveData.ActiveFloor_Basement = Main.Instance.ActiveFloor_Basement;
        saveData.ActiveFloor_Technical = Main.Instance.ActiveFloor_Technical;

        saveData.monsterList = GameManager.Monster.GetSaveData_Monster();
        saveData.tachnicalList = GameManager.Technical.GetSaveData_Technical();
        saveData.facilityList = GameManager.Facility.GetSaveData_Facility();


        if (EventManager.Instance.GuildQuestAdd != null)
        {
            saveData.guildQuestList = new List<int>(EventManager.Instance.GuildQuestAdd);
        }
        if (EventManager.Instance.CurrentQuestEvent_ForSave != null)
        {
            saveData.currentQuestList = new List<int>(EventManager.Instance.CurrentQuestEvent_ForSave);
        }
        if (EventManager.Instance.CurrentGuildData != null)
        {
            saveData.guildNPCList = new List<GuildNPC_Data>();
            foreach (var item in EventManager.Instance.CurrentGuildData)
            {
                var newData = new GuildNPC_Data();
                newData.SetData(item.Original_Index, new List<int>(item.InstanceQuestList), new List<int>(item.OptionList));
                saveData.guildNPCList.Add(newData);
            }
        }




        Add_File(saveData, $"{fileName}");
    }




    void SaveToStorage(SaveData data, string fileName)
    {
        //? 파일로 저장
        string jsonData = JsonConvert.SerializeObject(data);
        var result = FileOperation(FileMode.Create, $"{Application.dataPath}/Data/Savefile/{fileName}.json", jsonData);
        Debug.Log($"Save Sucess : {fileName}");
    }


    SaveData LoadToStorage(string fileName)
    {
        //? 저장된 파일 읽어옴
        var _fileData = FileOperation(FileMode.Open, $"{Application.dataPath}/Data/Savefile/{fileName}.json");
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
    void LoadGuildData(SaveData loadData)
    {
        EventManager.Instance.Reset_Singleton();

        EventManager.Instance.CurrentTurn = loadData.turn;
        EventManager.Instance.CurrentGuildData = loadData.guildNPCList;
        EventManager.Instance.GuildQuestAdd = loadData.guildQuestList;
        EventManager.Instance.Load_QuestEvent(loadData.currentQuestList);
    }

    #endregion




    #region FileOperation


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



    bool SaveFileSearch(string searchFileName)
    {
        string searchName;
        FileInfo fileInfo;

        searchName = $"{Application.dataPath}/Data/Savefile/{searchFileName}.json";
        fileInfo = new FileInfo(searchName);
        return fileInfo.Exists;
    }


    void DeleteSaveFile(string targetFile)
    {
        if (SaveFileSearch(targetFile))
        {
            File.Delete($"{Application.dataPath}/Data/Savefile/{targetFile}.json");
            Debug.Log(targetFile + " Delete Complete");
        }
        else
            Debug.Log(targetFile + " 이 존재하지 않습니다.");
    }


    #endregion
}