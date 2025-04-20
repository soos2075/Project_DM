using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Title_Content : UI_Base
{

    void Start()
    {
        Init();
    }



    enum Objects
    {
        mainText,
        Title_Content,
    }


    public override void Init()
    {
        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.mainText).GetComponent<TMPro.TextMeshProUGUI>().text = Data.Title;
    }


    SO_Title Data { get; set; }
    public void Set_TitleData(SO_Title _data, bool tooltip = true)
    {
        Data = _data;

        if (tooltip) //? 툴팁활성화 여부
        {
            var tool = gameObject.GetOrAddComponent<UI_Tooltip>();
            string content = $"{_data.Effect}\n\n<i><color=#395A2Cff>{_data.Detail}</color></i>";
            tool.SetTooltipContents(_data.Title, content, UI_TooltipBox.ShowPosition.LeftDown);
        }
    }
}
