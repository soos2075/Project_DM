using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class OpeningDirector : MonoBehaviour
{

    public Camera mainCam;
    PixelPerfectCamera pixelCam;

    public GameObject player;
    Animator playerAnim;

    public GameObject entrance;
    Animator entraceAnim;


    readonly Vector3 Scene_1_Cam_Pos = new Vector3(2, 0, -10);
    readonly int Scene_1_PPU = 16;
    readonly Vector3 Scene_1_Player_Pos = new Vector3(1.5f, -3.5f, 0);


    void Start()
    {


        StartScene_1();
    }


    public void StartScene_1()
    {
        pixelCam = mainCam.GetComponent<PixelPerfectCamera>();
        playerAnim = player.GetComponentInChildren<Animator>();
        entraceAnim = entrance.GetComponentInChildren<Animator>();


        mainCam.transform.position = Scene_1_Cam_Pos;
        pixelCam.assetsPPU = Scene_1_PPU;
        player.transform.position = Scene_1_Player_Pos;

        Managers.Dialogue.ShowDialogueUI("Opening_1", player.transform);

        StartCoroutine(DialogueOver(() => StartCoroutine(Scene_1_1())));
    }

    IEnumerator Scene_1_1()
    {
        float timer = 0;
        playerAnim.Play(Define.ANIM_Running);
        player.transform.localScale = new Vector3(-1, 1, 1);

        while (timer < 1.0f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            player.transform.Translate(Vector3.left * Time.unscaledDeltaTime);
        }

        timer = 0;
        entraceAnim.Play("Flash");

        while (timer < 2.5f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            player.transform.Translate(Vector3.left * Time.unscaledDeltaTime);
        }
        playerAnim.Play(Define.ANIM_Idle);

        Managers.Dialogue.ShowDialogueUI("Opening_2", player.transform);

        StartCoroutine(DialogueOver(() => StartCoroutine(Scene_1_2())));
    }

    IEnumerator Scene_1_2()
    {
        StartCoroutine(FadeOut(1.5f, 2));

        float timer = 0;
        playerAnim.Play(Define.ANIM_Running);
        player.transform.localScale = new Vector3(-1, 1, 1);

        while (timer < 3.5f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            player.transform.Translate(Vector3.left * Time.unscaledDeltaTime);
        }

        yield return new WaitForSecondsRealtime(2);

        StartCoroutine(FadeIn(1, 2));
        StartScene_2();
    }


    readonly Vector3 Scene_2_Cam_Pos = new Vector3(-19, 0, -10);
    readonly int Scene_2_PPU = 16;
    readonly Vector3 Scene_2_Player_Pos = new Vector3(-11.5f, -1f, 0);

    void StartScene_2()
    {
        Debug.Log("¾À2½ÃÀÛ");
        player.transform.position = Scene_2_Player_Pos;
        mainCam.transform.position = Scene_2_Cam_Pos;
        pixelCam.assetsPPU = Scene_2_PPU;


        StartCoroutine(Scene2_1());
    }

    IEnumerator Scene2_1()
    {
        yield return new WaitForSecondsRealtime(1);
        float timer = 0;
        playerAnim.Play(Define.ANIM_Running);
        player.transform.localScale = new Vector3(-1, 1, 1);

        while (timer < 4.0f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            player.transform.Translate(Vector3.left * Time.unscaledDeltaTime);
        }
        playerAnim.Play(Define.ANIM_Idle);

        Managers.Dialogue.ShowDialogueUI("Opening_3", player.transform);

        StartCoroutine(DialogueOver(() => StartCoroutine(Scene2_2())));
    }
    IEnumerator Scene2_2()
    {
        float timer = 0;
        playerAnim.Play(Define.ANIM_Running);
        player.transform.localScale = new Vector3(-1, 1, 1);

        while (timer < 4.5f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            player.transform.Translate(Vector3.left * Time.unscaledDeltaTime);
        }
        playerAnim.Play(Define.ANIM_Idle);


        Managers.Dialogue.ShowDialogueUI("Opening_4", player.transform);

        StartCoroutine(DialogueOver(() => GoToGame()));
    }

    void GoToGame()
    {
        Managers.Scene.LoadSceneAsync(SceneName._2_Management);
        Managers.Scene.AddLoadAction_OneTime(() => Main.Instance.NewGame_Init());
        //UserData.Instance.NewGameConfig();
    }




    IEnumerator DialogueOver(Action action)
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);
        action.Invoke();
    }

    IEnumerator FadeIn(float _count, float _duration)
    {
        yield return new WaitForSecondsRealtime(_count);

        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, _duration, true);
    }
    IEnumerator FadeOut(float _count, float _duration)
    {
        yield return new WaitForSecondsRealtime(_count);

        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackOut, _duration, false);
    }

}
