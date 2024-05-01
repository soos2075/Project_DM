using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{

    public bool isDemoVersion;






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

    }


    public string webpageURL = "https://store.steampowered.com/app/2886090/Novice_Dungeon_Master/";

    public void OpenWebPageButton()
    {
        Application.OpenURL(webpageURL);
    }

}
