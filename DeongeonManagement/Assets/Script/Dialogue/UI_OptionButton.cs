using System;
using TMPro;
using UnityEngine.EventSystems;

public class UI_OptionButton : UI_Base
{
    void Start()
    {
        //Init();
    }


    TextMeshProUGUI tmp;
    public override void Init()
    {
        
    }


    public void SetAction(Action<PointerEventData> action, string textContent)
    {
        gameObject.AddUIEvent((data) => action.Invoke(data));
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = textContent;
    }


}
