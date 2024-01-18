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
    public string Place { get; set; }


    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
        
    }

    void Start()
    {
        Init();
        FillContents();
    }


    void FillContents()
    {
        if (MonsterID == -1) return;

        GetObject((int)Contents.Image).GetComponent<Image>().sprite = Main.Instance.monsters[MonsterID].Sprite;
        GetObject((int)Contents.Textinfo).GetComponent<TextMeshProUGUI>().text =
            $"이름 : {Main.Instance.monsters[MonsterID].name}\n" +
            $"HP : {Main.Instance.monsters[MonsterID].HP}\n" +
            $"LV : {Main.Instance.monsters[MonsterID].LV}";


        switch (Main.Instance.monsters[MonsterID].State)
        {
            case Monster.MonsterState.Standby:
                GetObject((int)Contents.Textinfo).GetComponent<TextMeshProUGUI>().text +=
                $"\n상태 : 대기중";

                State = ContentState.White;
                gameObject.AddUIEvent(data =>
                    {
                        Main.Instance.monsters[MonsterID].Placement(Place);
                        State = ContentState.Blue;
                    });
                break;

            case Monster.MonsterState.Placement:
                GetObject((int)Contents.Textinfo).GetComponent<TextMeshProUGUI>().text +=
                $"\n상태 : {Main.Instance.monsters[MonsterID].Place}";

                if (Main.Instance.monsters[MonsterID].Place == Place)
                {
                    State = ContentState.Blue;
                    gameObject.AddUIEvent(data =>
                    {
                        Main.Instance.monsters[MonsterID].PlacementClear();
                        State = ContentState.White;
                    });
                }
                else
                {
                    State = ContentState.Green; //? 여긴 다른곳에 배치된 몬스터를 옮길거냐고 선택지 하나 띄워주는게 좋을듯.
                    gameObject.AddUIEvent(data => 
                    {
                        Main.Instance.monsters[MonsterID].Placement(Place);
                        State = ContentState.Blue;
                    });
                }
                break;

            case Monster.MonsterState.Injury:
                GetObject((int)Contents.Textinfo).GetComponent<TextMeshProUGUI>().text +=
                $"\n상태 : 부상중";

                State = ContentState.Red;
                break;
        }
    }


    void PlacementCheck() //? 이건 이제 층 / 공간오브젝트 + 컴포넌트가 필요함. 공간오브젝트에 해당 위치가 찼는지 안찼는지를 검사해서 해제/교체등의 작업을 진행하면댐
    {

    }




    public enum ContentState
    {
        White,
        Blue,
        Green,
        Red,
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
