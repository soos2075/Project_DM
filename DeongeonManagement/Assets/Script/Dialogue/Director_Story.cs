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
    }

    Camera mainCam;
    PixelPerfectCamera pixelCam;
    void Start()
    {
        //mainCam = Camera.main;
        //pixelCam = mainCam.GetComponent<PixelPerfectCamera>();

        //StartScene_1();
        //MoveOut();
    }


    public GameObject player;
    public GameObject villain;
    public GameObject heroin;
    public GameObject assist;

    public Sprite[] sprites;

    public void StartScene_1()
    {
        mainCam = Camera.main;
        pixelCam = mainCam.GetComponent<PixelPerfectCamera>();

        mainCam.transform.position = new Vector3(5, 8, -10);
        pixelCam.assetsPPU = 14;

        Managers.Dialogue.ShowDialogueUI("Scene1", player.transform);
        FindAnyObjectByType<UI_DialogueBubble>().delay = 0.04f;

        StartCoroutine(DialogueOver());
    }

    IEnumerator DialogueOver()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);
        Debug.Log("¹«ºù½ÃÀÛ");
        MoveOut();
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

        StartCoroutine(FadeOut());
        anim.Play("walk_l");

        while (timer < 4)
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
        assist.GetComponent<SpriteRenderer>().sprite = sprites[0];

    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSecondsRealtime(2);

        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.Out, 2, false);
    }
}
