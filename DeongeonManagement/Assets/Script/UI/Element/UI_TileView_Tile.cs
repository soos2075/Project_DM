using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_TileView_Tile : UI_Base
{
    void Start()
    {
        Init();
    }


    enum Contents
    {
        TileView_Tile,
    }




    public BasementTile Tile { get; set; }
    UI_TileView_Floor parent;


    IPlacementable CurrentTempData { get; set; }



    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
        parent = GetComponentInParent<UI_TileView_Floor>();

        gameObject.AddUIEvent((data) => TileMoveEvent(data), Define.UIEvent.Move);
        gameObject.AddUIEvent((data) => TileExitEvent(data), Define.UIEvent.Exit);
        gameObject.AddUIEvent((data) => TileClickEvent(data), Define.UIEvent.LeftClick);

        gameObject.AddUIEvent((data) => TileDownEvent(data), Define.UIEvent.Down);
        gameObject.AddUIEvent((data) => TileUpEvent(data), Define.UIEvent.Up);
    }





    void TileClickEvent(PointerEventData _data)
    {
        if (FindAnyObjectByType<UI_Monster_Management>())
        {
            Debug.Log("�߸���Ŭ��");
            return;
        }

        if (!Main.Instance.Management)
        {
            if (CurrentTempData != null)
            {
                Camera.main.GetComponent<CameraControl>().ChasingTarget_Continue(CurrentTempData.GetObject().transform);
            }
        }

        if (Tile.Original == null)
        {
            parent.InsteadOpenFloorEvent(_data);
        }
        else
        {
            parent.ChildExitEvent();
            Tile.Original.MouseClickEvent();
        }
    }



    void TileMoveEvent(PointerEventData _data)
    {
        if (isDownEvent) return;

        if (Main.Instance.QuickPlacement != null)
        {
            StopCoroutine(Main.Instance.QuickPlacement);
            Main.Instance.QuickPlacement = null;
            Debug.Log("����ġ ���");
            return;
        }

        //? Ÿ�Ϻ並 ������ �ٸ� �˾��� ���������� �����̺�Ʈ ����
        if (Managers.UI._popupStack.Count > 0 && Managers.UI.ContainsPopup(typeof(UI_TileView)) == false) return;


        //Debug.Log(Managers.UI._popupStack.Count);


        if (CurrentTempData != null)
        {
            if (CurrentTempData.GetObject().GetComponentInChildren<SpriteRenderer>(true).enabled)
            {
                parent.ChildMoveEvent_CurrentData(CurrentTempData);
            }
            else
            {
                CurrentTempData = null;
            }
        }
        else
        {
            parent.ChildMoveEvent(Tile, _data);
            if (Tile.Current != null)
            {
                if (Tile.Current.PlacementType == PlacementType.NPC || Tile.Current.PlacementType == PlacementType.Monster)
                {
                    CurrentTempData = Tile.Current;
                    StartCoroutine(TempData());
                }
            }
        }

        if (Tile.Original != null)
        {
            Tile.Original.MouseMoveEvent();
            //Debug.Log(Tile.Original + "��");
        }
    }


    void TileExitEvent(PointerEventData _data)
    {
        parent.ChildExitEvent();

        if (Tile.Original != null)
        {
            Tile.Original.MouseExitEvent();
            //Debug.Log(Tile.Original + "����");
        }
    }



    IEnumerator TempData()
    {
        yield return new WaitForSeconds(2.0f);
        CurrentTempData = null;
    }



    bool isDownEvent;

    void TileDownEvent(PointerEventData _data)
    {
        if (Tile.Original != null)
        {
            Tile.Original.MouseDownEvent();
            StartCoroutine(WaitCor());
        }
    }

    //? Up�� �ܵ����� ȣ��� �� ����. �� Down�� ȣ�� �� �ڿ� ���콺�� �����̴��� Down�� ȣ��� Ÿ�Ͽ��� �Ҹ�
    //? �� ���콺�� ������� Ŭ���� ������ ����̺�Ʈ�� �˾Ƽ� �� �߻��Ѵٴ� ����.
    void TileUpEvent(PointerEventData _data) 
    {
        if (Tile.Original != null)
        {
            //Debug.Log(Tile.Original);
            Tile.Original.MouseUpEvent();
        }
    }

    IEnumerator WaitCor()
    {
        isDownEvent = true;

        yield return null;
        yield return new WaitForSecondsRealtime(0.6f);
        yield return new WaitUntil(() => Main.Instance.QuickPlacement == null);

        isDownEvent = false;
    }

}
