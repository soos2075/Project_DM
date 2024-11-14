using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D.Animation;

public class ArtifactBox : MonoBehaviour
{
    SpriteResolver Resolver;

    private void Start()
    {
        Resolver = GetComponent<SpriteResolver>();
        Init_State();
    }

    string categoty = "Bronze";

    void Init_State()
    {
        int artifactCount = GameManager.Artifact.GetCurrentArtifact().Count;

        if (artifactCount > 0)
        {
            categoty = "Silver";
        }
        if (artifactCount > 5)
        {
            categoty = "Gold";
        }

        Resolver.SetCategoryAndLabel(categoty, "Inactive");
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


    #region ShowBox

    UI_TileView view;
    void MoveEvent()
    {
        if (Managers.UI._popupStack.Count > 0) return;
        if (view == null)
        {
            view = Managers.UI.ShowPopUpAlone<UI_TileView>();

            string title = $"{UserData.Instance.LocaleText_Tooltip("��Ƽ��Ʈ������")}";
            string detile = $"{UserData.Instance.LocaleText_Tooltip("��Ƽ��Ʈ������_����")}" +
                $"\n{UserData.Instance.LocaleText_Tooltip("�����Ѿ�Ƽ��Ʈ")} : <b>{GameManager.Artifact.GetArtifactCountAll()}</b>" +
                $"\n{UserData.Instance.LocaleText_Tooltip("����ȹ�淮")} : <b>{50 * GameManager.Artifact.GetArtifactCountAll()}</b>";

            view.ViewContents(title, detile);
        }


    }
    void CloseView()
    {
        if (view)
        {
            Managers.UI.ClosePopupPick(view);
            view = null;
        }
    }

    #endregion

    private void OnMouseOver()
    {
        if (Main.Instance.Management == false) return;
        MoveEvent();
        //Debug.Log("����");
    }



    private void OnMouseEnter()
    {
        EnterReserve = StartCoroutine(Wait_Enter());

    }

    IEnumerator Wait_Enter()
    {
        yield return new WaitUntil(() => ReturnCheck() == false);
        Resolver.SetCategoryAndLabel(categoty, "Inactive_line");
    }

    Coroutine EnterReserve;

    private void OnMouseExit()
    {
        if (EnterReserve != null)
        {
            StopCoroutine(EnterReserve);
        }
        StartCoroutine(Wait_Exit());
    }

    IEnumerator Wait_Exit()
    {
        yield return new WaitUntil(() => ReturnCheck() == false);
        Resolver.SetCategoryAndLabel(categoty, "Inactive");
        CloseView();
        //Debug.Log("����");
    }



    private void OnMouseUpAsButton()
    {
        if (ReturnCheck()) return;
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
