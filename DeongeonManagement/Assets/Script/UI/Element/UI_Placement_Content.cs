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


    void UIContentsUpdate()
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
                text.text += $"\n상태 : {monster.Place_Floor.Name_KR}";
                if (monster.Place_Floor == current)
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
        parent.ResumeCountUpdate();
    }

    void FillContents()
    {
        if (!monster) return;

        GetObject((int)Contents.Image).GetComponent<Image>().sprite = monster.Sprite;
        UIContentsUpdate();

        gameObject.AddUIEvent((data) => ClickEvent());
    }




    void ClickEvent()
    {
        switch (monster.State)
        {
            case Monster.MonsterState.Standby:
                if (Main.Instance.CurrentFloor.MaxMonsterSize > 0)
                {
                    parent.SetBoundary(Define.Boundary_1x1, () => parent.CreateAll(MonsterID));
                }
                break;

            case Monster.MonsterState.Placement:
                if (monster.Place_Floor == current)
                {
                    Main.Instance.Monsters[MonsterID].PlacementClear();
                }
                else if(Main.Instance.CurrentFloor.MaxMonsterSize > 0)
                {
                    Main.Instance.Monsters[MonsterID].PlacementClear();
                    parent.SetBoundary(Define.Boundary_1x1, () => parent.CreateAll(MonsterID));
                }
                break;

            case Monster.MonsterState.Injury:
                break;
        }

        UIContentsUpdate();
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
