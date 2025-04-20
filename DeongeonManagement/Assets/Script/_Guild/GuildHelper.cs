using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildHelper : MonoBehaviour
{
    private static GuildHelper _instance;
    public static GuildHelper Instance { get { return _instance; } }


    private void Awake()
    {
        _instance = this;
    }


    public Sprite board_empty;
    public Sprite board_little;
    public Sprite board_many;


    public GameObject eventUI;
    public Sprite[] event_Icon;


    public enum Icon
    {
        Question_Yellow,
        Question_Red,
        Question_Blue,
        Question_Gray,

        Bang_Y,
        Bang_R,
        Bang_B,
        Bang_Gray,

        Exit,
        Dialogue,
    }

    public GameObject GetIcon(Icon icon)
    {
        GameObject ui = Instantiate(eventUI);
        ui.GetComponentInChildren<SpriteRenderer>().sprite = event_Icon[(int)icon];

        return ui;
    }
    public Sprite GetIconSprite(Icon icon)
    {
        return event_Icon[(int)icon];
    }


    public Transform[] posList;

    public enum Pos
    {
        Start,
        Exit,
        Board,
        Table1,
        Table2,
        Table3,
        Table4,

        Hero,
        DeathMagician,

        Center_Left,
        Center_Right,
    }

    public Transform GetPos(Pos pos)
    {
        return posList[(int)pos];
    }

}
