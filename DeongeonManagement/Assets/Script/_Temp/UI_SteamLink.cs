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

            message += "���� ���� : Ŭ����!\n\n";
            //�ִ� ����/ �ִ� ���/ �÷���Ÿ�� �������� �ʿ���
            message += $"ȹ���� ���� : {data.mana}\n";
            message += $"ȹ���� ��� : {data.gold}\n";
            message += $"����ģ ���谡 :{data.kill}\n";
            message += $"�α⵵ : {data.pop}\n";
            message += $"���赵 : {data.danger}\n";
            message += $"���� ��ũ : {data.rank}\n";
            message += $"Ŭ���� �ð� : {(int)data.clearTime / 60}�� {(int)data.clearTime % 60}��\n\n";

            message += $"���� �� : {data.monsterCount}\n";
            message += $"���� ���� ���� : {data.highestMonster}\n";
            message += $"���� ���� ���� : {data.highestMonsterLv}\n\n";
        }
        else
        {
            message += $"���� ���� : {UserData.Instance.GetDataInt(PrefsKey.High_Turn)}����\n\n";
        }



        message += $"�� ���� Ƚ�� : {UserData.Instance.GetDataInt(PrefsKey.NewGameTimes)}\n";
        message += $"���� ���� Ƚ�� : {UserData.Instance.GetDataInt(PrefsKey.GameOverTimes)}\n\n";


        message += $"Ȱ��ȭ�� ���̺� : {Managers.Data.SaveFileCount()}��\n";
        message += $"���̺� Ƚ�� : {UserData.Instance.GetDataInt(PrefsKey.SaveTimes)}��\n";
        message += $"�ε� Ƚ�� : {UserData.Instance.GetDataInt(PrefsKey.LoadTimes)}��\n";


        UserData.Instance.SavePlayTime();
        var playtime = UserData.Instance.GetDataFloat(PrefsKey.PlayTime);
        string timeForm = $"{(int)playtime / 60}�� {(int)playtime % 60}��";
        message += $"�÷��� Ÿ�� : {timeForm}\n\n";

        message += $"���� ���� : {Application.version}";

        message += "\n�����մϴ�!";


        ui.Message = message;
    }



}
