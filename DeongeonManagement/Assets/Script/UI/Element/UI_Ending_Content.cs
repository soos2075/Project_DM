using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ending_Content : UI_Base
{


    void Start()
    {
        Init();
    }



    UI_Elbum Parent;
    SO_Ending Data { get; set; }
    bool isClear;


    public void Set_Data(UI_Elbum _p, SO_Ending _data)
    {
        Parent = _p;
        Data = _data;
    }


    Button btn;
    Image main;
    public override void Init()
    {
        btn = GetComponent<Button>();
        main = transform.GetChild(0).GetComponent<Image>();

        CollectionManager coll = CollectionManager.Instance;
        isClear = coll.RoundClearData.EndingClearCheck((Endings)Data.id);


        //? 확인한 엔딩은 썸네일 - 알이미지 (0번째 이미지)
        if (isClear)
        {
            main.sprite = coll.GetData_Ending((Endings)Data.id).cutSceneList[0].sprite;
        }
        else
        {
            main.sprite = Managers.Sprite.GetClear();
        }

        gameObject.AddUIEvent(data => Parent.Ending_ClickEvent(Data, isClear), Define.UIEvent.LeftClick);
    }




}
