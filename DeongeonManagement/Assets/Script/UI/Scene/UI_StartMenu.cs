using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_StartMenu : UI_Scene
{
    void Start()
    {
        Init();
    }



    enum Buttons
    {
        NewGame,
        Load,
    }



    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));


        GetButton(((int)Buttons.NewGame)).gameObject.AddUIEvent(data => NewGame());

        GetButton(((int)Buttons.Load)).gameObject.AddUIEvent(data => LoadGame());
    }




    void NewGame()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        var sce = SceneManager.LoadSceneAsync("2_Management");
    }

    IEnumerator Wait(AsyncOperation async)
    {
        yield return new WaitUntil(() => async.isDone == true);

        Debug.Log("완료");
        //? 근데 여기서 디버그는 안나옴. 왜냐면 완료가 뜨자마자 바로 씬이 넘어가버리기때문에.. 아니면 디버그도 안찍힐수도있음. 잘 모르겠다 
        //? 그래도 아마 구문까지는 다 실행하고나서 프레임이 끝나고 넘어가지않을까?
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"새 게임 시작");
        Main.Instance.NewGame_Init();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    void LoadGame()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_SaveLoad>();
        ui.SetMode(UI_SaveLoad.Buttons.Load);
    }

}
