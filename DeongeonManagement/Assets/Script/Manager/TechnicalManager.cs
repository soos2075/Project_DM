using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TechnicalManager
{
    public void Init()
    {
        AddContents();
    }

    public List<TechnicalData> Technicals { get; set; }


    void AddContents()
    {
        Technicals = new List<TechnicalData>();

        {
            TechnicalData content = new TechnicalData("HerbFarm");
            content.SetName("허브농장", "2일마다 주변의 층에 약초를 공급해줍니다. 가끔 좋은 약초가 나올때도 있어요!");
            content.SetCondition(50, 50, 1);
            content.sprite = Managers.Sprite.GetSprite("HerbFarm");
            content.action = (data) => CreateAction();

            Technicals.Add(content);
        }
    }



    void CreateAction()
    {
        var obj = Managers.Resource.Instantiate("Technical/HerbFarm");

        obj.transform.position = Main.Instance.CurrentTechnical.transform.position;

        var tech = obj.GetComponent<Technical>();

        Main.Instance.CurrentTechnical.Current = tech;
        tech.parent = Main.Instance.CurrentTechnical;


        Managers.UI.CloseAll();
    }



    public void Level_2()
    {
        {
            TechnicalData content = new TechnicalData("HerbFarm");
            content.SetName("레벨2 건물", "레벨 2가 되면 추가되는 시설 예정 / 던전레벨 시스템 만들고나서 수정필요");
            content.SetCondition(50, 50, 1);
            content.sprite = Managers.Sprite.GetSprite("HerbFarm");
            content.action = (data) => CreateAction();

            Technicals.Add(content);
        }
    }

}

public class TechnicalData
{
    public Action<PointerEventData> action;

    public Sprite sprite;
    public string name_Placement;
    public string name_Detail;

    public int need_Mana;
    public int need_Gold;
    public int need_LV;

    public string contentName;
    public string prefabPath;

    public TechnicalData(string _contentName)
    {
        contentName = _contentName;
    }

    public void SetName(string title, string box)
    {
        name_Placement = title;
        name_Detail = box;
    }
    public void SetCondition(int mana, int gold, int lv)
    {
        need_Mana = mana;
        need_Gold = gold;
        need_LV = lv;
    }
}
