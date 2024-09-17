using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpriteManager
{
    //Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();

    //Sprite[] ui_Cursor;
    //public enum Cursors
    //{
    //    orange = 4,
    //    strawberry = 5,
    //    outLine = 8,
    //}

    //public Sprite GetCursor(Cursors cursors)
    //{
    //    return ui_Cursor[(int)cursors];
    //}

    Sprite clear;
    Sprite[] ui_Area;

    Dictionary<string, Sprite> areaDict = new Dictionary<string, Sprite>();
    public void Init()
    {
        clear = Resources.Load<Sprite>("Sprite/UI/Clear");

        //ui_Cursor = Resources.LoadAll<Sprite>("Sprite/UI/Cursors");

        SLA_Array_Facility = Resources.LoadAll<SpriteLibraryAsset>("SpriteLibraryAsset/SLA_Facility");

        ui_Area = Resources.LoadAll<Sprite>("Sprite/_UI/Area");
        for (int i = 0; i < ui_Area.Length; i++)
        {
            areaDict.Add(ui_Area[i].name, ui_Area[i]);
        }

        Monster_Library = Resources.Load<SpriteLibraryAsset>("SpriteLibraryAsset/SLA_Monster");
        NPC_Library = Resources.Load<SpriteLibraryAsset>("SpriteLibraryAsset/SLA_NPC");
        UI_library = Resources.Load<SpriteLibraryAsset>("SpriteLibraryAsset/SLA_UI");
        Technical_library = Resources.Load<SpriteLibraryAsset>("SpriteLibraryAsset/SLA_Technical");
        Contents_library = Resources.Load<SpriteLibraryAsset>("SpriteLibraryAsset/SLA_Contents");
    }



    //public Sprite GetSprite (string path)
    //{
    //    Sprite sprite = null;
    //    if (spriteDict.TryGetValue(path, out sprite))
    //    {
    //        return sprite;
    //    }

    //    sprite = Resources.Load<Sprite>($"Sprite/{path}");
    //    if (sprite == null)
    //    {
    //        sprite = Resources.Load<Sprite>($"Sprite/Object/{path}");
    //        if (sprite == null)
    //        {
    //            sprite = GetSprite_SLA(path);
    //            if (sprite == null)
    //            {
    //                Debug.Log($"Sprite/{path} : Sprite Not Exist");
    //                return clear;
    //            }
    //        }
    //    }
    //    spriteDict.Add(path, sprite);
    //    return sprite;
    //}

    //public Sprite GetSprite_SLA(string _searchName)
    //{
    //    foreach (var item in SLA_Array_Facility)
    //    {
    //        foreach (var _category in item.GetCategoryNames())
    //        {
    //            if (_searchName == _category)
    //            {
    //                return item.GetSprite(_category, "Entry");
    //            }
    //            else
    //            {
    //                foreach (var _label in item.GetCategoryLabelNames(_category))
    //                {
    //                    if (_searchName == _label)
    //                    {
    //                        return item.GetSprite(_category, _label);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    Debug.Log($"{_searchName} : Sprite Not Exist from SLA Data");
    //    return null;
    //}


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

    SpriteLibraryAsset[] SLA_Array_Facility;

    SpriteLibraryAsset Monster_Library;
    SpriteLibraryAsset Technical_library;
    SpriteLibraryAsset NPC_Library;
    SpriteLibraryAsset UI_library;
    SpriteLibraryAsset Contents_library;

    public Sprite Get_SLA(Library type, string category, string label)
    {
        switch (type)
        {
            case Library.Contents:
                return Get_SLA(Contents_library, category, label);

            case Library.UI:
                return Get_SLA(UI_library, category, label);

            case Library.Monster:
                return Get_SLA(Monster_Library, category, label);

            case Library.NPC:
                return Get_SLA(NPC_Library, category, label);

            case Library.Technical:
                return Get_SLA(Technical_library, category, label);

            case Library.Facility:
                return GetSprite_SLA(category, label);

            default:
                return GetSprite_SLA(category, label);
        }
    }

    Sprite GetSprite_SLA(string category, string label)
    {
        foreach (var item in SLA_Array_Facility)
        {
            foreach (var _category in item.GetCategoryNames())
            {
                if (category == _category)
                {
                    return item.GetSprite(_category, label);
                }
            }
        }

        Debug.Log($"{category}-{label} : Sprite Not Exist from SLA Data");
        return null;
    }

    Sprite Get_SLA(SpriteLibraryAsset asset, string category, string label)
    {
        foreach (var _category in asset.GetCategoryNames())
        {
            if (category == _category)
            {
                return asset.GetSprite(_category, label);
            }
        }
        return null;
    }



    public enum Library
    {
        UI,
        Monster,
        NPC,
        Technical,
        Facility,
        Contents,
    }





    public Sprite GetClear()
    {
        return clear;
    }




}
