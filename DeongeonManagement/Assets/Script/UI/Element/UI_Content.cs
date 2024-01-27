using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Content : UI_Base
{
    enum Contents
    {
        Content,
        Image,
        Textinfo,
    }

    public int MonsterID { get; set; }

    Monster monster;
    //BasementFloor current;
    UI_Training parent;

    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
        //current = Main.Instance.CurrentFloor;
        parent = GetComponentInParent<UI_Training>();
        if (MonsterID != -1)
        {
            monster = Main.Instance.Monsters[MonsterID];
        }
    }

    void Start()
    {
        Init();
        FillContents();
    }

    void Update()
    {
        
    }

    void FillContents()
    {
        if (!monster) return;

        GetObject((int)Contents.Image).GetComponent<Image>().sprite = monster.Sprite;
        ContentsUpdate();
        gameObject.AddUIEvent((data) => ClickEvent());


        if (monster.isTraining || monster.State == Monster.MonsterState.Injury)
        {
            State = ContentState.Red;
        }
        else
        {
            State = ContentState.White;
        }
    }


    void ContentsUpdate()
    {
        GetObject((int)Contents.Textinfo).GetComponent<TextMeshProUGUI>().text =
            $"ÀÌ¸§ : {monster.name}\n" +
            $"HP : {monster.HP}\n" +
            $"LV : {monster.LV}";
    }

    void ClickEvent()
    {
        if (parent.resumeCount > 0 && State == ContentState.White)
        {
            State = ContentState.Blue;
            parent.ResumeCountUpdate(-1);
        }
        else if (State == ContentState.Blue)
        {
            State = ContentState.White;
            parent.ResumeCountUpdate(1);
        }
    }



    public enum ContentState
    {
        White,
        Green,
        Red,
        Blue,
    }

    ContentState _state;
    public ContentState State
    { 
        get { return _state; }
        set 
        { 
            _state = value;
            GetComponent<Image>().color = ColorTint(_state);
        }
    }
    Color32 ColorTint(ContentState _state)
    {
        switch (_state)
        {
            case ContentState.Green:
                return new Color32(100, 255, 100, 175);

            case ContentState.Red:
                return new Color32(255, 100, 100, 175);

            case ContentState.Blue:
                return new Color32(100, 100, 255, 175);

            default:
                return new Color32(255, 255, 255, 175);
        }
    }

}
