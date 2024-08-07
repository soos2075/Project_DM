using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Fade : UI_PopUp
{
    void Start()
    {
        Init();
    }
    private void Update()
    {
        if (canvas.isActiveAndEnabled == false)
        {
            canvas.enabled = true;
        }
    }

    enum Images
    {
        Block,
    }

    public enum FadeMode
    {
        BlackIn,
        BlackOut,
        WhiteIn,
        WhiteOut,
    }
    public FadeMode Mode { get; private set; }
    public bool SelfClose { get; private set; } = true;
    public bool isFade { get; private set; }
    public float FadeDuration { get; private set; } = 2;


    Canvas canvas;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.ScreenSpaceOverlay);
        canvas = GetComponent<Canvas>();
        Bind<Image>(typeof(Images));
        StartCoroutine(Fade(FadeDuration));
    }

    public void SetFadeOption(FadeMode mode, float duration = 2, bool selfClose = true)
    {
        Mode = mode;
        FadeDuration = duration;
        SelfClose = selfClose;
    }


    IEnumerator Fade(float time)
    {
        isFade = true;
        float timer = 0;
        Color color = Color.clear;

        switch (Mode)
        {
            case FadeMode.BlackIn:
                color = Color.black;
                break;
            case FadeMode.BlackOut:
                color = Color.clear;
                break;
            case FadeMode.WhiteIn:
                color = Color.white;
                break;
            case FadeMode.WhiteOut:
                color = Color.clear;
                break;
        }

        GetImage(((int)Images.Block)).color = color;
        yield return null;

        while (timer < time)
        {
            switch (Mode)
            {
                case FadeMode.BlackIn:
                    color.a -= (Time.unscaledDeltaTime / time);
                    break;
                case FadeMode.BlackOut:
                    color.a += (Time.unscaledDeltaTime / time);
                    break;
                case FadeMode.WhiteIn:
                    color.a -= (Time.unscaledDeltaTime / time);
                    break;
                case FadeMode.WhiteOut:
                    color.a += (Time.unscaledDeltaTime / time);
                    color.r += (Time.unscaledDeltaTime / time);
                    color.g += (Time.unscaledDeltaTime / time);
                    color.b += (Time.unscaledDeltaTime / time);
                    break;
            }

            GetImage(((int)Images.Block)).color = color;
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        //Debug.Log($"���̵� �� {Mode}");
        isFade = false;

        yield return new WaitForEndOfFrame();
        if (SelfClose)
        {
            Managers.UI.ClosePopupPick(this);
        }
    }
    
}
