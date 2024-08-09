using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Collection : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Buttons
    {
        Close,

        Monster,
        NPC,
        Facility,
        //Technical,
        //Ending,
    }

    enum Objects
    {
        MenuBar,

        MonsterBox,
        NPCBox,
        FacilityBox,
        //TechnicalBox,
        //EndingBox,

        ShowBox,
        //Content,
        VerticalBox,
    }

    enum ShowBoxText
    {
        TMP_Name,
        TMP_Number,
        TMP_Detail,
        TMP_Point,

        TMP_Option1,
        TMP_Option2,
        TMP_Option3,
        TMP_Option4,
        TMP_Option5,

        TMP_Stat_Main1,
        TMP_Stat_Main2,
        TMP_Stat_Main3,
        TMP_Stat_Main4,
        TMP_Stat_Main5,
        TMP_Stat_Main6,

        TMP_Stat_Sub1,
        TMP_Stat_Sub2,
        TMP_Stat_Sub3,
        TMP_Stat_Sub4,
    }

    enum ShowBoxImage
    {
        MainSprite,

        NoTouch,
        MainPanel,
    }


    public Sprite button_Active;
    public Sprite button_Inactive;

    public Sprite slot_Unlock;
    public Sprite slot_Lock;
    public Sprite icon_Lock;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(Objects));
        Bind<TextMeshProUGUI>(typeof(ShowBoxText));
        Bind<Image>(typeof(ShowBoxImage));


        GetImage((int)ShowBoxImage.NoTouch).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
        GetImage((int)ShowBoxImage.MainPanel).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);

        GetButton((int)Buttons.Close).gameObject.AddUIEvent(data => ClosePopUp());

        GetButton((int)Buttons.Monster).gameObject.AddUIEvent(data => MenuButton(Buttons.Monster));
        GetButton((int)Buttons.NPC).gameObject.AddUIEvent(data => MenuButton(Buttons.NPC));
        GetButton((int)Buttons.Facility).gameObject.AddUIEvent(data => MenuButton(Buttons.Facility));

        //GetButton((int)Buttons.Technical).gameObject.AddUIEvent(data => MenuButton(Buttons.Technical));
        //GetButton((int)Buttons.Ending).gameObject.AddUIEvent(data => MenuButton(Buttons.Ending));

        Create_CollectionUnit();

        MenuButton(Buttons.Monster);
        Clear_ShowBox();
    }


    void Create_CollectionUnit()
    {
        var monster = GetObject((int)Objects.MonsterBox).GetComponentInChildren<GridLayoutGroup>().transform;
        for (int i = 0; i < CollectionManager.Instance.Register_Monster.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Element/CollectionUnit", monster);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Monster(CollectionManager.Instance.Register_Monster[i], this);
        }

        var npc = GetObject((int)Objects.NPCBox).GetComponentInChildren<GridLayoutGroup>().transform;
        for (int i = 0; i < CollectionManager.Instance.Register_NPC.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Element/CollectionUnit", npc);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_NPC(CollectionManager.Instance.Register_NPC[i], this);
        }

        var facility = GetObject((int)Objects.FacilityBox).GetComponentInChildren<GridLayoutGroup>().transform;
        for (int i = 0; i < CollectionManager.Instance.Register_Facility.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Element/CollectionUnit_Facility", facility);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Facility(CollectionManager.Instance.Register_Facility[i], this);
        }

        //var tech = GetObject((int)Objects.TechnicalBox).GetComponentInChildren<GridLayoutGroup>().transform;
        //for (int i = 0; i < CollectionManager.Instance.Register_Technical.Count; i++)
        //{
        //    var unit = Managers.Resource.Instantiate("UI/PopUp/Element/CollectionUnit", tech);
        //    unit.GetComponent<UI_CollectionUnit>().SetUnit_Technical(CollectionManager.Instance.Register_Technical[i], this);
        //}

        //var ending = GetObject((int)Objects.EndingBox);
        //for (int i = 0; i < CollectionManager.Instance..Length; i++)
        //{
        //    Managers.Resource.Instantiate("UI/PopUp/Element/CollectionUnit", monster.transform);
        //}
    }



    void MenuButton(Buttons _button)
    {
        CloseMenuAll();

        switch (_button)
        {
            case Buttons.Monster:
                GetObject((int)Objects.MonsterBox).SetActive(true);
                GetButton((int)Buttons.Monster).GetComponent<Image>().sprite = button_Active;
                break;

            case Buttons.NPC:
                GetObject((int)Objects.NPCBox).SetActive(true);
                GetButton((int)Buttons.NPC).GetComponent<Image>().sprite = button_Active;
                break;

            case Buttons.Facility:
                GetObject((int)Objects.FacilityBox).SetActive(true);
                GetButton((int)Buttons.Facility).GetComponent<Image>().sprite = button_Active;
                break;

            //case Buttons.Technical:
            //    GetObject((int)Objects.TechnicalBox).SetActive(true);
            //    GetButton((int)Buttons.Technical).GetComponent<Image>().sprite = button_Active;
            //    break;

            //case Buttons.Ending:
            //    GetObject((int)Objects.EndingBox).SetActive(true);
            //    GetButton((int)Buttons.Ending).GetComponent<Image>().sprite = button_Active;
            //    break;
        }
    }

    void CloseMenuAll()
    {
        GetButton((int)Buttons.Monster).GetComponent<Image>().sprite = button_Inactive;
        GetButton((int)Buttons.NPC).GetComponent<Image>().sprite = button_Inactive;
        GetButton((int)Buttons.Facility).GetComponent<Image>().sprite = button_Inactive;

        //GetButton((int)Buttons.Technical).GetComponent<Image>().sprite = button_Inactive;
        //GetButton((int)Buttons.Ending).GetComponent<Image>().sprite = button_Inactive;

        GetObject((int)Objects.MonsterBox).SetActive(false);
        GetObject((int)Objects.NPCBox).SetActive(false);
        GetObject((int)Objects.FacilityBox).SetActive(false);

        //GetObject((int)Objects.TechnicalBox).SetActive(false);
        //GetObject((int)Objects.EndingBox).SetActive(false);
    }






    #region ShowBox


    IEnumerator WaitContentsizeFilter()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        //var box1 = GetImage((int)ShowBoxImage.TextBox1).GetComponent<RectTransform>();
        //var text1 = GetImage((int)ShowBoxImage.TextBox1).GetComponentInChildren<RectTransform>();
        var verticalBox = GetObject((int)Objects.VerticalBox).transform;
        var images = verticalBox.GetComponentsInChildren<Image>();
        var texts = verticalBox.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < verticalBox.childCount; i++)
        {
            images[i].rectTransform.sizeDelta = texts[i].rectTransform.sizeDelta;
        }




        var layout = GetObject((int)Objects.ShowBox).GetComponentInChildren<ContentSizeFitter>();

        layout.enabled = false;
        yield return null;
        yield return new WaitForEndOfFrame();
        layout.enabled = true;

        //GetObject((int)Objects.ShowBox).GetComponentInChildren<VerticalLayoutGroup>().SetLayoutVertical();
    }


    public void Clear_ShowBox()
    {
        GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetClear();

        GetTMP((int)ShowBoxText.TMP_Name).text = "? ? ?";
        GetTMP((int)ShowBoxText.TMP_Point).text = "";
        GetTMP((int)ShowBoxText.TMP_Detail).text = "";
        GetTMP((int)ShowBoxText.TMP_Number).text = "";



        StatContentsSet(ShowBoxText.TMP_Stat_Main1, "", $"");
        StatContentsSet(ShowBoxText.TMP_Stat_Main2, "", $"");
        StatContentsSet(ShowBoxText.TMP_Stat_Main3, "", $"");
        StatContentsSet(ShowBoxText.TMP_Stat_Main4, "", $"");
        StatContentsSet(ShowBoxText.TMP_Stat_Main5, "", $"");
        StatContentsSet(ShowBoxText.TMP_Stat_Main6, "", $"");
        StatContentsSet(ShowBoxText.TMP_Stat_Sub1, "", $"");
        StatContentsSet(ShowBoxText.TMP_Stat_Sub2, "", $"");
        StatContentsSet(ShowBoxText.TMP_Stat_Sub3, "", $"");
        StatContentsSet(ShowBoxText.TMP_Stat_Sub4, "", $"");


        OptionContentSet(ShowBoxText.TMP_Option1,"", false);
        OptionContentSet(ShowBoxText.TMP_Option2,"", false);
        OptionContentSet(ShowBoxText.TMP_Option3,"", false);
        OptionContentSet(ShowBoxText.TMP_Option4,"", false);
        OptionContentSet(ShowBoxText.TMP_Option5,"", false);

        //? 만약 내용이 있다면 ???로, 없으면 그냥 공백으로 하면됨. 내용은 Object csv 파일의 Option 내용으로 하면 될듯

        StartCoroutine(WaitContentsizeFilter());
    }

    void StatContentsSet(ShowBoxText textBox, string title, string content)
    {
        GetTMP((int)textBox).text = title;
        GetTMP((int)textBox).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = content;
    }


    void OptionContentSet(ShowBoxText OptionBox, string content, bool isOn = true)
    {
        if (isOn)
        {
            GetTMP((int)OptionBox).text = content;
            GetTMP((int)OptionBox).GetComponent<ContentSizeFitter>().enabled = true;

            if (string.IsNullOrEmpty(content) == false)
            {
                GetTMP((int)OptionBox).transform.parent.GetComponent<Image>().sprite = slot_Unlock;
            }
        }
        else
        {
            GetTMP((int)OptionBox).text = content;
            GetTMP((int)OptionBox).GetComponent<ContentSizeFitter>().enabled = false;
            GetTMP((int)OptionBox).GetComponent<RectTransform>().sizeDelta = new Vector2Int(380, 72);

            if (string.IsNullOrEmpty(content))
            {
                GetTMP((int)OptionBox).transform.parent.GetComponent<Image>().sprite = slot_Lock;
            }
        }
    }


    public void ShowBox_Monster(CollectionManager.CollectionUnitRegist<SO_Monster> data)
    {
        SO_Monster SO_Data = data.unit;

        GetTMP((int)ShowBoxText.TMP_Point).text = $"Point : {data.info.UnlockPoint}";
        GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";

        if (data.info.isRegist)
        {
            GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetSprite(SO_Data.spritePath);

            GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.labelName;
            GetTMP((int)ShowBoxText.TMP_Point).text = $"Point : {data.info.UnlockPoint}";
            GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";
            GetTMP((int)ShowBoxText.TMP_Detail).text = $"{SO_Data.detail}";

            StatContentsSet(ShowBoxText.TMP_Stat_Main1, "HP", $"{SO_Data.hp}");
            StatContentsSet(ShowBoxText.TMP_Stat_Main2, "MaxLv", $"{SO_Data.maxLv}");
            StatContentsSet(ShowBoxText.TMP_Stat_Main3, "ATK", $"{SO_Data.atk}");
            StatContentsSet(ShowBoxText.TMP_Stat_Main4, "DEF", $"{SO_Data.def}");
            StatContentsSet(ShowBoxText.TMP_Stat_Main5, "AGI", $"{SO_Data.agi}");
            StatContentsSet(ShowBoxText.TMP_Stat_Main6, "LUK", $"{SO_Data.luk}");

            if (data.info.level_1_Unlock)
            {
                StatContentsSet(ShowBoxText.TMP_Stat_Sub1, $"동시전투", $"{SO_Data.maxBattleCount}");
                StatContentsSet(ShowBoxText.TMP_Stat_Sub2, $"{UserData.Instance.LocaleText("AP")}", $"{SO_Data.battleAp}");
                StatContentsSet(ShowBoxText.TMP_Stat_Sub3, $"등급", $"{(Define.DungeonRank)SO_Data.unlockRank}");
            }
            if (data.info.level_2_Unlock)
            {
                string traitString = "";

                foreach (var item in SO_Data.TraitableList)
                {
                    traitString += $"[{GameManager.Trait.GetData(item).labelName}]  ";
                }
                OptionContentSet(ShowBoxText.TMP_Option1, traitString, true);
            }
            if (data.info.level_3_Unlock)
            {
                OptionContentSet(ShowBoxText.TMP_Option2, SO_Data.evolutionHint, true);
            }
            if (data.info.level_4_Unlock)
            {
                OptionContentSet(ShowBoxText.TMP_Option3, SO_Data.evolutionDetail, true);
            }
            if (data.info.level_5_Unlock)
            {
                //? 위에꺼에 내용을 추가한다든지
                //? 마스터리 추가효과로 실제 소환시 시작레벨이나 스탯보너스나 뭐 아무튼 보너스를 주는 내용을 넣어도 될 것 같다
            }
        }
    }

    public void ShowBox_NPC(CollectionManager.CollectionUnitRegist<SO_NPC> data)
    {
        SO_NPC SO_Data = data.unit;

        GetTMP((int)ShowBoxText.TMP_Point).text = $"Point : {data.info.UnlockPoint}";
        GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";

        if (data.info.isRegist)
        {
            GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetSprite_SLA(data.unit.SLA_category, data.unit.SLA_label);

            GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.labelName;
            GetTMP((int)ShowBoxText.TMP_Point).text = $"Point : {data.info.UnlockPoint}";
            GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";
            GetTMP((int)ShowBoxText.TMP_Detail).text = $"{SO_Data.detail}";

            if (data.info.level_1_Unlock)
            {
                StatContentsSet(ShowBoxText.TMP_Stat_Main1, "HP", $"{SO_Data.HP}");
                StatContentsSet(ShowBoxText.TMP_Stat_Main3, "ATK", $"{SO_Data.ATK}");
                StatContentsSet(ShowBoxText.TMP_Stat_Main4, "DEF", $"{SO_Data.DEF}");
                StatContentsSet(ShowBoxText.TMP_Stat_Main5, "AGI", $"{SO_Data.AGI}");
                StatContentsSet(ShowBoxText.TMP_Stat_Main6, "LUK", $"{SO_Data.LUK}");
            }
            if (data.info.level_2_Unlock)
            {
                StatContentsSet(ShowBoxText.TMP_Stat_Sub1, $"{UserData.Instance.LocaleText("Mana")}", $"{SO_Data.MP}");
                StatContentsSet(ShowBoxText.TMP_Stat_Sub2, $"{UserData.Instance.LocaleText("AP")}", $"{SO_Data.AP}");
                StatContentsSet(ShowBoxText.TMP_Stat_Sub3, $"등급", $"{(Define.DungeonRank)SO_Data.Rank}");
            }
            if (data.info.level_3_Unlock)
            {
                string tag = "";
                foreach (var item in SO_Data.FacilityTagList)
                {
                    tag += item.ToString();
                    tag += "\t";
                }
                OptionContentSet(ShowBoxText.TMP_Option1, $"고유 속성 : {tag}", true);
            }
            if (data.info.level_4_Unlock)
            {
                //? 여긴 가지고 있는 특성 나열하기 (npc도 특성을 가질 수 있음)
                string prefer = "";
                foreach (var item in SO_Data.PreferList)
                {
                    prefer += UserData.Instance.LocaleText_Label(item.ToString());
                    prefer += "\t";
                }
                string NonPrefer = "";
                foreach (var item in SO_Data.Non_PreferList)
                {
                    NonPrefer += UserData.Instance.LocaleText_Label(item.ToString());
                    NonPrefer += "\t";
                }
                OptionContentSet(ShowBoxText.TMP_Option2, $"선호 : {prefer}\n회피 : {NonPrefer}", true);
            }
            if (data.info.level_5_Unlock)
            {
                //? 위에꺼에 내용을 추가한다든지
                //? 마스터리 추가효과로 실제 소환시 시작레벨이나 스탯보너스나 뭐 아무튼 보너스를 주는 내용을 넣어도 될 것 같다
            }
        }
        // 선호 타입, 스탯, 랭크, 언제부터 나오는지, 행동력과 최대마나등등, 피하는타일
    }
    public void ShowBox_Facility(CollectionManager.CollectionUnitRegist<SO_Facility> data)
    {
        SO_Facility SO_Data = data.unit;

        GetTMP((int)ShowBoxText.TMP_Point).text = $"Point : {data.info.UnlockPoint}";
        GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";

        if (data.info.isRegist)
        {
            GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetSprite_SLA(data.unit.SLA_category, data.unit.SLA_label);

            GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.labelName;
            GetTMP((int)ShowBoxText.TMP_Point).text = $"Point : {data.info.UnlockPoint}";
            GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";
            GetTMP((int)ShowBoxText.TMP_Detail).text = $"{SO_Data.detail}";

            StatContentsSet(ShowBoxText.TMP_Stat_Main1, $"{UserData.Instance.LocaleText("분류")}", 
                $"{UserData.Instance.LocaleText_Label(SO_Data.category.ToString())}");

            if (data.info.level_1_Unlock)
            {
                StatContentsSet(ShowBoxText.TMP_Stat_Main3, $"{UserData.Instance.LocaleText("Mana")}", $"{SO_Data.mp_value}");
                StatContentsSet(ShowBoxText.TMP_Stat_Sub1, $"{UserData.Instance.LocaleText("횟수")}", $"{SO_Data.interactionOfTimes}");
                StatContentsSet(ShowBoxText.TMP_Stat_Sub2, $"{UserData.Instance.LocaleText("AP")}", $"{SO_Data.ap_value}");
            }
            if (data.info.level_2_Unlock)
            {
                {
                    string target = "";
                    foreach (var item in SO_Data.bonusTarget)
                    {
                        target += item.ToString();
                        target += "\t";
                    }
                    OptionContentSet(ShowBoxText.TMP_Option1, $"보너스 속성 : {target}", true);
                }
                {
                    string target = "";
                    foreach (var item in SO_Data.weakTarget)
                    {
                        target += item.ToString();
                        target += "\t";
                    }
                    OptionContentSet(ShowBoxText.TMP_Option2, $"비효율 속성 : {target}", true);
                }
                {
                    string target = "";
                    foreach (var item in SO_Data.invalidTarget)
                    {
                        target += item.ToString();
                        target += "\t";
                    }
                    OptionContentSet(ShowBoxText.TMP_Option3, $"무효 속성 : {target}", true);
                }
            }
            if (data.info.level_3_Unlock)
            {

            }
            if (data.info.level_4_Unlock)
            {

            }
            if (data.info.level_5_Unlock)
            {
                //? 위에꺼에 내용을 추가한다든지
                //? 마스터리 추가효과로 실제 소환시 시작레벨이나 스탯보너스나 뭐 아무튼 보너스를 주는 내용을 넣어도 될 것 같다
            }
        }
    }
    public void ShowBox_Technical(CollectionManager.CollectionUnitRegist<SO_Technical> data)
    {
        SO_Technical SO_Data = data.unit;
        Debug.Log(SO_Data.keyName);
    }


    #endregion


















    #region UI Pop 기본 열기/닫기 함수
    public override bool EscapeKeyAction()
    {
        return true;
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }

    #endregion

}
