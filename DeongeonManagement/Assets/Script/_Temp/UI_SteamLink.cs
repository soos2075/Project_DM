using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SteamLink : UI_PopUp
{
    void Start()
    {
        Init();
        UserData.Instance.SavePlayTime();
        //Debug.Log("시간초기화");
    }




    enum TMP_Texts
    {
        VersionText,
    }

    enum Buttons
    {
        SteamLink,
        Record,

        KR,
        EN,
        JP,
    }


    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TMPro.TextMeshProUGUI>(typeof(TMP_Texts));


        //GetButton((int)Buttons.SteamLink).gameObject.AddUIEvent(data => OpenWebPageButton());
        //GetButton((int)Buttons.Record).gameObject.AddUIEvent(data => ShowPlayRecord());

        GetButton((int)Buttons.KR).gameObject.AddUIEvent(data => Lan_KR());
        GetButton((int)Buttons.EN).gameObject.AddUIEvent(data => Lan_EN());
        GetButton((int)Buttons.JP).gameObject.AddUIEvent(data => Lan_JP());

        GetTMP((int)TMP_Texts.VersionText).text = $"v_{Application.version}";
    }



    void Lan_KR()
    {
        UserData.Instance.ChangeLanguage(Define.Language.KR);
    }
    void Lan_EN()
    {
        UserData.Instance.ChangeLanguage(Define.Language.EN);
    }
    void Lan_JP()
    {
        UserData.Instance.ChangeLanguage(Define.Language.JP);
    }





    public string webpageURL = "https://store.steampowered.com/app/2886090/Novice_Dungeon_Master/";

    public void OpenWebPageButton()
    {
        Application.OpenURL(webpageURL);
    }


    public void ShowPlayRecord()
    {
        var ui = Managers.UI.ShowPopUp<UI_Record>();
        ui.DelayTime = 1;

        var message_Play = "";

        if (CollectionManager.Instance.RoundClearData != null)
        {
            var data = CollectionManager.Instance.RoundClearData.dataLog;

            message_Play += "진행 상태 : 데모 클리어!\n\n";

            message_Play += $"클리어 시간 : {(int)data.clearTime / 60}분 {(int)data.clearTime % 60}초\n\n";

            message_Play += $"획득한 마나 : {data.mana}\n";
            message_Play += $"획득한 골드 : {data.gold}\n\n";


            message_Play += $"방문한 모험가 :{data.visit}\n";
            message_Play += $"만족한 모험가 :{data.satisfaction}\n";
            message_Play += $"돌아간 모험가 :{data.return_Empty}\n";
            message_Play += $"물리친 모험가 :{data.kill}\n\n";


            message_Play += $"인기도 : {data.pop}\n";
            message_Play += $"위험도 : {data.danger}\n";
            message_Play += $"던전 랭크 : {data.rank}\n\n";


            message_Play += $"몬스터 수 : {data.monsterCount}\n";
            message_Play += $"가장 강한 몬스터 : {data.highestMonster}\n";
            message_Play += $"최고 레벨 : {data.highestMonsterLv}\n\n";
        }
        else
        {
            message_Play += $"진행 상태 : 데모 진행중!\n\n";

            message_Play += $"최고 기록 : {UserData.Instance.GetDataInt(PrefsKey.High_Turn)}일차\n\n";
        }

        ui.Msg1 = message_Play;


        var message_System = "";

        message_System += $"새 게임 횟수 : {UserData.Instance.GetDataInt(PrefsKey.NewGameTimes)}\n";
        message_System += $"게임 오버 횟수 : {UserData.Instance.GetDataInt(PrefsKey.GameOverTimes)}\n\n";


        message_System += $"활성화된 세이브 : {Managers.Data.SaveFileCount()}개\n";
        message_System += $"세이브 횟수 : {UserData.Instance.GetDataInt(PrefsKey.SaveTimes)}번\n";
        message_System += $"로드 횟수 : {UserData.Instance.GetDataInt(PrefsKey.LoadTimes)}번\n\n";


        UserData.Instance.SavePlayTime();
        var playtime = UserData.Instance.GetDataFloat(PrefsKey.PlayTime);
        string timeForm = $"{(int)playtime / 60}분 {(int)playtime % 60}초";
        message_System += $"플레이 타임 : {timeForm}\n\n";

        message_System += $"데모 버전 : {Application.version}";

        message_System += "\n감사합니다!";


        ui.Msg2 = message_System;
    }



}
