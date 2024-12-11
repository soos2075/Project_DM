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


    enum MenuButtons
    {
        Monster,
        NPC,
        Facility,

        Technical,
        Artifact,
        Trait,
    }

    enum GridGroups
    {
        MonsterBox_Content,
        NPCBox_Content,
        FacilityBox_Content,

        TechnicalBox_Content,
        ArtifactBox_Content,
        TraitBox_Content,
    }

    enum Objects
    {
        Close,

        MenuBar,

        MonsterBox,
        NPCBox,
        FacilityBox,

        TechnicalBox,
        ArtifactBox,
        TraitBox,


        ShowBox,
        //Content,
        VerticalBox,

        TMP_Stat_Main1,
        TMP_Stat_Main2,
        TMP_Stat_Main3,
        TMP_Stat_Main4,
        TMP_Stat_Main5,
        TMP_Stat_Main6,

        TMP_Stat_Sub1,
        TMP_Stat_Sub2,
        TMP_Stat_Sub3,
    }

    enum ShowBoxText
    {
        TMP_Name,
        TMP_Number,
        TMP_Detail,
        TMP_Point,
    }

    enum ShowBoxImage
    {
        MainSprite,
        MainSprite_Facility,

        NoTouch,
        MainPanel,
    }


    public Sprite button_Active;
    public Sprite button_Inactive;

    public Sprite slot_Unlock;
    public Sprite slot_Lock;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Button>(typeof(MenuButtons));
        Bind<GameObject>(typeof(Objects));
        Bind<TextMeshProUGUI>(typeof(ShowBoxText));
        Bind<GridLayoutGroup>(typeof(GridGroups));
        Bind<Image>(typeof(ShowBoxImage));


        GetImage((int)ShowBoxImage.NoTouch).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
        GetImage((int)ShowBoxImage.MainPanel).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);

        GetObject((int)Objects.Close).AddUIEvent(data => ClosePopUp());

        GetButton((int)MenuButtons.Monster).gameObject.AddUIEvent(data => MenuButton(MenuButtons.Monster, Objects.MonsterBox));
        GetButton((int)MenuButtons.NPC).gameObject.AddUIEvent(data => MenuButton(MenuButtons.NPC, Objects.NPCBox));
        GetButton((int)MenuButtons.Facility).gameObject.AddUIEvent(data => MenuButton(MenuButtons.Facility, Objects.FacilityBox));

        GetButton((int)MenuButtons.Technical).gameObject.AddUIEvent(data => MenuButton(MenuButtons.Technical, Objects.TechnicalBox));
        GetButton((int)MenuButtons.Artifact).gameObject.AddUIEvent(data => MenuButton(MenuButtons.Artifact, Objects.ArtifactBox));
        GetButton((int)MenuButtons.Trait).gameObject.AddUIEvent(data => MenuButton(MenuButtons.Trait, Objects.TraitBox));


        Create_CollectionUnit();

        MenuButton(MenuButtons.Monster, Objects.MonsterBox);
        Clear_ShowBox();
    }


    void Create_CollectionUnit()
    {
        for (int i = 0; i < CollectionManager.Instance.Register_Monster.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit",
                Get<GridLayoutGroup>((int)GridGroups.MonsterBox_Content).transform);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Monster(CollectionManager.Instance.Register_Monster[i], this);
        }

        for (int i = 0; i < CollectionManager.Instance.Register_NPC.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit",
                Get<GridLayoutGroup>((int)GridGroups.NPCBox_Content).transform);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_NPC(CollectionManager.Instance.Register_NPC[i], this);
        }

        for (int i = 0; i < CollectionManager.Instance.Register_Facility.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit_Facility",
                Get<GridLayoutGroup>((int)GridGroups.FacilityBox_Content).transform);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Facility(CollectionManager.Instance.Register_Facility[i], this);
        }

        for (int i = 0; i < CollectionManager.Instance.Register_Technical.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit_Facility", 
                Get<GridLayoutGroup>((int)GridGroups.TechnicalBox_Content).transform);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Technical(CollectionManager.Instance.Register_Technical[i], this);
        }

        for (int i = 0; i < CollectionManager.Instance.Register_Artifact.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit_Facility",
                Get<GridLayoutGroup>((int)GridGroups.ArtifactBox_Content).transform);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Artifact(CollectionManager.Instance.Register_Artifact[i], this);
        }

        for (int i = 0; i < CollectionManager.Instance.Register_Trait.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit_TraitBar",
                Get<GridLayoutGroup>((int)GridGroups.TraitBox_Content).transform);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Trait(CollectionManager.Instance.Register_Trait[i], this);
        }



    }



    void MenuButton(MenuButtons _button, Objects _box)
    {
        CloseMenuAll();

        GetObject((int)_box).SetActive(true);
        GetButton((int)_button).GetComponent<Image>().sprite = button_Active;

        var rect = GetButton((int)_button).GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(40, rect.anchoredPosition.y);
    }

    void CloseMenuAll()
    {
        for (int i = 0; i < System.Enum.GetNames(typeof(MenuButtons)).Length; i++)
        {
            GetButton(i).GetComponent<Image>().sprite = button_Inactive;
            var rect = GetButton(i).GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(68, rect.anchoredPosition.y);
        }


        GetObject((int)Objects.MonsterBox).SetActive(false);
        GetObject((int)Objects.NPCBox).SetActive(false);
        GetObject((int)Objects.FacilityBox).SetActive(false);
        GetObject((int)Objects.TechnicalBox).SetActive(false);
        GetObject((int)Objects.ArtifactBox).SetActive(false);
        GetObject((int)Objects.TraitBox).SetActive(false);
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
        GetImage((int)ShowBoxImage.MainSprite_Facility).gameObject.SetActive(false);

        GetTMP((int)ShowBoxText.TMP_Name).text = "? ? ?";
        GetTMP((int)ShowBoxText.TMP_Point).text = "";
        GetTMP((int)ShowBoxText.TMP_Detail).text = "";
        GetTMP((int)ShowBoxText.TMP_Number).text = "";



        StatContentsSet(Objects.TMP_Stat_Main1, "", "", $"");
        StatContentsSet(Objects.TMP_Stat_Main2, "", "", $"");
        StatContentsSet(Objects.TMP_Stat_Main3, "", "", $"");
        StatContentsSet(Objects.TMP_Stat_Main4, "", "", $"");
        StatContentsSet(Objects.TMP_Stat_Main5, "", "", $"");
        StatContentsSet(Objects.TMP_Stat_Main6, "", "", $"");
        StatContentsSet(Objects.TMP_Stat_Sub1, "", "", $"");
        StatContentsSet(Objects.TMP_Stat_Sub2, "", "", $"");
        StatContentsSet(Objects.TMP_Stat_Sub3, "", "", $"");

        ResetOptionBox();

        //StartCoroutine(WaitContentsizeFilter());
    }

    void StatContentsSet(Objects textBox, string uiName, string title, string content)
    {
        if (string.IsNullOrEmpty(uiName))
        {
            GetObject((int)textBox).transform.GetChild(0).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
        }
        else
        {
            GetObject((int)textBox).transform.GetChild(0).GetComponent<Image>().sprite = 
                Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Stat_32", uiName);
        }

        GetObject((int)textBox).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = title;
        GetObject((int)textBox).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = content;
    }


    GameObject Add_TraitBox()
    {
        var pos = GetObject((int)Objects.VerticalBox).transform;
        var trait = Managers.Resource.Instantiate("UI/PopUp/Collection/TraitBox", pos);
        //trait.GetComponent<ContentSizeFitter>().enabled = false;
        return trait;
    }
    GameObject Add_Header(string text = "")
    {
        var pos = GetObject((int)Objects.VerticalBox).transform;
        var header = Managers.Resource.Instantiate("UI/PopUp/Collection/TMP_Header", pos);
        if (string.IsNullOrEmpty(text) == false)
        {
            header.GetComponent<TextMeshProUGUI>().text = text;
        }
        return header;
    }
    GameObject Add_TextBox()
    {
        var pos = GetObject((int)Objects.VerticalBox).transform;
        var trait = Managers.Resource.Instantiate("UI/PopUp/Collection/TextBox", pos);
        trait.GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("잠김")}";
        return trait;
    }
    void ResetOptionBox()
    {
        var pos = GetObject((int)Objects.VerticalBox).transform;

        for (int i = pos.childCount - 1; i >= 1; i--)
        {
            Managers.Resource.Destroy(pos.GetChild(i).gameObject);
        }
    }

    void TraitContentSet(GameObject traitBox, List<TraitGroup> traitList)
    {
        Managers.Resource.Destroy(traitBox.transform.GetChild(0).gameObject);
        for (int i = 0; i < traitList.Count; i++)
        {
            GameManager.Trait.CreateTraitBar(traitList[i], traitBox.transform);
        }
        //traitBox.GetComponent<Image>().sprite = slot_Unlock;
        traitBox.GetComponent<ContentSizeFitter>().enabled = true;
    }

    void HeaderContentSet(TextMeshProUGUI textBox, string content)
    {
        textBox.text = content;
    }

    void OptionContentSet(TextMeshProUGUI textBox, string content, bool isOn = true)
    {
        if (isOn)
        {
            textBox.text = content;
            textBox.GetComponent<ContentSizeFitter>().enabled = true;

            if (string.IsNullOrEmpty(content) == false)
            {
                textBox.transform.parent.GetComponent<Image>().sprite = slot_Unlock;
            }
        }
        else
        {
            textBox.text = content;
            textBox.GetComponent<ContentSizeFitter>().enabled = false;
            textBox.GetComponent<RectTransform>().sizeDelta = new Vector2Int(380, 72);

            if (string.IsNullOrEmpty(content))
            {
                textBox.transform.parent.GetComponent<Image>().sprite = slot_Lock;
            }
        }
    }




    //? 박스 채우기

    public void ShowBox_Monster(CollectionManager.CollectionUnitRegist<SO_Monster> data)
    {
        SO_Monster SO_Data = data.unit;

        GetTMP((int)ShowBoxText.TMP_Point).text = $"Point\n{data.info.UnlockPoint}";
        GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";

        if (data.info.isRegist)
        {
            GetImage((int)ShowBoxImage.MainSprite).sprite = 
                Managers.Sprite.Get_SLA(SpriteManager.Library.Monster, SO_Data.SLA_category, SO_Data.SLA_label);

            GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.labelName;
            GetTMP((int)ShowBoxText.TMP_Point).text = $"Point\n{data.info.UnlockPoint}";
            GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";
            GetTMP((int)ShowBoxText.TMP_Detail).text = $"{SO_Data.detail}";

            StatContentsSet(Objects.TMP_Stat_Main1,"hp", "HP", $"{SO_Data.hp}");
            StatContentsSet(Objects.TMP_Stat_Main4, "maxlv", "MaxLv", $"{SO_Data.maxLv}");

            StatContentsSet(Objects.TMP_Stat_Main2, "atk" ,"ATK", $"{SO_Data.atk}");
            StatContentsSet(Objects.TMP_Stat_Main3, "def" ,"DEF", $"{SO_Data.def}");
            StatContentsSet(Objects.TMP_Stat_Main5, "agi" ,"AGI", $"{SO_Data.agi}");
            StatContentsSet(Objects.TMP_Stat_Main6, "luk" ,"LUK", $"{SO_Data.luk}");

            var header = Add_Header($"{UserData.Instance.LocaleText("획득 가능한 특성")}");
            var traitPanel = Add_TraitBox();

            GameObject header2 = null;
            GameObject textBox1 = null;
            if (string.IsNullOrEmpty(SO_Data.evolutionHint) == false)
            {
                header2 = Add_Header($"{UserData.Instance.LocaleText("진화 힌트")}");
                textBox1 = Add_TextBox();
            }
            //GameObject header3 = null;
            //GameObject textBox2 = null;
            //if (string.IsNullOrEmpty(SO_Data.evolutionDetail) == false)
            //{
            //    header3 = Add_Header($"{UserData.Instance.LocaleText("진화 조건")}");
            //    textBox2 = string.IsNullOrEmpty(SO_Data.evolutionDetail) ? null : Add_TextBox();
            //}

            if (data.info.level_1_Unlock)
            {
                StatContentsSet(Objects.TMP_Stat_Sub1, "battle" , $"{UserData.Instance.LocaleText("전투횟수")}", $"{SO_Data.maxBattleCount}");
                StatContentsSet(Objects.TMP_Stat_Sub2, "ap", $"{UserData.Instance.LocaleText("피로도")}", $"{SO_Data.battleAp}");
                StatContentsSet(Objects.TMP_Stat_Sub3, "rank", $"{UserData.Instance.LocaleText("Rank")}", $"{(Define.DungeonRank)SO_Data.unlockRank}");
            }
            if (data.info.level_2_Unlock)
            {
                TraitContentSet(traitPanel, SO_Data.TraitableList);
            }
            if (data.info.level_3_Unlock)
            {
                if (textBox1 != null)
                {
                    OptionContentSet(textBox1.GetComponentInChildren<TextMeshProUGUI>(), SO_Data.evolutionHint, true);
                }
            }
            //if (data.info.level_4_Unlock)
            //{
            //    if (textBox2 != null)
            //    {
            //        OptionContentSet(textBox2.GetComponentInChildren<TextMeshProUGUI>(), SO_Data.evolutionDetail, true);
            //    }
            //}
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

        GetTMP((int)ShowBoxText.TMP_Point).text = $"Point\n{data.info.UnlockPoint}";
        GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";

        if (data.info.isRegist)
        {
            GetImage((int)ShowBoxImage.MainSprite).sprite =
                Managers.Sprite.Get_SLA(SpriteManager.Library.NPC, data.unit.SLA_category, data.unit.SLA_label);

            GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.labelName;
            GetTMP((int)ShowBoxText.TMP_Point).text = $"Point\n{data.info.UnlockPoint}";
            GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";
            GetTMP((int)ShowBoxText.TMP_Detail).text = $"{SO_Data.detail}";

            var header = Add_Header($"{UserData.Instance.LocaleText("특성")}");
            var traitPanel = Add_TraitBox();
            TraitContentSet(traitPanel, SO_Data.NPC_TraitList);

            if (data.info.level_1_Unlock)
            {
                StatContentsSet(Objects.TMP_Stat_Main1, "hp", "HP", $"{SO_Data.HP}");

                StatContentsSet(Objects.TMP_Stat_Main2, "atk", "ATK", $"{SO_Data.ATK}");
                StatContentsSet(Objects.TMP_Stat_Main3, "def", "DEF", $"{SO_Data.DEF}");
                StatContentsSet(Objects.TMP_Stat_Main5, "agi", "AGI", $"{SO_Data.AGI}");
                StatContentsSet(Objects.TMP_Stat_Main6, "luk", "LUK", $"{SO_Data.LUK}");
            }
            if (data.info.level_2_Unlock)
            {
                StatContentsSet(Objects.TMP_Stat_Sub1, "mana", $"{UserData.Instance.LocaleText("Mana")}", $"{SO_Data.MP}");
                StatContentsSet(Objects.TMP_Stat_Sub2, "ap", $"{UserData.Instance.LocaleText("AP")}", $"{SO_Data.AP}");
                StatContentsSet(Objects.TMP_Stat_Sub3, "rank", $"{UserData.Instance.LocaleText("Rank")}", $"{(Define.DungeonRank)SO_Data.Rank}");
            }
            if (data.info.level_3_Unlock)
            {
                //? 걍 첨부터 공개하는걸로
                //TraitContentSet(traitPanel, SO_Data.NPC_TraitList);
            }
            if (data.info.level_4_Unlock)
            {
                //string prefer = "";
                //foreach (var item in SO_Data.PreferList)
                //{
                //    prefer += UserData.Instance.LocaleText_Label(item.ToString());
                //    prefer += "\t";
                //}
                //string NonPrefer = "";
                //foreach (var item in SO_Data.Non_PreferList)
                //{
                //    NonPrefer += UserData.Instance.LocaleText_Label(item.ToString());
                //    NonPrefer += "\t";
                //}
                //OptionContentSet(ShowBoxText.TMP_Option2, $"선호 : {prefer}\n회피 : {NonPrefer}", true);
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

        GetTMP((int)ShowBoxText.TMP_Point).text = $"Point\n{data.info.UnlockPoint}";
        GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";

        if (data.info.isRegist)
        {
            GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetClear();
            GetImage((int)ShowBoxImage.MainSprite_Facility).gameObject.SetActive(true);
            GetImage((int)ShowBoxImage.MainSprite_Facility).sprite =
                Managers.Sprite.Get_SLA(SpriteManager.Library.Facility, data.unit.SLA_category, data.unit.SLA_label);

            GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.labelName;
            GetTMP((int)ShowBoxText.TMP_Point).text = $"Point\n{data.info.UnlockPoint}";
            GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";
            GetTMP((int)ShowBoxText.TMP_Detail).text = $"{SO_Data.detail}";

            StatContentsSet(Objects.TMP_Stat_Main1, "category",$"{UserData.Instance.LocaleText("분류")}", 
                $"{UserData.Instance.LocaleText_Label(SO_Data.category.ToString())}");

            var header1 = Add_Header($"{UserData.Instance.LocaleText("보너스")}");
            var traitPanel1 = Add_TraitBox();
            var header2 = Add_Header($"{UserData.Instance.LocaleText("패널티")}");
            var traitPanel2 = Add_TraitBox();
            var header3 = Add_Header($"{UserData.Instance.LocaleText("무효화")}");
            var traitPanel3 = Add_TraitBox();

            if (data.info.level_1_Unlock)
            {
                StatContentsSet(Objects.TMP_Stat_Main3, "mana", $"{UserData.Instance.LocaleText("Mana")}", $"{SO_Data.mp_value}");
                StatContentsSet(Objects.TMP_Stat_Sub1, "roof", $"{UserData.Instance.LocaleText("횟수")}", $"{SO_Data.interactionOfTimes}");
                StatContentsSet(Objects.TMP_Stat_Sub2, "ap",$"{UserData.Instance.LocaleText("AP")}", $"{SO_Data.ap_value}");

                if (SO_Data.category == InteractionGroup.Interaction_Trap)
                {
                    StatContentsSet(Objects.TMP_Stat_Main3, "atk", $"ATK", $"{SO_Data.hp_value}");
                }
            }
            if (data.info.level_2_Unlock)
            {
                {
                    TraitContentSet(traitPanel1, SO_Data.bonusTarget);
                }
                {
                    TraitContentSet(traitPanel2, SO_Data.weakTarget);
                }
                {
                    TraitContentSet(traitPanel3, SO_Data.invalidTarget);
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
        //Debug.Log(SO_Data.keyName);

        GetTMP((int)ShowBoxText.TMP_Point).text = $"Point\n{data.info.UnlockPoint}";
        GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";

        if (data.info.isRegist)
        {
            GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetClear();
            GetImage((int)ShowBoxImage.MainSprite_Facility).gameObject.SetActive(true);
            GetImage((int)ShowBoxImage.MainSprite_Facility).sprite =
                Managers.Sprite.Get_SLA(SpriteManager.Library.Technical, data.unit.SLA_category, data.unit.SLA_label);

            GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.labelName;
            GetTMP((int)ShowBoxText.TMP_Detail).text = $"{SO_Data.detail}";


            StatContentsSet(Objects.TMP_Stat_Main2, "mana", $"{UserData.Instance.LocaleText("Mana")}", $"{SO_Data.Mana}");
            StatContentsSet(Objects.TMP_Stat_Main3, "gold", $"{UserData.Instance.LocaleText("Gold")}", $"{SO_Data.Gold}");

            StatContentsSet(Objects.TMP_Stat_Sub2, "ap", $"{UserData.Instance.LocaleText("AP")}", $"{SO_Data.Ap}");
            StatContentsSet(Objects.TMP_Stat_Sub3, "rank", $"{UserData.Instance.LocaleText("Rank")}", $"{(Define.DungeonRank)SO_Data.UnlockRank}");

        }
    }
    public void ShowBox_Artifact(CollectionManager.CollectionUnitRegist<SO_Artifact> data)
    {
        SO_Artifact SO_Data = data.unit;
        //Debug.Log(SO_Data.keyName);

        GetTMP((int)ShowBoxText.TMP_Point).text = $"Point\n{data.info.UnlockPoint}";
        GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";

        if (data.info.isRegist)
        {
            GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetClear();
            GetImage((int)ShowBoxImage.MainSprite_Facility).gameObject.SetActive(true);
            GetImage((int)ShowBoxImage.MainSprite_Facility).sprite =
                Managers.Sprite.Get_SLA(SpriteManager.Library.Artifact, data.unit.SLA_category, data.unit.SLA_label);

            GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.labelName;
            GetTMP((int)ShowBoxText.TMP_Detail).text = $"{SO_Data.detail}";

            var header = Add_Header($"{UserData.Instance.LocaleText("고유 효과")}");
            GameObject textBox1 = Add_TextBox();
            OptionContentSet(textBox1.GetComponentInChildren<TextMeshProUGUI>(), SO_Data.tooltip_Effect, true);
        }
    }
    public void ShowBox_Trait(CollectionManager.CollectionUnitRegist<SO_Trait> data)
    {
        SO_Trait SO_Data = data.unit;
        //Debug.Log(SO_Data.keyName);

        GetTMP((int)ShowBoxText.TMP_Point).text = $"Point\n{data.info.UnlockPoint}";
        GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";

        GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetClear();
        GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.labelName;
        GetTMP((int)ShowBoxText.TMP_Detail).text = $"{SO_Data.detail}";


        if (string.IsNullOrEmpty(SO_Data.Acquire) == false)
        {
            var header = Add_Header($"{UserData.Instance.LocaleText("획득 조건")}");
            GameObject textBox1 = Add_TextBox();
            OptionContentSet(textBox1.GetComponentInChildren<TextMeshProUGUI>(), SO_Data.Acquire, true);
        }
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
