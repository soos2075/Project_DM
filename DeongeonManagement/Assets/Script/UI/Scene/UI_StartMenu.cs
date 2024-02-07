using UnityEngine;
using UnityEngine.UI;

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
        Managers.Scene.AddLoadAction_OneTime(() => NewGame_Action());

        Managers.Scene.LoadSceneAsync("2_Management");
    }


    void NewGame_Action()
    {
        Debug.Log($"새 게임 시작");
        Main.Instance.NewGame_Init();
    }



    void LoadGame()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_SaveLoad>();

        //var original = Resources.Load<GameObject>("Prefabs/UI/PopUp/UI_SaveLoad");

        //var ui = Instantiate(original).GetComponent<UI_SaveLoad>();



        //var ui = Managers.UI.ShowPopUpAlone<UI_SaveLoad>();



        ui.SetMode(UI_SaveLoad.Buttons.Load);
    }

}
