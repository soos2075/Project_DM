using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CollectionUnit : UI_Base
{
    private void Awake()
    {
        //Init();
    }
    void Start()
    {
        //Init();
    }

    enum Objects
    {
        UnitSprite,
        UnitName,
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(Objects));

        gameObject.AddUIEvent((data) => { SelectUnit(); }, Define.UIEvent.LeftClick);



        //? 얘 클릭해도 스크롤 가능하게
        scroll = GetComponentInParent<ScrollRect>();
        gameObject.AddUIEvent((data) => scroll.OnDrag(data), Define.UIEvent.Drag);
        gameObject.AddUIEvent((data) => scroll.OnBeginDrag(data), Define.UIEvent.BeginDrag);
        gameObject.AddUIEvent((data) => scroll.OnEndDrag(data), Define.UIEvent.EndDrag);
    }

    ScrollRect scroll;
    UI_Collection mainUI;
    void InitAndSetData(UI_Collection parent)
    {
        Init();
        mainUI = parent;
    }

    void SelectUnit()
    {
        //Debug.Log("클릭이야");
        mainUI.Clear_ShowBox();

        if (Data_Monster != null)
        {
            mainUI.ShowBox_Monster(Data_Monster);
        }
        else if (Data_NPC != null)
        {
            mainUI.ShowBox_NPC(Data_NPC);
        }
        else if (Data_Facility != null)
        {
            mainUI.ShowBox_Facility(Data_Facility);
        }
        else if (Data_Technical != null)
        {
            mainUI.ShowBox_Technical(Data_Technical);
        }
        else if (Data_Artifact != null)
        {
            mainUI.ShowBox_Artifact(Data_Artifact);
        }
        else if (Data_Trait != null)
        {
            mainUI.ShowBox_Trait(Data_Trait);
        }
        else if (Data_Title != null)
        {
            mainUI.ShowBox_Title(Data_Title);
        }
    }




    CollectionManager.CollectionUnitRegist<SO_Monster> Data_Monster;
    CollectionManager.CollectionUnitRegist<SO_NPC> Data_NPC;
    CollectionManager.CollectionUnitRegist<SO_Facility> Data_Facility;
    CollectionManager.CollectionUnitRegist<SO_Technical> Data_Technical;
    CollectionManager.CollectionUnitRegist<SO_Artifact> Data_Artifact;
    CollectionManager.CollectionUnitRegist<SO_Trait> Data_Trait;
    CollectionManager.CollectionUnitRegist<SO_Title> Data_Title;



    public void SetUnit_Monster(CollectionManager.CollectionUnitRegist<SO_Monster> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_Monster = data;

        GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite =
            Managers.Sprite.Get_SLA(SpriteManager.Library.Monster, data.unit.SLA_category, data.unit.SLA_label);
        GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.black;
        GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = "? ? ?";

        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.white;
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }
    public void SetUnit_Facility(CollectionManager.CollectionUnitRegist<SO_Facility> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_Facility = data;

        GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite =
            Managers.Sprite.Get_SLA(SpriteManager.Library.Facility, data.unit.SLA_category, data.unit.SLA_label);
        GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.black;
        GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = "? ? ?";

        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.white;
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }
    public void SetUnit_NPC(CollectionManager.CollectionUnitRegist<SO_NPC> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_NPC = data;

        GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite =
            Managers.Sprite.Get_SLA(SpriteManager.Library.NPC, data.unit.SLA_category, data.unit.SLA_label);
        GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.black;
        GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = "? ? ?";

        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.white;
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }
    public void SetUnit_Technical(CollectionManager.CollectionUnitRegist<SO_Technical> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_Technical = data;

        GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite =
            Managers.Sprite.Get_SLA(SpriteManager.Library.Technical, data.unit.SLA_category, data.unit.SLA_label);
        GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.black;
        GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = "? ? ?";

        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.white;
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }

    public void SetUnit_Artifact(CollectionManager.CollectionUnitRegist<SO_Artifact> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_Artifact = data;

        GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite =
            Managers.Sprite.Get_SLA(SpriteManager.Library.Artifact, data.unit.SLA_category, data.unit.SLA_label);
        GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.black;
        GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = "? ? ?";

        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.white;
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }

    public void SetUnit_Trait(CollectionManager.CollectionUnitRegist<SO_Trait> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_Trait = data;

        //GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.white;
        GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;

        string category = string.IsNullOrEmpty(data.unit.SLA_Category) ? "Basic" : data.unit.SLA_Category;
        string label = string.IsNullOrEmpty(data.unit.SLA_Label) ? "Entry" : data.unit.SLA_Label;

        GetObject((int)Objects.UnitSprite).GetComponentInChildren<Image>().sprite =
            Managers.Sprite.Get_SLA(SpriteManager.Library.Trait, category, label);
    }

    public void SetUnit_Title(CollectionManager.CollectionUnitRegist<SO_Title> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_Title = data;

        GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = "? ? ?";

        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.Title;
        }
    }

}
