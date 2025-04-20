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
        //? 현재 칭호리스트를 가져와서 정렬하면 댐. 가져오는건 TitleManager
        //List<GameObject> titleSortList = new List<GameObject>();
        var titleSortList = GameManager.Title.GetCurrentTitle();


        foreach (var item in titleSortList)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Title/Title_Content", GetObject((int)Objects.Content).transform);

            //? content에서 getcomponent해서 데이터 넣기
            content.GetComponent<UI_Title_Content>().Set_TitleData(item.Data);


            //? 베이스 스크롤렉트 방해 안하기
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


