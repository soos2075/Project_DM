using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{

    public bool isDemoVersion;

    public bool isEndingTest;


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


    public string webpageURL = "https://store.steampowered.com/app/2886090/Novice_Dungeon_Master/";

    public void OpenWebPageButton()
    {
        Application.OpenURL(webpageURL);
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
}
