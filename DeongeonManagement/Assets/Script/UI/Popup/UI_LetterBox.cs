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

        if (parent != null)
        {
            parent.OnPopupCloseEvent += () => Managers.Resource.Destroy(gameObject);
        }


        switch (option)
        {
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


    void Option_Dialogue()
    {
        GetObject((int)BoxOption.Build).SetActive(false);
        GetObject((int)BoxOption.Monster).SetActive(false);
    }

    void Option_Build()
    {
        GetObject((int)BoxOption.Dialogue).SetActive(false);
        GetObject((int)BoxOption.Monster).SetActive(false);
    }
    void Option_Monster()
    {
        GetObject((int)BoxOption.Build).SetActive(false);
        GetObject((int)BoxOption.Dialogue).SetActive(false);
    }


}
