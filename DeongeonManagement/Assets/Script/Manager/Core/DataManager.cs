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

        //? Trait - �ѿ��� �ϳ��� ���Ϸ� �Ľ�
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Trait/Trait_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_Trait(OnCSVLoaded(handle)); };

        //? Artifact
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Artifact/Artifact_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_Artifact(OnCSVLoaded(handle)); };

        //? Title
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Title/Title_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_Title(OnCSVLoaded(handle)); };

        //? RandomEvent
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/RandomEvent/RandomEvent_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_RandomEvent(OnCSVLoaded(handle)); };

        //? BattleStatue (�����̻�)
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/BattleStatus/BattleStatus_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_BattleStatus(OnCSVLoaded(handle)); };
    }


    string OnCSVLoaded(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // �ε�� CSV ���� �ؽ�Ʈ ������ ���
            TextAsset csvAsset = handle.Result;
            string csvText = csvAsset.text;

            // CSV ���� ó��
            return csvText;
            //Debug.Log(csvText);
        }
        else
        {
            return null;
            //Debug.LogError("Failed to load CSV file: " + handle.OperationException);
        }
    }


    //? ��� ��巹���� csv������ ���������� �ε� �ƴ��� Ȯ�� �� �ݹ�. ���ڴ� �߰��Ҷ����� �ٲ����. LoadSucceed ���� Ƚ�� = csvFileAll
    int csvLoadCount = 0;
    int csvFileAll = 7;
    public event Action OnAddressablesComplete;

    void LoadSucceed()
    {
        csvLoadCount++;
        if (csvLoadCount >= csvFileAll)
        {
            OnAddressablesComplete?.Invoke();
        }
    }


    public string[] GetTextData_Object(int id)
    {
        string[] datas = null;
        switch (UserData.Instance.Language)
        {
            case Define.Language.EN:
                Managers.Data.ObjectsLabel_EN.TryGetValue(id, out datas);
                break;

            case Define.Language.KR:
                Managers.Data.ObjectsLabel_KR.TryGetValue(id, out datas);
                break;

            case Define.Language.JP:
                Managers.Data.ObjectsLabel_JP.TryGetValue(id, out datas);
                break;

            case Define.Language.SCC:
                Managers.Data.ObjectsLabel_SCC.TryGetValue(id, out datas);
                break;
        }
        return datas;
    }
    public string[] GetTextData_Trait(int id)
    {
        string[] datas = null;
        switch (UserData.Instance.Language)
        {
            case Define.Language.EN:
                Managers.Data.Trait_EN.TryGetValue((TraitGroup)id, out datas);
                break;

            case Define.Language.KR:
                Managers.Data.Trait_KR.TryGetValue((TraitGroup)id, out datas);
                break;

            case Define.Language.JP:
                Managers.Data.Trait_JP.TryGetValue((TraitGroup)id, out datas);
                break;

            case Define.Language.SCC:
                Managers.Data.Trait_SCC.TryGetValue((TraitGroup)id, out datas);
                break;
        }
        return datas;
    }

    public string[] GetTextData_Artifact(int id)
    {
        string[] datas = null;
        switch (UserData.Instance.Language)
        {
            case Define.Language.EN:
                Managers.Data.Artifact_EN.TryGetValue((ArtifactLabel)id, out datas);
                break;

            case Define.Language.KR:
                Managers.Data.Artifact_KR.TryGetValue((ArtifactLabel)id, out datas);
                break;

            case Define.Language.JP:
                Managers.Data.Artifact_JP.TryGetValue((ArtifactLabel)id, out datas);
                break;

            case Define.Language.SCC:
                Managers.Data.Artifact_SCC.TryGetValue((ArtifactLabel)id, out datas);
                break;
        }
        return datas;
    }
    public string[] GetTextData_Title(int id)
    {
        string[] datas = null;
        switch (UserData.Instance.Language)
        {
            case Define.Language.EN:
                Managers.Data.Title_EN.TryGetValue(id, out datas);
                break;

            case Define.Language.KR:
                Managers.Data.Title_KR.TryGetValue(id, out datas);
                break;

            case Define.Language.JP:
                Managers.Data.Title_JP.TryGetValue(id, out datas);
                break;

            case Define.Language.SCC:
                Managers.Data.Title_SCC.TryGetValue(id, out datas);
                break;
        }
        return datas;
    }

    public string GetTextData_RandomEvent(int id)
    {
        string datas = null;
        switch (UserData.Instance.Language)
        {
            case Define.Language.EN:
                Managers.Data.RandomEvent_EN.TryGetValue(id, out datas);
                break;

            case Define.Language.KR:
                Managers.Data.RandomEvent_KR.TryGetValue(id, out datas);
                break;

            case Define.Language.JP:
                Managers.Data.RandomEvent_JP.TryGetValue(id, out datas);
                break;

            case Define.Language.SCC:
                Managers.Data.RandomEvent_SCC.TryGetValue(id, out datas);
                break;
        }
        return datas;
    }

    public string[] GetTextData_BattleStatus(int id)
    {
        string[] datas = null;
        switch (UserData.Instance.Language)
        {
            case Define.Language.EN:
                Managers.Data.BattleStatus_EN.TryGetValue(id, out datas);
                break;

            case Define.Language.KR:
                Managers.Data.BattleStatus_KR.TryGetValue(id, out datas);
                break;

            case Define.Language.JP:
                Managers.Data.BattleStatus_JP.TryGetValue(id, out datas);
                break;

            case Define.Language.SCC:
                Managers.Data.BattleStatus_SCC.TryGetValue(id, out datas);
                break;
        }
        return datas;
    }








    public Dictionary<int, string[]> ObjectsLabel_KR = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> ObjectsLabel_EN = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> ObjectsLabel_JP = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> ObjectsLabel_SCC = new Dictionary<int, string[]>();

    //? csv ������ �׸� - Option�� �� �� �󼼺з��� ���� - @Op1::Op1  �� �����ϸ� �ɼ��� �����ϴ°�
    // 0 KeyName / 1 id /
    // 2 Label_KR / 3 Detail_KR / 4 Option_KR
    // 5 Label_EN / 6 Detail_EN / 7 Option_EN
    // 8 Label_JP / 9 Detail_JP / 10 Option_JP
    // 11 Label_SCC / 12 Detail_SCC / 13 Option_SCC
    void CSV_File_Parsing_ObjectAll(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1])) //? ��ĭ�̸� ��������
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
            ObjectsLabel_SCC.Add(int.Parse(datas[1]), new string[] { datas[11], datas[12], datas[13] });
        }
        LoadSucceed();
    }


    public Dictionary<DialogueName, DialogueData> Dialogue_KR { get; } = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_EN { get; } = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_JP { get; } = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_SCC { get; } = new Dictionary<DialogueName, DialogueData>();

    //? csv ������ �׸�
    // 0 Type(Bubble/Quest) / 1 ID / 2 KeyName / 3 Index / 4 optionString
    // 5 mainText_KR / 11 Title_KR
    // 6 mainText_EN / 12 Title_EN
    // 7 mainText_JP / 13 Title_JP
    // 8 mainText_SCC / 14 Title_SCC (��ü��)

    void CSV_File_Parsing_DialogueAll(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 6; i < spl_n.Length; i++) //? ���� 7���� ����. (����������)
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
            var dialogue_SCC = new DialogueData(id, type, spl_comma[14]);

            while (string.IsNullOrEmpty(spl_comma[3]) == false)
            {
                //int index = int.Parse(spl_comma[3]);
                string optionString = spl_comma[4];

                string mainText_KR = ContainsAndJoin(spl_comma[5]);
                string mainText_EN = ContainsAndJoin(spl_comma[6]);
                string mainText_JP = ContainsAndJoin(spl_comma[7]);
                string mainText_SCC = ContainsAndJoin(spl_comma[8]);

                //var textData = new DialogueData.TextData(optionString, mainText);
                //dialogue.TextDataList.Add(textData);
                //Debug.Log(mainText);

                var textData_KR = new DialogueData.TextData(optionString, mainText_KR);
                var textData_EN = new DialogueData.TextData(optionString, mainText_EN);
                var textData_JP = new DialogueData.TextData(optionString, mainText_JP);
                var textData_SCC = new DialogueData.TextData(optionString, mainText_SCC);

                dialogue_KR.TextDataList.Add(textData_KR);
                dialogue_EN.TextDataList.Add(textData_EN);
                dialogue_JP.TextDataList.Add(textData_JP);
                dialogue_SCC.TextDataList.Add(textData_SCC);


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
            Dialogue_SCC.Add((DialogueName)id, dialogue_SCC);
        }
        LoadSucceed();
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


    public Dictionary<TraitGroup, string[]> Trait_KR = new Dictionary<TraitGroup, string[]>();
    public Dictionary<TraitGroup, string[]> Trait_EN = new Dictionary<TraitGroup, string[]>();
    public Dictionary<TraitGroup, string[]> Trait_JP = new Dictionary<TraitGroup, string[]>();
    public Dictionary<TraitGroup, string[]> Trait_SCC = new Dictionary<TraitGroup, string[]>();

    //? csv ������ �׸�
    // 0 ID, 1 TraitName,
    // 2 Name_KR, 3 Detail_KR, 4 Acquire_KR,
    // 5 Name_EN, 6 Detail_EN, 7 Acquire_EN,
    // 8 Name_JP, 9 Detail_JP, 10 Acquire_JP,
    // 9 Name_SCC, 10 Detail_SCC, 11 Acquire_SCC,
    void CSV_File_Parsing_Trait(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1])) //? ��ĭ�̸� ��������
            {
                continue;
            }
            string[] datas = spl_comma;

            Trait_KR.Add((TraitGroup)int.Parse(datas[0]), new string[] { datas[2], ContainsAndJoin(datas[3]), datas[4] });
            Trait_EN.Add((TraitGroup)int.Parse(datas[0]), new string[] { datas[5], ContainsAndJoin(datas[6]), datas[7] });
            Trait_JP.Add((TraitGroup)int.Parse(datas[0]), new string[] { datas[8], ContainsAndJoin(datas[9]), datas[10] });
            Trait_SCC.Add((TraitGroup)int.Parse(datas[0]), new string[] { datas[11], ContainsAndJoin(datas[12]), datas[13] });
        }
        LoadSucceed();
    }



    //? Artifact
    public Dictionary<ArtifactLabel, string[]> Artifact_KR = new Dictionary<ArtifactLabel, string[]>();
    public Dictionary<ArtifactLabel, string[]> Artifact_EN = new Dictionary<ArtifactLabel, string[]>();
    public Dictionary<ArtifactLabel, string[]> Artifact_JP = new Dictionary<ArtifactLabel, string[]>();
    public Dictionary<ArtifactLabel, string[]> Artifact_SCC = new Dictionary<ArtifactLabel, string[]>();

    //? csv ������ �׸� - �̸� / ���� / ȿ��
    // 0 ID, 1 KeyName,
    // 2 Name_KR, 3 Detail_KR, 4 Acquire_KR,
    // 5 Name_EN, 6 Detail_EN, 7 Acquire_EN,
    // 8 Name_JP, 9 Detail_JP, 10 Acquire_JP,
    // 11 Name_SCC, 12 Detail_SCC, 13 Acquire_SCC,
    void CSV_File_Parsing_Artifact(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1])) //? ��ĭ�̸� ��������
            {
                continue;
            }
            string[] datas = spl_comma;

            Artifact_KR.Add((ArtifactLabel)int.Parse(datas[0]), new string[] { datas[2], ContainsAndJoin(datas[3]), datas[4] });
            Artifact_EN.Add((ArtifactLabel)int.Parse(datas[0]), new string[] { datas[5], ContainsAndJoin(datas[6]), datas[7] });
            Artifact_JP.Add((ArtifactLabel)int.Parse(datas[0]), new string[] { datas[8], ContainsAndJoin(datas[9]), datas[10] });
            Artifact_SCC.Add((ArtifactLabel)int.Parse(datas[0]), new string[] { datas[11], ContainsAndJoin(datas[12]), datas[13] });
        }
        LoadSucceed();
    }

    //? Title
    public Dictionary<int, string[]> Title_KR = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Title_EN = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Title_JP = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Title_SCC = new Dictionary<int, string[]>();

    //? csv ������ �׸� - �̸� / ���� / ȿ��
    // 0 ID, 1 KeyName,
    // 2 Name_KR, 3 Detail_KR, 4 Effect_KR, 5 Acquire_KR,
    // 6 7 8 9
    // 10 11 12 13
    // 14 15 16 17
    void CSV_File_Parsing_Title(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1])) //? ��ĭ�̸� ��������
            {
                continue;
            }
            string[] datas = spl_comma;

            Title_KR.Add(int.Parse(datas[0]), new string[] { datas[2], ContainsAndJoin(datas[3]), datas[4], datas[5] });
            Title_EN.Add(int.Parse(datas[0]), new string[] { datas[6], ContainsAndJoin(datas[7]), datas[8], datas[9] });
            Title_JP.Add(int.Parse(datas[0]), new string[] { datas[10], ContainsAndJoin(datas[11]), datas[12], datas[13] });
            Title_SCC.Add(int.Parse(datas[0]), new string[] { datas[14], ContainsAndJoin(datas[15]), datas[16], datas[17] });
        }
        LoadSucceed();
    }


    //? RandomEvent
    public Dictionary<int, string> RandomEvent_KR = new Dictionary<int, string>();
    public Dictionary<int, string> RandomEvent_EN = new Dictionary<int, string>();
    public Dictionary<int, string> RandomEvent_JP = new Dictionary<int, string>();
    public Dictionary<int, string> RandomEvent_SCC = new Dictionary<int, string>();
    // 0 ID
    // 1 KR, 2 EN, 3 JP, 4 SCC,
    void CSV_File_Parsing_RandomEvent(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1])) //? ��ĭ�̸� ��������
            {
                continue;
            }
            string[] datas = spl_comma;

            RandomEvent_KR.Add(int.Parse(datas[0]), ContainsAndJoin(datas[1]));
            RandomEvent_EN.Add(int.Parse(datas[0]), ContainsAndJoin(datas[2]));
            RandomEvent_JP.Add(int.Parse(datas[0]), ContainsAndJoin(datas[3]));
            RandomEvent_SCC.Add(int.Parse(datas[0]), ContainsAndJoin(datas[4]));
        }
        LoadSucceed();
    }

    //? BattleStatus
    public Dictionary<int, string[]> BattleStatus_KR = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> BattleStatus_EN = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> BattleStatus_JP = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> BattleStatus_SCC = new Dictionary<int, string[]>();
    // 0 ID
    // 1 KR, 2 EN, 3 JP, 4 SCC,
    void CSV_File_Parsing_BattleStatus(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1])) //? ��ĭ�̸� ��������
            {
                continue;
            }
            string[] datas = spl_comma;

            BattleStatus_KR.Add(int.Parse(datas[0]), new string[] { datas[1], ContainsAndJoin(datas[2]) });
            BattleStatus_EN.Add(int.Parse(datas[0]), new string[] { datas[3], ContainsAndJoin(datas[4]) });
            BattleStatus_JP.Add(int.Parse(datas[0]), new string[] { datas[5], ContainsAndJoin(datas[6]) });
            BattleStatus_SCC.Add(int.Parse(datas[0]), new string[] { datas[7], ContainsAndJoin(datas[8]) });
        }
        LoadSucceed();
    }



    #endregion






    #region SaveClass

    public class SaveData
    {
        public string version_info;
        public int difficultyLevel;
        public float playTimes;

        public bool isClear;
        public Endings endgins;


        // ���̺� ���� ����
        public int saveIndex;
        public DateTime dateTime;


        //? ���� ���� ����
        public CurrentGameData mainData;

        //? ���� ��� ����
        public Statistics statistics;

        //? ���� �̺�Ʈ ����
        public List<RandomEventManager.CurrentRandomEventContent> currentRandomEventList;

        // Monster ���� - �Ϸ�
        public Save_MonsterData[] monsterList;

        // Facility ���� = �Ϸ�
        public Save_FacilityData[] facilityList;

        // Technical ���� - �Ϸ�
        public Save_TechnicalData[] tachnicalList;

        //? ����ũ NPC ����
        public Dictionary<NPC_Type_Unique, float> uniqueNPC_List;

        //? ��Ƽ��Ʈ ����
        public List<ArtifactManager.Artifact> artifactList;

        //? ���� Īȣ ����
        public List<TitleManager.DungeonTitle> titleList;


        public UserData.SavefileConfig savefileConfig;

        public BuffList buffList;

        public SaveData_EventData eventData;

        public HashSet<int> instanceGuildNPC;
        public HashSet<int> deleteGuildNPC;
    }



    public class SaveData_EventData
    {
        // ��� �����Ȳ ������ �����
        public List<GuildNPC_Data> CurrentGuildData;

        // �߰��ؾ��� ����Ʈ ��� - ���� ����(�˸� X)
        public List<int> AddQuest_Daily;

        // ���� �������� ����Ʈ ���
        public List<int> CurrentQuestAction_forSave;

        // ������� �� �̺�Ʈ
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
        DeleteSaveFile("Temp_GuildSave");

        SaveFileList = new Dictionary<string, SaveData>();
    }

    const int saveFileSlot = 6;
    const int saveFilePage = 10;

    void Scan_File()
    {
        for (int i = 1; i <= saveFileSlot * saveFilePage; i++)
        {
            if (Managers.Data.SaveFileSearch($"DM_Save_{i}"))
            {
                File_Defect_Testing($"DM_Save_{i}");
            }
        }

        if (Managers.Data.SaveFileSearch($"AutoSave"))
        {
            File_Defect_Testing($"AutoSave");
        }
    }

    void File_Defect_Testing(string fileName)
    {
        try
        {
            var data = LoadToStorage(fileName);
            if (data == null || data.version_info == null)
            {
                Debug.Log($"Deleting old save file: {fileName}");
                DeleteSaveFile(fileName);
                return;
            }

            if (data.version_info != null)
            {
                string[] savedParts = data.version_info.Split('.');
                float saveVersion = float.Parse(savedParts[0] + "." + savedParts[1]);

                Debug.Log($"Save file Version Num : {saveVersion}");
                if (saveVersion < 0.91f)
                {
                    Debug.Log($"ȣȯ���� �ʴ� ���̺� ���� ���� : {data.version_info}");
                    DeleteSaveFile(fileName);
                    return;
                }
            }


            SaveFileList.Add(fileName, data);
        }
        catch (Exception e)
        {
            Debug.Log($"Deleting old save file: {fileName}");
            DeleteSaveFile(fileName);
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

    //? �ڲ� SaveFile�� ���������� ������ ���Ͽ��� �о����
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
            UserData.Instance.FileConfig = data.savefileConfig.DeepCopy();
            UserData.Instance.FileConfig.Init_CurrentPlayTime();

            LoadGuildData(data);
            LoadFileApply(data);

            Debug.Log($"Load Success : {fileKey}");
            //UserData.Instance.SetData(PrefsKey.LoadTimes, UserData.Instance.GetDataInt(PrefsKey.LoadTimes) + 1);
            UserData.Instance.CurrentPlayerData.config.LoadCount++;
        }
        else
        {
            Debug.Log($"Load Fail : {fileKey}");
        }
    }
    //? ��忡�� ���ƿ� �� ������ �����͸� ������ �ȵǹǷ� �̰� ȣ��
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
            Debug.Log("�������� ����Ұ�");
            return null;
        }

        //? ������ ������ ���� ���
        SaveData saveData = new SaveData();

        saveData.version_info = Application.version;
        saveData.difficultyLevel = (int)UserData.Instance.FileConfig.Difficulty;
        saveData.playTimes = UserData.Instance.FileConfig.PlayTimes;


        saveData.saveIndex = index;
        saveData.dateTime = System.DateTime.Now;

        //? ���� ���� ������
        saveData.mainData = Main.Instance.Save_MainData();

        //? ���� ���� �����͸� �������� ��� ������Ʈ �� �� ������ ������Ʈ�ϱ�
        Main.Instance.CurrentStatistics.Update_ToSave(saveData.mainData);
        saveData.statistics = Main.Instance.CurrentStatistics?.DeepCopy();

        //? ���� �����Ϳ� ���ԵǾ������� ������ ��ũ��Ʈ�� ����
        saveData.currentRandomEventList = RandomEventManager.Instance.Save_RE_Seed();

        //? ��� ����Ÿ���̱��ѵ� ������ �� ����� ��Ÿ�ӿ� ���� �ʴ� �����Ͷ� ����� ����
        saveData.monsterList = GameManager.Monster.GetSaveData_Monster();
        saveData.tachnicalList = GameManager.Technical.GetSaveData_Technical();
        saveData.facilityList = GameManager.Facility.GetSaveData_Facility();
        saveData.uniqueNPC_List = GameManager.NPC.Save_NPCData();
        saveData.artifactList = GameManager.Artifact.Save_ArtifactData();
        saveData.titleList = GameManager.Title.Save_TitlesData();


        //? �Ʒ� �ΰ��� ������ ���� �����͸� �����ϴ� ����� �����Ҷ��� �ε��� �� ���� �� ��ī�Ǹ� ���� �������.
        saveData.eventData = EventManager.Instance.Data_SaveEventManager();
        saveData.buffList = GameManager.Buff.Save_Buff();


        saveData.instanceGuildNPC = GuildManager.Instance.Data_SaveInstanceNPC();
        saveData.deleteGuildNPC = GuildManager.Instance.Data_SaveDeleteNPC();

        UserData.Instance.FileConfig.PlayTimeApply();
        saveData.savefileConfig = UserData.Instance.FileConfig.DeepCopy();

        saveData.isClear = UserData.Instance.isClear;
        saveData.endgins = UserData.Instance.EndingState;

        //int highTurn = Mathf.Max(saveData.turn, UserData.Instance.GetDataInt(PrefsKey.High_Turn, 0));
        //UserData.Instance.SetData(PrefsKey.High_Turn, highTurn);

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
        //? ���Ϸ� ����
        string jsonData = JsonConvert.SerializeObject(data);
        var result = FileOperation(FileMode.Create, $"{Application.persistentDataPath}/Savefile/{fileName}.json", jsonData);
        Debug.Log($"Save Sucess : {fileName}");

        // ������ �� ���� ���� ��Ȳ�� ������Ʈ �Ǿ����.
        SaveCollectionData();
    }


    SaveData LoadToStorage(string fileName)
    {
        //? ����� ���� �о��
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
            // ���̺� ���� ���� �۾� ����
            DeleteSaveFile(fileName);
            return null;
        }
    }

    void LoadFileApply(SaveData loadData)
    {
        GameManager.Buff.Load_Buff(loadData.buffList);

        Main.Instance.SetLoadData(loadData);

        RandomEventManager.Instance.Load_RE_Seed(loadData.currentRandomEventList);

        GameManager.Monster.Load_MonsterData(loadData.monsterList);
        GameManager.Technical.Load_TechnicalData(loadData.tachnicalList);
        GameManager.Facility.Load_FacilityData(loadData.facilityList);
        GameManager.NPC.Load_NPCData(loadData.uniqueNPC_List);
        GameManager.Artifact.Load_ArtifactData(loadData.artifactList);
        GameManager.Title.Load_TitlesData(loadData.titleList);
    }
    void LoadGuildData(SaveData loadData)
    {
        GuildManager.Instance.Data_LoadInstanceNPC(loadData.instanceGuildNPC);
        GuildManager.Instance.Data_LoadDeleteNPC(loadData.deleteGuildNPC);
        EventManager.Instance.Data_LoadEventManager(loadData);
    }





    public bool Contains_FileID(int fileNumber)
    {
        if (SaveFileList != null)
        {
            foreach (var item in SaveFileList)
            {
                if (item.Value.savefileConfig.fileID == fileNumber)
                {
                    return true;
                }
            }
        }
        return false;
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
        //? ����� ���� �о��
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


    //public void SaveClearData()
    //{
    //    string jsonData = JsonConvert.SerializeObject(CollectionManager.Instance.SaveMultiData());
    //    var result = FileOperation(FileMode.Create, $"{Application.persistentDataPath}/Savefile/ClearData.json", jsonData);
    //}
    //public bool LoadClearData()
    //{
    //    if (SaveFileSearch("ClearData"))
    //    {
    //        var _fileData = FileOperation(FileMode.Open, $"{Application.persistentDataPath}/Savefile/ClearData.json");
    //        var loadData = JsonConvert.DeserializeObject<CollectionManager.RoundData>(_fileData);
    //        CollectionManager.Instance.LoadMultiData(loadData);
    //        return true;
    //    }
    //    else
    //    {
    //        Debug.Log("ClearData Not Exsit");
    //        return false;
    //    }
    //}



    #endregion


    #region PlayerDataJson

    public void SaveFile_PlayerData()
    {
        string jsonData = JsonConvert.SerializeObject(UserData.Instance.CurrentPlayerData);
        var result = FileOperation(FileMode.Create, $"{Application.persistentDataPath}/Savefile/PlayerData.json", jsonData);
    }

    public UserData.PlayerData LoadFile_PlayerData()
    {
        if (SaveFileSearch("PlayerData"))
        {
            var _fileData = FileOperation(FileMode.Open, $"{Application.persistentDataPath}/Savefile/PlayerData.json");
            var loadData = JsonConvert.DeserializeObject<UserData.PlayerData>(_fileData);
            return loadData;
        }
        else
        {
            Debug.Log("PlayerData.json Not Exsit");
            return null;
        }
    }

    public CollectionManager.RoundData LoadFile_LegacyClearData()
    {
        if (SaveFileSearch("ClearData"))
        {
            var _fileData = FileOperation(FileMode.Open, $"{Application.persistentDataPath}/Savefile/ClearData.json");
            var loadData = JsonConvert.DeserializeObject<CollectionManager.RoundData>(_fileData);

            //? �������ְ� ������ ����
            DeleteSaveFile("ClearData");
            return loadData;
        }
        else
        {
            Debug.Log("ClearData.json Not Exsit");
            return null;
        }
    }



    #endregion







    #region FileOperation


    //? dataPath = Asset���� /// persistentDataPath = C:\Users\USER\AppData\LocalLow\SeonghyunKim\DefenseGame_alpha
    //? �ȵ���̵忡�� �� �޶���. �Ͽ�ư ���ٰ����Ѱ�δ� persistentDataPath�� �����
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
        return $"�����߻� : {path}";
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
            Debug.Log(targetFile + " �� �������� �ʽ��ϴ�.");
    }


    #endregion
}