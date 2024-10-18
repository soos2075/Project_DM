using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Item_Artifact : UI_Base
{

    enum Objects
    {
        Artifact,
        Count,
    }


    public override void Init()
    {
        Bind<GameObject>(typeof(Objects));
    }


    public void DataSet(Sprite _image, string _count, bool init = false)
    {
        if (init)
        {
            Init();
        }

        GetObject((int)Objects.Artifact).GetComponent<Image>().sprite = _image;
        GetObject((int)Objects.Count).GetComponent<TMPro.TextMeshProUGUI>().text = _count;
    }


}
