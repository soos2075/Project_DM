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
        SLA_Array_NPC_Anim = Resources.LoadAll<SpriteLibraryAsset>("Animation/_NPC_Anim/SLA_NPC_Custom");

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
        Artifacts_Library = Resources.Load<SpriteLibraryAsset>("SpriteLibraryAsset/SLA_Artifact");
        Trait_Library = Resources.Load<SpriteLibraryAsset>("SpriteLibraryAsset/SLA_TraitBar");
        BattleStatus_Library = Resources.Load<SpriteLibraryAsset>("SpriteLibraryAsset/SLA_BattleStatus");
    }



    public SpriteLibraryAsset Get_NPC_Anim(string assetName)
    {
        foreach (var item in SLA_Array_NPC_Anim)
        {
            if (item.name == assetName)
            {
                return item;
            }
        }
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

    SpriteLibraryAsset[] SLA_Array_Facility;

    SpriteLibraryAsset[] SLA_Array_NPC_Anim;

    SpriteLibraryAsset Monster_Library;
    SpriteLibraryAsset Technical_library;
    SpriteLibraryAsset NPC_Library;
    SpriteLibraryAsset UI_library;
    SpriteLibraryAsset Contents_library;
    SpriteLibraryAsset Artifacts_Library;
    SpriteLibraryAsset BattleStatus_Library;
    SpriteLibraryAsset Trait_Library;

    public Sprite Get_SLA(Library type, string category, string label)
    {
        switch (type)
        {
            case Library.Artifact:
                return Get_SLA(Artifacts_Library, category, label);

            case Library.BattleStatus:
                return Get_SLA(BattleStatus_Library, category, label);

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

            case Library.Trait:
                return Get_SLA(Trait_Library, category, label);

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
        Artifact,
        Trait,
        BattleStatus,
    }





    public Sprite GetClear()
    {
        return clear;
    }




}
