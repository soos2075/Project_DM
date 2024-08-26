using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Managers.UI.SetCanvas(gameObject);

        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.ClosePanel).AddUIEvent(data => ClosePopUp(), Define.UIEvent.RightClick);
        GetObject((int)Objects.MainPanel).AddUIEvent(data => ClosePopUp(), Define.UIEvent.RightClick);
        GetObject((int)Objects.Close).AddUIEvent(data => ClosePopUp());

        logBox = GetObject((int)Objects.LogBox).transform;

        Init_LogButton();
    }


    Transform logBox;


    void Init_LogButton()
    {
        for (int i = 0; i < Main.Instance.Turn; i++)
        {
            var log = Managers.Resource.Instantiate("UI/PopUp/Element/DayLog", logBox);
            log.AddUIEvent(data => Show_DayResult(log.transform.GetSiblingIndex()));

            log.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{log.transform.GetSiblingIndex() + 1}";
        }
    }


    void Show_DayResult(int day)
    {
        var ui = Managers.UI.ShowPopUp<UI_DayResult>();

        Main.DayResult current = (day == Main.Instance.Turn - 1) ? Main.Instance.CurrentDay : Main.Instance.DayList[day + 1];

        ui.TextContents(Main.Instance.DayList[day], current);
    }
}
