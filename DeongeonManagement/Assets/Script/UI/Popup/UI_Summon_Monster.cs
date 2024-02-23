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
    }

    enum Buttons
    {
        Return,
        Confirm,
    }
    enum Info
    {
        CurrentMana,
        NeedMana,
    }

    enum Panels
    {
        Panel,
        ClosePanel,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Image>(typeof(Panels));
        Bind<GameObject>(typeof(Preview));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Info));

        Init_Panels();
        Init_Preview();
        Init_Buttons();
        Init_Texts();
        Clear_NeedText();
        Init_Contents();
    }

    void Init_Panels()
    {
        GetImage(((int)Panels.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        GetImage(((int)Panels.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
        GetImage(((int)Panels.Panel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
    }

    void Init_Preview()
    {
        GetObject((int)Preview.Preview_Image).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
        GetObject((int)Preview.Preview_Text_Title).GetComponent<TextMeshProUGUI>().text = "";
        GetObject((int)Preview.Preview_Text_Contents).GetComponent<TextMeshProUGUI>().text = "";
    }
    void Init_Buttons()
    {
        GetButton((int)Buttons.Return).gameObject.AddUIEvent(data => CloseAll());
    }
    void Init_Texts() //? 추후에 행동력이나 기타등등 필요하면 다시 추가
    {
        GetTMP((int)Info.CurrentMana).text = $"마나\t{Main.Instance.Player_Mana}";
    }

    void Init_Contents()
    {
        var pos = GetComponentInChildren<ContentSizeFitter>().transform;

        for (int i = 0; i < GameManager.Monster.MonsterDatas.Count; i++)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Monster/Monster_Content", pos).GetComponent<UI_Monster_Content>();
            content.Content = GameManager.Monster.MonsterDatas[i];
            content.Parent = this;

            //content.gameObject.name = Managers.Monster.MonsterDatas[i].Name;
            childList.Add(content);
        }
    }



    void Clear_NeedText()
    {
        GetTMP((int)Info.NeedMana).text = "";
    }
    void Set_NeedTexts(int mana, int gold, int ap)
    {
        if (mana == 0)
        {
            GetTMP((int)Info.NeedMana).text = $"\n";
        }
        else
        {
            GetTMP((int)Info.NeedMana).text = $"-{mana}";
        }

        if (gold == 0)
        {
            GetTMP((int)Info.NeedMana).text += $"\n";
        }
        else
        {
            GetTMP((int)Info.NeedMana).text += $"\n-{gold}";
        }

        if (ap == 0)
        {
            GetTMP((int)Info.NeedMana).text += $"\n";
        }
        else
        {
            GetTMP((int)Info.NeedMana).text += $"\n-{ap}";
        }
    }





    public MonsterData Current { get; set; }
    List<UI_Monster_Content> childList = new List<UI_Monster_Content>();

    public void SelectContent(MonsterData content)
    {
        Current = content;
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Color.clear);
        }
        PreviewRefresh(content);
        Set_NeedTexts(content.ManaCost, 0, 0);
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
        if (GameManager.Monster.MaximumCheck())
        {
            if (ConfirmCheck(data.ManaCost))
            {
                SummonConfirm(data);
            }
        }
        else
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "몬스터 슬롯이 부족합니다";
        }
    }


    void SummonConfirm(MonsterData data)
    {
        var mon = GameManager.Placement.CreatePlacementObject(data.prefabPath, null, PlacementType.Monster) as Monster;
        mon.MonsterInit();
        mon.Initialize_Status();

        GameManager.Monster.AddMonster(mon);

        //Debug.Log($"{data.ManaCost}마나를 사용하여 {data.Name_KR}을 소환");
        Main.Instance.CurrentDay.SubtractMana(data.ManaCost);

        Init_Texts();

        var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
        msg.Message = $"{data.Name_KR} 소환 성공!";
    }


    bool ConfirmCheck(int mana, int gold = 0, int lv = 0, int ap = 0)
    {
        if (Main.Instance.Player_Mana < mana)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "마나가 부족합니다";
            return false;
        }
        if (Main.Instance.Player_Gold < gold)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "골드가 부족합니다";
            return false;
        }
        if (Main.Instance.DungeonRank < lv)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "던전 등급이 부족합니다";
            return false;
        }
        if (Main.Instance.Player_AP < ap)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = "행동력이 부족합니다";
            return false;
        }

        return true;
    }



    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}

