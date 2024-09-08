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


    void Init_Object_CSV()
    {
        //? Object
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Object/Object_Result.csv").Completed +=
(handle) => { CSV_File_Parsing_ObjectAll(OnCSVLoaded(handle)); };

        //? Dialogue
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Dialogue/Dialogue_Result.csv").Completed +=
(handle) => { CSV_File_Parsing_DialogueAll(OnCSVLoaded(handle)); };

        //    Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Dialogue/Dialogue_KR.csv").Completed +=
        //(handle) => { CSV_File_Parsing_Dialogue(OnCSVLoaded(handle), Dialogue_KR); };

        //    Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Dialogue/Dialogue_EN.csv").Completed +=
        //(handle) => { CSV_File_Parsing_Dialogue(OnCSVLoaded(handle), Dialogue_EN); };

        //    Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Dialogue/Dialogue_JP.csv").Completed +=
        //(handle) => { CSV_File_Parsing_Dialogue(OnCSVLoaded(handle), Dialogue_JP); };


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



    public Dictionary<TraitGroup, string[]> Trait_KR = new Dictionary<TraitGroup, string[]>();
    public Dictionary<TraitGroup, string[]> Trait_EN = new Dictionary<TraitGroup, string[]>();
    public Dictionary<TraitGroup, string[]> Trait_JP = new Dictionary<TraitGroup, string[]>();
    
    //? csv 데이터 항목
    // 0 ID, 1 TraitName,
    // 2 Name_KR, 3 Detail_KR, 4 Acquire_KR,
    // 5 Name_EN, 6 Detail_EN, 7 Acquire_EN,
    // 8 Name_JP, 9 Detail_JP, 10 Acquire_JP,
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
            string[] datas = spl_comma;

            Trait_KR.Add((TraitGroup)int.Parse(datas[0]), new string[] { datas[2], datas[3], datas[4] });
            Trait_EN.Add((TraitGroup)int.Parse(datas[0]), new string[] { datas[5], datas[6], datas[7] });
            Trait_JP.Add((TraitGroup)int.Parse(datas[0]), new string[] { datas[8], datas[9], datas[10] });
        }
    }


    public Dictionary<int, string[]> ObjectsLabel_KR { get; } = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> ObjectsLabel_EN { get; } = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> ObjectsLabel_JP { get; } = new Dictionary<int, string[]>();

    //? csv 데이터 항목 - Option은 좀 더 상세분류로 나뉨 - @Op1::Op1  가 존재하면 옵션이 존재하는것
    // 0 KeyName / 1 id /
    // 2 Label_KR / 3 Detail_KR / 4 Option_KR
    // 5 Label_KR / 6 Detail_KR / 7 Option_KR
    // 8 Label_KR / 9 Detail_KR / 10 Option_KR
    void CSV_File_Parsing_ObjectAll(string _stringData)
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

            string[] datas = spl_comma;

            //Debug.Log(datas.Length);
            //foreach (var item in datas)
            //{
            //    Debug.Log(item);
            //}

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

            ObjectsLabel_KR.Add(int.Parse(datas[1]), new string[] { datas[2], datas[3], datas[4] });
            ObjectsLabel_EN.Add(int.Parse(datas[1]), new string[] { datas[5], datas[6], datas[7] });
            ObjectsLabel_JP.Add(int.Parse(datas[1]), new string[] { datas[8], datas[9], datas[10] });
        }
    }


    public Dictionary<DialogueName, DialogueData> Dialogue_KR = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_EN = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_JP = new Dictionary<DialogueName, DialogueData>();

    //? csv 데이터 항목
    // 0 Type(Bubble/Quest) / 1 ID / 2 KeyName / 3 Index / 4 optionString
    // 5 mainText_KR / 11 Title_KR
    // 6 mainText_EN / 12 Title_EN
    // 7 mainText_JP / 13 Title_JP

    void CSV_File_Parsing_DialogueAll(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 6; i < spl_n.Length; i++) //? 맨위 7줄은 무시. (목차같은거)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1]))
            {
                continue;
            }

            int id = int.Parse(spl_comma[1]);
            DialogueData.DialogueType type = (DialogueData.DialogueType)Enum.Parse(typeof(DialogueData.DialogueType), spl_comma[0]);

            var dialogue_KR = new DialogueData(id, type, spl_comma[11]);
            var dialogue_EN = new DialogueData(id, type, spl_comma[12]);
            var dialogue_JP = new DialogueData(id, type, spl_comma[13]);

            while (string.IsNullOrEmpty(spl_comma[3]) == false)
            {
                //int index = int.Parse(spl_comma[3]);
                string optionString = spl_comma[4];

                string mainText_KR = ContainsAndJoin(spl_comma[5]);
                string mainText_EN = ContainsAndJoin(spl_comma[6]);
                string mainText_JP = ContainsAndJoin(spl_comma[7]);

                //var textData = new DialogueData.TextData(optionString, mainText);
                //dialogue.TextDataList.Add(textData);
                //Debug.Log(mainText);

                var textData_KR = new DialogueData.TextData(optionString, mainText_KR);
                var textData_EN = new DialogueData.TextData(optionString, mainText_EN);
                var textData_JP = new DialogueData.TextData(optionString, mainText_JP);

                dialogue_KR.TextDataList.Add(textData_KR);
                dialogue_EN.TextDataList.Add(textData_EN);
                dialogue_JP.TextDataList.Add(textData_JP);


                i++;
                spl_comma = spl_n[i].Split(',');
                if (spl_comma.Length < 2)
                {
                    break;
                }
            }

            Dialogue_KR.Add((DialogueName)id, dialogue_KR);
            Dialogue_EN.Add((DialogueName)id, dialogue_EN);
            Dialogue_JP.Add((DialogueName)id, dialogue_JP);
        }
    }

    string ContainsAndJoin(string mainText)
    {
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

        return mainText;
    }




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
        public DateTime dateTime;

        // 세이브 슬롯에 표시할 게임정보
        public int turn;
        //public int Final_Score;

        // 게임 진행상황
        public int DungeonLV;
        public int FameOfDungeon;
        public int DangerOfDungeon;

        public int Player_Mana;
        public int Player_Gold;
        public int Player_AP;
        public int AP_MAX;

        //public int Prisoner;

        public Save_DayResult CurrentDay;
        public Save_DayResult[] DayResultList;


        // Floor 정보
        public int ActiveFloor_Basement; //? 확장된 계층정보
        public int ActiveFloor_Technical; //? 특수시설 계층


        // Monster 정보 - 완료
        public Save_MonsterData[] monsterList;

        // Facility 정보 = 완료
        public Save_FacilityData[] facilityList;

        // Technical 정보 - 완료
        public Save_TechnicalData[] tachnicalList;


        public UserData.SavefileConfig savefileConfig;

        public BuffList buffList;

        public SaveData_EventData eventData;

        public HashSet<int> instanceGuildNPC;
    }



    public class SaveData_EventData
    {
        // 길드 현재상황 데이터 저장용
        public List<GuildNPC_Data> CurrentGuildData;

        // 추가해야할 퀘스트 목록 - 매턴 갱신(알림 X)
        public List<int> AddQuest_Daily;

        // 현재 진행중인 퀘스트 목록
        public List<int> CurrentQuestAction_forSave;

        // 대기중인 턴 이벤트
        public List<EventManager.DayEvent> DayEventList;

        public List<EventManager.Quest_Reservation> Reservation_Quest;

        public EventManager.ClearEventData CurrentClearEventData;
    }
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
        for (int i = 1; i <= 6 * 10; i++)
        {
            if (Managers.Data.SaveFileSearch($"DM_Save_{i}"))
            {
                var data = LoadToStorage($"DM_Save_{i}");
                if (data != null)
                {
                    SaveFileList.Add($"DM_Save_{i}", data);
                }
            }
        }

        if (Managers.Data.SaveFileSearch($"AutoSave"))
        {
            var data = LoadToStorage($"AutoSave");
            if (data != null)
            {
                SaveFileList.Add($"AutoSave", data);
            }
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

            LoadGuildData(data);
            LoadFileApply(data);

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
        saveData.dateTime = System.DateTime.Now;

        saveData.turn = Main.Instance.Turn;
        //saveData.Final_Score = Main.Instance.Final_Score;
        //saveData.Prisoner = Main.Instance.Prisoner;

        saveData.DungeonLV = Main.Instance.DungeonRank;
        saveData.Player_Mana = Main.Instance.Player_Mana;
        saveData.Player_Gold = Main.Instance.Player_Gold;
        saveData.AP_MAX = Main.Instance.AP_MAX;

        //? 얘네는 단순 int값이 아니라 get set 메서드가 따로 구현된 프로퍼티라 따로 값복사를 해줘야함
        int _fame;
        int _danger;
        int _ap;
        Main.Instance.GetPropertyValue(out _fame, out _danger, out _ap);
        saveData.FameOfDungeon = _fame;
        saveData.DangerOfDungeon = _danger;
        saveData.Player_AP = _ap;

        //? 얘넨 깊은 복사가 되고있음(각각의 값들을 다 새로 복사중)
        saveData.CurrentDay = new Save_DayResult(Main.Instance.CurrentDay);
        saveData.DayResultList = new Save_DayResult[Main.Instance.DayList.Count];
        for (int i = 0; i < Main.Instance.DayList.Count; i++)
        {
            saveData.DayResultList[i] = new Save_DayResult(Main.Instance.DayList[i]);
        }

        saveData.ActiveFloor_Basement = Main.Instance.ActiveFloor_Basement;
        saveData.ActiveFloor_Technical = Main.Instance.ActiveFloor_Technical;

        //? 얘넨 참조타입이긴한데 저장할 때 빼고는 런타임에 쓰지 않는 데이터라 상관이 없음
        saveData.monsterList = GameManager.Monster.GetSaveData_Monster();
        saveData.tachnicalList = GameManager.Technical.GetSaveData_Technical();
        saveData.facilityList = GameManager.Facility.GetSaveData_Facility();


        //? 아래 두개는 실제로 쓰는 데이터를 저장하는 관계로 저장할때와 로드할 때 각각 다 딥카피를 따로 해줘야함.
        saveData.eventData = EventManager.Instance.Data_SaveEventManager();
        saveData.buffList = GameManager.Buff.Save_Buff();


        saveData.instanceGuildNPC = GuildManager.Instance.Data_SaveInstanceNPC();

        UserData.Instance.FileConfig.PlayTimeApply();
        saveData.savefileConfig = UserData.Instance.FileConfig.DeepCopy();

        saveData.isClear = UserData.Instance.isClear;
        saveData.endgins = UserData.Instance.EndingState;

        int highTurn = Mathf.Max(saveData.turn, UserData.Instance.GetDataInt(PrefsKey.High_Turn, 0));
        UserData.Instance.SetData(PrefsKey.High_Turn, highTurn);

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

        SaveData loadData; // = JsonConvert.DeserializeObject<SaveData>(_fileData);

        try
        {
            loadData = JsonConvert.DeserializeObject<SaveData>(_fileData);
            Console.WriteLine("SaveData loaded successfully.");
            return loadData;
        }
        catch (JsonException ex)
        {
            Console.WriteLine("Failed to load SaveData: " + ex.Message);
            // 세이브 파일 삭제 작업 수행
            DeleteSaveFile(fileName);
            return null;
        }
    }

    void LoadFileApply(SaveData loadData)
    {
        UserData.Instance.FileConfig = loadData.savefileConfig.DeepCopy();
        UserData.Instance.FileConfig.Init_CurrentPlayTime();

        GameManager.Buff.Load_Buff(loadData.buffList);

        Main.Instance.SetLoadData(loadData);

        GameManager.Monster.Load_MonsterData(loadData.monsterList);
        GameManager.Technical.Load_TechnicalData(loadData.tachnicalList);
        GameManager.Facility.Load_FacilityData(loadData.facilityList);
    }
    void LoadGuildData(SaveData loadData)
    {
        GuildManager.Instance.Data_LoadInstanceNPC(loadData.instanceGuildNPC);
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