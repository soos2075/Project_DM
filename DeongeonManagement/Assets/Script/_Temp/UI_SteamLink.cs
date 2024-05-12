using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SteamLink : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Buttons
    {
        SteamLink,
        Record,
    }


    public override void Init()
    {
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.SteamLink).gameObject.AddUIEvent(data => OpenWebPageButton());
        GetButton((int)Buttons.Record).gameObject.AddUIEvent(data => ShowPlayRecord());
    }


    public string webpageURL = "https://store.steampowered.com/app/2886090/Novice_Dungeon_Master/";

    public void OpenWebPageButton()
    {
        Application.OpenURL(webpageURL);
    }


    public void ShowPlayRecord()
    {
        var ui = Managers.UI.ShowPopUp<UI_SystemMessage>();
        ui.DelayTime = 1;

        var message = "<align=left>";

        if (CollectionManager.Instance.RoundClearData != null)
        {
            var data = CollectionManager.Instance.RoundClearData.dataLog;

            message += "진행 상태 : 클리어!\n\n";
            //최대 마나/ 최대 골드/ 플레이타임 세가지가 필요함
            message += $"획득한 마나 : {data.mana}\n";
            message += $"획득한 골드 : {data.gold}\n";
            message += $"물리친 모험가 :{data.kill}\n";
            message += $"인기도 : {data.pop}\n";
            message += $"위험도 : {data.danger}\n";
            message += $"던전 랭크 : {data.rank}\n";
            message += $"클리어 시간 : {(int)data.clearTime / 60}분 {(int)data.clearTime % 60}초\n\n";

            message += $"몬스터 수 : {data.monsterCount}\n";
            message += $"가장 강한 몬스터 : {data.highestMonster}\n";
            message += $"가장 높은 레벨 : {data.highestMonsterLv}\n\n";
        }
        else
        {
            message += $"진행 상태 : {UserData.Instance.GetDataInt(PrefsKey.High_Turn)}일차\n\n";
        }



        message += $"새 게임 횟수 : {UserData.Instance.GetDataInt(PrefsKey.NewGameTimes)}\n";
        message += $"게임 오버 횟수 : {UserData.Instance.GetDataInt(PrefsKey.GameOverTimes)}\n\n";


        message += $"활성화된 세이브 : {Managers.Data.SaveFileCount()}개\n";
        message += $"세이브 횟수 : {UserData.Instance.GetDataInt(PrefsKey.SaveTimes)}번\n";
        message += $"로드 횟수 : {UserData.Instance.GetDataInt(PrefsKey.LoadTimes)}번\n";


        UserData.Instance.SavePlayTime();
        var playtime = UserData.Instance.GetDataFloat(PrefsKey.PlayTime);
        string timeForm = $"{(int)playtime / 60}분 {(int)playtime % 60}초";
        message += $"플레이 타임 : {timeForm}\n\n";

        message += $"데모 버전 : {Application.version}";

        message += "\n감사합니다!";


        ui.Message = message;
    }



}
