using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DayLog : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Objects
    {
        ClosePanel,
        MainPanel,
        Close,
        LogBox,
    }


    public override void Init()
    {
        scroll = GetComponentInChildren<ScrollRect>();

        Managers.UI.SetCanvas(gameObject);

        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.ClosePanel).AddUIEvent(data => ClosePopUp(), Define.UIEvent.RightClick);
        GetObject((int)Objects.MainPanel).AddUIEvent(data => ClosePopUp(), Define.UIEvent.RightClick);
        GetObject((int)Objects.Close).AddUIEvent(data => ClosePopUp());

        logBox = GetObject((int)Objects.LogBox).transform;

        Init_LogButton();
    }


    Transform logBox;
    ScrollRect scroll;


    void Init_LogButton()
    {
        int turn = Main.Instance.Turn;
        if (Main.Instance.Management == false)
        {
            turn -= 1;
        }


        for (int i = 0; i < turn; i++)
        {
            var log = Managers.Resource.Instantiate("UI/PopUp/Element/DayLog", logBox);
            log.AddUIEvent(data => Show_DayResult(log.transform.GetSiblingIndex()));

            //? 부모의 스크롤렉트의 드래그이벤트 연결
            log.gameObject.AddUIEvent((data) => scroll.OnDrag(data), Define.UIEvent.Drag);
            log.gameObject.AddUIEvent((data) => scroll.OnBeginDrag(data), Define.UIEvent.BeginDrag);
            log.gameObject.AddUIEvent((data) => scroll.OnEndDrag(data), Define.UIEvent.EndDrag);


            log.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{log.transform.GetSiblingIndex() + 1}";
        }
    }


    void Show_DayResult(int day) //? day는 슬롯 index라서 0부터 시작 :: [0] == day 1
    {
        var ui = Managers.UI.ShowPopUp<UI_DayResult>();

        int currentTurn = Main.Instance.Turn;

        if (Main.Instance.Management == false)
        {
            currentTurn -= 1;
        }

        Main.DayResult current = (day == currentTurn - 1) ? Main.Instance.CurrentDay : Main.Instance.DayList[day + 1];

        ui.TextContents(Main.Instance.DayList[day], current);
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
