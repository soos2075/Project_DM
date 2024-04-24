using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpriteManager
{

    Sprite[] ui_large_buttons_horizontal;
    Sprite[] ui_large_buttons_vertical;

    Sprite[] ui_panels;

    Sprite[] ui_small_buttons;

    Sprite clear;


    Sprite[] ui_Cursor;


    SpriteLibraryAsset[] SLA_Array;


    Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();

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

        SLA_Array = Resources.LoadAll<SpriteLibraryAsset>("SpriteLabraryAsset");
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
        Sprite sprite = null;
        if (spriteDict.TryGetValue(path, out sprite))
        {
            return sprite;
        }

        sprite = Resources.Load<Sprite>($"Sprite/{path}");
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>($"Sprite/Object/{path}");
            if (sprite == null)
            {
                sprite = GetSprite_SLA(path);
                if (sprite == null)
                {
                    Debug.Log($"Sprite/{path} : Sprite Not Exist");
                    return clear;
                }
            }
        }
        spriteDict.Add(path, sprite);
        return sprite;
    }
    public Sprite GetSprite_SLA(string _searchName)
    {
        foreach (var item in SLA_Array)
        {
            foreach (var _category in item.GetCategoryNames())
            {
                if (_searchName == _category)
                {
                    return item.GetSprite(_category, "Entry");
                }
                else
                {
                    foreach (var _label in item.GetCategoryLabelNames(_category))
                    {
                        if (_searchName == _label)
                        {
                            return item.GetSprite(_category, _label);
                        }
                    }
                }
            }
        }

        Debug.Log($"{_searchName} : Sprite Not Exist from SLA Data");
        return null;
    }

    public Sprite GetClear()
    {
        return clear;
    }

}
