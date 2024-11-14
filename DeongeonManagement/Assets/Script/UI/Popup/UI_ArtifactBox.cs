using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ArtifactBox : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Images
    {
        NoTouch,
        Close,

    }

    enum Objects
    {
        Content
    }


    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(Objects));

        GetImage((int)Images.NoTouch).gameObject.AddUIEvent(data => ClosePopUp(), Define.UIEvent.RightClick);
        GetImage((int)Images.Close).gameObject.AddUIEvent(data => ClosePopUp(), Define.UIEvent.LeftClick);


        Init_Item();
        Set_Current();
    }


    public List<UI_Item_Artifact> itemArray;


    void Init_Item()
    {
        int size = GameManager.Artifact.DataSize();

        itemArray = new List<UI_Item_Artifact>();

        var contents = GetObject((int)Objects.Content).transform;
        for (int i = 0; i < size; i++)
        {
            var obj = Managers.Resource.Instantiate("UI/PopUp/Element/UI_Item_Artifact", contents);
            var item = obj.GetComponent<UI_Item_Artifact>();
            item.DataSet(Managers.Sprite.GetClear(), "", true);
            itemArray.Add(item);
        }
    }


    public void Set_Current()
    {
        var current = GameManager.Artifact.GetCurrentArtifact();

        for (int i = 0; i < current.Count; i++)
        {
            itemArray[i].DataSet(
                Managers.Sprite.Get_SLA(SpriteManager.Library.Artifact, current[i].Data.SLA_category, current[i].Data.SLA_label),
                $"{current[i].Count}");

            var tool = itemArray[i].gameObject.GetOrAddComponent<UI_Tooltip>();
            string content = $"{current[i].Data.tooltip_Effect}\n<i><color=#395A2Cff>{current[i].Data.detail}</color></i>";

            tool.SetTooltipContents(current[i].Data.labelName, content, UI_TooltipBox.ShowPosition.RightDown);
        }
    }

}
