using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    public Dictionary<int, string[]> ObjectsLabel_JP = new Dictionary<int, string[]>();

    void Init_Object_CSV()
    {
        //오브젝트
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Object/Object_KR.csv").Completed += 
    (handle) => { CSV_File_Parsing_Object(OnCSVLoaded(handle), ObjectsLabel_KR); };

        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Object/Object_EN.csv").Completed +=
    (handle) => { CSV_File_Parsing_Object(OnCSVLoaded(handle), ObjectsLabel_EN); };

        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Object/Object_JP.csv").Completed +=
    (handle) => { CSV_File_Parsing_Object(OnCSVLoaded(handle), ObjectsLabel_JP); };

        //대사
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Dialogue/Dialogue_KR.csv").Completed +=
    (handle) => { CSV_File_Parsing_Dialogue(OnCSVLoaded(handle), Dialogue_KR); };

        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Dialogue/Dialogue_EN.csv").Completed +=
    (handle) => { CSV_File_Parsing_Dialogue(OnCSVLoaded(handle), Dialogue_EN); };

        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Dialogue/Dialogue_JP.csv").Completed +=
    (handle) => { CSV_File_Parsing_Dialogue(OnCSVLoaded(handle), Dialogue_JP); };



        //CSV_File_Parsing_Object("Object/Object_KR", ObjectsLabel_KR);
        //CSV_File_Parsing_Object("Object/Object_EN", ObjectsLabel_EN);
        //CSV_File_Parsing_Object("Object/Object_JP", ObjectsLabel_JP);

        //CSV_File_Parsing_Dialogue("Dialogue/Dialogue_KR", Dialogue_KR);
        //CSV_File_Parsing_Dialogue("Dialogue/Dialogue_EN", Dialogue_EN);
        //CSV_File_Parsing_Dialogue("Dialogue/Dialogue_JP", Dialogue_JP);

        //? Trait - 한영일 하나의 파일로 파싱
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Trait/Trait_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_Trait(OnCSVLoaded(handle)); };

    }


    string OnCSVLoaded(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // 로드된 CSV 파일 텍스트 데이터 얻기
            TextAsset csvAsset = handle.Result;
            string csvText = csvAsset.text;

            // CSV 파일 처리
            return csvText;
            //Debug.Log(csvText);
        }
        else
        {
            return null;
            //Debug.LogError("Failed to load CSV file: " + handle.OperationException);
        }
    }



    public Dictionary<TraitGroup, (string, string)> Trait_KR = new Dictionary<TraitGroup, (string, string)>();
    public Dictionary<TraitGroup, (string, string)> Trait_EN = new Dictionary<TraitGroup, (string, string)>();
    public Dictionary<TraitGroup, (string, string)> Trait_JP = new Dictionary<TraitGroup, (string, string)>();

    // 0 ID, 1 TraitName, 2 Name_KR, 3 Detail_KR, 4 Name_EN, 5 Detail_EN, 6 Name_JP, 7 Detail_JP
    void CSV_File_Parsing_Trait(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1])) //? 빈칸이면 다음으로
            {
                continue;
            }

            //string[] datas = new string[] { spl_comma[0], spl_comma[1], spl_comma[2], spl_comma[3], spl_comma[4], spl_comma[5], spl_comma[6], spl_comma[7] };
            string[] datas = spl_comma;

            Trait_KR.Add((TraitGroup)int.Parse(datas[0]), (datas[2], datas[3]));
            Trait_EN.Add((TraitGroup)int.Parse(datas[0]), (datas[4], datas[5]));
            Trait_JP.Add((TraitGroup)int.Parse(datas[0]), (datas[6], datas[7]));
        }
    }




    // 0 x / 1 : id / 2 : Label / 3 : Detail / 4 : Option1 / 5 : Option2 / 6: Option3
    void CSV_File_Parsing_Object(string _stringData, Dictionary<int, string[]> _dict) 
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        //var obj_kr = FileOperation(FileMode.Open, $"{Application.streamingAssetsPath}/{_filePath}.csv");
        var spl_n = _stringData.Split('\n');
        
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

                if (datas[k].Contains('-'))
                {
                    var split = datas[k].Split('-');
                    datas[k] = string.Join(',', split);
                }

                if (datas[k].Contains('^'))
                {
                    var split = datas[k].Split('^');
                    datas[k] = string.Join('-', split);
                }
            }

            _dict.Add(int.Parse(spl_comma[1]), datas);
        }
    }


    public Dictionary<DialogueName, DialogueData> Dialogue_KR = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_EN = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_JP = new Dictionary<DialogueName, DialogueData>();

    void CSV_File_Parsing_Dialogue(string _stringData, Dictionary<DialogueName, DialogueData> _dict)
    {
        if (string.IsNullOrEmpty(_stringData)) return;
        //var obj_kr = FileOperation(FileMode.Open, $"{Application.streamingAssetsPath}/{_filePath}.csv");
        var spl_n = _stringData.Split('\n');

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
                if (mainText.Contains('^'))
                {
                    var split = mainText.Split('^');
                    mainText = string.Join('-', split);
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
        public bool isClear;
        public Endings endgins;

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


        //// Guild 정보
        //public List<GuildNPC_Data> guildNPCList;

        //// 조건채운 퀘스트 목록(길드에 추가되기전)
        //public List<int> guildQuestList;

        //// 현재 진행중인 퀘스트 목록
        //public List<int> currentQuestList;

        public UserData.SavefileConfig savefileConfig;

        public BuffList buffList;


        public SaveData_EventData eventData;
    }



    public class SaveData_EventData
    {
        // 길드 현재상황 데이터 저장용
        public List<GuildNPC_Data> CurrentGuildData;

        // 추가해야할 퀘스트 목록
        public List<int> AddQuest_Special;

        // 추가해야할 퀘스트 목록 - 매턴 갱신(알림 X)
        public List<int> AddQuest_Daily;

        // 현재 진행중인 퀘스트 목록
        public List<int> CurrentQuestAction_forSave;

        // 대기중인 턴 이벤트
        public List<EventManager.DayEvent> DayEventList;

        public List<EventManager.Quest_Reservation> Reservation_Quest;
    }
    //private SaveData tempData;
    #endregion


    #region SaveLoad

    Dictionary<string, SaveData> SaveFileList = new Dictionary<string, SaveData>();

    public int SaveFileCount()
    {
        return SaveFileList.Count;
    }

    public void DeleteSaveFileAll()
    {
        foreach (var savefile in SaveFileList)
        {
            DeleteSaveFile(savefile.Key);
        }

        SaveFileList = new Dictionary<string, SaveData>();
    }

    void Scan_File()
    {
        for (int i = 1; i <= 6 * 5; i++)
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

        UserData.Instance.SavePlayTime();
    }

    //? 자꾸 SaveFile이 덧씌워져서 강제로 파일에서 읽어오기
    //public void LoadGame_ToFile(int index)
    //{
    //    if (Managers.Data.SaveFileSearch($"DM_Save_{index}"))
    //    {
    //        var data = LoadToStorage($"DM_Save_{index}");

    //        UserData.Instance.CurrentSaveData = data;
    //        UserData.Instance.isClear = data.isClear;
    //        UserData.Instance.EndingState = data.endgins;
    //        LoadFileApply(data);
    //        LoadGuildData(data);
    //        Debug.Log($"Load Success : Slot_{index}");
    //        UserData.Instance.SetData(PrefsKey.LoadTimes, UserData.Instance.GetDataInt(PrefsKey.LoadTimes) + 1);
    //    }
    //    else
    //    {
    //        Debug.Log($"Load Fail : Slot_{index}");
    //    }
    //}


    //public SaveData TestLoadFile(string fileKey)
    //{
    //    SaveData data = null;
    //    SaveFileList.TryGetValue(fileKey, out data);
    //    return data;
    //}

    public void LoadGame(string fileKey)
    {
        SaveData data = null;
        if (SaveFileList.TryGetValue(fileKey, out data))
        {
            UserData.Instance.CurrentSaveData = data;
            UserData.Instance.isClear = data.isClear;
            UserData.Instance.EndingState = data.endgins;
            LoadFileApply(data);
            LoadGuildData(data);
            Debug.Log($"Load Success : {fileKey}");
            UserData.Instance.SetData(PrefsKey.LoadTimes, UserData.Instance.GetDataInt(PrefsKey.LoadTimes) + 1);
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
    public bool SaveFileExistCheck()
    {
        return SaveFileList.Count > 0 ? true : false;
    }




    public SaveData SaveCurrentData(string fileName, int index = -1)
    {
        if (Main.Instance.Management == false)
        {
            Debug.Log("낮동안은 저장불가");
            return null;
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


        saveData.eventData = EventManager.Instance.Data_SaveEventManager();

        //saveData.savefileConfig = UserData.Instance.FileConfig;
        UserData.Instance.FileConfig.PlayTimeApply();
        saveData.savefileConfig = UserData.Instance.FileConfig.Clone();

        saveData.isClear = UserData.Instance.isClear;
        saveData.endgins = UserData.Instance.EndingState;

        int highTurn = Mathf.Max(saveData.turn, UserData.Instance.GetDataInt(PrefsKey.High_Turn, 0));
        UserData.Instance.SetData(PrefsKey.High_Turn, highTurn);

        saveData.buffList = GameManager.Buff.Save_Buff();


        return saveData;
    }


    public void SaveAndAddFile(string fileName, int index)
    {
        var saveData = SaveCurrentData(fileName, index);

        Add_File(saveData, $"{fileName}");
    }
    public void SaveAndAddFile(SaveData data, string fileName, int index)
    {
        Add_File(data, $"{fileName}");
    }




    void SaveToStorage(SaveData data, string fileName)
    {
        //? 파일로 저장
        string jsonData = JsonConvert.SerializeObject(data);
        var result = FileOperation(FileMode.Create, $"{Application.persistentDataPath}/Savefile/{fileName}.json", jsonData);
        Debug.Log($"Save Sucess : {fileName}");

        // 저장할 때 마다 도감 상황도 업데이트 되어야함.
        SaveCollectionData();
    }


    SaveData LoadToStorage(string fileName)
    {
        //? 저장된 파일 읽어옴
        var _fileData = FileOperation(FileMode.Open, $"{Application.persistentDataPath}/Savefile/{fileName}.json");
        SaveData loadData = JsonConvert.DeserializeObject<SaveData>(_fileData);
        return loadData;
    }

    void LoadFileApply(SaveData loadData)
    {
        UserData.Instance.FileConfig = loadData.savefileConfig.Clone();
        UserData.Instance.FileConfig.Init_CurrentPlayTime();

        GameManager.Buff.Load_Buff(loadData.buffList);

        Main.Instance.SetLoadData(loadData);

        GameManager.Monster.Load_MonsterData(loadData.monsterList);
        GameManager.Technical.Load_TechnicalData(loadData.tachnicalList);
        GameManager.Facility.Load_FacilityData(loadData.facilityList);
    }
    void LoadGuildData(SaveData loadData)
    {
        EventManager.Instance.Data_LoadEventManager(loadData);
    }

    #endregion




    #region SaveCollectionData

    public void SaveCollectionData()
    {
        string jsonData = JsonConvert.SerializeObject(CollectionManager.Instance.SaveCollectionData());
        var result = FileOperation(FileMode.Create, $"{Application.persistentDataPath}/Savefile/CollectionData.json", jsonData);
    }

    public bool LoadCollectionData()
    {
        //? 저장된 파일 읽어옴
        if (SaveFileSearch("CollectionData"))
        {
            var _fileData = FileOperation(FileMode.Open, $"{Application.persistentDataPath}/Savefile/CollectionData.json");
            var loadData = JsonConvert.DeserializeObject<Dictionary<int, Regist_Info>>(_fileData);
            CollectionManager.Instance.LoadCollectionData(loadData);
            return true;
        }
        else
        {
            Debug.Log("CollectionData Not Exsit");
            return false;
        }
    }


    public void SaveClearData()
    {
        string jsonData = JsonConvert.SerializeObject(CollectionManager.Instance.SaveMultiData());
        var result = FileOperation(FileMode.Create, $"{Application.persistentDataPath}/Savefile/ClearData.json", jsonData);
    }
    public bool LoadClearData()
    {
        if (SaveFileSearch("ClearData"))
        {
            var _fileData = FileOperation(FileMode.Open, $"{Application.persistentDataPath}/Savefile/ClearData.json");
            var loadData = JsonConvert.DeserializeObject<CollectionManager.RoundData>(_fileData);
            CollectionManager.Instance.LoadMultiData(loadData);
            return true;
        }
        else
        {
            Debug.Log("ClearData Not Exsit");
            return false;
        }
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
                if (Directory.Exists($"{Application.persistentDataPath}/Savefile") == false)
                {
                    Directory.CreateDirectory($"{Application.persistentDataPath}/Savefile");
                }

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

        searchName = $"{Application.persistentDataPath}/Savefile/{searchFileName}.json";
        fileInfo = new FileInfo(searchName);
        return fileInfo.Exists;
    }


    public void DeleteSaveFile(string targetFile)
    {
        if (SaveFileSearch(targetFile))
        {
            File.Delete($"{Application.persistentDataPath}/Savefile/{targetFile}.json");
            Debug.Log(targetFile + " Delete Complete");
        }
        else
            Debug.Log(targetFile + " 이 존재하지 않습니다.");
    }


    #endregion
}