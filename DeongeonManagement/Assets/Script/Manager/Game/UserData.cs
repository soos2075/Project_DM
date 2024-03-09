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

        //Managers.Scene.LoadSceneAsync(SceneName._1_Start);
        Managers.Dialogue.Init_GetLocalizationData();
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



    #region GameClear

    public void GameClear()
    {
        //? 1. Main���� CurrentDayList�� �����ͼ� �̰����� ��ġ�� �̴´�.
        //? 2. �����͸� �̰����� Set ���ش�
        //? 3. UI�� ������ ���� �ű���� �ؼ� ���丮 ���� (�̰� ���� 1,2���� �س�����. �׷��� ���Űܵ� ��������)
        //? 4. �ٳ������� �������� ����. �׸��� ������ �������� �� ȸ�������� �������� �̰����� ����� ��.
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
        //? �������� ������ �� ������ ���⼭ �ϸ� ��. �ڷ�ƾ�� ������ �Ǳ��ϴµ� ���ϴ°� ������. �׳� ������ ���ð����ų� ���������ų� ��������.
    }



    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }

}

public enum PrefsKey
{
    Language,

    Volume_BGM,
    Volume_Effect,

    TextSpeed,

    User_Resolution,
    Full_Screen,

    FirstStart,
    FirstClear,

    High_Scroe,
    High_Turn,
    //? ��Ÿ ��ũ�� ����Ȯ��, �Ӵ�, ���, ���� ��� �߰��Ҹ��Ѱ� ������ ������ ���������̶� ������ �ǹ̾���.


    ClearTimes,

    PlayTime,


    Ending_Normal,
    Ending_Hidden,
}