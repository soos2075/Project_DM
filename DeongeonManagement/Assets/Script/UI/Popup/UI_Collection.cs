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
        Title,
    }

    enum GridGroups
    {
        MonsterBox_Content,
        NPCBox_Content,
        FacilityBox_Content,

        TechnicalBox_Content,
        ArtifactBox_Content,
        TraitBox_Content,
        TitleBox_Content,
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
        TitleBox,


        ShowBox,
        //Content,
        VerticalBox,

        //TMP_Stat_Main1,
        //TMP_Stat_Main2,
        //TMP_Stat_Main3,
        //TMP_Stat_Main4,
        //TMP_Stat_Main5,
        //TMP_Stat_Main6,

        //TMP_Stat_Sub1,
        //TMP_Stat_Sub2,
        //TMP_Stat_Sub3,
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
        GetButton((int)MenuButtons.Title).gameObject.AddUIEvent(data => MenuButton(MenuButtons.Title, Objects.TitleBox));


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

        for (int i = 0; i < CollectionManager.Instance.Register_Title.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit_Title_Content",
                Get<GridLayoutGroup>((int)GridGroups.TitleBox_Content).transform);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Title(CollectionManager.Instance.Register_Title[i], this);
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
        GetObject((int)Objects.TitleBox).SetActive(false);
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

        ResetOptionBox();
    }


    GameObject Add_StatField()
    {
        var pos = GetObject((int)Objects.VerticalBox).transform;
        var statField = Managers.Resource.Instantiate("UI/PopUp/Collection/StatField", pos);

        StatFieldContentsSet(statField, "TMP_Stat_Main1", "", "", $"");
        StatFieldContentsSet(statField, "TMP_Stat_Main2", "", "", $"");
        StatFieldContentsSet(statField, "TMP_Stat_Main3", "", "", $"");
        StatFieldContentsSet(statField, "TMP_Stat_Main4", "", "", $"");
        StatFieldContentsSet(statField, "TMP_Stat_Main5", "", "", $"");
        StatFieldContentsSet(statField, "TMP_Stat_Main6", "", "", $"");
        StatFieldContentsSet(statField, "TMP_Stat_Sub1", "", "", $"");
        StatFieldContentsSet(statField, "TMP_Stat_Sub2", "", "", $"");
        StatFieldContentsSet(statField, "TMP_Stat_Sub3", "", "", $"");

        return statField;
    }

    void StatFieldContentsSet(GameObject field, string fieldName, string uiName, string title, string content)
    {
        var target = field.transform.Find(fieldName);
        if (target == null)
        {
            Debug.Log($"{target} is Null");
        }

        if (string.IsNullOrEmpty(uiName))
        {
            target.transform.GetChild(0).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
        }
        else
        {
            target.transform.GetChild(0).GetComponent<Image>().sprite =
                Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Stat_32", uiName);
        }

        target.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = title;
        target.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = content;
    }


    //void StatContentsSet(Objects textBox, string uiName, string title, string content)
    //{
    //    if (string.IsNullOrEmpty(uiName))
    //    {
    //        GetObject((int)textBox).transform.GetChild(0).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
    //    }
    //    else
    //    {
    //        GetObject((int)textBox).transform.GetChild(0).GetComponent<Image>().sprite = 
    //            Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Stat_32", uiName);
    //    }

    //    GetObject((int)textBox).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = title;
    //    GetObject((int)textBox).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = content;
    //}


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

        for (int i = pos.childCount - 1; i >= 0; i--)
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


            var field = Add_StatField();
            StatFieldContentsSet(field, "TMP_Stat_Main1", "hp", "HP", $"{SO_Data.hp}");
            StatFieldContentsSet(field, "TMP_Stat_Main4", "maxlv", "MaxLv", $"{SO_Data.maxLv}");

            StatFieldContentsSet(field, "TMP_Stat_Main2", "atk" ,"ATK", $"{SO_Data.atk}");
            StatFieldContentsSet(field, "TMP_Stat_Main3", "def" ,"DEF", $"{SO_Data.def}");
            StatFieldContentsSet(field, "TMP_Stat_Main5", "agi" ,"AGI", $"{SO_Data.agi}");
            StatFieldContentsSet(field, "TMP_Stat_Main6", "luk", "LUK", $"{SO_Data.luk}");

            //var header = Add_Header($"{UserData.Instance.LocaleText("획득 가능한 특성")}");
            //var traitPanel = Add_TraitBox();

            GameObject header2 = null;
            GameObject textBox1 = null;
            if (string.IsNullOrEmpty(SO_Data.evolutionHint) == false)
            {
                header2 = Add_Header($"{UserData.Instance.LocaleText("진화 힌트")}");
                textBox1 = Add_TextBox();
            }

            {
                var header = Add_Header($"{UserData.Instance.LocaleText("고유 특성")}");
                var traitPanel = Add_TraitBox();
                TraitContentSet(traitPanel, SO_Data.traitList_Original);
            }

            if (data.info.level_1_Unlock)
            {
                StatFieldContentsSet(field, "TMP_Stat_Sub1", "battle", $"{UserData.Instance.LocaleText("전투횟수")}", $"{SO_Data.maxBattleCount}");
                StatFieldContentsSet(field, "TMP_Stat_Sub2", "ap", $"{UserData.Instance.LocaleText("피로도")}", $"{SO_Data.battleAp}");
                StatFieldContentsSet(field, "TMP_Stat_Sub3", "rank", $"{UserData.Instance.LocaleText("Rank")}", $"{(Define.DungeonRank)SO_Data.unlockRank}");
            }
            if (data.info.level_2_Unlock)
            {
                {
                    var header = Add_Header($"{UserData.Instance.LocaleText("경험 특성")}");
                    var traitPanel = Add_TraitBox();
                    TraitContentSet(traitPanel, SO_Data.traitList_Exp);
                }
                {
                    var header = Add_Header($"{UserData.Instance.LocaleText("랜덤 특성")}");
                    var traitPanel = Add_TraitBox();
                    TraitContentSet(traitPanel, SO_Data.traitList_Random);
                }
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

            //}
            //if (data.info.level_5_Unlock)
            //{

            //}
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


            GameObject field = null;

            if (data.info.level_1_Unlock)
            {
                field = Add_StatField();
                StatFieldContentsSet(field, "TMP_Stat_Main1", "hp", "HP", $"{SO_Data.HP}");

                StatFieldContentsSet(field, "TMP_Stat_Main2", "atk", "ATK", $"{SO_Data.ATK}");
                StatFieldContentsSet(field, "TMP_Stat_Main3", "def", "DEF", $"{SO_Data.DEF}");
                StatFieldContentsSet(field, "TMP_Stat_Main5", "agi", "AGI", $"{SO_Data.AGI}");
                StatFieldContentsSet(field, "TMP_Stat_Main6", "luk", "LUK", $"{SO_Data.LUK}");
            }
            if (data.info.level_2_Unlock)
            {
                StatFieldContentsSet(field, "TMP_Stat_Sub1", "mana", $"{UserData.Instance.LocaleText("Mana")}", $"{SO_Data.MP}");
                StatFieldContentsSet(field, "TMP_Stat_Sub2", "ap", $"{UserData.Instance.LocaleText("AP")}", $"{SO_Data.AP}");
                StatFieldContentsSet(field, "TMP_Stat_Sub3", "rank", $"{UserData.Instance.LocaleText("Rank")}", $"{(Define.DungeonRank)SO_Data.Rank}");
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

            var header = Add_Header($"{UserData.Instance.LocaleText("특성")}");
            var traitPanel = Add_TraitBox();
            TraitContentSet(traitPanel, SO_Data.NPC_TraitList);
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


            GameObject field = Add_StatField();
            StatFieldContentsSet(field, "TMP_Stat_Main1", "category", $"{UserData.Instance.LocaleText("분류")}",
                $"{UserData.Instance.LocaleText_Label(SO_Data.category.ToString())}");

            var header1 = Add_Header($"{UserData.Instance.LocaleText("보너스")}");
            var traitPanel1 = Add_TraitBox();
            var header2 = Add_Header($"{UserData.Instance.LocaleText("패널티")}");
            var traitPanel2 = Add_TraitBox();
            var header3 = Add_Header($"{UserData.Instance.LocaleText("무효화")}");
            var traitPanel3 = Add_TraitBox();

            if (data.info.level_1_Unlock)
            {
                StatFieldContentsSet(field, "TMP_Stat_Main3", "mana", $"{UserData.Instance.LocaleText("Mana")}", $"{SO_Data.mp_value}");
                StatFieldContentsSet(field, "TMP_Stat_Sub1", "roof", $"{UserData.Instance.LocaleText("횟수")}", $"{SO_Data.interactionOfTimes}");
                StatFieldContentsSet(field, "TMP_Stat_Sub2", "ap", $"{UserData.Instance.LocaleText("AP")}", $"{SO_Data.ap_value}");

                if (SO_Data.category == InteractionGroup.Interaction_Trap)
                {
                    StatFieldContentsSet(field, "TMP_Stat_Main3", "atk", $"ATK", $"{SO_Data.hp_value}");
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

            GameObject field = Add_StatField();
            StatFieldContentsSet(field, "TMP_Stat_Main2", "mana", $"{UserData.Instance.LocaleText("Mana")}", $"{SO_Data.Mana}");
            StatFieldContentsSet(field, "TMP_Stat_Main3", "gold", $"{UserData.Instance.LocaleText("Gold")}", $"{SO_Data.Gold}");

            StatFieldContentsSet(field, "TMP_Stat_Sub2", "ap", $"{UserData.Instance.LocaleText("AP")}", $"{SO_Data.Ap}");
            StatFieldContentsSet(field, "TMP_Stat_Sub3", "rank", $"{UserData.Instance.LocaleText("Rank")}", $"{(Define.DungeonRank)SO_Data.UnlockRank}");
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

    public void ShowBox_Title(CollectionManager.CollectionUnitRegist<SO_Title> data)
    {
        SO_Title SO_Data = data.unit;
        //Debug.Log(SO_Data.keyName);

        GetTMP((int)ShowBoxText.TMP_Point).text = $"Point\n{data.info.UnlockPoint}";
        GetTMP((int)ShowBoxText.TMP_Number).text = $"No.{data.CollectionNumber}";

        GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetClear();

        GetTMP((int)ShowBoxText.TMP_Name).text = "? ? ?";
        GetTMP((int)ShowBoxText.TMP_Detail).text = "? ? ?";

        if (data.info.isRegist)
        {
            GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.Title;
            GetTMP((int)ShowBoxText.TMP_Detail).text = $"{SO_Data.Detail}";

            if (string.IsNullOrEmpty(SO_Data.Effect) == false)
            {
                var header = Add_Header($"{UserData.Instance.LocaleText("고유 효과")}");
                GameObject textBox1 = Add_TextBox();
                OptionContentSet(textBox1.GetComponentInChildren<TextMeshProUGUI>(), SO_Data.Effect, true);
            }
        }

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
