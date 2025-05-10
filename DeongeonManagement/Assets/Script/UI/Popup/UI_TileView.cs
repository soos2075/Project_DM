using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_TileView : UI_PopUp, IWorldSpaceUI
{
    void Awake()
    {
        Init();
        SetCanvasWorldSpace();
    }
    void Start()
    {
        if (Main.Instance.CurrentAction != null)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetCanvasWorldSpace()
    {
        //Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace);

        //GetComponent<Canvas>().worldCamera = Camera.main;

        Managers.UI.SetCanvas_SubCamera(gameObject, RenderMode.ScreenSpaceCamera, false);

        panel = transform.GetChild(0).GetComponent<RectTransform>();
    }


    RectTransform panel;


    private void LateUpdate()
    {
        //? 기존
        //Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        //Vector3 centeredMousePosition = Input.mousePosition - screenCenter;
        //panel.localPosition = new Vector3(centeredMousePosition.x - 5, centeredMousePosition.y + 5, 0);

        //? 스크린사이즈 테스트
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 centeredMousePosition = Input.mousePosition - screenCenter;

        float offset = (float)Screen.width / 1280f;
        centeredMousePosition /= offset;

        panel.localPosition = new Vector3(centeredMousePosition.x + (57600 / Screen.width), centeredMousePosition.y - (32400 / Screen.height), 0);
    }


    enum Contents
    {
        Panel,
        Name,
        Contents,
        Detail,

        State_Detail,

        ProfilePanel,
    }

    TextMeshProUGUI text_Name;
    TextMeshProUGUI text_Contents;
    TextMeshProUGUI text_Detail;

    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));

        text_Name = GetObject((int)Contents.Name).GetComponent<TextMeshProUGUI>();
        text_Contents = GetObject((int)Contents.Contents).GetComponent<TextMeshProUGUI>();
        text_Detail = GetObject((int)Contents.Detail).GetComponent<TextMeshProUGUI>();

        text_Detail.text = "";


        State_Detail = GetObject((int)Contents.State_Detail).GetComponent<TextMeshProUGUI>();
        StatePanel = State_Detail.transform.parent.gameObject;
        State_Detail.text = "";
        StatePanel.SetActive(false);


        Init_StatBox();
    }


    enum Texts
    {
        State,

        Status_HP,
        Status_ATK,
        Status_DEF,
        Status_AGI,
        Status_LUK,
    }
    //? 스탯 박스 (npc랑 유닛만 떠야함)
    void Init_StatBox()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        for (int i = 0; i < System.Enum.GetValues(typeof(Texts)).Length; i++)
        {
            GetTMP(i).text = "";
        }
        GetObject((int)Contents.ProfilePanel).SetActive(false);
    }


    void Close_StatBox()
    {
        GetObject((int)Contents.ProfilePanel).SetActive(false);
    }

    public void View_StatBox<T>(T target) where T : I_BattleStat
    {
        GetObject((int)Contents.ProfilePanel).SetActive(true);

        GetTMP((int)Texts.Status_HP).text = $"{target.B_HP}/{target.B_HP_Max}";

        string atk = target.B_ATK - target.Base_ATK >= 0 ? $"+{target.B_ATK - target.Base_ATK}" : $"{target.B_ATK - target.Base_ATK}";
        GetTMP((int)Texts.Status_ATK).text = $"{target.B_ATK} ({atk})";

        string def = target.B_DEF - target.Base_DEF >= 0 ? $"+{target.B_DEF - target.Base_DEF}" : $"{target.B_DEF - target.Base_DEF}";
        GetTMP((int)Texts.Status_DEF).text = $"{target.B_DEF} ({def})";

        string agi = target.B_AGI - target.Base_AGI >= 0 ? $"+{target.B_AGI - target.Base_AGI}" : $"{target.B_AGI - target.Base_AGI}";
        GetTMP((int)Texts.Status_AGI).text = $"{target.B_AGI} ({agi})";

        string luk = target.B_LUK - target.Base_LUK >= 0 ? $"+{target.B_LUK - target.Base_LUK}" : $"{target.B_LUK - target.Base_LUK}";
        GetTMP((int)Texts.Status_LUK).text = $"{target.B_LUK} ({luk})";

        View_CurrentStatus(target);
    }

    void View_CurrentStatus(I_BattleStat target)
    {
        var data = target.BattleStatus.GetCurrentBattleStatus_Active();

        string detail = "";

        foreach (var item in data)
        {
            var strData = GameManager.Buff.GetData(item.Key);

            if (strData.statusType == SO_BattleStatus.StatusType.Up)
            {
                detail += $"[<b>{strData.labelName.SetTextColorTag(Define.TextColor.Plus_Green)}</b>]";
            }
            else if (strData.statusType == SO_BattleStatus.StatusType.Down)
            {
                detail += $"[<b>{strData.labelName.SetTextColorTag(Define.TextColor.Plus_Red)}</b>]";
            }

            detail += $"<b> x {item.Value}\n</b>";
        }

        GetTMP((int)Texts.State).text = detail;
    }






    public void ViewContents(string name, string contents)
    {
        text_Name.text = name;
        text_Contents.text = contents;
    }


    public void ViewDetail(string addContents)
    {
        text_Detail.text = addContents;
        Close_StatBox();
    }


    TextMeshProUGUI State_Detail;
    GameObject StatePanel;
    public void View_State(string contents)
    {
        StatePanel.SetActive(true);
        State_Detail.text = contents;
    }


}
