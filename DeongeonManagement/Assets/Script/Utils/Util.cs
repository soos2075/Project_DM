﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>(true))
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }


    public static List<T> ListShuffle<T>(List<T> _list)
    {

        for (int i = _list.Count - 1; i > 0; i--)
        {
            int ran = UnityEngine.Random.Range(0, i);

            T temp = _list[i];
            _list[i] = _list[ran];
            _list[ran] = temp;

        }

        return _list;
    }

    public static List<T> ListDistinct<T>(List<T> _list)
    {
        HashSet<T> uniqueSet = new HashSet<T>(_list);
        return new List<T>(uniqueSet);
    }



    public static T GetClassToString<T>(string className) where T : class
    {
        Type type = Type.GetType(className);
        T instance = Activator.CreateInstance(type) as T;
        return instance;
    }
    public static Type GetTypeToString(string className)
    {
        Type type = Type.GetType(className);
        return type;
    }



    public static string GetDiffStar(Define.DifficultyLevel _lv)
    {
        switch (_lv)
        {
            case Define.DifficultyLevel.Easy:
                return "☆";

            case Define.DifficultyLevel.Normal:
                return "★";

            case Define.DifficultyLevel.Hard:
                return "★★★";

            case Define.DifficultyLevel.Master:
                return "★★★★★";
        }
        return "";
    }
    public static string GetDiffStar(int _lv)
    {
        return GetDiffStar((Define.DifficultyLevel)_lv);
    }


    public static Vector2Int[] GetBoundary(Define.Boundary _boundary)
    {
        switch (_boundary)
        {
            case Define.Boundary.Boundary_1x1:
                return Define.Boundary_1x1;
            case Define.Boundary.Boundary_1x2:
                return Define.Boundary_1x2;
            case Define.Boundary.Boundary_1x3:
                return Define.Boundary_1x3;
            case Define.Boundary.Boundary_2x1:
                return Define.Boundary_2x1;
            case Define.Boundary.Boundary_2x2:
                return Define.Boundary_2x2;
            case Define.Boundary.Boundary_2x3:
                return Define.Boundary_2x3;
            case Define.Boundary.Boundary_3x1:
                return Define.Boundary_3x1;
            case Define.Boundary.Boundary_3x2:
                return Define.Boundary_3x2;
            case Define.Boundary.Boundary_3x3:
                return Define.Boundary_3x3;
            case Define.Boundary.Boundary_4x4:
                return Define.Boundary_4x4;
            case Define.Boundary.Boundary_5x5:
                return Define.Boundary_5x5;


            case Define.Boundary.Boundary_Cross_1:
                return Define.Boundary_Cross_1;
            case Define.Boundary.Boundary_Cross_2:
                return Define.Boundary_Cross_2;
            case Define.Boundary.Boundary_Cross_3:
                return Define.Boundary_Cross_3;
            case Define.Boundary.Boundary_Cross_4:
                return Define.Boundary_Cross_4;


            case Define.Boundary.Boundary_X_1:
                return Define.Boundary_X_1;
            case Define.Boundary.Boundary_X_Big:
                return Define.Boundary_X_Big;


            case Define.Boundary.Boundary_Diagonal__Small:
                return Define.Boundary_Diagonal__Small;
            case Define.Boundary.Boundary_Diagonal__Big:
                return Define.Boundary_Diagonal__Big;
            case Define.Boundary.Boundary_Diagonal_Reverse_Small:
                return Define.Boundary_Diagonal_Reverse_Small;
            case Define.Boundary.Boundary_Diagonal_Reverse_Big:
                return Define.Boundary_Diagonal_Reverse_Big;




            case Define.Boundary.Boundary_V_1:
                return Define.Boundary_V_1;

            case Define.Boundary.Boundary_Side_All:
                return Define.Boundary_Side_All;

            case Define.Boundary.Boundary_Side_X:
                return Define.Boundary_Side_X;

            case Define.Boundary.Boundary_Side_Cross:
                return Define.Boundary_Side_Cross;

            case Define.Boundary.Boundary_Side_Cross_2:
                return Define.Boundary_Side_Cross_2;

            case Define.Boundary.Boundary_Empty_rhombus:
                return Define.Boundary_Empty_rhombus;

            case Define.Boundary.Boundary_H:
                return Define.Boundary_H;
            case Define.Boundary.Boundary_I:
                return Define.Boundary_I;
            case Define.Boundary.Boundary_3x3_empty:
                return Define.Boundary_3x3_empty;
        }
        return null;
    }



    public static string SetTextColorTag(string text, Define.TextColor color)
    {
        switch (color)
        {
            case Define.TextColor.Bold:
                return $"<b>{text}</b>";

            case Define.TextColor.Italic:
                return $"<i>{text}</i>";

            case Define.TextColor.red:
                return $"<color=red>{text}</color>";

            case Define.TextColor.green:
                return $"<color=green>{text}</color>";

            case Define.TextColor.blue:
                return $"<color=blue>{text}</color>";

            case Define.TextColor.yellow:
                return $"<color=yellow>{text}</color>";

            case Define.TextColor.white:
                return $"<color=white>{text}</color>";

            case Define.TextColor.black:
                return $"<color=black>{text}</color>";



            case Define.TextColor.npc_red:
                return $"<color=#992222ff>{text}</color>";

            case Define.TextColor.monster_green:
                return $"<color=#229922ff>{text}</color>";


            case Define.TextColor.SkyBlue:
                return $"<color=#55bbffff>{text}</color>";

            case Define.TextColor.HeavyGreen:
                return $"<color=#007700ff>{text}</color>";

            case Define.TextColor.HeavyYellow:
                return $"<color=#bb7700ff>{text}</color>";

            case Define.TextColor.Plus_Green:
                return $"<color=#4c7c48ff>{text}</color>";

            case Define.TextColor.Plus_Blue:
                return $"<color=#4377baff>{text}</color>";

            case Define.TextColor.Plus_Red:
                return $"<color=#e24242ff>{text}</color>";


            case Define.TextColor.New_Brown:
                return $"<color=#975135ff>{text}</color>";
            case Define.TextColor.New_BrownLight:
                return $"<color=#d0a187ff>{text}</color>";
            case Define.TextColor.New_Beige:
                return $"<color=#ffe2caff>{text}</color>";

            case Define.TextColor.LightYellow:
                return $"<color=#FFFED0ff>{text}</color>";


            default:
                return $"<color=white>{text}</color>";
        }
    }
}
