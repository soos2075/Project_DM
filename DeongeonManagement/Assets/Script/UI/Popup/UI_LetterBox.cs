using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LetterBox : UI_PopUp
{
    //void Start()
    //{
    //    Init();
    //}


    public enum BoxOption
    {
        NoSkip_Dialogue,
        Dialogue,
        Build,
        Monster,

    }



    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<GameObject>(typeof(BoxOption));
    }



    public void SetBoxOption(BoxOption option, UI_PopUp parent) //? 얜 PopupStack에 Push를 안하는 타입으로
    {
        Init();

        System.Action dest = () => Managers.Resource.Destroy(gameObject);
        if (parent != null)
        {
            parent.OnPopupCloseEvent += dest;
        }

        AllClear();

        switch (option)
        {
            case BoxOption.NoSkip_Dialogue:
                parent.OnPopupCloseEvent -= dest;
                Option_NoSkip();
                break;

            case BoxOption.Dialogue:
                Option_Dialogue();
                break;

            case BoxOption.Build:
                Option_Build();
                break;

            case BoxOption.Monster:
                Option_Monster();
                break;
        }
    }


    void AllClear()
    {
        GetObject((int)BoxOption.NoSkip_Dialogue).SetActive(false);
        GetObject((int)BoxOption.Dialogue).SetActive(false);
        GetObject((int)BoxOption.Build).SetActive(false);
        GetObject((int)BoxOption.Monster).SetActive(false);
    }

    void Option_NoSkip()
    {
        GetObject((int)BoxOption.NoSkip_Dialogue).SetActive(true);
    }

    void Option_Dialogue()
    {
        GetObject((int)BoxOption.Dialogue).SetActive(true);
    }

    void Option_Build()
    {
        GetObject((int)BoxOption.Build).SetActive(true);
    }
    void Option_Monster()
    {
        GetObject((int)BoxOption.Monster).SetActive(true);
    }


}
