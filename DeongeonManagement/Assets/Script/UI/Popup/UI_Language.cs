using System.Collections;
using UnityEngine;

public class UI_Language : UI_PopUp
{
    void Start()
    {
        Init();
    }


    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(Selection));
        Init_CountryEvent();
    }


    enum Selection
    {
        Close,

        Country_EN,
        Country_KO,
        Country_JP,
        //Country_CN,
        Country_SCC,
    }


    void Init_CountryEvent()
    {
        GetObject((int)Selection.Close).AddUIEvent((data) => ClosePopUp());

        GetObject((int)Selection.Country_EN).AddUIEvent((data) => ChangeLanguage(Define.Language.EN));
        GetObject((int)Selection.Country_KO).AddUIEvent((data) => ChangeLanguage(Define.Language.KR));
        GetObject((int)Selection.Country_JP).AddUIEvent((data) => ChangeLanguage(Define.Language.JP));
        GetObject((int)Selection.Country_SCC).AddUIEvent((data) => ChangeLanguage(Define.Language.SCC));
    }


    void ChangeLanguage(Define.Language language)
    {
        var confirm = Managers.UI.ShowPopUp<UI_Confirm>();
        confirm.SetText(UserData.Instance.LocaleText("LanguageChange"), () => UserData.Instance.ChangeLanguage((int)language));
    }





    public override bool EscapeKeyAction()
    {
        return true;
    }
}
