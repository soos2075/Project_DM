using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Placement_Monster : UI_PopUp
{
    public string Place { get; set; }


    enum Objects
    {
        Return,
        Content,

    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.Return).gameObject.AddUIEvent(data => ClosePopUp());


    }
    void Start()
    {
        Init();
        GenerateContents();
    }


    void GenerateContents()
    {
        for (int i = 0; i < Main.Instance.monsters.Length; i++)
        {
            UI_Placement_Content content = Managers.Resource.Instantiate("UI/PopUp/Element/Placement_Content", GetObject((int)Objects.Content).transform).
                GetComponent<UI_Placement_Content>();

            if (Main.Instance.monsters[i] != null)
            {
                content.MonsterID = i;
                content.Place = Place;
            }
            else
            {
                content.MonsterID = -1;
            }
        }
    }


}
