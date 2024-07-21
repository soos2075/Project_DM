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
        Technical,
        Ending,
    }

    enum Objects
    {
        MenuBar,

        MonsterBox,
        NPCBox,
        FacilityBox,
        TechnicalBox,
        EndingBox,

        ShowBox,
    }

    enum ShowBoxText
    {
        TMP_Point,
        TMP_Name,
        TMP_Stat,
        TMP_Option1,
        TMP_Option2,
        TMP_Option3,
        TMP_Option4,
        TMP_Option5,
    }

    enum ShowBoxImage
    {
        MainSprite,
    }


    public Sprite button_Active;
    public Sprite button_Inactive;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(Objects));
        Bind<TextMeshProUGUI>(typeof(ShowBoxText));
        Bind<Image>(typeof(ShowBoxImage));

        GetButton((int)Buttons.Close).gameObject.AddUIEvent(data => ClosePopUp());

        GetButton((int)Buttons.Monster).gameObject.AddUIEvent(data => MenuButton(Buttons.Monster));
        GetButton((int)Buttons.NPC).gameObject.AddUIEvent(data => MenuButton(Buttons.NPC));
        GetButton((int)Buttons.Facility).gameObject.AddUIEvent(data => MenuButton(Buttons.Facility));
        GetButton((int)Buttons.Technical).gameObject.AddUIEvent(data => MenuButton(Buttons.Technical));
        GetButton((int)Buttons.Ending).gameObject.AddUIEvent(data => MenuButton(Buttons.Ending));

        Create_CollectionUnit();

        MenuButton(Buttons.Monster);
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
            var unit = Managers.Resource.Instantiate("UI/PopUp/Element/CollectionUnit", facility);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Facility(CollectionManager.Instance.Register_Facility[i], this);
        }

        var tech = GetObject((int)Objects.TechnicalBox).GetComponentInChildren<GridLayoutGroup>().transform;
        for (int i = 0; i < CollectionManager.Instance.Register_Technical.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Element/CollectionUnit", tech);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Technical(CollectionManager.Instance.Register_Technical[i], this);
        }

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

            case Buttons.Technical:
                GetObject((int)Objects.TechnicalBox).SetActive(true);
                GetButton((int)Buttons.Technical).GetComponent<Image>().sprite = button_Active;
                break;

            case Buttons.Ending:
                GetObject((int)Objects.EndingBox).SetActive(true);
                GetButton((int)Buttons.Ending).GetComponent<Image>().sprite = button_Active;
                break;
        }

        CurrentMenu = _button;
    }

    void CloseMenuAll()
    {
        GetButton((int)Buttons.Monster).GetComponent<Image>().sprite = button_Inactive;
        GetButton((int)Buttons.NPC).GetComponent<Image>().sprite = button_Inactive;
        GetButton((int)Buttons.Facility).GetComponent<Image>().sprite = button_Inactive;
        GetButton((int)Buttons.Technical).GetComponent<Image>().sprite = button_Inactive;
        GetButton((int)Buttons.Ending).GetComponent<Image>().sprite = button_Inactive;

        GetObject((int)Objects.MonsterBox).SetActive(false);
        GetObject((int)Objects.NPCBox).SetActive(false);
        GetObject((int)Objects.FacilityBox).SetActive(false);
        GetObject((int)Objects.TechnicalBox).SetActive(false);
        GetObject((int)Objects.EndingBox).SetActive(false);
    }






    #region ShowBox

    Buttons CurrentMenu;

    //public void ShowBox(CollectionManager.CollectionUnitRegist<I_SO_Collection> data)
    //{
    //    if (data == null || data.info.isRegist == false) return;

    //    Clear_ShowBox();

    //    switch (CurrentMenu)
    //    {
    //        case Buttons.Monster:
    //            ShowBox_Monster(data);
    //            break;

    //        case Buttons.NPC:
    //            ShowBox_NPC(data);
    //            break;

    //        case Buttons.Facility:
    //            ShowBox_Facility(data);
    //            break;

    //        case Buttons.Technical:
    //            ShowBox_Technical(data);
    //            break;

    //        case Buttons.Ending:
    //            break;
    //    }
    //}


    public void Clear_ShowBox()
    {
        GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetClear();

        GetTMP((int)ShowBoxText.TMP_Point).text = "0";
        GetTMP((int)ShowBoxText.TMP_Name).text = "? ? ?";
        GetTMP((int)ShowBoxText.TMP_Stat).text = "? ? ?";
        GetTMP((int)ShowBoxText.TMP_Option1).text = "? ? ?";
        GetTMP((int)ShowBoxText.TMP_Option2).text = "? ? ?";
        GetTMP((int)ShowBoxText.TMP_Option3).text = "? ? ?";
        GetTMP((int)ShowBoxText.TMP_Option4).text = "? ? ?";
        GetTMP((int)ShowBoxText.TMP_Option5).text = "? ? ?";

        //? ���� ������ �ִٸ� ???��, ������ �׳� �������� �ϸ��. ������ Object csv ������ Option �������� �ϸ� �ɵ�
    }
    public void ShowBox_Monster(CollectionManager.CollectionUnitRegist<SO_Monster> data)
    {
        SO_Monster SO_Data = data.unit;
        Debug.Log($"{SO_Data.keyName} - {data.info.UnlockPoint}UnlockPoint");


        if (data.info.isRegist)
        {
            GetTMP((int)ShowBoxText.TMP_Point).text = $"{data.info.UnlockPoint}";

            string stat = $"HP : {SO_Data.hp}\t MaxLv : {SO_Data.maxLv}" +
                $"\nATK : {SO_Data.atk}\tDEF : {SO_Data.def}" +
                $"\nAGI : {SO_Data.agi}\tLUK : {SO_Data.luk}";

            GetImage((int)ShowBoxImage.MainSprite).sprite = Managers.Sprite.GetSprite(SO_Data.spritePath);
            GetTMP((int)ShowBoxText.TMP_Name).text = SO_Data.labelName;
            GetTMP((int)ShowBoxText.TMP_Stat).text = stat;


            GetTMP((int)ShowBoxText.TMP_Option1).text = SO_Data.detail;


            if (data.info.level_1_Unlock)
            {
                GetTMP((int)ShowBoxText.TMP_Option2).text = $"�ʿ� ���� ��� : {(Define.DungeonRank)SO_Data.unlockRank}" +
                    $"\n�ִ� ���� ���� : {SO_Data.maxBattleCount}" +
                    $"\n���� �� �Ƿε� : {SO_Data.battleAp}";
            }
            if (data.info.level_2_Unlock)
            {
                string traitString = "";
                foreach (var item in SO_Data.TraitableList)
                {
                    traitString += $"[{item.ToString()}]  ";
                }
                GetTMP((int)ShowBoxText.TMP_Option3).text = traitString;
                //? Ư������ -> ���� �� �ִ� Ư���� �����ϸ� ������. �������� ���Ư�� �پ� ������°͵� �̻���. �� �Ұ������� �ʰ�����..
                //? �װ͵� �׷��� ����Ư�������͵� �׷���. �� 2ȸ�����̶�� 3ȸ ���� 4ȸ ���� �߿� �������� 2ȸ�� ���� �� �ְ� ���� 4ȸ�� ���� �� �ְ� �̷���
            }
            if (data.info.level_3_Unlock)
            {
                GetTMP((int)ShowBoxText.TMP_Option4).text = SO_Data.evolutionHint;
            }
            if (data.info.level_4_Unlock)
            {
                GetTMP((int)ShowBoxText.TMP_Option5).text = SO_Data.evolutionDetail;
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
        Debug.Log(SO_Data.keyName);

        // ��ȣ Ÿ��, ����, ��ũ, �������� ��������, �ൿ�°� �ִ븶�����, ���ϴ�Ÿ��
    }
    public void ShowBox_Facility(CollectionManager.CollectionUnitRegist<SO_Facility> data)
    {
        SO_Facility SO_Data = data.unit;
        Debug.Log(SO_Data.keyName);

        // �ü�Ÿ��, �ִ� ������, �����ʽ�, 
    }
    public void ShowBox_Technical(CollectionManager.CollectionUnitRegist<SO_Technical> data)
    {
        SO_Technical SO_Data = data.unit;
        Debug.Log(SO_Data.keyName);
    }


    void View_ShowBox(Sprite main, string name, string stat, string op1, string op2, string op3, string op4, string op5)
    {
        GetImage((int)ShowBoxImage.MainSprite).sprite = main;

        GetTMP((int)ShowBoxText.TMP_Name).text = name;
        GetTMP((int)ShowBoxText.TMP_Stat).text = stat;
        GetTMP((int)ShowBoxText.TMP_Option1).text = op1;
        GetTMP((int)ShowBoxText.TMP_Option2).text = op2;
        GetTMP((int)ShowBoxText.TMP_Option3).text = op3;
        GetTMP((int)ShowBoxText.TMP_Option4).text = op4;
        GetTMP((int)ShowBoxText.TMP_Option4).text = op5;
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
