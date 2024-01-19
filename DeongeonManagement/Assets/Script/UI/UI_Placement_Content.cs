using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Placement_Content : UI_Base
{
    enum Contents
    {
        Placement_Content,
        Image,
        Textinfo,
    }


    public int MonsterID { get; set; }

    Monster monster;
    BasementFloor current;
    UI_Placement_Monster parent;


    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
        current = Main.Instance.CurrentFloor;
        parent = GetComponentInParent<UI_Placement_Monster>();
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


    void ContentsUpdate()
    {
        var text = GetObject((int)Contents.Textinfo).GetComponent<TextMeshProUGUI>();

        text.text =
            $"이름 : {monster.name}\n" +
            $"HP : {monster.HP}\n" +
            $"LV : {monster.LV}";

        switch (monster.State)
        {
            case Monster.MonsterState.Standby:
                text.text += $"\n상태 : 대기중";
                PanelState = ContentState.White;
                break;

            case Monster.MonsterState.Placement:
                text.text += $"\n상태 : {monster.Place.Name_KR}";
                if (monster.Place == current)
                {
                    PanelState = ContentState.Blue;
                }
                else
                {
                    PanelState = ContentState.Green;
                }
                break;

            case Monster.MonsterState.Injury:
                text.text += $"\n상태 : 부상중";
                PanelState = ContentState.Red;
                break;
        }
    }

    void FillContents()
    {
        if (!monster) return;

        GetObject((int)Contents.Image).GetComponent<Image>().sprite = monster.Sprite;
        ContentsUpdate();

        gameObject.AddUIEvent((data) => ClickEvent());
    }


    void ClickEvent()
    {
        switch (monster.State)
        {
            case Monster.MonsterState.Standby:
                if (parent.resumeCount > 0)
                {
                    monster.Placement(current);
                    parent.ResumeCountUpdate(-1);
                }
                break;

            case Monster.MonsterState.Placement:
                if (monster.Place == current)
                {
                    Main.Instance.Monsters[MonsterID].PlacementClear();
                    parent.ResumeCountUpdate(1);
                }
                else if(parent.resumeCount > 0)
                {
                    Main.Instance.Monsters[MonsterID].PlacementClear();
                    Main.Instance.Monsters[MonsterID].Placement(current);
                    parent.ResumeCountUpdate(-1);
                }
                break;

            case Monster.MonsterState.Injury:
                break;
        }

        ContentsUpdate();
    }







    #region Panel
    public enum ContentState
    {
        White,
        Blue,
        Green,
        Red,
    }

    ContentState _panelState;
    public ContentState PanelState
    {
        get { return _panelState; }
        set
        {
            _panelState = value;
            GetComponent<Image>().color = ColorTint(_panelState);
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
    #endregion
}
