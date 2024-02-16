using System.Collections;
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
            foreach (T component in go.GetComponentsInChildren<T>())
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
                return $"<color=#ff4444ff>{text}</color>";

            case Define.TextColor.monster_green:
                return $"<color=#44ff44ff>{text}</color>";


            case Define.TextColor.SkyBlue:
                return $"<color=#55bbffff>{text}</color>";

            case Define.TextColor.LightGreen:
                return $"<color=#55ff55ff>{text}</color>";

            case Define.TextColor.LightYellow:
                return $"<color=#ffff55ff>{text}</color>";


            default:
                return $"<color=white>{text}</color>";
        }
    }
}
