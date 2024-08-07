using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        CalculationPanel,

        LetterBox,
    }

    enum Images
    {
        Wall_Up,
        Wall_Down,
    }

    enum TextsInfo
    {
        TMP_mana,
        TMP_gold,
        TMP_ap,
    }



    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<GameObject>(typeof(BoxOption));
        Bind<TMPro.TextMeshProUGUI>(typeof(TextsInfo));
        Bind<Image>(typeof(Images));
    }

    public void ShowCalculationPanel(Main.PurchaseInfo info)
    {
        GetObject((int)BoxOption.CalculationPanel).SetActive(true);
        currentInfo = info;
        ShowUpdate(currentInfo);
    }

    void ShowUpdate(Main.PurchaseInfo info)
    {
        GetTMP((int)TextsInfo.TMP_mana).text = $"{Main.Instance.Player_Mana}\n{Util.SetTextColorTag(info.mana.ToString(), Define.TextColor.red)}";
        GetTMP((int)TextsInfo.TMP_gold).text = $"{Main.Instance.Player_Gold}\n{Util.SetTextColorTag(info.gold.ToString(), Define.TextColor.red)}";
        if (info.ap != 0)
        {
            GetTMP((int)TextsInfo.TMP_ap).text = $"{Main.Instance.Player_AP}\n{info.ap}";
        }
        else
        {
            GetTMP((int)TextsInfo.TMP_ap).gameObject.SetActive(false);
        }
    }

    Main.PurchaseInfo currentInfo;

    private void LateUpdate()
    {
        if (GetObject((int)BoxOption.CalculationPanel).activeInHierarchy)
        {
            ShowUpdate(currentInfo);
        }
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
            case BoxOption.LetterBox:
                parent.OnPopupCloseEvent -= dest;
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
        GetObject((int)BoxOption.Dialogue).SetActive(false);
        GetObject((int)BoxOption.Build).SetActive(false);
        GetObject((int)BoxOption.Monster).SetActive(false);

        GetObject((int)BoxOption.CalculationPanel).SetActive(false);
    }


    void Option_Dialogue()
    {
        GetObject((int)BoxOption.Dialogue).SetActive(true);
    }

    void Option_Build()
    {
        GetObject((int)BoxOption.Build).SetActive(true);
        Wall_Opacity();
    }
    void Option_Monster()
    {
        GetObject((int)BoxOption.Monster).SetActive(true);
        Wall_Opacity();
    }


    void Wall_Opacity()
    {
        GetImage((int)Images.Wall_Up).GetComponent<Image>().color = new Color(0, 0, 0, 0.7f);
        GetImage((int)Images.Wall_Down).GetComponent<Image>().color = new Color(0, 0, 0, 0.7f);
    }

}
