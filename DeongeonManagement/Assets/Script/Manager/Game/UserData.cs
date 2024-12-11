using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
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
        Application.runInBackground = true;
        //Application.targetFrameRate = -1;
        Init_Resolution();
        Init_Cursor();
        Init_Language();
    }


    #region Localization
    public Define.Language Language { get; set; } = Define.Language.KR;


    void Init_Language()
    {
        int lan = GetDataInt(PrefsKey.Language, -1);
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
        SetData(PrefsKey.Language, (int)Language);

        Managers.Dialogue.Init_GetLocalizationData();
        if (Managers.Scene.GetCurrentScene() == SceneName._2_Management)
        {
            GameManager.Content.Init_LocalData();
            GameManager.Facility.Init_LocalData();
            GameManager.Monster.Init_LocalData();
            GameManager.NPC.Init_LocalData();
            GameManager.Technical.Init_LocalData();
            GameManager.Trait.Init_LocalData();
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


        if (PlayerPrefs.GetInt(PrefsKey.Full_Screen.ToString(), 0) == 1)
        {
            screenMode = true;
        }
        else
        {
            screenMode = false;
        }


        current_Index = PlayerPrefs.GetInt(PrefsKey.User_Resolution.ToString(), 0);


        Screen.SetResolution(resolution[current_Index].x, resolution[current_Index].y, screenMode);
    }

    bool FirstSetting()
    {
        if (PlayerPrefs.GetInt(PrefsKey.FirstStart.ToString(), 0) == 0)
        {
            SetData(PrefsKey.FirstStart, 1);
            Screen.SetResolution(1920, 1080, true);
            current_Index = 0;
            screenMode = true;
            SetData(PrefsKey.User_Resolution, current_Index);
            SetData(PrefsKey.Full_Screen, 1);

            return true;
        }
        else
            return false;
    }


    //? PrefsKey.User_Resolution �� ���� ���� �ػ� // 0 = 1920*1080 // 1 = 1280*720
    readonly Vector2Int[] resolution = new Vector2Int[2] { new Vector2Int(1920, 1080), new Vector2Int(1280, 720) };

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
            SetData(PrefsKey.User_Resolution, current_Index);
            Debug.Log("�ػ� ����");
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
                SetData(PrefsKey.Full_Screen, 1);
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                SetData(PrefsKey.Full_Screen, 0);
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
        CollectionManager.Instance.GameClear(data);
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
        var saveTime = GetDataFloat(PrefsKey.PlayTime) + (Time.unscaledTime - currentTime);
        //Debug.Log($"���� �÷��� �ð� : {GetDataFloat(PrefsKey.PlayTime)} + {Time.unscaledTime - currentTime}");
        SetData(PrefsKey.PlayTime, saveTime);
        currentTime = Time.unscaledTime;
    }

    private void OnApplicationQuit()
    {
        //? �������� ������ �� ������ ���⼭ �ϸ� ��. �ڷ�ƾ�� ������ �Ǳ��ϴµ� ���ϴ°� ������. �׳� ������ ���ð����ų� ���������ų� ��������.
        SavePlayTime();
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
        public bool Unit_Salinu;
        public bool Unit_HellHound;
        public bool Unit_Griffin;
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
            if (CollectionManager.Instance.RoundClearData != null)
            {
                PlayRounds = CollectionManager.Instance.RoundClearData.clearCounter + 1;
            }
            else
            {
                PlayRounds = 1;
            }
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




    public void SetData(PrefsKey _key, float _float)
    {
        PlayerPrefs.SetFloat(_key.ToString(), _float);
    }
    public void SetData(PrefsKey _key, int _int)
    {
        PlayerPrefs.SetInt(_key.ToString(), _int);
    }
    public void SetData(PrefsKey _key, string _string)
    {
        PlayerPrefs.SetString(_key.ToString(), _string);
    }


    public float GetDataFloat(PrefsKey _key, float _defalutValue = 0)
    {
        return PlayerPrefs.GetFloat(_key.ToString(), _defalutValue);
    }
    public int GetDataInt(PrefsKey _key, int _defalutValue = 0)
    {
        return PlayerPrefs.GetInt(_key.ToString(), _defalutValue);
    }
    public string GetDataString(PrefsKey _key)
    {
        return PlayerPrefs.GetString(_key.ToString());
    }








    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }

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

    // �ִ� ������ ��(Ŭ������ 30, �پ������ ����)
    High_Turn,
    //? ��Ÿ ��ũ�� ����Ȯ��, �Ӵ�, ���, ���� ��� �߰��Ҹ��Ѱ� ������ ������ ���������̶� ������ �ǹ̾���.


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