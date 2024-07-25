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


    public Sprite Select;
    public Sprite NonSelect;

    public Sprite face_Standby;
    public Sprite face_Placement;
    public Sprite face_Injury;


    enum Contents
    {
        BG,
        Line,
        Sprite,
        Name,
        State,
        Lv,
        Face,
    }




    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));

        ShowContents();

        gameObject.AddUIEvent((data) => ParentUpdate());

        GetObject(((int)Contents.Line)).GetComponent<Image>().enabled = false;
    }

    
    void ParentUpdate()
    {
        if (parent != null)
        {
            parent.MonsterBox_ClickEvent(this);
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
                GetObject(((int)Contents.State)).GetComponent<TextMeshProUGUI>().text = UserData.Instance.LocaleText("대기중").SetTextColorTag(Define.TextColor.green);
                GetObject((int)Contents.Face).GetComponent<Image>().sprite = face_Standby;
                break;

            case Monster.MonsterState.Placement:
                GetObject(((int)Contents.State)).GetComponent<TextMeshProUGUI>().text = $"{monster.PlacementInfo.Place_Floor.LabelName}".SetTextColorTag(Define.TextColor.blue);
                GetObject((int)Contents.Face).GetComponent<Image>().sprite = face_Placement;
                break;

            case Monster.MonsterState.Injury:
                GetObject(((int)Contents.State)).GetComponent<TextMeshProUGUI>().text = UserData.Instance.LocaleText("부상중").SetTextColorTag(Define.TextColor.red);
                GetObject((int)Contents.Face).GetComponent<Image>().sprite = face_Injury;
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

        GetObject(((int)Contents.Line)).GetComponent<Image>().enabled = false;
        GetObject((int)Contents.Face).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
    }

    public void ChangePanelColor(Color color)
    {
        if (parent.Current == this)
        {
            GetObject(((int)Contents.Line)).GetComponent<Image>().enabled = true;
            GetObject(((int)Contents.BG)).GetComponent<Image>().sprite = Select;
            return;
        }


        GetObject(((int)Contents.Line)).GetComponent<Image>().enabled = false;
        GetObject(((int)Contents.BG)).GetComponent<Image>().sprite = NonSelect;


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
