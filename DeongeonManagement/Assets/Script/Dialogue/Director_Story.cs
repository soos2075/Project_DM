using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Director_Story : MonoBehaviour
{
    static Director_Story instance;
    public static Director_Story Instance { get { Init(); return instance; } }

    static void Init()
    {
        instance = FindAnyObjectByType<Director_Story>();
    }

    private void Awake()
    {
        Init();
        AddEvent();
    }


    void Start() //? 테스트 용도로만 사용
    {
        //Testing();
    }
    [System.Obsolete]
    void Testing()
    {
        //mainCam = Camera.main;
        //pixelCam = mainCam.GetComponent<PixelPerfectCamera>();

        StartScene_1();
        //MoveOut();
    }


    void AddEvent()
    {
        EventManager.Instance.AddCustomAction("LookElf", () => LookElf());
        EventManager.Instance.AddCustomAction("LookHeroin", () => LookHeroin());
        EventManager.Instance.AddCustomAction("LookPlayer", () => LookPlayer());
    }


    void LookElf()
    {
        villain.GetComponent<SpriteRenderer>().sprite = sprites[5];
    }

    void LookHeroin()
    {
        villain.GetComponent<SpriteRenderer>().sprite = sprites[4];
        assist.GetComponent<SpriteRenderer>().sprite = sprites[1];
    }

    void LookPlayer()
    {
        villain.GetComponent<SpriteRenderer>().sprite = sprites[3];
        assist.GetComponent<SpriteRenderer>().sprite = sprites[2];
    }



    Camera mainCam;
    PixelPerfectCamera pixelCam;

    public GameObject player;
    public GameObject villain;
    public GameObject heroin;
    public GameObject assist;

    //? 아래 스프라이트 순서 - 엘프 0좌 / 1우 / 2위 / 빌런 3좌 / 4우 / 5아래
    public Sprite[] sprites; 
    Animator playerAnim;

    #region Scene_1
    readonly Vector3 Scene_1_CameraPos = new Vector3(5, 8, -10);
    readonly int Scene_1_PPU = 14;
    readonly Vector3 scene1_playerPos = new Vector3(0.5f, 7.5f, 0);

    public void StartScene_1()
    {
        mainCam = Camera.main;
        pixelCam = mainCam.GetComponent<PixelPerfectCamera>();
        playerAnim = player.GetComponent<Animator>();

        mainCam.transform.position = Scene_1_CameraPos;
        pixelCam.assetsPPU = Scene_1_PPU;

        player.transform.position = scene1_playerPos;

        playerAnim.Play("idle_r");
        Managers.Dialogue.ShowDialogueUI("Scene1", player.transform);
        //FindAnyObjectByType<UI_DialogueBubble>().delay = 0.04f;

        StartCoroutine(DialogueOver(() => MoveOut()));
    }

    void MoveOut()
    {
        StartCoroutine(PlayerMove());
    }

    IEnumerator PlayerMove()
    {
        float timer = 0;
        var anim = player.GetComponent<Animator>();

        anim.Play("walk_f");

        StartCoroutine(elfMove());
        while (timer < 1)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            player.transform.Translate(Vector3.down * Time.unscaledDeltaTime * 2);
        }

        StartCoroutine(Scene1_FadeOut());
        anim.Play("walk_l");

        while (timer < 5)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            player.transform.Translate(Vector3.left * Time.unscaledDeltaTime * 2);
        }
    }
    IEnumerator elfMove()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        float timer = 0;
        while (timer < 0.5f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            assist.transform.Translate(Vector3.down * Time.unscaledDeltaTime * 0.5f);
        }

        yield return new WaitForSecondsRealtime(0.5f);
        assist.GetComponent<SpriteRenderer>().sprite = sprites[0];
    }
    IEnumerator Scene1_FadeOut()
    {
        yield return new WaitForSecondsRealtime(2);

        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackOut, 2, false);

        //yield return new WaitForSecondsRealtime(4);
        //Destroy(fade.gameObject);
        yield return new WaitForSecondsRealtime(3f);
        StartScene_2();
    }
    #endregion




    #region Scene_2
    readonly Vector3 Scene_2_CameraPos = new Vector3(-12, -12, -10);
    readonly int Scene_2_PPU = 12;

    readonly Vector3 scene2_playerPos = new Vector3(-13.5f, -9f, 0);


    public void StartScene_2()
    {
        mainCam.transform.position = Scene_2_CameraPos;
        pixelCam.assetsPPU = Scene_2_PPU;

        player.transform.position = scene2_playerPos;

        playerAnim.SetFloat("walk_l", 0.2f);
        playerAnim.Play("walk_l");

        StartCoroutine(Scene2_Move());
    }
    IEnumerator Scene2_Move()
    {
        StartCoroutine(FadeIn());
        yield return new WaitForSecondsRealtime(1f);

        float timer = 0;
        while (timer < 7)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            player.transform.Translate(Vector3.left * 0.5f * Time.unscaledDeltaTime);
        }

        playerAnim.Play("idle");
        yield return new WaitForSecondsRealtime(0.5f);

        Managers.Dialogue.ShowDialogueUI("Scene2", player.transform);

        StartCoroutine(DialogueOver(() => StartCoroutine(Scene2_FadeOut())));
    }


    IEnumerator Scene2_FadeOut()
    {
        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackOut, 2, false);

        yield return new WaitForSecondsRealtime(3f);
        StartScene_3();
        Debug.Log("Scene3 시작");
    }



    #endregion



    #region Scene_3
    readonly Vector3 Scene_3_CameraPos = new Vector3(17.5f, -14.5f, -10);
    readonly int Scene_3_PPU = 14;

    readonly Vector3 scene3_playerPos = new Vector3(24.5f, -14.5f, 0);


    public void StartScene_3()
    {
        mainCam.transform.position = Scene_3_CameraPos;
        pixelCam.assetsPPU = Scene_3_PPU;

        player.transform.position = scene3_playerPos;

        playerAnim.Play("idle");

        StartCoroutine(Scene3_Move());
    }

    IEnumerator Scene3_Move()
    {
        StartCoroutine(FadeIn());
        yield return new WaitForSecondsRealtime(1f);

        playerAnim.SetFloat("walk_l", 0.5f);
        playerAnim.Play("walk_l");

        float timer = 0;
        while (timer < 4.5f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            player.transform.Translate(Vector3.left * 1 * Time.unscaledDeltaTime);
        }

        playerAnim.Play("idle_b");
        yield return new WaitForSecondsRealtime(1);
        playerAnim.Play("idle_r");
        yield return new WaitForSecondsRealtime(1);
        playerAnim.Play("idle");
        yield return new WaitForSecondsRealtime(1);
        playerAnim.Play("idle_l");
        yield return new WaitForSecondsRealtime(2);

        playerAnim.Play("walk_l");
        timer = 0;
        while (timer < 4.5f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            player.transform.Translate(Vector3.left * 1 * Time.unscaledDeltaTime);
        }
        playerAnim.Play("idle_l");

        Managers.Dialogue.ShowDialogueUI("Scene3", player.transform);

        StartCoroutine(DialogueOver(() => SceneOver_GotoManagement()));
    }

    void SceneOver_GotoManagement()
    {
        Managers.Scene.LoadSceneAsync(SceneName._2_Management);
        Managers.Scene.AddLoadAction_OneTime(() => Main.Instance.NewGame_Init());
        UserData.Instance.NewGameConfig();


        //var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        //fade.SetFadeOption(UI_Fade.FadeMode.Out, 2, false);

        //yield return new WaitForSecondsRealtime(2f);

        //Debug.Log("게임화면으로");
    }

    #endregion





    IEnumerator DialogueOver(System.Action action)
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);
        action.Invoke();
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(2);

        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 2, true);
    }

}
