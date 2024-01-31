using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Summon_Monster : UI_PopUp
{
    void Start()
    {
        Init();


    }
    enum Preview
    {
        Preview_Image,
        Preview_Text_Title,
        Preview_Text_Contents,
        Preview_Option_1,
        Preview_Option_2,
        Preview_Option_3,
    }

    enum Buttons
    {
        Return,
        Confirm,
    }
    enum Info
    {
        CurrentMana,
    }

    public override void Init()
    {
        base.Init();
        AddRightClickCloseAllEvent();

        Bind<GameObject>(typeof(Preview));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Info));

        Init_Preview();
        Init_Buttons();
        Init_Texts();

        Init_Contents();
    }


    void Init_Preview()
    {
        GetObject((int)Preview.Preview_Image);

        for (int i = 0; i < 3; i++)
        {
            GetObject(i + 3).GetComponent<Image>().color = Color.clear;
            GetObject(i + 3).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }
    void Init_Buttons()
    {
        GetButton((int)Buttons.Return).gameObject.AddUIEvent(data => CloseAll());
    }
    void Init_Texts()
    {
        GetTMP((int)Info.CurrentMana).text = $"현재 마나 : {Main.Instance.Player_Mana}";
    }


    void Init_Contents()
    {
        var pos = GetComponentInChildren<ContentSizeFitter>().transform;

        for (int i = 0; i < Managers.Monster.MonsterDatas.Count; i++)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Element/Monster_Content", pos).GetComponent<UI_Monster_Content>();
            content.Content = Managers.Monster.MonsterDatas[i];
            content.Parent = this;

            //content.gameObject.name = Managers.Monster.MonsterDatas[i].Name;
            childList.Add(content);
        }
    }


    public MonsterData Current { get; set; }
    List<UI_Monster_Content> childList = new List<UI_Monster_Content>();

    public void SelectContent(MonsterData content)
    {
        Current = content;
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Define.Color_Gray);
        }
        PreviewRefresh(content);
    }


    void PreviewRefresh(MonsterData content)
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = content.sprite;
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = content.Name_KR;
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = content.detail;


        GetButton((int)Buttons.Confirm).gameObject.RemoveUIEventAll();
        GetButton((int)Buttons.Confirm).gameObject.AddUIEvent((data) => MonsterSummon(content));
    }



    void MonsterSummon(MonsterData data)
    {
        if (Managers.Monster.MaximumCheck() && Main.Instance.Player_Mana >= data.ManaCost)
        {
            SummonConfirm(data);
        }
        else
        {
            Debug.Log("자리없어인마 아님 마나부족일수도");
        }
    }


    void SummonConfirm(MonsterData data)
    {
        var mon = Managers.Placement.CreatePlacementObject(data.prefabPath, null, Define.PlacementType.Monster);
        Managers.Monster.AddMonster(mon as Monster);

        Debug.Log($"{data.ManaCost}마나를 사용하여 {data.Name_KR}을 소환");
        Main.Instance.CurrentDay.SubtractMana(data.ManaCost);

        Init_Texts();
    }



}

