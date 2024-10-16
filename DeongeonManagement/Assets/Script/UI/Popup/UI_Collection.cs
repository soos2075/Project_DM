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
            var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit", monster);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Monster(CollectionManager.Instance.Register_Monster[i], this);
        }

        var npc = GetObject((int)Objects.NPCBox).GetComponentInChildren<GridLayoutGroup>().transform;
        for (int i = 0; i < CollectionManager.Instance.Register_NPC.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit", npc);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_NPC(CollectionManager.Instance.Register_NPC[i], this);
        }

        var facility = GetObject((int)Objects.FacilityBox).GetComponentInChildren<GridLayoutGroup>().transform;
        for (int i = 0; i < CollectionManager.Instance.Register_Facility.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit_Facility", facility);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Facility(CollectionManager.Instance.Register_Facility[i], this);
        }

        //var tech = GetObject((int)Objects.TechnicalBox).GetComponentInChildren<GridLayoutGroup>().transform;
        //for (int i = 0; i < CollectionManager.Instance.Register_Technical.Count; i++)
        //{
        //    var unit = Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit", tech);
        //    unit.GetComponent<UI_CollectionUnit>().SetUnit_Technical(CollectionManager.Instance.Register_Technical[i], this);
        //}

        //var ending = GetObject((int)Objects.EndingBox);
        //for (int i = 0; i < CollectionManager.Instance..Length; i++)
        //{
        //    Managers.Resource.Instantiate("UI/PopUp/Collection/CollectionUnit", monster.transform);
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
                //GetButton((int)Buttons.Monster).GetComponent<Image>().color = Color.white;
                break;

            case Buttons.NPC:
                GetObject((int)Objects.NPCBox).SetActive(true);
                GetButton((int)Buttons.NPC).GetComponent<Image>().sprite = button_Active;
                //GetButton((int)Buttons.NPC).GetComponent<Image>().color = Color.white;
                break;

            case Buttons.Facility:
                GetObject((int)Objects.FacilityBox).SetActive(true);
                GetButton((int)Buttons.Facility).GetComponent<Image>().sprite = button_Active;
                //GetButton((int)Buttons.Facility).GetComponent<Image>().color = Color.white;
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

        //GetButton((int)Buttons.Monster).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
        //GetButton((int)Buttons.NPC).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
        //GetButton((int)Buttons.Facility).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);


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
        trait.GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("���")}";
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




    //? �ڽ� ä���

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

            var header = Add_Header($"{UserData.Instance.LocaleText("ȹ�� ������ Ư��")}");
            var traitPanel = Add_TraitBox();

            GameObject header2 = null;
            GameObject textBox1 = null;
            if (string.IsNullOrEmpty(SO_Data.evolutionHint) == false)
            {
                header2 = Add_Header($"{UserData.Instance.LocaleText("��ȭ ��Ʈ")}");
                textBox1 = Add_TextBox();
            }
            GameObject header3 = null;
            GameObject textBox2 = null;
            if (string.IsNullOrEmpty(SO_Data.evolutionDetail) == false)
            {
                header3 = Add_Header($"{UserData.Instance.LocaleText("��ȭ ����")}");
                textBox2 = string.IsNullOrEmpty(SO_Data.evolutionDetail) ? null : Add_TextBox();
            }

            if (data.info.level_1_Unlock)
            {
                StatContentsSet(Objects.TMP_Stat_Sub1, "battle" , $"{UserData.Instance.LocaleText("����Ƚ��")}", $"{SO_Data.maxBattleCount}");
                StatContentsSet(Objects.TMP_Stat_Sub2, "ap", $"{UserData.Instance.LocaleText("�Ƿε�")}", $"{SO_Data.battleAp}");
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
            if (data.info.level_4_Unlock)
            {
                if (textBox2 != null)
                {
                    OptionContentSet(textBox2.GetComponentInChildren<TextMeshProUGUI>(), SO_Data.evolutionDetail, true);
                }
            }
            if (data.info.level_5_Unlock)
            {
                //? �������� ������ �߰��Ѵٵ���
                //? �����͸� �߰�ȿ���� ���� ��ȯ�� ���۷����̳� ���Ⱥ��ʽ��� �� �ƹ�ư ���ʽ��� �ִ� ������ �־ �� �� ����
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

            var header = Add_Header($"{UserData.Instance.LocaleText("Ư��")}");
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
                //? �� ÷���� �����ϴ°ɷ�
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
                //OptionContentSet(ShowBoxText.TMP_Option2, $"��ȣ : {prefer}\nȸ�� : {NonPrefer}", true);
            }
            if (data.info.level_5_Unlock)
            {
                //? �������� ������ �߰��Ѵٵ���
                //? �����͸� �߰�ȿ���� ���� ��ȯ�� ���۷����̳� ���Ⱥ��ʽ��� �� �ƹ�ư ���ʽ��� �ִ� ������ �־ �� �� ����
            }
        }
        // ��ȣ Ÿ��, ����, ��ũ, �������� ��������, �ൿ�°� �ִ븶�����, ���ϴ�Ÿ��
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

            StatContentsSet(Objects.TMP_Stat_Main1, "category",$"{UserData.Instance.LocaleText("�з�")}", 
                $"{UserData.Instance.LocaleText_Label(SO_Data.category.ToString())}");

            var header1 = Add_Header($"{UserData.Instance.LocaleText("���ʽ�")}");
            var traitPanel1 = Add_TraitBox();
            var header2 = Add_Header($"{UserData.Instance.LocaleText("�г�Ƽ")}");
            var traitPanel2 = Add_TraitBox();
            var header3 = Add_Header($"{UserData.Instance.LocaleText("��ȿȭ")}");
            var traitPanel3 = Add_TraitBox();

            if (data.info.level_1_Unlock)
            {
                StatContentsSet(Objects.TMP_Stat_Main3, "mana", $"{UserData.Instance.LocaleText("Mana")}", $"{SO_Data.mp_value}");
                StatContentsSet(Objects.TMP_Stat_Sub1, "roof", $"{UserData.Instance.LocaleText("Ƚ��")}", $"{SO_Data.interactionOfTimes}");
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
                //? �������� ������ �߰��Ѵٵ���
                //? �����͸� �߰�ȿ���� ���� ��ȯ�� ���۷����̳� ���Ⱥ��ʽ��� �� �ƹ�ư ���ʽ��� �ִ� ������ �־ �� �� ����
            }
        }
    }
    public void ShowBox_Technical(CollectionManager.CollectionUnitRegist<SO_Technical> data)
    {
        SO_Technical SO_Data = data.unit;
        Debug.Log(SO_Data.keyName);
    }


    #endregion


















    #region UI Pop �⺻ ����/�ݱ� �Լ�
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
