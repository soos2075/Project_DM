using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MonsterBox : UI_Base
{
    void Start()
    {
        Init();
    }


    public Monster monster;
    public UI_Monster_Management parent;


    enum Contents
    {
        BG,
        Line,
        Sprite,
        Name,
        State,
        Lv,
    }




    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));

        ShowContents();

        gameObject.AddUIEvent((data) => ParentUpdate());
    }

    
    void ParentUpdate()
    {
        if (parent != null)
        {
            parent.ShowDetail(this);
        }
    }

   
    public void ShowContents()
    {
        if (monster == null)
        {
            Clear();
            return;
        }

        GetObject(((int)Contents.Sprite)).GetComponent<Image>().sprite = Managers.Sprite.GetSprite(monster.Data.spritePath);
        GetObject(((int)Contents.Name)).GetComponent<TextMeshProUGUI>().text = monster.Data.labelName;
        switch (monster.State)
        {
            case Monster.MonsterState.Standby:
                GetObject(((int)Contents.State)).GetComponent<TextMeshProUGUI>().text = "대기중".SetTextColorTag(Define.TextColor.green);
                break;

            case Monster.MonsterState.Placement:
                GetObject(((int)Contents.State)).GetComponent<TextMeshProUGUI>().text = $"{monster.PlacementInfo.Place_Floor.LabelName}".SetTextColorTag(Define.TextColor.blue);
                break;

            case Monster.MonsterState.Injury:
                GetObject(((int)Contents.State)).GetComponent<TextMeshProUGUI>().text = "부상중".SetTextColorTag(Define.TextColor.red);
                break;
        }

        GetObject(((int)Contents.Lv)).GetComponent<TextMeshProUGUI>().text = $"Lv.{monster.LV}";
    }

    void Clear()
    {
        GetObject(((int)Contents.Sprite)).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
        GetObject(((int)Contents.Name)).GetComponent<TextMeshProUGUI>().text = "";
        GetObject(((int)Contents.State)).GetComponent<TextMeshProUGUI>().text = "";
        GetObject(((int)Contents.Lv)).GetComponent<TextMeshProUGUI>().text = "";
    }

    public void ChangePanelColor(Color color)
    {
        if (parent.Current == this) return;

        GetObject(((int)Contents.BG)).GetComponent<Image>().color = color;
    }



    private void OnEnable()
    {
        if (GetObject(((int)Contents.BG)) != null)
        {
            ShowContents();
        }
    }
}
