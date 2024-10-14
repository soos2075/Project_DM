using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Overlay : UI_Base
{
    void Start()
    {
        //Init();
    }




    public override void Init()
    {
        SizeAdjust();
    }
    void SizeAdjust()
    {
        float offsetY = transform.parent.GetComponent<RectTransform>().sizeDelta.y / 10;

        var rectPos = GetComponent<RectTransform>().position;
        GetComponent<RectTransform>().position = new Vector3(rectPos.x, offsetY, rectPos.z);
    }


    Action<PointerEventData> autoDest;
    GameObject parent;

    public void SetOverlay(Sprite _sprite, GameObject _parent, string setboolName)
    {
        autoDest = data => { AutoDestroy(setboolName); };

        GetComponent<Image>().sprite = _sprite;
        //GetComponent<Image>().SetNativeSize();
        GetComponent<RectTransform>().sizeDelta = new Vector2(_sprite.rect.width * 2, _sprite.rect.height * 2);

        parent = _parent;
        parent.AddUIEvent(autoDest);
    }




    void AutoDestroy(string setboolName)
    {
        Debug.Log("자동삭제");
        UserData.Instance.FileConfig.SetBoolValue(setboolName, false);
        parent.RemoveUIEvent(autoDest);
        Managers.Resource.Destroy(this.gameObject);
    }


}
