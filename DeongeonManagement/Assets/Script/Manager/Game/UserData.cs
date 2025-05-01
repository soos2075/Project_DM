using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class UserData : MonoBehaviour
{
    #region Singleton
    private static UserData _instance;
    public static UserData Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<UserData>();
            if (_instance == null)
            {
                GameObject go = new GameObject { name = "@UserData" };
                _instance = go.AddComponent<UserData>();
            }
            DontDestroyOnLoad(_instance);
        }
    }


    private void Awake()
    {
        Initialize();
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    #endregion

    private void Start()
    {
#if STEAM_BUILD || STEAM_DEMO_BUILD
        Steam_Authorize();
        SteamInit();
#endif

        Application.runInBackground = true;
        //Application.targetFrameRate = -1;
        Application.targetFrameRate = 120;




        Init_PlayerData();

        Init_Resolution();
        Init_Cursor();
        Init_Language();

        SavePlayTime();
        //? �ð��ʱ�ȭ
    }


    #region Steam SDK

    public const uint STEAM_APP_ID = 2886090;
    public const uint STEAM_APP_ID_DEMO = 3241770;

    public uint Current_APP_ID = 480;
    void Steam_Authorize()
    {
#if STEAM_BUILD
        Current_APP_ID = STEAM_APP_ID;
#elif STEAM_DEMO_BUILD
        Current_APP_ID = STEAM_APP_ID_DEMO;
#endif

        try
        {
            // ���� �ʱ�ȭ üũ
            if (!SteamAPI.Init())
            {
                Debug.LogError("Steam �ʱ�ȭ ����!");
                Application.Quit();
                return;
            }

            // ��ID üũ
            if (SteamUtils.GetAppID().m_AppId != Current_APP_ID)
            {
                Debug.LogError("�߸��� AppID!");
                Application.Quit();
                return;
            }

            Debug.Log("Steam ���� ����!");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            Application.Quit();
        }
    }

    void SteamInit()
    {
        try
        {
            if (SteamAPI.Init())
            {
                string name = SteamFriends.GetPersonaName();
                Debug.Log($"Steam �ʱ�ȭ ����! ����� �̸�: {name}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Steam ����: {e.Message}");
            Application.Quit();
        }
    }

    void Steam_Quit() //? �ڵ�Ŭ���嶧�� �̰� ȣ�� ���ϴ°� ������
    {
        SteamAPI.Shutdown();
    }


#endregion







#region Localization
    public Define.Language Language { get; set; } = Define.Language.KR;


    void Init_Language()
    {
        //int lan = GetDataInt(PrefsKey.Language, -1);
        int lan = CurrentPlayerData.option.Language;
        if (lan == -1)
        {
            var country = System.Globalization.CultureInfo.CurrentCulture;


            if (country.Name.Contains("KR"))
            {
                ChangeLanguage(1);
            }
            else if (country.Name.Contains("JP"))
            {
                ChangeLanguage(2);
            }
            else if (country.Name.Contains("zh-"))
            {
                ChangeLanguage(3);
            }
            else
            {
                ChangeLanguage(0);
            }
        }
        else
        {
            ChangeLanguage(lan);
        }
    }


    public void ChangeLanguage(int index)
    {
        Cor_Operation_ChangeLanguage = StartCoroutine(ChangeCor(index));
    }
    public void ChangeLanguage(Define.Language index)
    {
        Cor_Operation_ChangeLanguage = StartCoroutine(ChangeCor((int)index));
    }

    IEnumerator ChangeCor(int index)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        Language = (Define.Language)index;
        CurrentPlayerData.option.Language = (int)Language;
        //SetData(PrefsKey.Language, (int)Language);

        Managers.Dialogue.Init_GetLocalizationData();
        RandomEventManager.Instance.Init_LocalData();
        if (Managers.Scene.GetCurrentScene() == SceneName._2_Management)
        {
            GameManager.Artifact.Init_LocalData();
            GameManager.Content.Init_LocalData();
            GameManager.Facility.Init_LocalData();
            GameManager.Monster.Init_LocalData();
            GameManager.NPC.Init_LocalData();
            GameManager.Technical.Init_LocalData();
            GameManager.Trait.Init_LocalData();
            GameManager.Title.Init_LocalData();
        }

        Cor_Operation_ChangeLanguage = null;

    }

    Coroutine Cor_Operation_ChangeLanguage;


    public string LocaleText(string keyString)
    {
        //Debug.Log(LocalizationSettings.InitializationOperation.Status);
        if (LocalizationSettings.InitializationOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.Log("�۾���");
            return null;
        }

        return LocalizationSettings.StringDatabase.GetLocalizedString("UI Table", keyString, LocalizationSettings.SelectedLocale);
    }

    public string LocaleText_Tooltip(string keyString)
    {
        if (LocalizationSettings.InitializationOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.Log("�۾���");
            return null;
        }

        return LocalizationSettings.StringDatabase.GetLocalizedString("UI_Tooltip", keyString, LocalizationSettings.SelectedLocale);
    }

    public string LocaleText_Label(string keyString)
    {
        if (LocalizationSettings.InitializationOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.Log("�۾���");
            return null;
        }

        return LocalizationSettings.StringDatabase.GetLocalizedString("Label Group", keyString, LocalizationSettings.SelectedLocale);
    }
    public string LocaleText_NGP(string keyString)
    {
        if (LocalizationSettings.InitializationOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.Log("�۾���");
            return null;
        }

        return LocalizationSettings.StringDatabase.GetLocalizedString("NewGamePlus", keyString, LocalizationSettings.SelectedLocale);
    }



    public string GetLocalDateTime(DateTime saveTime)
    {
        CultureInfo cultureInfo;

        switch (Language)
        {
            case Define.Language.EN:
                cultureInfo = new CultureInfo("en-US");
                break;
            case Define.Language.KR:
                cultureInfo = new CultureInfo("ko-KR");
                break;
            case Define.Language.JP:
                cultureInfo = new CultureInfo("ja-JP");
                break;

            default:
                cultureInfo = new CultureInfo("en-US");
                break;
        }

        return saveTime.ToString("F", cultureInfo);
    }
#endregion




#region Cursor

    public Texture2D CursorImage;
    //public Texture2D CursorImage_1280;
    void Init_Cursor()
    {
        Cursor.SetCursor(CursorImage, Vector2.zero, CursorMode.ForceSoftware);

        if (Screen.fullScreen)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //void ChangeCursorSize()
    //{
    //    //if (CurrentResolution == 0)
    //    //{
    //    //    Cursor.SetCursor(CursorImage_1920, Vector2.zero, CursorMode.ForceSoftware);
    //    //}
    //    //else if (CurrentResolution == 1)
    //    //{
    //    //    Cursor.SetCursor(CursorImage_1280, Vector2.zero, CursorMode.ForceSoftware);
    //    //}

    //    float current = (float)Screen.width / 1280f;
    //    Texture2D scaledCursorTexture = ScaleTexture(CursorImage_1920, current * 3);
    //    Cursor.SetCursor(scaledCursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    //}

    //Texture2D ScaleTexture(Texture2D source, float scaleFactor) //? gpt�亯
    //{
    //    int newWidth = Mathf.RoundToInt(source.width * scaleFactor);
    //    int newHeight = Mathf.RoundToInt(source.height * scaleFactor);

    //    Texture2D result = new Texture2D(newWidth, newHeight, source.format, false);
    //    Color[] pixels = source.GetPixels();
    //    Color[] scaledPixels = new Color[newWidth * newHeight];

    //    for (int y = 0; y < newHeight; y++)
    //    {
    //        for (int x = 0; x < newWidth; x++)
    //        {
    //            float u = x / (float)newWidth;
    //            float v = y / (float)newHeight;
    //            int sourceX = Mathf.FloorToInt(u * source.width);
    //            int sourceY = Mathf.FloorToInt(v * source.height);
    //            scaledPixels[y * newWidth + x] = pixels[sourceY * source.width + sourceX];
    //        }
    //    }

    //    result.SetPixels(scaledPixels);
    //    result.Apply();
    //    return result;
    //}


#endregion





#region Resolution

    void Init_Resolution()
    {
        if (FirstSetting())
        {
            return;
        }


        if (CurrentPlayerData.option.Full_Screen == 1)
        {
            screenMode = true;
        }
        else
        {
            screenMode = false;
        }

        current_Index = CurrentPlayerData.option.User_Resolution;

        Screen.SetResolution(resolution[current_Index].x, resolution[current_Index].y, screenMode);
    }

    bool FirstSetting()
    {
        if (CurrentPlayerData.config.NewGameCount == 0)
        {
            Screen.SetResolution(1920, 1080, true);
            current_Index = 0;
            screenMode = true;

            CurrentPlayerData.config.NewGameCount++;
            CurrentPlayerData.option.User_Resolution = current_Index;
            CurrentPlayerData.option.Full_Screen = 1;
            return true;
        }
        else
            return false;


        //if (PlayerPrefs.GetInt(PrefsKey.FirstStart.ToString(), 0) == 0)
        //{
        //    SetData(PrefsKey.FirstStart, 1);
        //    Screen.SetResolution(1920, 1080, true);
        //    current_Index = 0;
        //    screenMode = true;
        //    SetData(PrefsKey.User_Resolution, current_Index);
        //    SetData(PrefsKey.Full_Screen, 1);

        //    return true;
        //}
        //else
        //    return false;
    }


    //? PrefsKey.User_Resolution �� ���� ���� �ػ� // 0 = 1920*1080 (����Ʈ) // ù�� = 16:9 , �ι�°�� = 16:10
    readonly Vector2Int[] resolution = new Vector2Int[] 
    {   new Vector2Int(1920, 1080), new Vector2Int(2560 , 1440 ), new Vector2Int(1600 , 900 ), new Vector2Int(1280, 720),
        new Vector2Int(1920 , 1200 ), new Vector2Int(2560  , 1600  ),new Vector2Int(1680, 1050), new Vector2Int(1440, 900), };

    int current_Index;
    public int CurrentResolution { get { return current_Index; } set { SetResolution(value); } }

    //? FullscreenMode�� 4������ ����. �Ϲ� Ǯ��ũ��, ��üâ���(������������), ��üâ���(����), �Ϲ�â��� ��� ���� �ɵ�.
    //? ����Ѵٸ� 1���� 3���� ����ϴ°� ������.
    bool screenMode;
    public bool FullScreen { get { return screenMode; } set { SetScreenMode(value); } }




    void SetResolution(int _value)
    {
        if (current_Index != _value)
        {
            current_Index = _value;
            Screen.SetResolution(resolution[current_Index].x, resolution[current_Index].y, screenMode);
            CurrentPlayerData.option.User_Resolution = current_Index;
            //SetData(PrefsKey.User_Resolution, current_Index);
            Debug.Log($"Resolution Change - [{resolution[current_Index].x} * {resolution[current_Index].y}]");
            Debug.Log($"ScreenMode - [{screenMode}]");
        }
    }

    void SetScreenMode(bool _value)
    {
        if (screenMode != _value)
        {
            screenMode = _value;
            Screen.SetResolution(resolution[current_Index].x, resolution[current_Index].y, screenMode);

            if (screenMode)
            {
                CurrentPlayerData.option.Full_Screen = 1;
                //SetData(PrefsKey.Full_Screen, 1);
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                CurrentPlayerData.option.Full_Screen = 0;
                //SetData(PrefsKey.Full_Screen, 0);
                Cursor.lockState = CursorLockMode.None;
            }
            
            Debug.Log("��ũ����� ����");
        }
    }

#endregion





#region GameMode
    private Define.GameMode _gameMode;
    public Define.GameMode GameMode
    {
        get
        {
            return _gameMode;
        } 
        set
        {
            _gameMode = value;

            switch (_gameMode)
            {
                case Define.GameMode.Normal:
                    GamePlay();
                    break;
                case Define.GameMode.Stop:
                    GameStop();
                    break;
            }
        }
    }

    public int GameSpeed { get; set; } = 1;

    public int GameSpeed_GuildReturn { get; set; }


    public WaitUntil Wait_GamePlay { get; set; }



    void GameStop()
    {
        Time.timeScale = 1;
        Wait_GamePlay = new WaitUntil(() => GameMode != Define.GameMode.Stop);
    }

    public void GamePlay()
    {
        //Debug.Log("��� �ٽ� Ȱ��ȭ?");
        Time.timeScale = GameSpeed;
        //GameSpeed = speed;
        if (Managers.Scene.GetCurrentScene() == SceneName._2_Management)
        {
            FindAnyObjectByType<UI_Management>()?.SpeedButtonImage();
        }
    }

    public void GamePlay_Normal()
    {
        GameSpeed = 1;
        Time.timeScale = 1;
        //GameMode = Define.GameMode.Normal;
        _gameMode = Define.GameMode.Normal;
    }

    public void GamePlay(int speed)
    {
        GameSpeed = speed;
        Time.timeScale = speed;
        if (Managers.Scene.GetCurrentScene() == SceneName._2_Management)
        {
            FindAnyObjectByType<UI_Management>()?.SpeedButtonImage();
        }
    }


    //bool IsPlaying()
    //{
    //    if (GameMode == Define.GameMode.Stop)
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        return true;
    //    }
    //}


#endregion






#region GameClear
    public DataManager.SaveData CurrentSaveData { get; set; }

    public bool isClear { get; set; }
    public Endings EndingState { get; set; }
    public Save_MonsterData SelectedMonster { get; set; }
    public void GameClear(DataManager.SaveData data)
    {
        //? �������� �� �Լ��� ȣ������ ����. 
        //? �� �Լ��� �� ������ ������ �� �ڿ� ���������� ȣ���(���� ������ �������)

        //todo ���Ŀ� �� �Լ��� ���Ѹ�� or ���������� ���� �� ����
        //todo (Ŭ���� �� ������ ������ �ҷ����� ��, �� �Լ��� ȣ��Ǵµ� ���⼭ �б�� ���Ѹ��� ������ ����� ��)

        Debug.Log("Regular Game Clear");

        isClear = true;
        //CollectionManager.Instance.GameClear(data);

        CurrentPlayerData.GameClear(data);

        //? �÷��� ������ ������Ʈ
        Managers.Data.SaveCollectionData();
    }


    //IEnumerator Init_MultiData(Save_MonsterData[] data)
    //{
    //    var fade = Managers.UI.ShowPopUp<UI_Fade>();
    //    fade.SetFadeOption(UI_Fade.FadeMode.BlackOut, 2, false);

    //    yield return new WaitForSecondsRealtime(2);


    //    SelectedMonster = null;

    //    var monster = Managers.UI.ShowPopUp<UI_Ending_Monster>("Monster/UI_Ending_Monster");
    //    monster.datas = data;


    //    yield return new WaitUntil(() => SelectedMonster != null);

    //    //Debug.Break();
    //    var ClearSaveData = new CollectionManager.RoundData();
    //    ClearSaveData.Init_Count(UserData.Instance.GetDataInt(PrefsKey.ClearTimes, 0) + 1);
    //    ClearSaveData.Init_Bonus("SuperBonus");
    //    ClearSaveData.Init_Monster(SelectedMonster);

    //    CollectionManager.Instance.RoundClearData = ClearSaveData;

    //    Managers.Data.SaveClearData();

    //    yield return null;

    //    Managers.Scene.LoadSceneAsync(SceneName._1_Start);
    //}





#endregion



#region PlayTime Managed

    float currentTime;
    public void SavePlayTime()
    {
        //var saveTime = GetDataFloat(PrefsKey.PlayTime) + (Time.unscaledTime - currentTime);
        ////Debug.Log($"���� �÷��� �ð� : {GetDataFloat(PrefsKey.PlayTime)} + {Time.unscaledTime - currentTime}");
        //SetData(PrefsKey.PlayTime, saveTime);
        //currentTime = Time.unscaledTime;


        var saveTime = CurrentPlayerData.config.PlayTime + (Time.unscaledTime - currentTime);
        //Debug.Log($"���� �÷��� �ð� : {saveTime}");
        CurrentPlayerData.config.PlayTime = saveTime;
        currentTime = Time.unscaledTime;
    }

    private void OnApplicationQuit()
    {
        SavePlayTime();
        Save_PlayerData();

        Debug.Log($"Quit_Save_Success");
    }

#endregion



#region SavefileConfig

    public class SavefileConfig
    {
        //? �����ѹ�(Ŭ���� �����͸� �ߺ��ؼ� �����Ű�� �ʰ� �ϱ� ����. �����ӽ� ���Ӱ� �ο�)
        public int fileID;

        //? ���̵�
        public Define.DifficultyLevel Difficulty;

        // ��ȸ�������� ���� ����
        public int PlayRounds;

        // �̹� ȸ���� �÷��̽ð� - ����
        public float PlayTimes;

        // ������ or ���̺������� �ε����� ���� �ð� = Time.unscaledTime�� ���� - ������ �� ������ ����ð����� �� ���� ���� ����
        float PlayTime_Current;


        // ù ���� �̺�Ʈ Ȯ�ο� Bool
        public bool firstCheck_RandomEvent;

        public bool firstAppear_Herbalist;
        public bool firstAppear_Miner;
        public bool firstAppear_Adventurer;
        public bool firstAppear_Elf;
        public bool firstAppear_Wizard;

        public bool firstAppear_Hunter_Slime;
        public bool firstAppear_Hunter_EarthGolem;

        public bool firstAppear_Catastrophe;
        public bool firstReturn_Catastrophe;

        public bool firstAppear_Heroine;

        //? Config Option - ���� ����ȯ�� �ɼ�
        public bool Placement_Continuous;


        //? ���ο�� �˸�
        public bool Notice_Facility;
        public bool Notice_Monster;
        public bool Notice_Guild;
        public bool Notice_Quest;
        public bool Notice_DungeonEdit;

        //? 2�ܰ� UI �˸�
        public bool Notice_Summon;
        public bool Notice_Ex4;
        public bool Notice_Ex5;


#region Ŭ���� Ư��
        //? ������
        public bool Statue_Mana;
        public bool Statue_Gold;
        public bool Statue_Dog;
        public bool Statue_Cat;
        public bool Statue_Dragon;
        public bool Statue_Ravi;
        public bool Statue_Demon;
        public bool Statue_Hero;


        //? ����ȿ��
        public bool Buff_ApBonusOne;
        public bool Buff_ApBonusTwo;
        public bool Buff_PopBonus;
        public bool Buff_DangerBonus;
        public bool Buff_ManaBonus;
        public bool Buff_GoldBonus;

        //? ����
        public bool Unit_BloodySlime;
        public bool Unit_FlameGolem;
        public bool Unit_Pixie;
        public bool Unit_Salinu;
        public bool Unit_HellHound;
        public bool Unit_Griffin;
        public bool Unit_Lilith;
        public bool Unit_Rena;
        public bool Unit_Ravi;
        public bool Unit_Lievil;
        public bool Unit_Rideer;

        //? ��Ƽ��Ʈ
        public bool Arti_Hero;
        public bool Arti_Decay;
        public bool Arti_Pop;
        public bool Arti_Danger;
        public bool Arti_DownDanger;
        public bool Arti_DownPop;
#endregion

        public void SetBoolValue(string boolName, bool value)
        {
            // �ʵ� ������ ������
            var field = this.GetType().GetField(boolName);

            if (field != null && field.FieldType == typeof(bool))
            {
                field.SetValue(this, value);
            }
            else
            {
                Debug.LogError("Invalid field name or type: " + boolName);
            }
        }

        public SavefileConfig DeepCopy()
        {
            //? �Ʒ� �޼���� �������� �ʵ带 �������� �ϴ� �޼�����. �ٸ� ���� ��� �ʵ尪�� ��Ÿ���̶� �����簡 �ɻ���.
            SavefileConfig newConfig = (SavefileConfig)this.MemberwiseClone();
            return newConfig;
        }

        //? �ϴ� ���� MemberwiseClone �׽�Ʈ���غ���
        //private SavefileConfig CopyFields(SavefileConfig newConfig)
        //{
        //    // ���� �ν��Ͻ��� ��� �ʵ带 �����ͼ� ����
        //    foreach (FieldInfo field in typeof(SavefileConfig).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        //    {
        //        field.SetValue(newConfig, field.GetValue(this));
        //    }
        //    return newConfig;
        //}



        public void PlayTimeApply()
        {
            PlayTimes += (Time.unscaledTime - PlayTime_Current);
            PlayTime_Current = Time.unscaledTime;
        }
        public void Init_CurrentPlayTime()
        {
            PlayTime_Current = Time.unscaledTime;
        }

        public void Apply_ClearInfo()
        {
            //? Ŭ���� ������ ����
            //if (CollectionManager.Instance.RoundClearData != null)
            //{
            //    PlayRounds = CollectionManager.Instance.RoundClearData.clearCounter + 1;
            //}
            //else
            //{
            //    PlayRounds = 1;
            //}

            PlayRounds = UserData.Instance.CurrentPlayerData.GetClearCount() + 1;
        }
    }

    public SavefileConfig FileConfig { get; set; }

    public void NewGameConfig()
    {
        var config = new SavefileConfig();

        var ranID = UnityEngine.Random.Range(0, int.MaxValue);
        while (Managers.Data.Contains_FileID(ranID))
        {
            ranID = UnityEngine.Random.Range(0, int.MaxValue);
        }

        config.fileID = ranID;

        config.Init_CurrentPlayTime();

        //? ���� Ŭ���� �����Ϳ� ���� SavefileConfig ���� �־������
        config.Apply_ClearInfo();

        FileConfig = config;
    }

#endregion




    //public void SetData(PrefsKey _key, float _float)
    //{
    //    PlayerPrefs.SetFloat(_key.ToString(), _float);
    //}
    //public void SetData(PrefsKey _key, int _int)
    //{
    //    PlayerPrefs.SetInt(_key.ToString(), _int);
    //}
    //public void SetData(PrefsKey _key, string _string)
    //{
    //    PlayerPrefs.SetString(_key.ToString(), _string);
    //}


    //public float GetDataFloat(PrefsKey _key, float _defalutValue = 0)
    //{
    //    return PlayerPrefs.GetFloat(_key.ToString(), _defalutValue);
    //}
    //public int GetDataInt(PrefsKey _key, int _defalutValue = 0)
    //{
    //    return PlayerPrefs.GetInt(_key.ToString(), _defalutValue);
    //}
    //public string GetDataString(PrefsKey _key)
    //{
    //    return PlayerPrefs.GetString(_key.ToString());
    //}


    //public void DeleteAllData()
    //{
    //    PlayerPrefs.DeleteAll();
    //}





    #region PlayerData.Json (New)


    public PlayerData CurrentPlayerData { get; set; }

    void Init_PlayerData()
    {
        //? PlayerData.json �ε� / ���� PlayerData.json �� ���ٸ� ���� ����
        var playerdata = Load_PlayerData();
        if (playerdata == null)
        {
            playerdata = new PlayerData();
        }

        //? �Ʒ������� �ٴ��� ������Ʈ���� ����. ���ķδ� ClearData.json������ �����ϱ� (Ȥ�� �׳� ����)
        //? ���� ClearData.Json �� �ִٸ� PlayerData�� �ű�� �� ClearData.json�� ����
        var legacyData = Managers.Data.LoadFile_LegacyClearData();
        if (legacyData != null)
        {
            foreach (var item in legacyData.dataLog)
            {
                PlayerData.ClearDataLog newLog = new PlayerData.ClearDataLog();

                newLog.ID = item.Key;
                newLog.mana = item.Value.mana;
                newLog.gold = item.Value.gold;
                newLog.rank = item.Value.rank;
                newLog.pop = item.Value.pop;
                newLog.danger = item.Value.danger;
                newLog.visit = item.Value.visit;
                newLog.endings = item.Value.endings;
                newLog.clearTime = item.Value.clearTime;
                newLog.difficultyLevel = item.Value.difficultyLevel;

                playerdata.clearLog.Add(newLog);
            }
        }


        //? ������ DataManager���� �޾ƿ� Ŭ������ ����ֱ�
        CurrentPlayerData = playerdata;
        Save_PlayerData();
    }

    //? PlayerData�� ���̺����Ͽ� ���ӵ��� �ʱ⿡ ��� ���� �Ѱ� ������ ����/�ε� �ϸ� �Ǳ��ϴµ�.. ���������� ���� ���Ҷ��� ������.
    //? �ٵ� �װ� ����ڹ����ϱ� �˺���. �ϴ��� Ŭ����ö� ����,���� �� ��Ÿ�ֿ̹��� �ϵ��� ����.

    public void Save_PlayerData()
    {
        Managers.Data.SaveFile_PlayerData();
    }
    public PlayerData Load_PlayerData()
    {
        return Managers.Data.LoadFile_PlayerData();
    }

    public class PlayerData
    {
        /*
        1. ���� ���� (�ΰ��Ӽ���) - ���, �ػ�, ����, �ؽ�Ʈ���ǵ�, ��Ÿ �������� �ִٸ� �߰��ϸ� ��
        2. �ڵ�����Ǵ� ���� - ������ ���̺꽽��, ������Ƚ��, �� ���̺� Ƚ��, �� �ε� Ƚ��, �� �÷��� �ð�, Ʃ�丮��üũ, ù���ӽ��� üũ, 
        3. �������� - �ִ� ����, Ŭ���� Ƚ��, ���̵�, ��ȭ ���� ���
        4. Ŭ������ ������ �α�
         */

        public Option option;
        public Config config;
        public Statistics statistics;


        public PlayerData()
        {
            option = new Option();
            option.Init();

            config = new Config();
            statistics = new Statistics();

            Init_ClearLog();
        }

        public struct Option
        {
            public int Language;
            public int User_Resolution;
            public int Full_Screen;

            public float Volume_BGM;
            public float Volume_Effect;
            public int TextSpeed;

            public int Tutorial_Skip; //? 1�̸� 2ȸ�� �̻󿡼� Ʃ�丮���ȭ�� ��ŵ, 0�̸� ���

            public void Init()
            {
                Language = PlayerPrefs.GetInt("Language", -1);
                User_Resolution = PlayerPrefs.GetInt("User_Resolution", -1);
                Full_Screen = PlayerPrefs.GetInt("Full_Screen", -1);

                Volume_BGM = 0.7f;
                Volume_Effect = 0.7f;
                TextSpeed = 5;

                Tutorial_Skip = 0;
            }
        }

        public struct Config
        {
            public int NewGameCount;
            public int GameOverCount;
            public int SaveCount;
            public int LoadCount;
            public float PlayTime;

            public int LastSaveSlotIndex;

            //? ��Ÿ �����Ҹ��Ѱ� ������ �߰�


        }

        public struct Statistics //? �� Ŭ��� �ƴϿ��� �����ǵ��� �ؾ��ҵ�. �����Ҷ����� ������Ʈ�ϴ°�? �ƹ�ư �̰��� ������ ������ ��ü�����
        {
            public int total_mana;
            public int total_gold;
            public int total_visit;
            public int total_satisfaction;
            public int total_defeat;

            public int total_pop;
            public int total_danger;

            //? �߰��Ҹ��Ѱ� - Main�� Statistics�� �ִ� �׸��? �� õõ�� �߰�����.

            //? �Ϸ簡 ���� �� ȣ���ϱ�. �ٵ� ���̺�Ÿ�ֿ̹� �ϸ� �ȵ�. ���̺� �ݺ��ϸ� ���εǴϱ�. �� ���������� �Ϸ簡 ���������� ȣ���ϱ�.
            public void Update_DayOver(Save_DayResult day)
            {
                total_mana = total_mana +
                    day.Mana_Get_Facility +
                    day.Mana_Get_Artifacts +
                    day.Mana_Get_Monster +
                    day.Mana_Get_Etc +
                    day.Mana_Get_Bonus;

                total_gold = total_gold +
                    day.Gold_Get_Facility +
                    day.Gold_Get_Monster +
                    day.Gold_Get_Technical +
                    day.Gold_Get_Etc +
                    day.Gold_Get_Bonus;

                total_visit += day.NPC_Visit;
                total_satisfaction += day.NPC_Satisfaction;
                total_defeat += day.NPC_Defeat;

                total_pop += day.GetPopularity;
                total_danger += day.GetDanger;
            }
        }



        public void Init_ClearLog()
        {
            clearLog = new List<ClearDataLog>();
        }


        //? �ߺ���� X���� ��� O���� �ٲ���. ���� ��� Ŭ������ �����ʹ� �����.
        //? Ŭ���� Ƚ���� 1���� ���̵�� 1���� ����.
        public List<ClearDataLog> clearLog;

        public struct ClearDataLog
        {
            public int ID;

            public Endings endings;
            public int difficultyLevel;
            public float clearTime;

            public int rank;
            public int pop;
            public int danger;

            public int mana;
            public int gold;

            public int visit;
            public int satisfaction;
            public int defeat;

            public int unit_Size;
            public int unit_Lv_Sum;

            public void Set_Data(DataManager.SaveData data)
            {
                ID = data.savefileConfig.fileID;

                endings = data.endgins;
                difficultyLevel = data.difficultyLevel;
                clearTime = data.playTimes;

                rank = data.mainData.DungeonLV;
                pop = data.mainData.FameOfDungeon;
                danger = data.mainData.DangerOfDungeon;

                mana = data.statistics.Total_Mana;
                gold = data.statistics.Total_Gold;

                visit = data.statistics.Total_Visit;
                satisfaction = data.statistics.Total_Stisfaction;
                defeat = data.statistics.Total_Defeat;

                foreach (var item in data.monsterList)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    unit_Size++;
                    unit_Lv_Sum += item.LV;
                }
            }
        }


        public void Add_ClearLog(DataManager.SaveData data) 
        {
            var newLog = new ClearDataLog();
            newLog.Set_Data(data);
            clearLog.Add(newLog);
        }



        #region �ʿ��� �� ��������
        //? ������ �÷�������Ʈ ���
        public int GetClearPoint()
        {
            int point = 0;

            point += GetClearCount() * 5;
            point += GetHighestDifficultyLevel() * 10;
            point += EndingClearNumber() * 10;

            return point;
        }

        //? Ŭ������ ��������
        public int EndingClearNumber()
        {
            int anotherEnding = 0;
            foreach (Endings item in Enum.GetValues(typeof(Endings)))
            {
                if (EndingClearCheck(item))
                {
                    anotherEnding++;
                }
            }
            return anotherEnding;
        }


        //? ���� Ŭ���� üũ
        public bool EndingClearCheck(Endings ending)
        {
            foreach (var item in clearLog)
            {
                if (item.endings == ending)
                {
                    return true;
                }
            }
            return false;
        }

        //? ������ Ŭ���� Ƚ�� (�ߺ� O)
        public int GetEndingCount(Endings ending)
        {
            int count = 0;
            foreach (var item in clearLog)
            {
                if (item.endings == ending)
                {
                    count++;
                }
            }
            return count;
        }

        //? �����ٹ��� ǥ���� ����
        public ClearDataLog GetDataLog(Endings ending)
        {
            ClearDataLog log = new ClearDataLog();
            log.difficultyLevel = -1;

            foreach (var item in clearLog)
            {
                if (item.endings == ending)
                {
                    if (log.difficultyLevel < item.difficultyLevel)
                    {
                        log = item;
                    }
                }
            }
            return log;
        }


        //? Ŭ���� Ƚ�� (�ߺ� X)
        public int GetClearCount()
        {
            HashSet<int> ID_Count = new HashSet<int>();

            foreach (var item in clearLog)
            {
                ID_Count.Add(item.ID);
            }

            return ID_Count.Count;
        }

        //? �ְ� ���̵� ��������
        public int GetHighestDifficultyLevel()
        {
            int lv = 0;
            foreach (var item in clearLog)
            {
                if (lv < item.difficultyLevel)
                {
                    lv = item.difficultyLevel;
                }
            }
            return lv;
        }

        #endregion



        public void GameClear(DataManager.SaveData data)
        {
            Add_ClearLog(data);

            UserData.Instance.Save_PlayerData();
        }



        //? ��� �������縦 ���ϰ�����. ������ ������ ����־ ���ܰ� ���ٴ� �����ΰű� �ѵ�, ���׳����� ��ġ��.
        public PlayerData DeepCopy()
        {
            PlayerData newData = (PlayerData)this.MemberwiseClone();
            return newData;
        }
    }



    #endregion



}

public enum PrefsKey
{
    //? �ý��� ����
    Language,

    Volume_BGM,
    Volume_Effect,

    LastSaveSlotIndex,

    TextSpeed,

    User_Resolution,
    Full_Screen,


    FirstStart,
    FirstClear,

    High_Scroe,

    High_Turn,

    PlayTime,
    ClearTimes,

    NewGameTimes,
    GameOverTimes,

    SaveTimes,
    LoadTimes,
}

public enum Endings
{
    //? ���� �����Ȱ� ������ �߻�
    Dog = 710,

    //? ���赵�� �� ������ �巡��(��� �� ��Ʈ�� ������Ѿߵǰų� ���谡�� ��ƾ� ���� ������ ��������
    Dragon = 720,

    //? ���赵�� 300���� �۰� �α⵵�� ���赵���� ������
    Ravi = 730,

    //? ������ ����
    Cat = 740,

    //? ���ΰ� ���� ����
    Demon = 750,

    //? ���ΰ� ��� ����
    Hero = 760,
}