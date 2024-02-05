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

    //    // �� �ε尡 �Ϸ�� �Ŀ� SceneManager.sceneLoaded �̺�Ʈ�� �������� ȣ��
    //    OnSceneLoaded(SceneManager.GetSceneByName(sceneName), LoadSceneMode.Single);
    //}
    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log("����ȣ�� ����ǳ�?");

    //    // �̰��� SceneManager.sceneLoaded �̺�Ʈ���� �����ϰ��� �ϴ� ������ �ۼ�
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



    void LoadActionOver() //? �갡 sceneLoaded�� ù��° �Լ��μ� ������ �Ǽ� sceneLoaded�� ü���� 0�̵Ǿ �ϴ� �̹� ����� ���� �� ������ ��.
    {
        for (int i = 0; i < OneTimeAction.Count; i++)
        {
            //Debug.Log($"{i}��° �׼� ����");
            SceneManager.sceneLoaded -= OneTimeAction[i];
        }

        OneTimeAction.Clear();
    }

    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }


}
