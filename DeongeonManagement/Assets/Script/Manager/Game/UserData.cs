using System.Collections;
using System.Collections.Generic;
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
        StartCoroutine(ChangeCor(index));
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
        }
    }




    public string GetLocaleText(string keyString)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("UI Table", keyString, LocalizationSettings.SelectedLocale);
    }


    #endregion




    #region Cursor

    public Texture2D CursorImage;
    void Init_Cursor()
    {
        Cursor.SetCursor(CursorImage, Vector2.zero, CursorMode.Auto);


        if (Screen.fullScreen)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }



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


    //? PrefsKey.User_Resolution 의 값에 따른 해상도 // 0 = 1920*1080 // 1 = 1280*720
    readonly Vector2Int[] resolution = new Vector2Int[2] { new Vector2Int(1920, 1080), new Vector2Int(1280, 720) };

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
            SetData(PrefsKey.User_Resolution, current_Index);
            Debug.Log("해상도 변경");
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


    public WaitUntil Wait_GamePlay;

    void GameStop()
    {
        // 연출중인 캐릭터들을 제외한 모든 npc, monster의 움직임 제어
        // 돌고있는 모든 코루틴 일시정지
        // 카메라 연출
        // 버튼 입출력 제어(이건 캔버스를 사라지게 만들어서 괜찮을듯?)

        Time.timeScale = 1;
        Wait_GamePlay = new WaitUntil(() => GameMode != Define.GameMode.Stop);
    }

    public void GamePlay()
    {
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

    public bool isClear;
    public Endings EndingState { get; set; }
    public Save_MonsterData SelectedMonster { get; set; }
    public void GameClear(DataManager.SaveData data = null)
    {
        //? 데모판은 이 함수를 호출하지 않음. 
        //? 이 함수는 각 엔딩이 끝나고 난 뒤에 공통적으로 호출됨(엔딩 종류와 관계없음)

        //todo 추후에 이 함수는 무한모드 or 업적작으로 사용될 수 있음
        //todo (클리어 후 저장한 파일을 불러왔을 때, 이 함수가 호출되는데 여기서 분기로 무한모드로 빠지게 만들면 됨)

        Debug.Log("Regular Game Clear");


        Save_MonsterData[] monsterDAta;
        if (data != null)
        {
            monsterDAta = data.monsterList;
        }
        else
        {
            monsterDAta = CurrentSaveData.monsterList;
        }

        StartCoroutine(Init_MultiData(monsterDAta));
    }

    IEnumerator Init_MultiData(Save_MonsterData[] data)
    {
        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackOut, 2, false);

        yield return new WaitForSecondsRealtime(2);


        SelectedMonster = null;

        var monster = Managers.UI.ShowPopUp<UI_Ending_Monster>("Monster/UI_Ending_Monster");
        monster.datas = data;


        yield return new WaitUntil(() => SelectedMonster != null);

        //Debug.Break();
        var ClearSaveData = new CollectionManager.MultiplayData();
        ClearSaveData.Init_Count(UserData.Instance.GetDataInt(PrefsKey.ClearTimes, 0) + 1);
        ClearSaveData.Init_Bonus("SuperBonus");
        ClearSaveData.Init_Monster(SelectedMonster);

        CollectionManager.Instance.PlayData = ClearSaveData;

        Managers.Data.SaveClearData();

        yield return null;

        Managers.Scene.LoadSceneAsync(SceneName._1_Start);
    }

    #endregion



    



    #region SavefileConfig

    public class SavefileConfig
    {
        // 몇회차인지에 대한 정보
        public int PlayTimes;

        // 어떤 엔딩을 봤는지에 대한 정보(클리어 특전 = 조각상 인터렉션)
        public bool Statue_Dog;
        public bool Statue_Dragon;


        // 첫 등장 이벤트 확인용 Bool
        public bool firstAppear_Herbalist;
        public bool firstAppear_Miner;
        public bool firstAppear_Adventurer;
        public bool firstAppear_Elf;
        public bool firstAppear_Wizard;

        public bool firstAppear_Hunter_Slime;
        public bool firstAppear_Hunter_EarthGolem;


        public SavefileConfig Clone()
        {
            SavefileConfig newConfig = new SavefileConfig();

            newConfig.PlayTimes = PlayTimes;

            newConfig.firstAppear_Herbalist = firstAppear_Herbalist;
            newConfig.firstAppear_Miner = firstAppear_Miner;
            newConfig.firstAppear_Adventurer = firstAppear_Adventurer;
            newConfig.firstAppear_Elf = firstAppear_Elf;
            newConfig.firstAppear_Wizard = firstAppear_Wizard;

            newConfig.firstAppear_Hunter_Slime = firstAppear_Hunter_Slime;
            newConfig.firstAppear_Hunter_EarthGolem = firstAppear_Hunter_EarthGolem;

            return newConfig;
        }

    }

    public SavefileConfig FileConfig { get; set; }


    public void NewGameConfig()
    {
        var config = new SavefileConfig();

        if (GetDataInt(PrefsKey.Clear_Dog) == 1)
        {
            config.Statue_Dog = true;
        }
        if (GetDataInt(PrefsKey.Clear_Dragon) == 1)
        {
            config.Statue_Dragon = true;
        }
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




    private void OnApplicationQuit()
    {
        //? 종료전에 저장할 거 있으면 여기서 하면 됨. 코루틴을 돌려도 되긴하는데 안하는게 나은듯. 그냥 윈도우 세팅같은거나 볼륨같은거나 저장하자.
    }



    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }

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
    //? 기타 랭크나 던전확장, 머니, 골드, 몬스터 등등 추가할만한건 많은데 어차피 업적관련이라 지금은 의미없음.


    ClearTimes,

    PlayTime,


    Clear_Dog,
    Clear_Dragon,
    //Clear_Slime,
}

public enum Endings
{
    // 딱히 결정된게 없으면 노말 혹은 배드
    Dog,
    Dragon,
    Slime,
}