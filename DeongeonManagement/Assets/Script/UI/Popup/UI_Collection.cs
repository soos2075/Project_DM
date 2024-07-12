using System.Collections;
using System.Collections.Generic;
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
    }


    public Sprite button_Active;
    public Sprite button_Inactive;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(Objects));

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
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Monster(CollectionManager.Instance.Register_Monster[i]);
        }

        var npc = GetObject((int)Objects.NPCBox).GetComponentInChildren<GridLayoutGroup>().transform;
        for (int i = 0; i < CollectionManager.Instance.Register_NPC.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Element/CollectionUnit", npc);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_NPC(CollectionManager.Instance.Register_NPC[i]);
        }

        var facility = GetObject((int)Objects.FacilityBox).GetComponentInChildren<GridLayoutGroup>().transform;
        for (int i = 0; i < CollectionManager.Instance.Register_Facility.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Element/CollectionUnit", facility);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Facility(CollectionManager.Instance.Register_Facility[i]);
        }

        var tech = GetObject((int)Objects.TechnicalBox).GetComponentInChildren<GridLayoutGroup>().transform;
        for (int i = 0; i < CollectionManager.Instance.Register_Technical.Count; i++)
        {
            var unit = Managers.Resource.Instantiate("UI/PopUp/Element/CollectionUnit", tech);
            unit.GetComponent<UI_CollectionUnit>().SetUnit_Technical(CollectionManager.Instance.Register_Technical[i]);
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
