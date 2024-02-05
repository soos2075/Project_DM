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
    }



    List<UnityAction<Scene, LoadSceneMode>> OneTimeAction;


    AsyncOperation CurrentOperation { get; set; }

    public void LoadSceneAsync(string sceneName)
    {
        CurrentOperation = SceneManager.LoadSceneAsync(sceneName);

        //Managers.Instance.StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    //private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    //{
    //    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
    //    yield return asyncOperation;

    //    // 씬 로드가 완료된 후에 SceneManager.sceneLoaded 이벤트를 수동으로 호출
    //    OnSceneLoaded(SceneManager.GetSceneByName(sceneName), LoadSceneMode.Single);
    //}
    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log("먼저호출 보장되냐?");

    //    // 이곳에 SceneManager.sceneLoaded 이벤트에서 실행하고자 하는 내용을 작성
    //    Debug.Log("Scene loaded: " + scene.name);
    //}


    public void AddLoadAction_Usual(Action action)
    {
        SceneManager.sceneLoaded += (scene, mode) => action.Invoke();
    }
    public void AddLoadAction_OneTime(Action action)
    {
        OneTimeAction.Add((scene, mode) => action.Invoke());
        SceneManager.sceneLoaded += OneTimeAction[OneTimeAction.Count - 1];
    }



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
