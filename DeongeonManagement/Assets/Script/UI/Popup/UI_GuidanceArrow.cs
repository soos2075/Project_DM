using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuidanceArrow : UI_PopUp
{
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        //Bind<Image>(typeof(Images));
        //GetImage((int)Images.MaskArea).gameObject.AddUIEvent((data) => TargetAreaClickEvent(), Define.UIEvent.LeftClick);
        //GetImage((int)Images.BlackPanel).gameObject.AddUIEvent((data) => BlackArea(), Define.UIEvent.LeftClick);

        StartCoroutine(WaitUntilTurnOver());
    }


    enum Images
    {
        MaskArea,
        BlackPanel,
        GuidanceArrow,
    }


    IEnumerator WaitUntilTurnOver()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Main.Instance.Management == false);

        //Managers.UI.ClosePopupPick(this);
        //? 논푸쉬라 셀프로 꺼야함
        Managers.Resource.Destroy(gameObject);
    }


    void TargetAreaClickEvent()
    {
        Debug.Log("클릭이야");
    }
    void BlackArea()
    {
        Debug.Log("클릭이야22222");
    }





}
