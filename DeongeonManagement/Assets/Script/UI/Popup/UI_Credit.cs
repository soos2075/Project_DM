using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Credit : UI_PopUp
{
    void Start()
    {
        Init();
    }

    private void LateUpdate()
    {
        //Debug.Log(stateInfo.normalizedTime);

        if (isOver) return;

        if (Input.anyKey)
        {
            anim.speed = 2.0f;
        }
        else
        {
            anim.speed = 1.0f;
        }


        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1.0f)
        {
            Close_Credit();
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close_Credit();
        }
    }


    Animator anim;
    AnimatorStateInfo stateInfo;

    bool isOver;
    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        SoundManager.Instance.PlaySound("BGM/_Credit_Noname", Define.AudioType.BGM);

        anim = GetComponentInChildren<Animator>();
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        var obj = Util.FindChild(gameObject, "Panel");
        if (obj)
        {
            obj.AddUIEvent((data) => Close_Credit(), Define.UIEvent.RightClick);
        }
    }

    void Close_Credit()
    {
        SoundManager.Instance.PlaySound("BGM/_Title_Arcade", Define.AudioType.BGM);
        isOver = true;
        ClosePopUp();
    }


}
