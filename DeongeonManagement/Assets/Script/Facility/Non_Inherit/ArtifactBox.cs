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


    bool ReturnCheck() //? �̺�Ʈ �߻� ����
    {
        if (Main.Instance.Management == false) return true;
        if (Main.Instance.CurrentAction != null) return true;
        if (UserData.Instance.GameMode == Define.GameMode.Stop) return true;
        if (Time.timeScale == 0) return true;


        // PointerEventData�� �����ϰ� ���� ���콺 ��ġ�� ����
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            //Debug.Log("���� ���콺�� �ö� �ִ� UI: " + results[0].gameObject.name);
            //? �˾� ui�� ���� ui ������ �۵����ϵ��� ����
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


    IEnumerator WaitFrame() //? ClearPanel�� Ÿ�� �̿��� ���� ��Ŭ�� = CloseAll �� �س��⶧���� 1������ ��ٷ�����
    {
        yield return new WaitForEndOfFrame();
        MouseClickEvent();
    }




    void MouseClickEvent()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_ArtifactBox>();
    }



   


}
