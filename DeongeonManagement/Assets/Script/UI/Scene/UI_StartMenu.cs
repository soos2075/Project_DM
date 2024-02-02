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

        Debug.Log("�Ϸ�");
        //? �ٵ� ���⼭ ����״� �ȳ���. �ֳĸ� �Ϸᰡ ���ڸ��� �ٷ� ���� �Ѿ�����⶧����.. �ƴϸ� ����׵� ��������������. �� �𸣰ڴ� 
        //? �׷��� �Ƹ� ���������� �� �����ϰ��� �������� ������ �Ѿ��������?
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"�� ���� ����");
        Main.Instance.NewGame_Init();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    void LoadGame()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_SaveLoad>();
        ui.SetMode(UI_SaveLoad.Buttons.Load);
    }

}
