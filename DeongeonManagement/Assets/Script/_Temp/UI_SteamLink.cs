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
        //Debug.Log("�ð��ʱ�ȭ");
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

            message_Play += "���� ���� : ���� Ŭ����!\n\n";

            message_Play += $"Ŭ���� �ð� : {(int)data.clearTime / 60}�� {(int)data.clearTime % 60}��\n\n";

            message_Play += $"ȹ���� ���� : {data.mana}\n";
            message_Play += $"ȹ���� ��� : {data.gold}\n\n";


            message_Play += $"�湮�� ���谡 :{data.visit}\n";
            message_Play += $"������ ���谡 :{data.satisfaction}\n";
            message_Play += $"���ư� ���谡 :{data.return_Empty}\n";
            message_Play += $"����ģ ���谡 :{data.kill}\n\n";


            message_Play += $"�α⵵ : {data.pop}\n";
            message_Play += $"���赵 : {data.danger}\n";
            message_Play += $"���� ��ũ : {data.rank}\n\n";


            message_Play += $"���� �� : {data.monsterCount}\n";
            message_Play += $"���� ���� ���� : {data.highestMonster}\n";
            message_Play += $"�ְ� ���� : {data.highestMonsterLv}\n\n";
        }
        else
        {
            message_Play += $"���� ���� : ���� ������!\n\n";

            message_Play += $"�ְ� ��� : {UserData.Instance.GetDataInt(PrefsKey.High_Turn)}����\n\n";
        }

        ui.Msg1 = message_Play;


        var message_System = "";

        message_System += $"�� ���� Ƚ�� : {UserData.Instance.GetDataInt(PrefsKey.NewGameTimes)}\n";
        message_System += $"���� ���� Ƚ�� : {UserData.Instance.GetDataInt(PrefsKey.GameOverTimes)}\n\n";


        message_System += $"Ȱ��ȭ�� ���̺� : {Managers.Data.SaveFileCount()}��\n";
        message_System += $"���̺� Ƚ�� : {UserData.Instance.GetDataInt(PrefsKey.SaveTimes)}��\n";
        message_System += $"�ε� Ƚ�� : {UserData.Instance.GetDataInt(PrefsKey.LoadTimes)}��\n\n";


        UserData.Instance.SavePlayTime();
        var playtime = UserData.Instance.GetDataFloat(PrefsKey.PlayTime);
        string timeForm = $"{(int)playtime / 60}�� {(int)playtime % 60}��";
        message_System += $"�÷��� Ÿ�� : {timeForm}\n\n";

        message_System += $"���� ���� : {Application.version}";

        message_System += "\n�����մϴ�!";


        ui.Msg2 = message_System;
    }



}
