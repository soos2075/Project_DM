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
        AddLoadAction_Usual(() => LoadActionOver());
        AddLoadAction_Usual(() => Managers.UI.SceneChange());
        AddLoadAction_Usual(() => FadeIn());

        //loadAction = new List<Action>();
        //SceneManager.sceneLoaded += CustomSceneLoadAction;
    }

    List<UnityAction<Scene, LoadSceneMode>> OneTimeAction;
    AsyncOperation CurrentOperation { get; set; }

    public void LoadSceneAsync(string sceneName, bool _fade = true)
    {
        CurrentOperation = SceneManager.LoadSceneAsync(sceneName);

        if (_fade)
        {
            Managers.Instance.StartCoroutine(FadeOut());
        }
    }
    IEnumerator FadeOut()
    {
        CurrentOperation.allowSceneActivation = false;

        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.Out, 2);
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => fade.isFade == false);

        CurrentOperation.allowSceneActivation = true;
    }
    void FadeIn()
    {
        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.In, 2);
    }


    public void AddLoadAction_Usual(Action action)
    {
        SceneManager.sceneLoaded += (scene, mode) => action.Invoke();
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

    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }


}
