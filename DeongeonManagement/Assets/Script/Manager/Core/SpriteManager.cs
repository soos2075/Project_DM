using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager
{

    Sprite[] ui_large_buttons_horizontal;
    Sprite[] ui_large_buttons_vertical;

    Sprite[] ui_panels;

    Sprite[] ui_small_buttons;




    public enum UI_Small_Buttons
    {
        X_Normal = 128,
        X_Pressed = 129,

        Plus_Normal = 148,
        Plus_Pressed = 149,
    }


    public void Init()
    {
        ui_small_buttons = Resources.LoadAll<Sprite>("Sprite/UI/ui-small-buttons");
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
            Debug.Log($"Sprite Not Exist : Sprite/{path}");
        }
        return sprite;
    }


}
