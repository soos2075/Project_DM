using UnityEngine;
using UnityEngine.UI;

public class UI_Return : UI_Base
{
    void Start()
    {
        Init();
    }

    enum Buttons
    {
        Return,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Button>(typeof(Buttons));

        GetButton(((int)Buttons.Return)).gameObject.AddUIEvent((data) => ReturnGameScene());
    }




    void ReturnGameScene()
    {
        Managers.Scene.AddLoadAction_OneTime(() => LoadAutoSave());

        Managers.Scene.LoadSceneAsync("2_Management");
    }


    void LoadAutoSave()
    {
        Main.Instance.Default_Init();

        Managers.Data.LoadToStorage("AutoSave");

        Debug.Log(GuildManager.Instance.myInt);
    }
}
