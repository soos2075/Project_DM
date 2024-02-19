using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public void Init()
    {
        OneTimeAction = new List<UnityAction<Scene, LoadSceneMode>>();
        AddLoadAction_Usual((scene, mode) => LoadActionOver());
        AddLoadAction_Usual((scene, mode) => Managers.UI.SceneChange());
        AddLoadAction_Usual((scene, mode) => FadeIn());
        AddLoadAction_Usual((scene, mode) => BGM_Change(GetSceneEnum(scene.name)));

        //loadAction = new List<Action>();
        //SceneManager.sceneLoaded += CustomSceneLoadAction;
    }

    List<UnityAction<Scene, LoadSceneMode>> OneTimeAction;
    AsyncOperation CurrentOperation { get; set; }

    public void LoadSceneAsync(SceneName _sceneName, bool _fade = true)
    {
        CurrentOperation = SceneManager.LoadSceneAsync(_sceneName.ToString());

        if (_fade)
        {
            Managers.Instance.StartCoroutine(FadeOut());
        }
    }
    IEnumerator FadeOut()
    {
        CurrentOperation.allowSceneActivation = false;

        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackOut, 2);
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => fade.isFade == false);

        CurrentOperation.allowSceneActivation = true;
    }
    void FadeIn()
    {
        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 2);
    }


    void BGM_Change(SceneName _scene)
    {
        switch (_scene)
        {
            case SceneName._1_Start:
                SoundManager.Instance.PlaySound("BGM/StartScene", Define.AudioType.BGM);
                break;

            case SceneName._2_Management:
                SoundManager.Instance.PlaySound("BGM/MainThema", Define.AudioType.BGM);
                break;

            case SceneName._3_Guild:
                SoundManager.Instance.PlaySound("BGM/Guild", Define.AudioType.BGM);
                break;

            case SceneName._4_Direction:
                SoundManager.Instance.PlaySound("BGM/Opening", Define.AudioType.BGM);
                break;

            case SceneName._5_Ending:
                SoundManager.Instance.StopMusic();
                break;

            default:
                break;
        }
    }




    public void AddLoadAction_Usual(Action<Scene, LoadSceneMode> action)
    {
        SceneManager.sceneLoaded += (scene, mode) => action.Invoke(scene, mode);
    }
    public void AddLoadAction_OneTime(Action action)
    {
        OneTimeAction.Add((scene, mode) => action.Invoke());
        SceneManager.sceneLoaded += OneTimeAction[OneTimeAction.Count - 1];
    }


    //? 위에거로도 되긴하는데 순서를 확실하게 알고싶거나 조정하고싶다면 아래꺼 쓰면 됨.
    //List<Action> loadAction;
    //public void AddLoadAction_Once(Action action)
    //{
    //    loadAction.Add(action);
    //}
    //void CustomSceneLoadAction(Scene _scene, LoadSceneMode _mode)
    //{
    //    for (int i = 0; i < loadAction.Count; i++)
    //    {
    //        loadAction[i].Invoke();
    //    }

    //    loadAction.Clear();
    //}

    void LoadActionOver() //? 얘가 sceneLoaded의 첫번째 함수로서 실행이 되서 sceneLoaded의 체인이 0이되어도 일단 이미 명령이 들어가서 다 실행은 함.
    {
        for (int i = 0; i < OneTimeAction.Count; i++)
        {
            //Debug.Log($"{i}번째 액션 삭제");
            SceneManager.sceneLoaded -= OneTimeAction[i];
        }

        OneTimeAction.Clear();
    }




    public SceneName GetCurrentScene()
    {
        string sName = SceneManager.GetActiveScene().name;
        //Debug.Log(sName);
        switch (sName)
        {
            case "_1_Start":
                return SceneName._1_Start;

            case "_2_Management":
                return SceneName._2_Management;

            case "_3_Guild":
                return SceneName._3_Guild;

            case "_4_Direction":
                return SceneName._4_Direction;

            case "_5_Ending":
                return SceneName._5_Ending;

            default:
                return SceneName._1_Start;
        }
    }

    SceneName GetSceneEnum(string _name)
    {
        switch (_name)
        {
            case "_1_Start":
                return SceneName._1_Start;

            case "_2_Management":
                return SceneName._2_Management;

            case "_3_Guild":
                return SceneName._3_Guild;

            case "_4_Direction":
                return SceneName._4_Direction;

            case "_5_Ending":
                return SceneName._5_Ending;

            default:
                return SceneName._1_Start;
        }
    }

}

public enum SceneName
{
    _1_Start = 1,
    _2_Management = 2,
    _3_Guild = 3,
    _4_Direction = 4,
    _5_Ending = 5,
}
