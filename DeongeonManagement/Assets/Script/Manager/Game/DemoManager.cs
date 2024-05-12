using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
#if DEMO_BUILD

    #region Singleton
    private static DemoManager _instance;
    public static DemoManager Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<DemoManager>();
            if (_instance == null)
            {
                GameObject go = new GameObject { name = "@DemoManager" };
                _instance = go.AddComponent<DemoManager>();
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

#endif


    public bool isDemoVersion;

    public bool isEndingTest;

    public bool isManagementTest;


    void Start()
    {

#if DEMO_BUILD
        // 데모 빌드 전용 코드
        Debug.Log("This is demo build.");
        isDemoVersion = true;
#else
        // 일반 빌드 전용 코드
        Debug.Log("This is regular build.");
        isDemoVersion = false;
#endif


#if DEMO_BUILD

#endif


        StartCoroutine(EndingTestCor());
    }




    IEnumerator EndingTestCor()
    {
        yield return null;

        if (Managers.UI._popupStack.Count == 0 && isEndingTest)
        {
            TempEndingTest();
        }
    }


    [System.Obsolete]
    void TempEndingTest()
    {
        //Managers.UI.ShowPopUp<UI_Ending>();
        //return;

        UserData.Instance.EndingState = Endings.Dog;
        Managers.Scene.LoadSceneAsync(SceneName._7_NewEnding, false);
    }

#if CHEAT_BUILD
    private void Update()
    {
        SetManagementValue();
    }


    public GameObject UI_Cheat;

    public bool ui_Off;

    Canvas[] current_Canvas;

    void SetManagementValue()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (UI_Cheat == null)
            {
                UI_Cheat = transform.GetChild(0).gameObject;
            }

            if (UI_Cheat.activeInHierarchy)
            {
                UI_Cheat.SetActive(false);
            }
            else
            {
                UI_Cheat.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (ui_Off)
            {
                ui_Off = false;
                if (current_Canvas == null) return;


                foreach (var item in current_Canvas)
                {
                    item.enabled = true;
                }
            }
            else
            {
                ui_Off = true;
                current_Canvas = GameObject.FindObjectsOfType<Canvas>();
                foreach (var item in current_Canvas)
                {
                    item.enabled = false;
                }
            }
        }
    }
#endif


    public void DemoClearData(CollectionManager.ClearDataLog datalog)
    {
        var ClearSaveData = new CollectionManager.RoundData();
        ClearSaveData.dataLog = datalog;

        CollectionManager.Instance.RoundClearData = ClearSaveData;

        Managers.Data.SaveClearData();
    }

}
