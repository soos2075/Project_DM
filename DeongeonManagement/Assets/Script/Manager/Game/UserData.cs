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
        //? 시간초기화
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
            // 스팀 초기화 체크
            if (!SteamAPI.Init())
            {
                Debug.LogError("Steam 초기화 실패!");
                Application.Quit();
                return;
            }

            // 앱ID 체크
            if (SteamUtils.GetAppID().m_AppId != Current_APP_ID)
            {
                Debug.LogError("잘못된 AppID!");
                Application.Quit();
                return;
            }

            Debug.Log("Steam 연동 성공!");
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
                Debug.Log($"Steam 초기화 성공! 사용자 이름: {name}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Steam 에러: {e.Message}");
            Application.Quit();
        }
    }

    void Steam_Quit() //? 자동클라우드때메 이거 호출 안하는게 나을듯
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
            Debug.Log("작업중");
            return null;
        }

        return LocalizationSettings.StringDatabase.GetLocalizedString("UI Table", keyString, LocalizationSettings.SelectedLocale);
    }

    public string LocaleText_Tooltip(string keyString)
    {
        if (LocalizationSettings.InitializationOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.Log("작업중");
            return null;
        }

        return LocalizationSettings.StringDatabase.GetLocalizedString("UI_Tooltip", keyString, LocalizationSettings.SelectedLocale);
    }

    public string LocaleText_Label(string keyString)
    {
        if (LocalizationSettings.InitializationOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.Log("작업중");
            return null;
        }

        return LocalizationSettings.StringDatabase.GetLocalizedString("Label Group", keyString, LocalizationSettings.SelectedLocale);
    }
    public string LocaleText_NGP(string keyString)
    {
        if (LocalizationSettings.InitializationOperation.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.Log("작업중");
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

    //Texture2D ScaleTexture(Texture2D source, float scaleFactor) //? gpt답변
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


    //? PrefsKey.User_Resolution 의 값에 따른 해상도 // 0 = 1920*1080 (디폴트) // 첫줄 = 16:9 , 두번째줄 = 16:10
    readonly Vector2Int[] resolution = new Vector2Int[] 
    {   new Vector2Int(1920, 1080), new Vector2Int(2560 , 1440 ), new Vector2Int(1600 , 900 ), new Vector2Int(1280, 720),
        new Vector2Int(1920 , 1200 ), new Vector2Int(2560  , 1600  ),new Vector2Int(1680, 1050), new Vector2Int(1440, 900), };

    int current_Index;
    public int CurrentResolution { get { return current_Index; } set { SetResolution(value); } }

    //? FullscreenMode는 4가지가 있음. 일반 풀스크린, 전체창모드(임의조정가능), 전체창모드(고정), 일반창모드 라고 보면 될듯.
    //? 사용한다면 1번과 3번을 사용하는게 맞을듯.
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
            
            Debug.Log("스크린모드 변경");
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
        //Debug.Log("어디서 다시 활성화?");
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
        //? 데모판은 이 함수를 호출하지 않음. 
        //? 이 함수는 각 엔딩이 끝나고 난 뒤에 공통적으로 호출됨(엔딩 종류와 관계없음)

        //todo 추후에 이 함수는 무한모드 or 업적작으로 사용될 수 있음
        //todo (클리어 후 저장한 파일을 불러왔을 때, 이 함수가 호출되는데 여기서 분기로 무한모드로 빠지게 만들면 됨)

        Debug.Log("Regular Game Clear");

        isClear = true;
        //CollectionManager.Instance.GameClear(data);

        CurrentPlayerData.GameClear(data);

        //? 컬렉션 데이터 업데이트
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
        ////Debug.Log($"누적 플레이 시간 : {GetDataFloat(PrefsKey.PlayTime)} + {Time.unscaledTime - currentTime}");
        //SetData(PrefsKey.PlayTime, saveTime);
        //currentTime = Time.unscaledTime;


        var saveTime = CurrentPlayerData.config.PlayTime + (Time.unscaledTime - currentTime);
        //Debug.Log($"누적 플레이 시간 : {saveTime}");
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
        //? 고유넘버(클리어 데이터를 중복해서 적용시키지 않게 하기 위함. 뉴게임시 새롭게 부여)
        public int fileID;

        //? 난이도
        public Define.DifficultyLevel Difficulty;

        // 몇회차인지에 대한 정보
        public int PlayRounds;

        // 이번 회차의 플레이시간 - 누적
        public float PlayTimes;

        // 새시작 or 세이브파일을 로드했을 때의 시간 = Time.unscaledTime을 받음 - 저장할 때 가져온 현재시간에서 이 값을 빼기 위함
        float PlayTime_Current;


        // 첫 등장 이벤트 확인용 Bool
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

        //? Config Option - 각종 개인환경 옵션
        public bool Placement_Continuous;


        //? 새로운거 알림
        public bool Notice_Facility;
        public bool Notice_Monster;
        public bool Notice_Guild;
        public bool Notice_Quest;
        public bool Notice_DungeonEdit;

        //? 2단계 UI 알림
        public bool Notice_Summon;
        public bool Notice_Ex4;
        public bool Notice_Ex5;


#region 클리어 특전
        //? 조각상
        public bool Statue_Mana;
        public bool Statue_Gold;
        public bool Statue_Dog;
        public bool Statue_Cat;
        public bool Statue_Dragon;
        public bool Statue_Ravi;
        public bool Statue_Demon;
        public bool Statue_Hero;


        //? 고유효과
        public bool Buff_ApBonusOne;
        public bool Buff_ApBonusTwo;
        public bool Buff_PopBonus;
        public bool Buff_DangerBonus;
        public bool Buff_ManaBonus;
        public bool Buff_GoldBonus;

        //? 유닛
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

        //? 아티팩트
        public bool Arti_Hero;
        public bool Arti_Decay;
        public bool Arti_Pop;
        public bool Arti_Danger;
        public bool Arti_DownDanger;
        public bool Arti_DownPop;
#endregion

        public void SetBoolValue(string boolName, bool value)
        {
            // 필드 정보를 가져옴
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
            //? 아래 메서드는 어디까지나 필드를 얕은복사 하는 메서드임. 다만 현재 모든 필드값이 값타입이라 값복사가 될뿐임.
            SavefileConfig newConfig = (SavefileConfig)this.MemberwiseClone();
            return newConfig;
        }

        //? 일단 위에 MemberwiseClone 테스트좀해보고
        //private SavefileConfig CopyFields(SavefileConfig newConfig)
        //{
        //    // 현재 인스턴스의 모든 필드를 가져와서 복사
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
            //? 클리어 데이터 관련
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

        //? 현재 클리어 데이터에 따라서 SavefileConfig 값을 넣어줘야함
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
        //? PlayerData.json 로드 / 만약 PlayerData.json 이 없다면 새로 생성
        var playerdata = Load_PlayerData();
        if (playerdata == null)
        {
            playerdata = new PlayerData();
        }

        //? 아래구간은 다다음 업데이트에서 삭제. 이후로는 ClearData.json파일은 무시하기 (혹은 그냥 삭제)
        //? 만약 ClearData.Json 이 있다면 PlayerData로 옮기기 후 ClearData.json은 삭제
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


        //? 있으면 DataManager에서 받아온 클래스를 들고있기
        CurrentPlayerData = playerdata;
        Save_PlayerData();
    }

    //? PlayerData는 세이브파일에 종속되지 않기에 사실 게임 켜고 끌때만 저장/로드 하면 되긴하는데.. 정상적으로 종료 안할때가 문제임.
    //? 근데 그건 사용자문제니까 알빠노. 일단은 클리어시랑 시작,종료 이 세타이밍에만 하도록 하자.

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
        1. 유저 세팅 (인게임설정) - 언어, 해상도, 볼륨, 텍스트스피드, 기타 설정값이 있다면 추가하면 됨
        2. 자동저장되는 값들 - 마지막 세이브슬롯, 새게임횟수, 총 세이브 횟수, 총 로드 횟수, 총 플레이 시간, 튜토리얼체크, 첫게임시작 체크, 
        3. 게임정보 - 최대 일차, 클리어 횟수, 난이도, 재화 총합 등등
        4. 클리어한 데이터 로그
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

            public int Tutorial_Skip; //? 1이면 2회차 이상에서 튜토리얼대화는 스킵, 0이면 재생

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

            //? 기타 저장할만한게 있으면 추가


        }

        public struct Statistics //? 꼭 클리어가 아니여도 누적되도록 해야할듯. 저장할때마다 업데이트하는게? 아무튼 이거의 목적은 업적용 전체통계임
        {
            public int total_mana;
            public int total_gold;
            public int total_visit;
            public int total_satisfaction;
            public int total_defeat;

            public int total_pop;
            public int total_danger;

            //? 추가할만한거 - Main의 Statistics에 있는 항목들? 뭐 천천히 추가하자.

            //? 하루가 지날 때 호출하기. 근데 세이브타이밍에 하면 안됨. 세이브 반복하면 펌핑되니까. 걍 정상적으로 하루가 지났을때만 호출하기.
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


        //? 중복허용 X에서 허용 O으로 바꿨음. 이제 모든 클리어한 데이터는 저장됨.
        //? 클리어 횟수는 1개의 아이디당 1번만 적용.
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



        #region 필요한 값 가져오기
        //? 뉴겜플 플러스포인트 계산
        public int GetClearPoint()
        {
            int point = 0;

            point += GetClearCount() * 5;
            point += GetHighestDifficultyLevel() * 10;
            point += EndingClearNumber() * 10;

            return point;
        }

        //? 클리어한 엔딩종류
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


        //? 엔딩 클리어 체크
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

        //? 엔딩별 클리어 횟수 (중복 O)
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

        //? 엔딩앨범에 표시할 정보
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


        //? 클리어 횟수 (중복 X)
        public int GetClearCount()
        {
            HashSet<int> ID_Count = new HashSet<int>();

            foreach (var item in clearLog)
            {
                ID_Count.Add(item.ID);
            }

            return ID_Count.Count;
        }

        //? 최고 난이도 가져오기
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



        //? 얘는 깊은복사를 안하고있음. 오히려 참조로 들고있어서 나쁠게 없다는 생각인거긴 한데, 버그나오면 고치자.
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
    //? 시스템 관련
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
    //? 딱히 결정된게 없으면 발생
    Dog = 710,

    //? 위험도가 더 높으면 드래곤(사실 이 루트는 몰살시켜야되거나 모험가를 잡아야 거의 가능한 정도긴함
    Dragon = 720,

    //? 위험도가 300보다 작고 인기도가 위험도보다 높을때
    Ravi = 730,

    //? 히로인 엔딩
    Cat = 740,

    //? 주인공 마왕 엔딩
    Demon = 750,

    //? 주인공 용사 엔딩
    Hero = 760,
}