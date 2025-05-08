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

    public void View_StatBox(NPC npc)
    {
        GetObject((int)Contents.ProfilePanel).SetActive(true);

        GetTMP((int)Texts.Status_HP).text = $"{npc.B_HP}/{npc.B_HP_Max}";

        string atk = npc.B_ATK - npc.ATK >= 0 ? $"+{npc.B_ATK - npc.ATK}" : $"{npc.B_ATK - npc.ATK}";
        GetTMP((int)Texts.Status_ATK).text = $"{npc.B_ATK} ({atk})";

        string def = npc.B_DEF - npc.DEF >= 0 ? $"+{npc.B_DEF - npc.DEF}" : $"{npc.B_DEF - npc.DEF}";
        GetTMP((int)Texts.Status_DEF).text = $"{npc.B_DEF} ({def})";

        string agi = npc.B_AGI - npc.AGI >= 0 ? $"+{npc.B_AGI - npc.AGI}" : $"{npc.B_AGI - npc.AGI}";
        GetTMP((int)Texts.Status_AGI).text = $"{npc.B_AGI} ({agi})";

        string luk = npc.B_LUK - npc.LUK >= 0 ? $"+{npc.B_LUK - npc.LUK}" : $"{npc.B_LUK - npc.LUK}";
        GetTMP((int)Texts.Status_LUK).text = $"{npc.B_LUK} ({luk})";

        View_CurrentStatus(npc);
    }

    public void View_StatBox(Monster mon)
    {
        GetObject((int)Contents.ProfilePanel).SetActive(true);

        View_CurrentStatus(mon);
    }




    void View_CurrentStatus(I_BattleStat target)
    {
        var data = target.BattleStatus.GetCurrentBattleStatus_Active();

        string detail = "";

        foreach (var item in data)
        {
            var strData = GameManager.Buff.GetData(item.Key);

            detail += strData.detail;
            detail += $" x {item.Value}";
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
