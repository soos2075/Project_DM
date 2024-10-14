using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Confirm : UI_PopUp
{
    private void Start()
    {
        Init();
    }

    enum Contents
    {
        NoTouch,
        Panel,
        Yes,
        No,
        Content,

        Panel_Calculation,
    }



    //public UI_Base Parent { get; set; }
    string textContent;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        CloseFloorUI();

        Bind<GameObject>(typeof(Contents));

        GetObject(((int)Contents.Yes)).gameObject.AddUIEvent((data) => SayYes());

        GetObject(((int)Contents.NoTouch)).gameObject.AddUIEvent((data) => SayNo(), Define.UIEvent.RightClick);
        GetObject(((int)Contents.Panel)).gameObject.AddUIEvent((data) => SayNo(), Define.UIEvent.RightClick);
        GetObject(((int)Contents.No)).gameObject.AddUIEvent((data) => SayNo());

        GetObject(((int)Contents.Content)).GetComponent<TextMeshProUGUI>().text = textContent;


        Cor_ComfirmAction = StartCoroutine(ConfirmAction());

        if (View_CalcPanel == false)
        {
            GetObject(((int)Contents.Panel_Calculation)).gameObject.SetActive(false);
        }
    }

    Coroutine Cor_ComfirmAction;

    enum State
    {
        Wait,
        Yes,
        No,
    }

    State state = State.Wait;

    public void SetText(string _content, Action yes, Action no = null)
    {
        textContent = _content;
        YesAction = yes;
        NoAction = no;
    }

    Action YesAction;
    Action NoAction;

    void SayYes()
    {
        state = State.Yes;
    }
    void SayNo()
    {
        state = State.No;
    }

    IEnumerator ConfirmAction()
    {
        yield return new WaitUntil(() => state != State.Wait);

        if (state == State.Yes)
        {
            YesAction?.Invoke();
        }
        else if (state == State.No)
        {
            NoAction?.Invoke();
        }

        Cor_ComfirmAction = null;
        Managers.UI.ClosePopupPick(this);
    }



    enum CalcTexts
    {
        NeedRank,
        NeedMana,
        NeedGold,
        NeedAp,
    }

    bool View_CalcPanel = false;

    public void SetMode_Calculation(Define.DungeonRank rank, string mana, string gold, string ap)
    {
        View_CalcPanel = true;

        Bind<TextMeshProUGUI>(typeof(CalcTexts));

        GetTMP((int)CalcTexts.NeedRank).text = $"{(Define.DungeonRank)rank}";
        GetTMP((int)CalcTexts.NeedMana).text = mana;
        GetTMP((int)CalcTexts.NeedGold).text = gold;
        GetTMP((int)CalcTexts.NeedAp).text = ap;
    }










    void CloseFloorUI()
    {
        var typeUI = FindAnyObjectByType<UI_Placement_TypeSelect>();
        if (typeUI)
        {
            Managers.UI.ClosePopupPick(typeUI);
            FindObjectOfType<UI_Management>().FloorPanelClear();
        }
    }


    public override bool EscapeKeyAction()
    {
        return true;
    }


    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }
}