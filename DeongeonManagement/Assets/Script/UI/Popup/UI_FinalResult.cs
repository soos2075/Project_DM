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
        AddContents(UserData.Instance.LocaleText("생존한 날짜"), main.Turn);

        AddContents(UserData.Instance.LocaleText("방문한 모험가"), main.GetTotalVisit());
        AddContents(UserData.Instance.LocaleText("만족한 모험가"), main.GetTotalSatisfaction().ToString());
        AddContents(UserData.Instance.LocaleText("패배한 모험가"), main.GetTotalKill().ToString());

        AddContents(UserData.Instance.LocaleText("획득한 마나"), main.GetTotalMana().ToString());
        //AddContents(UserData.Instance.LocaleText("사용한 마나"), main.GetTotalVisit().ToString());
        AddContents(UserData.Instance.LocaleText("획득한 골드"), main.GetTotalVisit().ToString());
        //AddContents(UserData.Instance.LocaleText("사용한 골드"), main.GetTotalVisit().ToString());

        AddContents(UserData.Instance.LocaleText("Rank"), (Define.DungeonRank)main.DungeonRank);
        AddContents(UserData.Instance.LocaleText("Popularity"), main.PopularityOfDungeon);
        AddContents(UserData.Instance.LocaleText("Danger"), main.DangerOfDungeon);

        AddContents(UserData.Instance.LocaleText("플레이 시간"), 
            $"{(int)UserData.Instance.FileConfig.PlayTimes / 60}m {(int)UserData.Instance.FileConfig.PlayTimes % 60}s");
    }




    void AddContents(string subtitle, object value)
    {
        var box = Managers.Resource.Instantiate("UI/PopUp/Element/FinalResult_ContentsBox", grid);
        box.transform.Find("Subtitle").GetComponent<TextMeshProUGUI>().text = subtitle;
        box.transform.Find("Result").GetComponent<TextMeshProUGUI>().text = value.ToString();
    }

}
