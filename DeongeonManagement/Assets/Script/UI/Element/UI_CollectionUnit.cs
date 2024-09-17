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
    }




    CollectionManager.CollectionUnitRegist<SO_Monster> Data_Monster;
    CollectionManager.CollectionUnitRegist<SO_NPC> Data_NPC;
    CollectionManager.CollectionUnitRegist<SO_Facility> Data_Facility;
    CollectionManager.CollectionUnitRegist<SO_Technical> Data_Technical;



    public void SetUnit_Monster(CollectionManager.CollectionUnitRegist<SO_Monster> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_Monster = data;

        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.white;
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite = 
                Managers.Sprite.Get_SLA(SpriteManager.Library.Monster, data.unit.SLA_category, data.unit.SLA_label);
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }
    public void SetUnit_Facility(CollectionManager.CollectionUnitRegist<SO_Facility> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_Facility = data;

        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.white;
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite = 
                Managers.Sprite.Get_SLA(SpriteManager.Library.Facility, data.unit.SLA_category, data.unit.SLA_label);
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }
    public void SetUnit_NPC(CollectionManager.CollectionUnitRegist<SO_NPC> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_NPC = data;

        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.white;
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite = 
                Managers.Sprite.Get_SLA(SpriteManager.Library.NPC, data.unit.SLA_category, data.unit.SLA_label);
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }
    public void SetUnit_Technical(CollectionManager.CollectionUnitRegist<SO_Technical> data, UI_Collection parent)
    {
        InitAndSetData(parent);
        Data_Technical = data;

        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().color = Color.white;
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite = 
                Managers.Sprite.Get_SLA(SpriteManager.Library.Technical, data.unit.SLA_categoty, data.unit.SLA_label);
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }

}
