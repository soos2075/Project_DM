using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ending_Monster : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Panels
    {
        Panel,
        //ClosePanel,

        SubPanel,
        GridPanel,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Image>(typeof(Panels));

        CreateSavedMonster();
    }

    public Save_MonsterData[] datas;

    public void CreateSavedMonster()
    {
        childList = new List<UI_MonsterBox_Clear>();

        for (int i = 0; i < datas.Length; i++)
        {
            var obj = Managers.Resource.Instantiate("UI/PopUp/Monster/MonsterBox_Clear", GetImage(((int)Panels.GridPanel)).transform);
            var box = obj.GetComponent<UI_MonsterBox_Clear>();
            box.monster = datas[i];
            childList.Add(box);
        }
    }




    //public UI_MonsterBox_Clear Current { get; private set; }
    List<UI_MonsterBox_Clear> childList;








}
