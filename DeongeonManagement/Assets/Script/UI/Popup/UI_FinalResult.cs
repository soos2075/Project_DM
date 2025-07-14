using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_FinalResult : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Objects
    {
        Close,
        ContentArea,
    }


    public override void Init()
    {
        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.Close).AddUIEvent(data => ClosePopUp());

        grid = GetObject((int)Objects.ContentArea).transform;
        main = Main.Instance;

        DefaultResult();
    }


    

    Transform grid;


    Main main;


    void DefaultResult()
    {
        AddContents(UserData.Instance.LocaleText("������ ��¥"), main.Turn);

        AddContents(UserData.Instance.LocaleText("�湮�� ���谡"), main.GetTotalVisit());
        AddContents(UserData.Instance.LocaleText("������ ���谡"), main.GetTotalSatisfaction().ToString());
        AddContents(UserData.Instance.LocaleText("�й��� ���谡"), main.GetTotalKill().ToString());

        AddContents(UserData.Instance.LocaleText("ȹ���� ����"), main.GetTotalMana().ToString());
        //AddContents(UserData.Instance.LocaleText("����� ����"), main.GetTotalVisit().ToString());
        AddContents(UserData.Instance.LocaleText("ȹ���� ���"), main.GetTotalVisit().ToString());
        //AddContents(UserData.Instance.LocaleText("����� ���"), main.GetTotalVisit().ToString());

        AddContents(UserData.Instance.LocaleText("Rank"), (Define.DungeonRank)main.DungeonRank);
        AddContents(UserData.Instance.LocaleText("Popularity"), main.PopularityOfDungeon);
        AddContents(UserData.Instance.LocaleText("Danger"), main.DangerOfDungeon);

        AddContents(UserData.Instance.LocaleText("�÷��� �ð�"), 
            $"{(int)UserData.Instance.FileConfig.PlayTimes / 60}m {(int)UserData.Instance.FileConfig.PlayTimes % 60}s");
    }




    void AddContents(string subtitle, object value)
    {
        var box = Managers.Resource.Instantiate("UI/PopUp/Element/FinalResult_ContentsBox", grid);
        box.transform.Find("Subtitle").GetComponent<TextMeshProUGUI>().text = subtitle;
        box.transform.Find("Result").GetComponent<TextMeshProUGUI>().text = value.ToString();
    }

}
