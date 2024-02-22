using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager
{

    Sprite[] ui_large_buttons_horizontal;
    Sprite[] ui_large_buttons_vertical;

    Sprite[] ui_panels;

    Sprite[] ui_small_buttons;

    Sprite clear;


    Sprite[] ui_Cursor;


    public enum UI_Small_Buttons
    {
        X_Normal = 128,
        X_Pressed = 129,

        Plus_Normal = 148,
        Plus_Pressed = 149,
    }

    public enum Cursors
    {
        orange = 4,
        strawberry = 5,
        outLine = 8,
    }


    public void Init()
    {
        ui_small_buttons = Resources.LoadAll<Sprite>("Sprite/UI/ui-small-buttons");
        clear = Resources.Load<Sprite>("Sprite/UI/Clear");

        ui_Cursor = Resources.LoadAll<Sprite>("Sprite/UI/Cursors");
    }


    public Sprite GetCursor(Cursors cursors)
    {
        return ui_Cursor[(int)cursors];
    }

    public Sprite SmallButtons (UI_Small_Buttons name)
    {
        return ui_small_buttons[(int)name];
    }

    public Sprite GetSprite (string path)
    {
        Sprite sprite = Resources.Load<Sprite>($"Sprite/{path}");
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>($"Sprite/Object/{path}");
            if (sprite == null)
            {
                Debug.Log($"Sprite Not Exist : Sprite/{path}");
                return clear;
            }
        }
        return sprite;
    }

    public Sprite GetClear()
    {
        return clear;
    }

}
