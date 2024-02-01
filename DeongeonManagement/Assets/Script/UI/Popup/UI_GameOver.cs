using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameOver : UI_PopUp
{
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        StartCoroutine(GameOver());
    }



    IEnumerator GameOver()
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1;
        SceneManager.LoadScene("1_Start");

    }
}
