using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArtifactBox : MonoBehaviour
{
    //SpriteResolver Resolver;

    private void Start()
    {
        //Resolver = GetComponent<SpriteResolver>();
    }


    bool ReturnCheck() //? 이벤트 발생 조건
    {
        if (Main.Instance.Management == false) return true;
        if (Main.Instance.CurrentAction != null) return true;
        if (UserData.Instance.GameMode == Define.GameMode.Stop) return true;
        if (Time.timeScale == 0) return true;


        // PointerEventData를 생성하고 현재 마우스 위치를 설정
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            //Debug.Log("현재 마우스가 올라가 있는 UI: " + results[0].gameObject.name);
            //? 팝업 ui랑 메인 ui 위에선 작동안하도록 변경
            if (results[0].gameObject.GetComponentInParent<UI_Management>(true)) return true;
            if (results[0].gameObject.GetComponentInParent<UI_PopUp>(true))
            {
                if (results[0].gameObject.GetComponentInParent<UI_DungeonPlacement>(true) == null)
                {
                    return true;
                }
            }
        }

        return false;
    }


    //private void OnMouseEnter()
    //{
    //    EnterReserve = StartCoroutine(Wait_Enter());
    //}

    //IEnumerator Wait_Enter()
    //{
    //    yield return new WaitUntil(() => ReturnCheck() == false);

    //    if (isActive)
    //    {
    //        //Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Active_line");
    //    }
    //    else
    //    {
    //        Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Inactive_line");
    //    }
    //}

    //Coroutine EnterReserve;

    //private void OnMouseExit()
    //{
    //    if (EnterReserve != null)
    //    {
    //        StopCoroutine(EnterReserve);
    //    }
    //    StartCoroutine(Wait_Exit());
    //}

    //IEnumerator Wait_Exit()
    //{
    //    yield return new WaitUntil(() => ReturnCheck() == false);

    //    if (isActive)
    //    {
    //        //Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Active");
    //    }
    //    else
    //    {
    //        Resolver.SetCategoryAndLabel(_OrbType.ToString(), "Inactive");
    //    }
    //}



    private void OnMouseUpAsButton()
    {
        if (ReturnCheck()) return;

        //Debug.Log(_OrbType + "Click");
        StartCoroutine(WaitFrame());
    }


    IEnumerator WaitFrame() //? ClearPanel이 타일 이외의 지역 좌클릭 = CloseAll 로 해놨기때문에 1프레임 기다려야함
    {
        yield return new WaitForEndOfFrame();
        MouseClickEvent();
    }




    void MouseClickEvent()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_ArtifactBox>();
    }



   


}
