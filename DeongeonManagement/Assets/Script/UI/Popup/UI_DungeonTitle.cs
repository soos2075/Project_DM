using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DungeonTitle : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Objects
    {
        //NoTouch,
        Panel,
        //Close,

        Content,
    }


    ScrollRect scroll;

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(Objects));

        scroll = GetComponentInChildren<ScrollRect>(true);

        Init_Contents();
    }

    void Init_Contents()
    {
        //? ���� Īȣ����Ʈ�� �����ͼ� �����ϸ� ��. �������°� TitleManager
        //List<GameObject> titleSortList = new List<GameObject>();
        var titleSortList = GameManager.Title.GetCurrentTitle();


        foreach (var item in titleSortList)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Title/Title_Content", GetObject((int)Objects.Content).transform);

            //? content���� getcomponent�ؼ� ������ �ֱ�
            content.GetComponent<UI_Title_Content>().Set_TitleData(item.Data);


            //? ���̽� ��ũ�ѷ�Ʈ ���� ���ϱ�
            content.AddUIEvent((data) => scroll.OnDrag(data), Define.UIEvent.Drag);
            content.AddUIEvent((data) => scroll.OnBeginDrag(data), Define.UIEvent.BeginDrag);
            content.AddUIEvent((data) => scroll.OnEndDrag(data), Define.UIEvent.EndDrag);
        }

    }





    public override bool EscapeKeyAction()
    {
        return true;
    }
    //private void OnEnable()
    //{
    //    Time.timeScale = 0;
    //}
    //private void OnDestroy()
    //{
    //    PopupUI_OnDestroy();
    //}


}


