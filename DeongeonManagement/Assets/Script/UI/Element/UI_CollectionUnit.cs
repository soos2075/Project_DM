using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CollectionUnit : UI_Base
{
    private void Awake()
    {
        Init();
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
    }



    public void SetUnit_Monster(CollectionManager.CollectionUnitRegist<SO_Monster> data)
    {
        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite = Managers.Sprite.GetSprite(data.unit.spritePath);
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }
    public void SetUnit_Facility(CollectionManager.CollectionUnitRegist<SO_Facility> data)
    {
        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite = Managers.Sprite.GetSprite(data.unit.spritePath);
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }
    public void SetUnit_NPC(CollectionManager.CollectionUnitRegist<SO_NPC> data)
    {
        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite = Managers.Sprite.GetSprite_SLA(data.unit.SLA_category, data.unit.SLA_label);
            //GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite = Managers.Sprite.GetSprite(data.unit.SLA_category);
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }
    public void SetUnit_Technical(CollectionManager.CollectionUnitRegist<SO_Technical> data)
    {
        if (data.info.isRegist)
        {
            GetObject((int)Objects.UnitSprite).GetComponent<Image>().sprite = Managers.Sprite.GetSprite(data.unit.spritePath);
            GetObject((int)Objects.UnitName).GetComponent<TextMeshProUGUI>().text = data.unit.labelName;
        }
    }

}
