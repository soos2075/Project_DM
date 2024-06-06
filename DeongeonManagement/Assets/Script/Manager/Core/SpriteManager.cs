using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpriteManager
{
    //Sprite[] ui_large_buttons_horizontal;
    //Sprite[] ui_large_buttons_vertical;
    //Sprite[] ui_panels;

    Sprite[] ui_small_buttons;
    Sprite clear;
    Sprite[] ui_Cursor;
    SpriteLibraryAsset[] SLA_Array;
    Sprite[] ui_Area;

    Dictionary<string, Sprite> areaDict = new Dictionary<string, Sprite>();

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


        ui_Area = Resources.LoadAll<Sprite>("Sprite/_UI/Area");
        for (int i = 0; i < ui_Area.Length; i++)
        {
            areaDict.Add(ui_Area[i].name, ui_Area[i]);
        }
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

    public Sprite Get_Area(string area)
    {
        Sprite sprite = areaDict["Clean"];
        if (string.IsNullOrEmpty(area)) //? area가 null이면 바로 리턴
        {
            return sprite;
        }

        areaDict.TryGetValue(area, out sprite);
        return sprite;
    }

    public Sprite GetClear()
    {
        return clear;
    }

}
