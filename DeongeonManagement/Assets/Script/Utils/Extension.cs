using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    public static void AddUIEvent(this GameObject go, Action<PointerEventData> act, Define.UIEvent event_type = Define.UIEvent.LeftClick)
    {
        UI_Base.AddUIEvent(go, act, event_type);
    }

    public static void RemoveUIEventAll(this GameObject go)
    {
        UI_Base.RemoveUIEventAll(go);
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    public static string SetTextColorTag(this string text, Define.TextColor color)
    {
        return Util.SetTextColorTag(text, color);
    }


}
