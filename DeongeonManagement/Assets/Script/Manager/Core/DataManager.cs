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
        //? Dialogue
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/CsvData/Dialogue_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_DialogueAll(OnCSVLoaded(handle)); };

        //? Object
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/CsvData/Object_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_ObjectAll(OnCSVLoaded(handle)); };

        //? Monster
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/CsvData/Monster_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_Monster(OnCSVLoaded(handle)); };

        //? ���� (����Ʈ�� �̺�Ʈ ǥ��)
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/CsvData/Journal_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_Journal(OnCSVLoaded(handle)); };

        //? Trait - �ѿ��� �ϳ��� ���Ϸ� �Ľ�
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/CsvData/Trait_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_Trait(OnCSVLoaded(handle)); };

        //? Artifact
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/CsvData/Artifact_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_Artifact(OnCSVLoaded(handle)); };

        //? Title
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/CsvData/Title_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_Title(OnCSVLoaded(handle)); };

        //? RandomEvent
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/CsvData/RandomEvent_Result.csv").Completed +=
    (handle) => { CSV_File_Parsing_RandomEvent(OnCSVLoaded(handle)); };

        //? BattleStatue (�����̻�)
        Addressables.LoadAssetAsync<TextAsset>("Assets/Data/CsvData/BattleStatus_Result.csv").Completed +=
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
    int csvFileAll = 9;
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

            case Define.Language.SC:
                Managers.Data.ObjectsLabel_SC.TryGetValue(id, out datas);
                break;

            case Define.Language.TC:
                Managers.Data.ObjectsLabel_TC.TryGetValue(id, out datas);
                break;
        }
        return datas;
    }
    public string[] GetTextData_Monster(int id)
    {
        string[] datas = null;
        switch (UserData.Instance.Language)
        {
            case Define.Language.EN:
                Managers.Data.Monster_EN.TryGetValue(id, out datas);
                break;

            case Define.Language.KR:
                Managers.Data.Monster_KR.TryGetValue(id, out datas);
                break;

            case Define.Language.JP:
                Managers.Data.Monster_JP.TryGetValue(id, out datas);
                break;

            case Define.Language.SC:
                Managers.Data.Monster_SC.TryGetValue(id, out datas);
                break;

            case Define.Language.TC:
                Managers.Data.Monster_TC.TryGetValue(id, out datas);
                break;
        }
        return datas;
    }
    public string[] GetTextData_Journal(int id)
    {
        string[] datas = null;
        switch (UserData.Instance.Language)
        {
            case Define.Language.EN:
                Managers.Data.Journal_EN.TryGetValue(id, out datas);
                break;

            case Define.Language.KR:
                Managers.Data.Journal_KR.TryGetValue(id, out datas);
                break;

            case Define.Language.JP:
                Managers.Data.Journal_JP.TryGetValue(id, out datas);
                break;

            case Define.Language.SC:
                Managers.Data.Journal_SC.TryGetValue(id, out datas);
                break;

            case Define.Language.TC:
                Managers.Data.Journal_TC.TryGetValue(id, out datas);
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

            case Define.Language.SC:
                Managers.Data.Trait_SC.TryGetValue((TraitGroup)id, out datas);
                break;

            case Define.Language.TC:
                Managers.Data.Trait_TC.TryGetValue((TraitGroup)id, out datas);
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

            case Define.Language.SC:
                Managers.Data.Artifact_SC.TryGetValue((ArtifactLabel)id, out datas);
                break;

            case Define.Language.TC:
                Managers.Data.Artifact_TC.TryGetValue((ArtifactLabel)id, out datas);
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

            case Define.Language.SC:
                Managers.Data.Title_SC.TryGetValue(id, out datas);
                break;

            case Define.Language.TC:
                Managers.Data.Title_TC.TryGetValue(id, out datas);
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

            case Define.Language.SC:
                Managers.Data.RandomEvent_SC.TryGetValue(id, out datas);
                break;

            case Define.Language.TC:
                Managers.Data.RandomEvent_TC.TryGetValue(id, out datas);
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

            case Define.Language.SC:
                Managers.Data.BattleStatus_SC.TryGetValue(id, out datas);
                break;

            case Define.Language.TC:
                Managers.Data.BattleStatus_TC.TryGetValue(id, out datas);
                break;
        }
        return datas;
    }


    //? �Ʒ����� ������ �Ľ� @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
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




    public Dictionary<DialogueName, DialogueData> Dialogue_KR { get; } = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_EN { get; } = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_JP { get; } = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_SC { get; } = new Dictionary<DialogueName, DialogueData>();
    public Dictionary<DialogueName, DialogueData> Dialogue_TC { get; } = new Dictionary<DialogueName, DialogueData>();

    //? csv ������ �׸�
    // 0 Type(Bubble/Quest) / 1 ID / 2 KeyName / 3 Index / 4 optionString
    // 5 mainText_KR / 11 Title_KR
    // 6 mainText_EN / 12 Title_EN
    // 7 mainText_JP / 13 Title_JP
    // 8 mainText_SC / 14 Title_SC

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
            var dialogue_SC = new DialogueData(id, type, spl_comma[14]);
            var dialogue_TC = new DialogueData(id, type, spl_comma[15]);

            while (string.IsNullOrEmpty(spl_comma[3]) == false)
            {
                //int index = int.Parse(spl_comma[3]);
                string optionString = spl_comma[4];

                string mainText_KR = ContainsAndJoin(spl_comma[5]);
                string mainText_EN = ContainsAndJoin(spl_comma[6]);
                string mainText_JP = ContainsAndJoin(spl_comma[7]);
                string mainText_SC = ContainsAndJoin(spl_comma[8]);
                string mainText_TC = ContainsAndJoin(spl_comma[9]);

                //var textData = new DialogueData.TextData(optionString, mainText);
                //dialogue.TextDataList.Add(textData);
                //Debug.Log(mainText);

                var textData_KR = new DialogueData.TextData(optionString, mainText_KR);
                var textData_EN = new DialogueData.TextData(optionString, mainText_EN);
                var textData_JP = new DialogueData.TextData(optionString, mainText_JP);
                var textData_SC = new DialogueData.TextData(optionString, mainText_SC);
                var textData_TC = new DialogueData.TextData(optionString, mainText_TC);

                dialogue_KR.TextDataList.Add(textData_KR);
                dialogue_EN.TextDataList.Add(textData_EN);
                dialogue_JP.TextDataList.Add(textData_JP);
                dialogue_SC.TextDataList.Add(textData_SC);
                dialogue_TC.TextDataList.Add(textData_TC);

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
            Dialogue_SC.Add((DialogueName)id, dialogue_SC);
            Dialogue_TC.Add((DialogueName)id, dialogue_TC);
        }
        LoadSucceed();
    }



    public Dictionary<int, string[]> ObjectsLabel_KR = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> ObjectsLabel_EN = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> ObjectsLabel_JP = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> ObjectsLabel_SC = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> ObjectsLabel_TC = new Dictionary<int, string[]>();

    //? option�� monster�� ��µ�, monster �׸��� �и��԰� ���ÿ� option ���� ��������
    // 0 KeyName / 1 id /
    // 2 Label_KR / 3 Detail_KR / X option
    void CSV_File_Parsing_ObjectAll(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1])) //? ID�� ��ĭ�̸� ��������
            {
                continue;
            }

            string[] datas = spl_comma;

            //Debug.Log(datas.Length);
            //foreach (var item in datas)
            //{
            //    Debug.Log(item);
            //}

            ObjectsLabel_KR.Add(int.Parse(datas[1]), new string[] { datas[2], ContainsAndJoin(datas[3]) });
            ObjectsLabel_EN.Add(int.Parse(datas[1]), new string[] { datas[4], ContainsAndJoin(datas[5]) });
            ObjectsLabel_JP.Add(int.Parse(datas[1]), new string[] { datas[6], ContainsAndJoin(datas[7]) });
            ObjectsLabel_SC.Add(int.Parse(datas[1]), new string[] { datas[8], ContainsAndJoin(datas[9]) });
            ObjectsLabel_TC.Add(int.Parse(datas[1]), new string[] { datas[10], ContainsAndJoin(datas[11]) });
        }
        LoadSucceed();
    }


    public Dictionary<int, string[]> Monster_KR = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Monster_EN = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Monster_JP = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Monster_SC = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Monster_TC = new Dictionary<int, string[]>();

    // 0 KeyName / 1 id /
    // 2 Label_KR / 3 Detail_KR / 4 Option_KR / 5 Option2_KR
    void CSV_File_Parsing_Monster(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[1])) //? ID�� ��ĭ�̸� ��������
            {
                continue;
            }

            string[] datas = spl_comma;

            Monster_KR.Add(int.Parse(datas[1]), new string[] { datas[2], ContainsAndJoin(datas[3]), ContainsAndJoin(datas[4]), ContainsAndJoin(datas[5]) });
            Monster_EN.Add(int.Parse(datas[1]), new string[] { datas[6], ContainsAndJoin(datas[7]), ContainsAndJoin(datas[8]), ContainsAndJoin(datas[9]) });
            Monster_JP.Add(int.Parse(datas[1]), new string[] { datas[10], ContainsAndJoin(datas[11]), ContainsAndJoin(datas[12]), ContainsAndJoin(datas[13]) });
            Monster_SC.Add(int.Parse(datas[1]), new string[] { datas[14], ContainsAndJoin(datas[15]), ContainsAndJoin(datas[16]), ContainsAndJoin(datas[17]) });
            Monster_TC.Add(int.Parse(datas[1]), new string[] { datas[18], ContainsAndJoin(datas[19]), ContainsAndJoin(datas[20]), ContainsAndJoin(datas[21]) });
        }
        LoadSucceed();
    }




    





    
    public Dictionary<int, string[]> Journal_KR = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Journal_EN = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Journal_JP = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Journal_SC = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Journal_TC = new Dictionary<int, string[]>();

    //? csv ������ �׸�
    // 0 ID, 1 KeyName,
    // 2 Name_KR, 3 Detail_KR, 
    // 4 Name_EN, 5 Detail_EN, 
    // 6 Name_JP, 7 Detail_JP, 
    // 8 Name_SC, 9 Detail_SC,
    void CSV_File_Parsing_Journal(string _stringData)
    {
        if (string.IsNullOrEmpty(_stringData)) return;

        var spl_n = _stringData.Split('\n');

        for (int i = 1; i < spl_n.Length; i++)
        {
            var spl_comma = spl_n[i].Split(',');

            if (spl_comma.Length < 2 || string.IsNullOrEmpty(spl_comma[0]))
            {
                continue;
            }
            string[] datas = spl_comma;

            Journal_KR.Add(int.Parse(datas[0]), new string[] { datas[2], ContainsAndJoin(datas[3]) });
            Journal_EN.Add(int.Parse(datas[0]), new string[] { datas[4], ContainsAndJoin(datas[5]) });
            Journal_JP.Add(int.Parse(datas[0]), new string[] { datas[6], ContainsAndJoin(datas[7]) });
            Journal_SC.Add(int.Parse(datas[0]), new string[] { datas[8], ContainsAndJoin(datas[9]) });
            Journal_TC.Add(int.Parse(datas[0]), new string[] { datas[10], ContainsAndJoin(datas[11]) });
        }
        LoadSucceed();
    }





    public Dictionary<TraitGroup, string[]> Trait_KR = new Dictionary<TraitGroup, string[]>();
    public Dictionary<TraitGroup, string[]> Trait_EN = new Dictionary<TraitGroup, string[]>();
    public Dictionary<TraitGroup, string[]> Trait_JP = new Dictionary<TraitGroup, string[]>();
    public Dictionary<TraitGroup, string[]> Trait_SC = new Dictionary<TraitGroup, string[]>();
    public Dictionary<TraitGroup, string[]> Trait_TC = new Dictionary<TraitGroup, string[]>();

    //? csv ������ �׸�
    // 0 ID, 1 TraitName,
    // 2 Name_KR, 3 Detail_KR, 4 Acquire_KR,
    // 5 Name_EN, 6 Detail_EN, 7 Acquire_EN,
    // 8 Name_JP, 9 Detail_JP, 10 Acquire_JP,
    // 9 Name_SC, 10 Detail_SC, 11 Acquire_SC,
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
            Trait_SC.Add((TraitGroup)int.Parse(datas[0]), new string[] { datas[11], ContainsAndJoin(datas[12]), datas[13] });
            Trait_TC.Add((TraitGroup)int.Parse(datas[0]), new string[] { datas[14], ContainsAndJoin(datas[15]), datas[16] });
        }
        LoadSucceed();
    }



    //? Artifact
    public Dictionary<ArtifactLabel, string[]> Artifact_KR = new Dictionary<ArtifactLabel, string[]>();
    public Dictionary<ArtifactLabel, string[]> Artifact_EN = new Dictionary<ArtifactLabel, string[]>();
    public Dictionary<ArtifactLabel, string[]> Artifact_JP = new Dictionary<ArtifactLabel, string[]>();
    public Dictionary<ArtifactLabel, string[]> Artifact_SC = new Dictionary<ArtifactLabel, string[]>();
    public Dictionary<ArtifactLabel, string[]> Artifact_TC = new Dictionary<ArtifactLabel, string[]>();

    //? csv ������ �׸� - �̸� / ���� / ȿ��
    // 0 ID, 1 KeyName,
    // 2 Name_KR, 3 Detail_KR, 4 Acquire_KR,
    // 5 Name_EN, 6 Detail_EN, 7 Acquire_EN,
    // 8 Name_JP, 9 Detail_JP, 10 Acquire_JP,
    // 11 Name_SC, 12 Detail_SC, 13 Acquire_SC,
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
            Artifact_SC.Add((ArtifactLabel)int.Parse(datas[0]), new string[] { datas[11], ContainsAndJoin(datas[12]), datas[13] });
            Artifact_TC.Add((ArtifactLabel)int.Parse(datas[0]), new string[] { datas[14], ContainsAndJoin(datas[15]), datas[16] });
        }
        LoadSucceed();
    }

    //? Title
    public Dictionary<int, string[]> Title_KR = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Title_EN = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Title_JP = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Title_SC = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> Title_TC = new Dictionary<int, string[]>();

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
            Title_SC.Add(int.Parse(datas[0]), new string[] { datas[14], ContainsAndJoin(datas[15]), datas[16], datas[17] });
            Title_TC.Add(int.Parse(datas[0]), new string[] { datas[18], ContainsAndJoin(datas[19]), datas[20], datas[21] });
        }
        LoadSucceed();
    }


    //? RandomEvent
    public Dictionary<int, string> RandomEvent_KR = new Dictionary<int, string>();
    public Dictionary<int, string> RandomEvent_EN = new Dictionary<int, string>();
    public Dictionary<int, string> RandomEvent_JP = new Dictionary<int, string>();
    public Dictionary<int, string> RandomEvent_SC = new Dictionary<int, string>();
    public Dictionary<int, string> RandomEvent_TC = new Dictionary<int, string>();
    // 0 ID
    // 1 KR, 2 EN, 3 JP, 4 SC,
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
            RandomEvent_SC.Add(int.Parse(datas[0]), ContainsAndJoin(datas[4]));
            RandomEvent_TC.Add(int.Parse(datas[0]), ContainsAndJoin(datas[5]));
        }
        LoadSucceed();
    }

    //? BattleStatus
    public Dictionary<int, string[]> BattleStatus_KR = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> BattleStatus_EN = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> BattleStatus_JP = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> BattleStatus_SC = new Dictionary<int, string[]>();
    public Dictionary<int, string[]> BattleStatus_TC = new Dictionary<int, string[]>();
    // 0 ID
    // 23 KR, 45 EN, 67 JP, 89 SC,
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

            BattleStatus_KR.Add(int.Parse(datas[0]), new string[] { datas[2], ContainsAndJoin(datas[3]) });
            BattleStatus_EN.Add(int.Parse(datas[0]), new string[] { datas[4], ContainsAndJoin(datas[5]) });
            BattleStatus_JP.Add(int.Parse(datas[0]), new string[] { datas[6], ContainsAndJoin(datas[7]) });
            BattleStatus_SC.Add(int.Parse(datas[0]), new string[] { datas[8], ContainsAndJoin(datas[9]) });
            BattleStatus_TC.Add(int.Parse(datas[0]), new string[] { datas[10], ContainsAndJoin(datas[11]) });
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

        //? ���� ����(����Ʈ) ����
        public List<JournalManager.JournalData> journalList;


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
            Debug.Log($"Exception {e} : Deleting old save file: {fileName}");
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
        saveData.journalList = JournalManager.Instance.Save_JournalData();


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
        JournalManager.Instance.Load_JournalData(loadData.journalList);
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

            if (SaveFileList.ContainsKey(targetFile))
            {
                SaveFileList[targetFile] = null;
            }
            Debug.Log(targetFile + " Delete Complete");
        }
        else
            Debug.Log(targetFile + " �� �������� �ʽ��ϴ�.");
    }


    #endregion
}